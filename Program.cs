using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1;
//add also dapper 
using Microsoft.Data.SqlClient;
using Dapper;
using MySqlX.XDevAPI.Common;

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

//ROUTES 

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
    var todos = await db.QueryAsync<ToDo>(sql); //QueryAsync : we expect multiple rows from query REturn all rows as a collection
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

//new todo used EF CORE 
app.MapPost("/newtodo-efcore", async (AppDbContext db, ToDo todo) =>
{
    try
    {
        todo.CreatedDate = DateTime.Now;
        todo.UpdatedDate = DateTime.Now;

        //add new todo
        db.Todos.Add(todo);

        //save 
        await db.SaveChangesAsync();
        return Results.Ok("NEW TODO CREATED EF CORE METHOD");

    }
    catch (Exception ex)
    {
        return Results.Problem($"Error creating ToDo: {ex.Message}");

    }
});

//new TODO DELETE WITH ENTITY FRAMEWORK CORE

app.MapDelete("/delete/{id}", async (AppDbContext db, int id) =>
{

    try
    {
        if (await db.Todos.FindAsync(id) is ToDo todo)
        {
            db.Todos.Remove(todo);
            await db.SaveChangesAsync();
            return Results.Ok("To do deleted");
        }
        return Results.NotFound("this tood was not found");



    }
    catch (Exception ex)
    {
        return Results.Problem($"Problem {ex.Message} ");
    }

});

//DELETE WITH DAPPER 

app.MapDelete("/delete-dapper/{id}", async (SqlConnection db, int id) =>
{
    try
    {
        var sql = "DELETE FROM Todos WHERE ToDoId = @Id";

        var result = await db.ExecuteAsync(sql, new { Id = id });

        if (result > 0)
        {
            return Results.Ok("ToDo deleted successfully");
        }
        else
        {
            return Results.NotFound("This ToDo was not found");
        }
    }
    catch (Exception ex)
    {
        return Results.Problem($"Problem {ex.Message}");
    }
});

//GET ONE TODO BY ID EFC 

app.MapGet("/todo/{id}", async (int id, AppDbContext db) =>
    await db.Todos.FindAsync(id)
    is ToDo toDo
    ? Results.Ok(toDo)
    : Results.NotFound($"There is not any todo with id  {id}"));



//GET ONE TODO BY ID DAPPER 

app.MapGet("/todo-dapper/id", async (int id, SqlConnection db) =>
{
    try
    {
        var sql = "SELECT * FROM Todos WHERE ToDoId=@Id";
        var todo = await db.QuerySingleOrDefaultAsync<ToDo>(sql, new { Id = id });
        if (todo != null)
        {
            return Results.Ok(todo);
        }
        else
        {
            return Results.NotFound("Not existed");
        }
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }



});

//PUt with Entity Frame Core 

app.MapPut("/update/{id}", async (int id, AppDbContext db, ToDo inputTodo) =>
{
    try
    {
        var todo = await db.Todos.FindAsync(id);
        if (todo is null) return Results.NotFound($"Todo with id: {id} was not found");

        // Update fields
        todo.Title = inputTodo.Title;
        todo.Description = inputTodo.Description;
        todo.Status = inputTodo.Status;
        todo.Priority = inputTodo.Priority;
        todo.DueDate = inputTodo.DueDate;
        todo.UpdatedDate = DateTime.Now;  // Set updated timestamp
        todo.CategoryId = inputTodo.CategoryId;
        todo.AssignedToUserId = inputTodo.AssignedToUserId;

        // Save the changes
        await db.SaveChangesAsync();

        return Results.Ok("Todo updated successfully.");
    }
    catch (Exception ex)
    {
        return Results.Problem($"Problem updating ToDo: {ex.Message}");
    }
});




app.Run();
