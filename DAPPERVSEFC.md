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
