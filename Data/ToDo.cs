

//todo class 

public class ToDo
{
    public int ToDoId { get; set; }  // Primary key
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string Status { get; set; } = "";
    public string Priority { get; set; } = "";
    public DateTime DueDate { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public DateTime? CompletedDate { get; set; }

    public int CategoryId { get; set; }  // Foreign key to Category
    public Category Category { get; set; } = new Category(); //will come from category class

    public int AssignedToUserId { get; set; } // Foreign key to User
    public User AssignedTo { get; set; } = new User(); //will come from user class 
}
