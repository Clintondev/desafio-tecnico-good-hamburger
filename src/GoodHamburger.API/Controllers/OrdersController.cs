using GoodHamburger.Application.Common;
using GoodHamburger.Application.DTOs;
using GoodHamburger.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace GoodHamburger.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly OrderService _service;

    public OrdersController(OrderService service)
    {
        _service = service;
    }

    /// <summary>Lista todos os pedidos cadastrados.</summary>
    /// <returns>Pedidos ordenados do mais recente para o mais antigo.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<OrderResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result.Value);
    }

    /// <summary>Busca um pedido pelo identificador.</summary>
    /// <param name="id">Identificador do pedido.</param>
    /// <returns>Pedido encontrado.</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        if (!result.IsSuccess)
        {
            return NotFound(new { error = result.Error });
        }

        return Ok(result.Value);
    }

    /// <summary>Cria um pedido com um sanduiche e acompanhamentos opcionais.</summary>
    /// <remarks>
    /// O payload aceita os campos booleanos hasFries/hasDrink ou a lista items.
    /// Quando items contem valores repetidos, a API retorna 422 com uma mensagem clara.
    /// </remarks>
    /// <param name="request">Dados do novo pedido.</param>
    /// <returns>Pedido criado com precos e descontos calculados.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create([FromBody] CreateOrderRequest request)
    {
        var result = await _service.CreateAsync(request);
        if (!result.IsSuccess)
        {
            return UnprocessableEntity(new { error = result.Error });
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }

    /// <summary>Atualiza os itens de um pedido existente.</summary>
    /// <param name="id">Identificador do pedido.</param>
    /// <param name="request">Novos itens do pedido.</param>
    /// <returns>Pedido atualizado com precos e descontos recalculados.</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateOrderRequest request)
    {
        var result = await _service.UpdateAsync(id, request);
        if (!result.IsSuccess)
        {
            if (result.ErrorCode == ErrorCode.NotFound)
            {
                return NotFound(new { error = result.Error });
            }

            return UnprocessableEntity(new { error = result.Error });
        }

        return Ok(result.Value);
    }

    /// <summary>Remove um pedido pelo identificador.</summary>
    /// <param name="id">Identificador do pedido.</param>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _service.DeleteAsync(id);
        if (!result.IsSuccess)
        {
            return NotFound(new { error = result.Error });
        }

        return NoContent();
    }
}
