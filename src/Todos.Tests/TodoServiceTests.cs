using FluentAssertions;
using Todos.Application.DTOs;
using Todos.Application.Services;
using Todos.Core.Entities;
using Todos.Core.Interfaces;

namespace Todos.Tests;

public class InMemoryRepo : ITodoRepository
{
    private readonly List<TodoItem> _items = new();
    private int _nextId = 1;

    public Task<TodoItem?> GetByIdAsync(int id, CancellationToken ct = default)
        => Task.FromResult(_items.FirstOrDefault(i => i.Id == id));

    public Task<List<TodoItem>> GetAllAsync(CancellationToken ct = default)
        => Task.FromResult(_items.OrderBy(i => i.Id).ToList());

    public Task<TodoItem> AddAsync(TodoItem item, CancellationToken ct = default)
    {
        item.Id = _nextId++;
        _items.Add(item);
        return Task.FromResult(item);
    }

    public Task UpdateAsync(TodoItem item, CancellationToken ct = default)
    {
        var idx = _items.FindIndex(i => i.Id == item.Id);
        if (idx >= 0) _items[idx] = item;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id, CancellationToken ct = default)
    {
        _items.RemoveAll(i => i.Id == id);
        return Task.CompletedTask;
    }
}

public class TodoServiceTests
{
    private readonly ITodoService _service;

    public TodoServiceTests()
    {
        ITodoRepository repo = new InMemoryRepo();
        _service = new TodoService(repo);
    }

    [Fact]
    public async Task Create_Should_Throw_When_Title_Empty()
    {
        var act = async () => await _service.CreateAsync(new CreateTodoRequest(" "));
        await act.Should().ThrowAsync<ArgumentException>()
                 .WithMessage("*cannot be empty*");
    }

    [Fact]
    public async Task Create_And_GetAll_Should_Work()
    {
        var created = await _service.CreateAsync(new CreateTodoRequest("Buy milk"));
        created.Id.Should().BeGreaterThan(0);

        var list = await _service.GetAllAsync();
        list.Should().ContainSingle(x => x.Title == "Buy milk" && x.IsCompleted == false);
    }
}
