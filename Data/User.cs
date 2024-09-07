

public class User
{
    public int UserId { get; set; }  // Primary key
    public string UserName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";

    public List<ToDo> ToDos { get; set; } = new List<ToDo>();  // Navigation property
}
