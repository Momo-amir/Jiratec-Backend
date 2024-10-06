using System;
using System.Collections.Generic;
using System.Linq;
using DAL.Enums;

namespace DAL.Models
{
    public class ProjectDTO
    {
        public int ProjectID { get; set; } // For new projects, this can be omitted

        public string? Title { get; set; } // Required but set nullable
        public string? Description { get; set; } // Required but set nullable

        public UserDTO? CreatedBy { get; set; } // Optional, populated by backend
        public string? CreatedByName { get; set; } // Optional, populated by backend
        public DateTime? CreatedDate { get; set; } // Optional, populated by backend

        public List<TaskDTO>? Tasks { get; set; } // Optional
        public List<UserDTO>? Users { get; set; } // Optional

        // Mapping method
        public ProjectDTO? MapProjectToDto(Project? project)
        {
            if (project == null) return null;

            return new ProjectDTO // Mapping Project to ProjectDTO - This is done to avoid circular references when serializing the object - e.g. Project -> User -> Project -> User -> etc.
            {
                ProjectID = project.ProjectID,
                Title = project.Title,
                Description = project.Description,
                CreatedBy = project.CreatedBy != null
                    ? new UserDTO
                    {
                        UserID = project.CreatedBy.UserID,
                        Name = project.CreatedBy.Name,
                        Email = project.CreatedBy.Email,
                        Role = project.CreatedBy.Role
                    }
                    : null,
                CreatedByName = project.CreatedBy?.Name,
                CreatedDate = project.CreatedDate,
                Tasks = project.Tasks?.Select(t => new TaskDTO
                {
                    TaskID = t.TaskID,
                    Title = t.Title,
                    Description = t.Description,
                    AssignedTo = t.AssignedTo,
                    Status = t.Status,
                    Priority = t.Priority,
                    DueDate = t.DueDate
                }).ToList(),
                Users = project.Users?.Select(u => new UserDTO
                {
                    UserID = u.UserID,
                    Name = u.Name,
                    Email = u.Email,
                    Role = u.Role
                }).ToList()
            };
        }
    }

    public class TaskDTO // I should have made this a file called TaskDTO.cs
    {
        public int TaskID { get; set; }
        public int ProjectID { get; set; } // Include ProjectID for easier mapping
        public string Title { get; set; }  
        public string? Description { get; set; } 
        public int AssignedTo { get; set; } 
        public string? AssignedToName { get; set; } 
        public TaskStatusEnum Status { get; set; } 
        public TaskPriorityEnum Priority { get; set; } 
        public DateTime? DueDate { get; set; } 
    }
}