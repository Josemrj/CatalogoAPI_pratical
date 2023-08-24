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

#region Category Endpoints
app.MapGet("/categories", async (AppDbContext db) =>
{
    var categories = await db.Categories.ToListAsync();

    if (categories == null || categories.Count == 0)
        return Results.NotFound("RECURSO NAO ENCONTRADO...");

    return Results.Ok(categories);

});

app.MapGet("/categories/{id:int}", async (int id, AppDbContext db) =>
{
    var categories = await db.Categories.FirstOrDefaultAsync(c => c.CategoryId == id);

    if (categories == null)
        return Results.NotFound("RECURSO NAO ENCONTRADO...");

    return Results.Ok(categories);
});

app.MapPost("/categories", async (Category ct, AppDbContext db) =>
{
    var create = await db.Categories.AddAsync(ct);

    if (create == null)
        return Results.NotFound("RECURSO NAO ENCONTRADO...");

    db.SaveChanges();

    return Results.Created($"/categories{ct.CategoryId}:int", ct);
});

app.MapPut("/categories/{id:int}", async (int id, AppDbContext db, Category catInput) =>
{
    var category = await db.Categories.FirstOrDefaultAsync(c => c.CategoryId == id);

    if (category == null)
        return Results.NotFound("RECURSO NAO ENCONTRADO");

    category.CategoryId = catInput.CategoryId;
    category.Name = catInput.Name;
    category.Description = catInput.Description;

    if (catInput.CategoryId != id)
        return Results.BadRequest($"DADOS À ENVIAR NAO CORRESPONDEM COM DADOS INTERNOS \t\tID:{id}");

    await db.SaveChangesAsync();

    return Results.Created($"/categories/{category.CategoryId}", category);
});

app.MapDelete("/categories/{id:int}", async (int id, AppDbContext db) =>
{
    var category = await db.Categories.FindAsync(id);

    if (category == null)
        return Results.NotFound("RECURSO NAO ENCONTRADO");

    db.Categories.Remove(category);
    await db.SaveChangesAsync();

    return Results.NoContent();
});
#endregion 

#region Product Endpoints
app.MapGet("/products", async (AppDbContext db) => await db.Products.ToListAsync());

app.MapGet("/products/{id:int}", async(int id, AppDbContext db) =>
{
    var product = await db.Products.FindAsync(id);

    if (product == null)
        return Results.NotFound("RECURSO NAO ENCONTRADO");

    return Results.Ok(product);
});

app.MapPost("/products", async (Product prod, AppDbContext db) =>
{
    var product = await db.Products.AddAsync(prod);

    if(product == null) 
        return Results.NotFound(product);

    await db.SaveChangesAsync();

    return Results.Created($"/products/{prod.ProductId}", prod);
});

app.MapPut("/products/{id:int}", async(int id, Product prdInput, AppDbContext db) =>
{
    var products = await db.Products.FirstOrDefaultAsync(c => c.CategoryId == id);

    if (products == null)
        return Results.NotFound(products);

    products.ProductId = prdInput.ProductId;
    products.Name = prdInput.Name;
    products.Description = prdInput.Description;
    products.Price = prdInput.Price;
    products.Image = prdInput.Image;
    products.PurchaseDate = DateTime.Now;
    products.Stock = prdInput.Stock;

    await db.SaveChangesAsync();

    return Results.Created($"products/{products.ProductId}", products);
});

app.MapDelete("/products/{id:int}", async (int id, AppDbContext db) =>
{
    var productDelete = await db.Products.FirstOrDefaultAsync(c => c.CategoryId == id);

    if (productDelete == null)
        return Results.BadRequest("ESSE RECURSO NAO EXISTE");

    db.Products.Remove(productDelete);
    await db.SaveChangesAsync();

    return Results.NoContent();
});
#endregion



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();
