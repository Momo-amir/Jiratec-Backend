using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models;

public class ActivityLog
{
    [Key] // Specifies that LogID is the primary key
    public int LogID { get; set; }

    [Required]
    public int TaskID { get; set; } // Foreign key to the Task entity

    [Required]
    public int UserID { get; set; } // Foreign key to the User entity

    [Required]
    [StringLength(100)]
    public string Action { get; set; } // Describes the action taken (e.g., "Task Created", "Status Updated")

    public DateTime Timestamp { get; set; } = DateTime.Now; // Defaults to the current time

    // Navigation properties
    [ForeignKey("TaskID")]
    public Task Task { get; set; } // The task associated with the activity

    [ForeignKey("UserID")]
    public User User { get; set; } // The user who performed the action
}