using GoodHamburger.Application.DTOs;
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

    /// <summary>Retorna o cardapio com sanduiches, acompanhamentos e precos.</summary>
    /// <returns>Itens disponiveis para montar um pedido.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(MenuResponse), StatusCodes.Status200OK)]
    public IActionResult Get()
    {
        var menu = _service.GetMenu();
        return Ok(menu);
    }
}
