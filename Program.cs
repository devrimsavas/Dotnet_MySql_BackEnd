using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1;
//add also dapper 
using Microsoft.Data.SqlClient;
using Dapper;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext to the services for EF CORE 
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


//ADD SqlConnection for Dapper USAGE 

builder.Services.AddScoped<SqlConnection>(Sp =>
    new SqlConnection(builder.Configuration.GetConnectionString("DefaultConnection"))

);

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


//get all todos EFC 
app.MapGet("/todos", async (AppDbContext db) =>
{
    var todos = await db.Todos.ToListAsync();
    return Results.Ok(todos);
});


//get all Todos Dapper 
app.MapGet("/todos-dapper", async (SqlConnection db) =>
{
    //sql string 
    var sql = @"SELECT * FROM Todos";
    var todos = await db.QueryAsync<ToDo>(sql);
    return Results.Ok(todos);
});

//search a query search query class inside SearchInputs 

app.MapPost("/search-todos", async (SearchToDoInput searchInput, SqlConnection db) =>
{
    var sql = @"SELECT * FROM ToDos WHERE 1=1";  // Start with base query

    // Dynamically build the query based on input
    if (!string.IsNullOrEmpty(searchInput.Title))
    {
        sql += " AND Title LIKE @Title";
    }

    if (!string.IsNullOrEmpty(searchInput.Status))
    {
        sql += " AND Status = @Status";
    }

    if (!string.IsNullOrEmpty(searchInput.Description))
    {
        sql += " AND Description LIKE @Description";
    }

    // Perform the search with Dapper
    var todos = await db.QueryAsync<ToDo>(sql, new
    {
        Title = $"%{searchInput.Title}%",
        Status = searchInput.Status,
        Description = $"%{searchInput.Description}%"
    });

    Console.WriteLine(todos);

    return Results.Ok(todos);
});

//used dapper 
app.MapPost("/newtodo", async (SqlConnection db, ToDo todo) =>
{

    try
    {


        // Insert query for the new ToDo
        var sql = @"INSERT INTO Todos (Title, Description, Status, Priority, DueDate, CreatedDate, UpdatedDate, CategoryId, AssignedToUserId)
                    VALUES (@Title, @Description, @Status, @Priority, @DueDate, @CreatedDate, @UpdatedDate, @CategoryId, @AssignedToUserId)";

        //execute with Dapper 
        var result = await db.ExecuteAsync(sql, new
        {

            todo.Title,
            todo.Description,
            todo.Status,
            todo.Priority,
            todo.DueDate,
            CreatedDate = DateTime.Now,  // Set current timestamp
            UpdatedDate = DateTime.Now,  // Set current timestamp
            todo.CategoryId,
            todo.AssignedToUserId
        }

        );
        return Results.Ok("NEW TODO CREATED ");


    }
    catch (Exception ex)
    {
        return Results.Problem($"Error creating ToDo: {ex.Message}");
    }



});



app.Run();
