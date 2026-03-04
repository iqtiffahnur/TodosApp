using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Todos.Infrastructure.Data;

public class DesignTimeTodosDbContextFactory : IDesignTimeDbContextFactory<TodosDbContext>
{
    public TodosDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<TodosDbContext>();
        // Fallback connection for design-time (used by dotnet-ef)
        optionsBuilder.UseSqlite("Data Source=todos.db");
        return new TodosDbContext(optionsBuilder.Options);
    }
}