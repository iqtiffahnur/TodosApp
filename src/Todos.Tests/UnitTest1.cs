using System.Threading.Tasks;
using TodoApp.Application.Services;
using TodoApp.Core.Entities;
using TodoApp.Core.Interfaces;
using Xunit;
using System.Collections.Generic;

class FakeRepo : ITodoRepository
{
    private readonly List<TodoItem> _items = new();
    private int _nextId = 1;

    public Task<TodoItem> AddAsync(TodoItem item)
    {
        item.Id = _nextId++;
        _items.Add(item);
        return Task.FromResult(item);
    }
    public Task DeleteAsync(int id)
    {
        _items.RemoveAll(i => i.Id == id);
        return Task.CompletedTask;
    }
    public Task<List<TodoItem>> GetAllAsync() => Task.FromResult(new List<TodoItem>(_items));
    public Task<TodoItem?> GetByIdAsync(int id) => Task.FromResult(_items.Find(i => i.Id == id));
    public Task UpdateAsync(TodoItem item) => Task.CompletedTask;
}

public class TodoServiceTests
{
    [Fact]
    public async Task CreateAsync_Throws_When_Title_Empty()
    {
        var svc = new TodoApp.Application.Services.TodoService(new FakeRepo());
        await Assert.ThrowsAsync<System.ArgumentException>(() => svc.CreateAsync("  "));
    }

    [Fact]
    public async Task CreateAsync_Adds_Item()
    {
        var svc = new TodoService(new FakeRepo());
        var created = await svc.CreateAsync("Test");
        Assert.Equal("Test", created.Title);
        Assert.False(created.IsCompleted);
        Assert.True(created.Id > 0);
    }
}