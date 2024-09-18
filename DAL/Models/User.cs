using System.ComponentModel.DataAnnotations;
using DAL.Enums;


namespace DAL.Models;

public class User
{
    [Key]
    public int UserID { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
   
    public RoleEnum Role { get; set; }
    
    public ICollection<Task>? AssignedTo { get; set; }    
    
    public ICollection<Project> Projects { get; set; } = new List<Project>();

    
    
    
}
