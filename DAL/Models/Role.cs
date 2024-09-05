using System.ComponentModel.DataAnnotations;

namespace DAL.Models;

public class Role
{
    [Key]
    public int RoleID { get; set; }
    public string RoleName { get; set; }
}