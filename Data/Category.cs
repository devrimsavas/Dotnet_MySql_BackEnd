

public class Category
{
    public int CategoryId { get; set; }  // Primary key
    public string CategoryName { get; set; } = "";

    public List<ToDo> ToDos { get; set; } = new List<ToDo>();  // Navigation property
}
