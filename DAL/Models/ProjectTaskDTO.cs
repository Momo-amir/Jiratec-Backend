using System;
using System.Collections.Generic;
using System.Linq;
using DAL.Enums;


namespace DAL.Models
{
     public class ProjectDTO
    {
        public int ProjectID { get; set; } // For new projects, this can be 0 or omitted

        public string Title { get; set; } // Required
        public string Description { get; set; } // Required

        public UserDTO? CreatedBy { get; set; } // Optional, populated by backend
        public string? CreatedByName { get; set; } // Optional, populated by backend
        public DateTime? CreatedDate { get; set; } // Optional, populated by backend

        public List<TaskDTO>? Tasks { get; set; } // Optional
        public List<UserDTO>? Users { get; set; } // Optional

        // Mapping method
        public ProjectDTO MapProjectToDto(Project project)
        {
            if (project == null) return null;

            return new ProjectDTO
            {
                ProjectID = project.ProjectID,
                Title = project.Title,
                Description = project.Description,
                CreatedBy = project.CreatedBy != null ? new UserDTO
                {
                    UserID = project.CreatedBy.UserID,
                    Name = project.CreatedBy.Name,
                    Email = project.CreatedBy.Email,
                    Role = project.CreatedBy.Role
                } : null,
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


    public class TaskDTO
    {
        public int TaskID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int AssignedTo { get; set; }
        public string AssignedToName { get; set; } // Optional: Include the assignee's name
        public TaskStatusEnum Status { get; set; }
        public TaskPriorityEnum Priority { get; set; }
        public DateTime? DueDate { get; set; }
    }
}
