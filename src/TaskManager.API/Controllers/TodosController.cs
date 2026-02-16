using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.DTOs.Todos;
using TaskManager.Application.Features.Todos.Commands.CreateTodo;
using TaskManager.Application.Features.Todos.Commands.DeleteTodo;
using TaskManager.Application.Features.Todos.Commands.UpdateTodo;
using TaskManager.Application.Features.Todos.Queries.GetTodoById;
using TaskManager.Application.Features.Todos.Queries.GetTodos;

namespace TaskManager.API.Controllers;

[ApiController]
[Authorize]
[Route("api/todos")]
public sealed class TodosController : ControllerBase
{
    private readonly ISender _sender;

    public TodosController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTodos([FromQuery] GetTodosRequest request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetTodosQuery(request), cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetTodoByIdQuery(id), cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { error = result.Error });
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateTodoDto request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new CreateTodoCommand(request), cancellationToken);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value)
            : BadRequest(new { error = result.Error });
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTodoDto request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new UpdateTodoCommand(id, request), cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { error = result.Error });
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DeleteTodoCommand(id), cancellationToken);
        return result.IsSuccess ? NoContent() : NotFound(new { error = result.Error });
    }
}
