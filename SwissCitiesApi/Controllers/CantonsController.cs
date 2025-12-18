using DataSource;
using DataSource.DataStructures;
using Microsoft.AspNetCore.Mvc;

namespace SwissCitiesApi.Controllers;

[ApiController]
[Route("[controller]")]
public class CantonsController : ControllerBase
{
    /// <summary>
    /// Lists all cantons and their population.
    /// </summary>
    /// <returns>All cantons with their total population.</returns>
    [HttpGet]
    public ActionResult<IEnumerable<CantonSummaryDto>> Get()
    {
        return Ok(InMemoryCantonRepository.GetCantons());
    }

    /// <summary>
    /// Gets the canton and all it's cities.
    /// </summary>
    /// <param name="cantonName">The name of the canton.</param>
    /// <returns>A dto describing the canton.</returns>
    [HttpGet("{cantonName}")]
    public ActionResult<CantonDto> Get([FromRoute] string cantonName)
    {
        return Ok(InMemoryCantonRepository.Get(cantonName));
    }

    /// <summary>
    /// Updates the population of a canton and all it's cities.
    /// </summary>
    /// <param name="updatedCanton">The updated population of a canton and all it's cities.</param>
    /// <returns>A dto describing the updated canton.</returns>
    [HttpPut("[action]")]
    public ActionResult<CantonDto> UpdateCantonPopulation([FromBody] CantonDto updatedCanton)
    {
        return Ok(InMemoryCantonRepository.UpdatePopulations(updatedCanton));
    }
}