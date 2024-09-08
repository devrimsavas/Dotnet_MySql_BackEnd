# Dapper and EFC Methods and Their Comparison

his table compares the Dapper and Entity Framework Core (EFC) methods used for CRUD operations in this application.

| **Operation**       | **Entity Framework Core (EFC)**                                                             | **Dapper**                                                                                                                        |
| ------------------- | ------------------------------------------------------------------------------------------- | --------------------------------------------------------------------------------------------------------------------------------- |
| **Get All Todos**   | `var todos = await db.Todos.ToListAsync();`                                                 | `var todos = await db.QueryAsync<ToDo>("SELECT * FROM Todos");`                                                                   |
| **Get Todo by ID**  | `var todo = await db.Todos.FindAsync(id);`                                                  | `var todo = await db.QuerySingleOrDefaultAsync<ToDo>("SELECT * FROM Todos WHERE ToDoId = @Id", new { Id = id });`                 |
| **Insert New Todo** | `db.Todos.Add(todo); await db.SaveChangesAsync();`                                          | `await db.ExecuteAsync("INSERT INTO Todos (Title, Description, ...) VALUES (@Title, @Description, ...)", new { ... });`           |
| **Update Todo**     | `db.Todos.Update(todo); await db.SaveChangesAsync();`                                       | (No direct equivalent, you would write a `UPDATE` SQL query)                                                                      |
| **Delete Todo**     | `db.Todos.Remove(todo); await db.SaveChangesAsync();`                                       | `await db.ExecuteAsync("DELETE FROM Todos WHERE ToDoId = @Id", new { Id = id });`                                                 |
| **Search Todos**    | `var todos = await db.Todos.Where(t => t.Title.Contains(searchInput.Title)).ToListAsync();` | `var todos = await db.QueryAsync<ToDo>("SELECT * FROM Todos WHERE Title LIKE @Title", new { Title = $"%{searchInput.Title}%" });` |

## Explanation of Each Method:

1. Get All Todos:
   o EFC: Uses LINQ (ToListAsync) to retrieve all records from the Todos table.
   o Dapper: Executes a raw SQL query (QueryAsync) to get all records.
2. Get Todo by ID:
   o EFC: Uses FindAsync to retrieve a single record by its primary key.
   o Dapper: Uses QuerySingleOrDefaultAsync to run a SQL query and fetch one record based on the ID.
3. Insert New Todo:
   o EFC: Adds the new ToDo entity to the context and calls SaveChangesAsync to persist it to the database.
   o Dapper: Directly executes an INSERT INTO SQL command with the new ToDo data.
4. Update Todo:
   o EFC: You'd call Update(todo) and SaveChangesAsync. (Dapper would require a custom UPDATE SQL query, not shown in this example.)
5. Delete Todo:
   o EFC: Removes the entity from the context and saves changes.
   o Dapper: Executes a DELETE SQL query to remove the record.
6. Search Todos:
   o EFC: Uses LINQ to filter todos based on a search condition (like Title).
   o Dapper: Executes a SQL query using LIKE to perform a similar search.

   ### Final Thoughts:

   Both approaches work well, but each has its pros and cons:
   • Entity Framework Core: Offers more abstraction, automatic change tracking, and easier relationship management, but can be slower in performance for complex queries.
   • Dapper: Provides more control over SQL queries and is generally faster but requires more manual SQL handling.
