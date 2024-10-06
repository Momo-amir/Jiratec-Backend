using System.ComponentModel.DataAnnotations;

namespace DAL.Models;

public class Role // Role is not yet implemented in the application. Role enum is used instead to simplify the application and reduce complexity
// if ever in production this should be removed or atleast implemented properly
{
    [Key]
    public int RoleID { get; set; }
    public string RoleName { get; set; }
}