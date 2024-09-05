using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DAL.Enums;

namespace DAL.Models;

public class Task
{
    [Key]
    public int TaskID { get; set; }

    [Required]
    public int ProjectID { get; set; }

    [Required]
    [StringLength(100)]
    public string Title { get; set; }

    public string Description { get; set; }

    public int AssignedTo { get; set; }

    [Required]
    [EnumDataType(typeof(TaskStatusEnum))]
    public TaskStatusEnum Status { get; set; } = TaskStatusEnum.ToDo; // Default to 'ToDo'

    [Required]
    [EnumDataType(typeof(TaskPriorityEnum))]
    public TaskPriorityEnum Priority { get; set; } = TaskPriorityEnum.Medium; // Default to 'Medium'

    public DateTime DueDate { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.Now;

    [ForeignKey("ProjectID")]
    public Project Project { get; set; }

    [ForeignKey("AssignedTo")]
     public User AssignedToUser { get; set; }
}
