using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models;

public class Comment
{
    [Key] // Specifies that CommentID is the primary key
    public int CommentID { get; set; }

    [Required]
    public int TaskID { get; set; } // Foreign key to the Task entity

    [Required]
    public int UserID { get; set; } // Foreign key to the User entity

    [Required]
    [StringLength(500)] // Limit the content length for comments
    public string Content { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.Now;

    // Navigation properties
    [ForeignKey("TaskID")]
    public Task Task { get; set; } // The task associated with the comment

    [ForeignKey("UserID")]
    public User User { get; set; } // The user who made the comment
}