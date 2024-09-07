using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext to the services
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Call the database initializer to create the admin user if necessary
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await DatabaseInitializer.InitializeAsync(db); //initialize Admin 
    await ToDoInitializer.InitializeAsync(db);
}

app.MapGet("/", () => "Hello World!");

app.MapGet("/test-connection", async (AppDbContext db) =>
{
    try
    {
        // Check if you can connect to the database
        await db.Database.CanConnectAsync();
        return Results.Ok("Connection Successful!");
    }
    catch (Exception ex)
    {
        return Results.Problem("Connection Failed: " + ex.Message);
    }
});

app.MapGet("/users", async (AppDbContext db) =>
{
    var users = await db.Users.ToListAsync();
    return Results.Ok(users);
});

app.MapGet("/categories", async (AppDbContext db) =>
{
    var categories = await db.Categories.ToListAsync();
    return Results.Ok(categories);
});


//get all todos 
app.MapGet("/todos", async (AppDbContext db) =>
{
    var todos = await db.Todos.ToListAsync();
    return Results.Ok(todos);
});

//search a query 


app.Run();
