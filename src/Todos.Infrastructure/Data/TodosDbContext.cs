using Microsoft.EntityFrameworkCore;
using Todos.Core.Entities;

namespace Todos.Infrastructure.Data;

public class TodosDbContext : DbContext
{
    public TodosDbContext(DbContextOptions<TodosDbContext> options) : base(options) { }

    public DbSet<TodoItem> Todos => Set<TodoItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var todo = modelBuilder.Entity<TodoItem>();
        todo.HasKey(t => t.Id);
        todo.Property(t => t.Title).IsRequired().HasMaxLength(200);
        todo.Property(t => t.IsCompleted).HasDefaultValue(false);
        todo.Property(t => t.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
    }
}