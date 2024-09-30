using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using DAL.Enums;

namespace DAL.Models
{
    public class User
    {
        [Key]
        public int UserID { get; set; }

        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }

        public RoleEnum Role { get; set; }

        // Collection of tasks assigned to this user
        public ICollection<Task> AssignedTasks { get; set; } = new List<Task>();

        public ICollection<Project> Projects { get; set; } = new List<Project>();
    }
}