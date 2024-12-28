using HyperCache.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace HyperCache.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomPropertiesController : ControllerBase
{
    public CustomPropertiesController()
    {
    }

    [HttpGet]
    public ActionResult<CustomProperties> Get()
    {
        return Ok(new CustomProperties());
    }
}