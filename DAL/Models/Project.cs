using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace DAL.Models
{
    public class Project
    {
        [Key]
        public int ProjectID { get; set; }

        [Required]
        [StringLength(100)] // Optional: Max length for the title - could be adjusted
        public string Title { get; set; } // Non-nullable

        [Required]
        public string Description { get; set; } // Non-nullable

        [Required]
        public User CreatedBy { get; set; } // Non-nullable

        public DateTime CreatedDate { get; set; } = DateTime.Now; // Non-nullable

        public ICollection<Task>? Tasks { get; set; } // Optional

        public ICollection<User> Users { get; set; } = new List<User>(); // Non-nullable, initialized
    }
}