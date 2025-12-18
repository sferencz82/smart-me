using System.Diagnostics;
using DataSource.DataStructures;

namespace DataSource;

public static class InMemoryCantonRepository
{
    private static readonly Dictionary<string, Canton> CantonCache = new();

    private static readonly object TransactionLock = new();

    public static void Initialize()
    {
        var lines = File.ReadAllLines("swiss-cities.csv").Skip(1);
        foreach (var line in lines)
        {
            var split = line.Split(",");
            var cityName = split[0];
            var cantonName = split[1];
            var population = uint.Parse(split[2]);
            if (CantonCache.TryGetValue(cantonName, out var canton))
            {
                canton.Cities.Add(new City { Name = cityName, Population = population });
            }
            else
            {
                canton = new Canton
                {
                    Name = cantonName,
                    TotalPopulation = 0,
                    Cities = [new City { Name = cityName, Population = population }]
                };
                CantonCache.Add(cantonName, canton);
            }
        
            canton.TotalPopulation += population;
        }
    }
    
    public static CantonDto Get(string cantonName)
    {
        lock (TransactionLock)
        {
            if (!CantonCache.TryGetValue(cantonName, out var canton))
            {
                throw new InvalidOperationException($"Canton {cantonName} does not exist.");
            }
            return new CantonDto(canton.Name, canton.TotalPopulation, canton.Cities.ToList());
        }
    }
    
    /// <summary>
    /// Updates the populations of all cities in a canton and the total population of the canton. 
    /// </summary>
    /// <param name="updatedCanton">A dto containing the info to the whole canton.</param>
    /// <returns>The updated canton.</returns>
    /// <exception cref="InvalidOperationException">Throws when the canton does not exist.</exception>
    public static CantonDto UpdatePopulations(CantonDto updatedCanton)
    {
        lock (TransactionLock)
        {
            if (!CantonCache.TryGetValue(updatedCanton.Name, out var canton))
            {
                throw new InvalidOperationException($"Canton {updatedCanton.Name} does not exist.");
            }
            
            foreach (var city in canton.Cities)
            {
                city.Population = updatedCanton.Cities.First(c => c.Name == city.Name).Population;
            }
            canton.TotalPopulation = (uint)canton.Cities.Sum(x => x.Population);

            CantonCache[canton.Name] = canton;

            return new CantonDto(canton.Name, canton.TotalPopulation, canton.Cities.ToList());
        }
    }

    /// <summary>
    /// Returns all cities across all cantons.
    /// </summary>
    /// <returns>A read-only collection of all cities with their population.</returns>
    public static IReadOnlyCollection<City> GetCities()
    {
        lock (TransactionLock)
        {
            return CantonCache.Values
                .SelectMany(canton => canton.Cities)
                .Select(city => new City { Name = city.Name, Population = city.Population })
                .ToList();
        }
    }

    /// <summary>
    /// Returns all cantons with their total population.
    /// </summary>
    /// <returns>A read-only collection of all cantons.</returns>
    public static IReadOnlyCollection<CantonSummaryDto> GetCantons()
    {
        lock (TransactionLock)
        {
            return CantonCache.Values
                .Select(canton => new CantonSummaryDto(canton.Name, canton.TotalPopulation))
                .ToList();
        }
    }
}