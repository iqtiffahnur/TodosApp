using Microsoft.AspNetCore.Mvc;
using Todos.Application.DTOs;
using Todos.Application.Services;

namespace Todos.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TodosController : ControllerBase
{
    private readonly ITodoService _service;
    public TodosController(ITodoService service) => _service = service;

    // GET: /api/todos
    [HttpGet]
    public async Task<ActionResult<List<TodoResponse>>> GetAll(CancellationToken ct)
        => Ok(await _service.GetAllAsync(ct));

    // GET: /api/todos/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<TodoResponse>> GetById(int id, CancellationToken ct)
    {
        var result = await _service.GetByIdAsync(id, ct);
        return result is null ? NotFound() : Ok(result);
    }

    // POST: /api/todos
    [HttpPost]
    public async Task<ActionResult<TodoResponse>> Create([FromBody] CreateTodoRequest request, CancellationToken ct)
    {
        try
        {
            var created = await _service.CreateAsync(request, ct);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // PUT: /api/todos/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTodoRequest request, CancellationToken ct)
    {
        try
        {
            var ok = await _service.UpdateAsync(id, request, ct);
            return ok ? NoContent() : NotFound();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // DELETE: /api/todos/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var ok = await _service.DeleteAsync(id, ct);
        return ok ? NoContent() : NotFound();
    }
}
