using CatalogoAPI_pratical.Context;
using CatalogoAPI_pratical.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(opt =>
            opt.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

var app = builder.Build();

app.MapGet("/categories", async (AppDbContext db) =>
{
    var categories = await db.Categories.ToListAsync();

    if (categories == null || categories.Count == 0)
        return Results.NotFound("RECURSO NAO ENCONTRADO...");
    
    return Results.Ok(categories);
    
});

app.MapGet("/categories/{id:int}", async (int id, AppDbContext db) =>
{
    var categoria = await db.Categories.FirstOrDefaultAsync(c => c.CategoryId == id);

    if (categoria == null)
        return Results.NotFound("RECURSO NAO ENCONTRADO...");

    return Results.Ok(categoria);
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();
