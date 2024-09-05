using System.ComponentModel.DataAnnotations;

namespace DAL.Models;

public class Project
{
    [Key]
    public int ProjectID { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.Now;

    public User CreatedByUser { get; set; }
}