using BCrypt.Net;
using Microsoft.EntityFrameworkCore;

public class DatabaseInitializer
{
    public static async Task InitializeAsync(AppDbContext db)  //added admin 
    {
        try
        {
            Console.WriteLine("Checking if any users exist...");

            if (!await db.Users.AnyAsync())
            {
                Console.WriteLine("No users found. Creating admin user...");

                var adminUser = new User
                {
                    UserName = "admin",
                    Email = "admin@example.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("admin123")  // Hash the password
                };

                db.Users.Add(adminUser);
                await db.SaveChangesAsync();

                Console.WriteLine("Admin user created successfully.");
            }
            else
            {
                Console.WriteLine("Users already exist. No admin user created.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error initializing database: {ex.Message}");
        }
    }
}
