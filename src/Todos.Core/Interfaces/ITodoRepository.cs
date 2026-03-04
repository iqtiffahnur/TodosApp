using Todos.Core.Entities;

namespace Todos.Core.Interfaces;

public interface ITodoRepository
{
    Task<TodoItem?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<List<TodoItem>> GetAllAsync(CancellationToken ct = default);
    Task<TodoItem> AddAsync(TodoItem item, CancellationToken ct = default);
    Task UpdateAsync(TodoItem item, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}