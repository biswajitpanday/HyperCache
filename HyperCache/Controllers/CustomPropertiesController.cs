using HyperCache.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HyperCache.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomPropertiesController(AppDbContext context) : ControllerBase
{

    [HttpGet]
    public async Task<IActionResult> GetPagedProperties([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var properties = await context.CustomProperties
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return Ok(properties);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPropertyDetails(string id)
    {
        var customProperty = await context.CustomProperties.FindAsync(id);
        if (customProperty == null) 
            return NotFound();
        return Ok(customProperty);
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var customProperties = await context.CustomProperties
            .ToListAsync();
        return Ok(customProperties);
    }

    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] string keyword)
    {
        if (string.IsNullOrEmpty(keyword))
            return BadRequest("Keyword can't be empty!");

        var customProperties = await context.CustomProperties
            .Where(x => x.Name.Contains(keyword))
            .ToListAsync();
        return Ok(customProperties);
    }
}