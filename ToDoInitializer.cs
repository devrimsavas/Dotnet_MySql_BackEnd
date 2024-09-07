using Microsoft.EntityFrameworkCore;

public class ToDoInitializer
{
    public static async Task InitializeAsync(AppDbContext db)
    {
        try
        {
            Console.WriteLine("Checking if any ToDos exist...");

            if (!await db.Todos.AnyAsync())
            {
                Console.WriteLine("No ToDos found. Creating first ToDo...");

                // Step 1: Check if Category exists, otherwise create it
                var category = await db.Categories.FirstOrDefaultAsync(c => c.CategoryName == "Daily Tasks");
                if (category == null)
                {
                    category = new Category
                    {
                        CategoryName = "Daily Tasks"
                    };
                    db.Categories.Add(category);
                    await db.SaveChangesAsync();
                    Console.WriteLine("Category 'Daily Tasks' created.");
                }

                // Step 2: Check if admin user exists
                var adminUser = await db.Users.FirstOrDefaultAsync(u => u.UserName == "admin");
                if (adminUser == null)
                {
                    Console.WriteLine("Admin user not found. Please create admin user first.");
                    return;
                }

                // Step 3: Create the first ToDo and assign it to the admin user
                var firstTodo = new ToDo
                {
                    Title = "Check Members Activity",
                    Description = "Admin needs to check all members' activity every day.",
                    Status = "In Progress",
                    Priority = "High",
                    DueDate = DateTime.Now.AddDays(1),
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now,
                    Category = category,  // Assign the category
                    AssignedToUserId = adminUser.UserId,  // Assign to admin user
                    AssignedTo = adminUser
                };

                db.Todos.Add(firstTodo);
                await db.SaveChangesAsync();

                Console.WriteLine("First ToDo created successfully.");
            }
            else
            {
                Console.WriteLine("ToDos already exist.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error initializing ToDos: {ex.Message}");
        }
    }
}
