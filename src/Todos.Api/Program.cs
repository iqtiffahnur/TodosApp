using Microsoft.EntityFrameworkCore;
using Todos.Application.Services;
using Todos.Core.Interfaces;
using Todos.Infrastructure.Data;
using Todos.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Controllers (API)
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// SQLite DbContext
var conn = builder.Configuration.GetConnectionString("Default") ?? "Data Source=todos.db";
builder.Services.AddDbContext<TodosDbContext>(options => options.UseSqlite(conn));

// Dependency Injection
builder.Services.AddScoped<ITodoRepository, EfTodoRepository>();
builder.Services.AddScoped<ITodoService, TodoService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<Todos.Infrastructure.Data.TodosDbContext>();
    db.Database.Migrate();
}

// ---- Pipeline ----
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

// Serve the React static files from wwwroot
app.UseDefaultFiles();   
app.UseStaticFiles();

app.Use(async (ctx, next) =>
{
    Console.WriteLine($"{ctx.Request.Method} {ctx.Request.Scheme}://{ctx.Request.Host}{ctx.Request.Path}{ctx.Request.QueryString}");
    await next();
});

// API endpoints
app.MapControllers();

// SPA fallback (any non-API route returns index.html)
app.MapFallbackToFile("/index.html");

app.Run();