using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;

namespace DAL.Models
{
    public class Project
    {
        [Key]
        public int ProjectID { get; set; }

        [Required]
        [StringLength(100)] // Optional: Add a maximum length for the title
        public string Title { get; set; }

        public string Description { get; set; }

        // Remove the int CreatedBy property
        // [Required]
        // public int CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Keep only one property for the creator
        [Required]
        public User CreatedBy { get; set; }

        // Collection of Tasks associated with the Project
        public ICollection<Task> Tasks { get; set; } = new List<Task>();

        // Collection of Users assigned to the project
        public ICollection<User> Users { get; set; } = new List<User>();

        // Mapping method inside the model entity
        public Project MapDtoToProject(ProjectDTO projectDto)
        {
            if (projectDto == null) return null; // Null check

            var project = new Project
            {
                ProjectID = projectDto.ProjectID,
                Title = projectDto.Title,
                Description = projectDto.Description,
                CreatedDate = projectDto.CreatedDate
                // Removed CreatedBy mapping
            };

            // Map the CreatedBy property
            if (projectDto.CreatedBy != null)
            {
                project.CreatedBy = new User
                {
                    UserID = projectDto.CreatedBy.UserID,
                    Name = projectDto.CreatedBy.Name,
                    Email = projectDto.CreatedBy.Email,
                    Role = projectDto.CreatedBy.Role
                };
            }

            // Map the users assigned to the project
            if (projectDto.Users != null)
            {
                project.Users = projectDto.Users.Select(u => new User
                {
                    UserID = u.UserID,
                    Name = u.Name,
                    Email = u.Email,
                    Role = u.Role
                }).ToList();
            }

            // Map tasks if necessary
            if (projectDto.Tasks != null)
            {
                project.Tasks = projectDto.Tasks.Select(t => new Task
                {
                    TaskID = t.TaskID,
                    Title = t.Title,
                    Description = t.Description,
                    AssignedTo = t.AssignedTo,
                    Status = t.Status,
                    Priority = t.Priority,
                    DueDate = t.DueDate
                }).ToList();
            }

            return project;
        }
    }
}
