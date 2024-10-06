namespace DAL.Models;

public class CommentDto
{
    public int CommentID { get; set; }
    public int TaskID { get; set; }
    public int UserID { get; set; }
    public string Content { get; set; }
    public DateTime CreatedDate { get; set; }
    public UserDTO? User { get; set; }
}