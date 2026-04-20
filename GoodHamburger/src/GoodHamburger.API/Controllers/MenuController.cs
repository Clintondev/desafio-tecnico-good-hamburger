using GoodHamburger.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace GoodHamburger.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MenuController : ControllerBase
{
    private readonly OrderService _service;

    public MenuController(OrderService service)
    {
        _service = service;
    }

    [HttpGet]
    public IActionResult Get()
    {
        var menu = _service.GetMenu();
        return Ok(menu);
    }
}
