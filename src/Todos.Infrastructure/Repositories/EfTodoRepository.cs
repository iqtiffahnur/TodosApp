using Microsoft.EntityFrameworkCore;
using Todos.Core.Entities;
using Todos.Core.Interfaces;
using Todos.Infrastructure.Data;

namespace Todos.Infrastructure.Repositories;

public class EfTodoRepository : ITodoRepository
{
    private readonly TodosDbContext _db;
    public EfTodoRepository(TodosDbContext db) => _db = db;

    public Task<TodoItem?> GetByIdAsync(int id, CancellationToken ct = default)
        => _db.Todos.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id, ct);

    public Task<List<TodoItem>> GetAllAsync(CancellationToken ct = default)
        => _db.Todos.AsNoTracking().OrderBy(t => t.Id).ToListAsync(ct);

    public async Task<TodoItem> AddAsync(TodoItem item, CancellationToken ct = default)
    {
        _db.Todos.Add(item);
        await _db.SaveChangesAsync(ct);
        return item;
    }

    public async Task UpdateAsync(TodoItem item, CancellationToken ct = default)
    {
        _db.Todos.Update(item);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var existing = await _db.Todos.FindAsync(new object?[] { id }, ct);
        if (existing is null) return;
        _db.Todos.Remove(existing);
        await _db.SaveChangesAsync(ct);
    }
}