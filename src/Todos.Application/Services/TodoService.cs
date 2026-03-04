using Todos.Core.Entities;
using Todos.Core.Interfaces;
using Todos.Application.DTOs;

namespace Todos.Application.Services;

public interface ITodoService
{
    Task<List<TodoResponse>> GetAllAsync(CancellationToken ct = default);
    Task<TodoResponse?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<TodoResponse> CreateAsync(CreateTodoRequest request, CancellationToken ct = default);
    Task<bool> UpdateAsync(int id, UpdateTodoRequest request, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}

public class TodoService : ITodoService
{
    private readonly ITodoRepository _repo;

    public TodoService(ITodoRepository repo) => _repo = repo;

    public async Task<List<TodoResponse>> GetAllAsync(CancellationToken ct = default)
        => (await _repo.GetAllAsync(ct)).Select(Map).ToList();

    public async Task<TodoResponse?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var item = await _repo.GetByIdAsync(id, ct);
        return item is null ? null : Map(item);
    }

    public async Task<TodoResponse> CreateAsync(CreateTodoRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
            throw new ArgumentException("Title cannot be empty.");
        if (request.Title.Length > 200)
            throw new ArgumentException("Title must be 200 characters or less.");

        var entity = new TodoItem { Title = request.Title.Trim() };
        var created = await _repo.AddAsync(entity, ct);
        return Map(created);
    }

    public async Task<bool> UpdateAsync(int id, UpdateTodoRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
            throw new ArgumentException("Title cannot be empty.");
        if (request.Title.Length > 200)
            throw new ArgumentException("Title must be 200 characters or less.");

        var existing = await _repo.GetByIdAsync(id, ct);
        if (existing is null) return false;

        existing.Title = request.Title.Trim();
        existing.IsCompleted = request.IsCompleted;
        existing.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(existing, ct);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(id, ct);
        if (existing is null) return false;

        await _repo.DeleteAsync(id, ct);
        return true;
    }

    private static TodoResponse Map(TodoItem i)
        => new(i.Id, i.Title, i.IsCompleted, i.CreatedAt, i.UpdatedAt);
}
