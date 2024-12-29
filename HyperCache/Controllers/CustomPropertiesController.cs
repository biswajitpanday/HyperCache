using HyperCache.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HyperCache.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomPropertiesController(AppDbContext context) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
        var customProperty = await context.CustomProperties
                .FirstOrDefaultAsync(x => x.Id.ToString() == id);
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
    public async Task<IActionResult> Search(string keyword)
    {
        if (string.IsNullOrEmpty(keyword))
            return BadRequest("Keyword can't be empty!");

        var customProperties = await context.CustomProperties
            .Where(x => x.Name.Contains(keyword))
            .ToListAsync();
        return Ok(customProperties);
    }
}