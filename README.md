# STEP BY STEP MYSQL- BACKEND ASP NET CONFIGURATION

## THIS TUTORIAL CREATES DATABASE AND ITS TABLE FROM SCRACTCH...

### Database initialize is created with `EF CORE CRUDS` are written with `Dapper`

## IF YOU WANTED TO CREATE TABLES AND DATABASE MANUALLY JUMP TO STEP 2 AND THEN STEP 7**\*\***\*\***\*\***\*\*\***\*\***\*\***\*\***

# 1- INSTALL PACKAGES

For working with MySQL in your ASP.NET backend, you will need to adjust your package selection since the ones you've listed are mainly for SQL Server. Here's how you can modify the packages for MySQL:

MySQL Connector/NET: To connect to a MySQL database, use the MySQL connector package:

dotnet and dapper packages
Dapper: Dapper works with any ADO.NET connection, so you can keep the Dapper package for your lightweight ORM:

```bash
dotnet add package MySql.Data

dotnet add package Dapper
```

Entity Framework Core for MySQL: If you're using Entity Framework Core, you'll need the MySQL provider:

```bash
dotnet add package Pomelo.EntityFrameworkCore.MySql
```

- These packages should help you set up your ASP.NET project for working with MySQL.

# 2- CREATE TABLES

In this example, we have created tables manually with Azure Data Studio : To connect
used

- look at `sql server configuration` and name is `SQLEXPRESS`
- Server Name: localhost\SQLEXPRESS

## TODO TABLES AND RELATIONSHIPS BETWEEN TABLES

### First CREATE DATABASE and SCHEMA

```bash
CREATE DATABASE FirstToDo
GO
USE FirstToDo
GO

CREATE SCHEMA TodoAppSchema
GO
```

### we can create tables and relationships

```bash

CREATE TABLE TodoAppSchema.[User] (
UserId INT IDENTITY(1,1) PRIMARY KEY,
UserName NVARCHAR(70),
Email NVARCHAR(100)
);

CREATE TABLE TodoAppSchema.Category (
CategoryId INT IDENTITY(1,1) PRIMARY KEY,
CategoryName NVARCHAR(50)
);

CREATE TABLE TodoAppSchema.ToDo(
ToDoId INT IDENTITY(1,1) PRIMARY KEY,
Title NVARCHAR(50),
Description NVARCHAR(255),
Status NVARCHAR(30) CHECK (Status IN ('Pending', 'In Progress', 'Completed')),
Priority NVARCHAR(30) CHECK (Priority IN ('Low', 'Medium', 'High')),
DueDate DATE,
CreatedDate DATETIME,
UpdatedDate DATETIME,
CompletedDate DATETIME,
CategoryId INT FOREIGN KEY REFERENCES TodoAppSchema.Category(CategoryId),
AssignedToUserId INT FOREIGN KEY REFERENCES TodoAppSchema.[User](UserId)
);

```

- and after created DATABASE and Schema
  `Todo , User , Category` are connected so we will create these tables manually and connect with foreign keys

# 3- CONFIGURE `appsettings.json` MANUALLY

MySql.Data, don't automatically appear in your appsettings.json file. The appsettings.json file is typically used for configuration settings, such as your database connection string, logging settings, and other configuration options, not for package management.

To connect your ASP.NET app to MySQL, you'll need to manually add the connection string for your database in appsettings.json. Here's an example of how you might configure it:

- Connection string

```bash
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=DotNetCourseDatabase;User Id=your_user;Password=your_password;TrustServerCertificate=true;"
}
```

here whole `appsettings.json` file

```bash
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=FirstToDo;TrustServerCertificate=true;Trusted_Connection=true;"
  },
  "AllowedHosts": "*"
}
```

# 4- CREATE CLASSES

so we will create classes as seperate files inside Data folder to use in AppDbContext.cs

- Todo.cs

```bash
public class ToDo
{
    public int ToDoId { get; set; }  // Primary key
    public string Title { get; set; }
    public string Description { get; set; }
    public string Status { get; set; }
    public string Priority { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public DateTime? CompletedDate { get; set; }

    public int CategoryId { get; set; }  // Foreign key to Category
    public Category Category { get; set; }

    public int AssignedToUserId { get; set; }  // Foreign key to User
    public User AssignedTo { get; set; }
}

```

- Category.cs

```bash
public class Category
{
    public int CategoryId { get; set; }  // Primary key
    public string CategoryName { get; set; }

    public List<ToDo> ToDos { get; set; }  // Navigation property
}

```

-User.cs

```bash
public class User
{
    public int UserId { get; set; }  // Primary key
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }

    public List<ToDo> ToDos { get; set; }  // Navigation property
}
```

and we can add them to our AppDbContext.cs in next step

# 5- RETRIEVE CONNECTION STRING

now we can create a class to retrieve connection string and test. But first we create a Data folder and create
AppDbContext.cs . it is DbContext Class

```bash
/this is Database connection manager

using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<ToDo> ToDos { get; set; }
    public DbSet<Category> Categories { get; set; }
}
```

# 6- Add AppDbContext.cs to Program.cs file

this is our CONNECTION MANAGER we added to our program

```bash
builder.Services.AddDbContext<AppDbContext>(options=>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
```

# 6 - Immigration \*\* ?? why

You should focus on running the necessary Entity Framework Core migration commands to create the tables in your database, as I mentioned earlier:
but first if it is not installed

```bash
dotnet tool install --global dotnet-ef
```

ADD ALSO

```bash
dotnet add package Microsoft.EntityFrameworkCore.Design
```

and now :

- RUN THE IMMIGRATION After creating the classes and updating AppDbContext.cs, run this command in the terminal:

```bash
dotnet ef migrations add InitialCreate

```

# 7- THE TUTORIAL OVER CREATES A DATABASE AND TABLES FROM SCRACTCH IF YOU CREATED TABLES MANUALL FOLLOW THESE STEPS DIRECTLY FOR CRUD

## Install Packages

Install Necessary Packages: You’ll need the following packages:

- Entity Framework Core SQL Server

```bash

dotnet add package Microsoft.EntityFrameworkCore.SqlServer
```

- Entity Framework Core Tools (for potential future migrations or commands):

```bash

dotnet add package Microsoft.EntityFrameworkCore.Tools

```

- Entity Framework Core Design (for design-time operations):

```bash

dotnet add package Microsoft.EntityFrameworkCore.Design

```

# 8- ADJUST AND CREATE CONNECTION STRING

```bash
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=FirstToDo;TrustServerCertificate=true;Trusted_Connection=true;"
}

```

# 9- CREATE `AppDbContext.cs`

```bash
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }  // Add other tables when ready
    public DbSet<ToDo> Todos { get; set; }
    public DbSet<Category> Categories { get; set; }
}

```

# 10 - Modify Program.cs: You’ve got it right with your setup. Here's how it looks:

```bash
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext to the services
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Test connection with SQL query
app.MapGet("/test-connection", async (AppDbContext db) =>
{
    var currentDate = await db.Database.ExecuteSqlRawAsync("SELECT GETDATE()");
    return Results.Ok(currentDate);
});

app.Run();

```

## 11 Add initializer File

- install bcrypt

  ```bash

  dotnet add package BCrypt.Net-Next
  ```

- create an initializer File

`DatabaseInitializer.cs`

```bash
using BCrypt.Net; // You need to install BCrypt.Net package if not done already
using Microsoft.EntityFrameworkCore;

public class DatabaseInitializer
{
    public static async Task InitializeAsync(AppDbContext db)
    {
        // Check if there are any users already, if not, create the admin user
        if (!await db.Users.AnyAsync())
        {
            // Create admin user with hashed password
            var admin = new User
            {
                UserName = "admin",
                Email = "admin@example.com",
                Password = BCrypt.Net.BCrypt.HashPassword("admin123") // Hash the admin password
            };

            db.Users.Add(admin);
            await db.SaveChangesAsync();
        }
    }
}

```
