namespace Todos.Application.DTOs;

public record CreateTodoRequest(string Title);
public record UpdateTodoRequest(string Title, bool IsCompleted);
public record TodoResponse(int Id, string Title, bool IsCompleted, DateTime CreatedAt, DateTime? UpdatedAt);