using HyperCache.Api.Data;
using HyperCache.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HyperCache.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomPropertiesController(AppDbContext context) : ControllerBase
{

    [HttpGet("paged")]
    public async Task<IActionResult> GetPagedProperties([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        if (page < 1 || pageSize < 1)
        {
            return BadRequest("Page and PageSize must be greater than 0.");
        }

        var totalCount = await context.CustomProperties.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var properties = await context.CustomProperties
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var response = new
        {
            Items = properties.Select(p => new CustomProperty
            {
                Id = p.Id,
                Name = p.Name,
                Value = p.Value,
                ParentTable = p.ParentTable,
                CreatedBy = p.CreatedBy,
                ModifiedBy = p.ModifiedBy
            }).ToList(),
            CurrentPage = page,
            TotalPages = totalPages,
            HasPreviousPage = page > 1,
            HasNextPage = page < totalPages
        };

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPropertyDetails(string id)
    {
        var customProperty = await context.CustomProperties.FindAsync(Guid.Parse(id));
        if (customProperty == null) 
            return NotFound();
        return Ok(customProperty);
    }

    [HttpGet("all")]
    public async Task<IActionResult> Get()
    {
        var customProperties = await context.CustomProperties
            .ToListAsync();
        return Ok(customProperties);
    }

    [HttpGet("search")]
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