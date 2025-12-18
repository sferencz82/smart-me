using DataSource;
using DataSource.DataStructures;
using Microsoft.AspNetCore.Mvc;

namespace SwissCitiesApi.Controllers;

[ApiController]
[Route("[controller]")]
public class CitiesController : ControllerBase
{
    /// <summary>
    /// Lists all cities and their population.
    /// </summary>
    /// <returns>All cities with their population.</returns>
    [HttpGet]
    public ActionResult<IEnumerable<City>> Get()
    {
        return Ok(InMemoryCantonRepository.GetCities());
    }
}
