using Microsoft.AspNetCore.Identity;
using SchedulerAPI.DTOs;
using System.Text.Json.Serialization;

namespace SchedulerAPI.Data.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string UserId => base.Id;
        public ICollection<Project> Projects { get; set; }
        public ICollection<Job> Jobs { get; set; }
        public ICollection<Notification> Notifications { get; set; }
    }
    public class Job
    {
        public string JobId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public Priority Priority { get; set; }
        public JobStatus Status { get; set; }
        public Project Project { get; set; }
        public string UserId { get; set; }
        public string ProjectId { get; set; }
        public ApplicationUser User { get; set; }
    }

    public enum Priority
    {
        Low,
        Medium,
        High
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum JobStatus
    {
        Pending,
        InProgress,
        Completed
    }

    public class Project
    {
        public string ProjectId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual ICollection<Job> Jobs { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }


    public class Notification
    {
        public string NotificationId { get; set; }
        public NotificationType Type { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
        public string UserId { get; set; }
        public bool IsRead { get; set; }
        public ApplicationUser User { get; set; }

    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum NotificationType
    {
        DueDateReminder,
        StatusUpdate
    }

    public static class ModelExtensions
    {
        public static ApplicationUserDTO ToDto(this ApplicationUser user)
        {
            return new ApplicationUserDTO
            {
                UserId = user.Id,
            };
        }

        public static JobDTO ToDto(this Job job)
        {
            return new JobDTO
            {
                JobId = job.JobId,
                Title = job.Title,
                Description = job.Description,
                DueDate = job.DueDate,
                Priority = job.Priority,
                Status = job.Status,
                ProjectId = job.ProjectId,
                UserId = job.UserId,
            };
        }

        public static ProjectDTO ToDto(this Project project)
        {
            return new ProjectDTO
            {
                ProjectId = project.ProjectId,
                Name = project.Name,
                Description = project.Description,
                UserId = project.UserId,
            };
        }

        public static NotificationDTO ToDto(this Notification notification)
        {
            return new NotificationDTO
            {
                NotificationId = notification.NotificationId,
                Type = notification.Type,
                Message = notification.Message,
                Timestamp = notification.Timestamp,
                IsRead = notification.IsRead,
                UserId = notification.UserId,
            };
        }

        public static ICollection<ApplicationUserDTO> ToDtoList(this ICollection<ApplicationUser> users)
        {
            return users.Select(user => user.ToDto()).ToList();
        }

        public static ICollection<JobDTO> ToDtoList(this ICollection<Job> jobs)
        {
            return jobs.Select(job => job.ToDto()).ToList();
        }

        public static ICollection<ProjectDTO> ToDtoList(this ICollection<Project> projects)
        {
            return projects.Select(project => project.ToDto()).ToList();
        }

        public static ICollection<NotificationDTO> ToDtoList(this ICollection<Notification> notifications)
        {
            return notifications.Select(notification => notification.ToDto()).ToList();
        }

    }
}
