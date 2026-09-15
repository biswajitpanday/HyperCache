using HyperCache.Api.Data;
using HyperCache.Api.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HyperCache.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomPropertiesController(AppDbContext context) : ControllerBase
{

    [HttpGet("paged")]
    public async Task<IActionResult> GetPagedProperties([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        if (page < 1 || pageSize < 1)
            return BadRequest("Page and PageSize must be greater than 0.");

        var response = await context.CustomProperties
            .Select(p => new
            {
                p.Id,
                p.Name,
                p.Value,
                p.ParentTable,
                p.CreatedBy,
                p.ModifiedBy
            })
            .ToPagedAsync(page, pageSize, cancellationToken);

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPropertyDetails(string id, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(id, out var propertyId))
            return BadRequest("Invalid GUID format.");

        var customProperty = await context.CustomProperties.FindAsync([propertyId], cancellationToken);
        if (customProperty == null)
            return NotFound();

        return Ok(customProperty);
    }

    [HttpGet("all")]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var customProperties = await context.CustomProperties.ToListAsync(cancellationToken);
        return Ok(customProperties);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string keyword, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            return BadRequest("Keyword can't be empty!");

        var customProperties = await context.CustomProperties
            .Where(x => x.Name.Contains(keyword))
            .ToListAsync(cancellationToken);

        return Ok(customProperties);
    }
}
