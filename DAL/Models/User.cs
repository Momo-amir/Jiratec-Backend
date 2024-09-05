using System.ComponentModel.DataAnnotations;

namespace DAL.Models;

public class User
{
    [Key]
    public int UserID { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public int RoleID { get; set; }

    public Role Role { get; set; }
}