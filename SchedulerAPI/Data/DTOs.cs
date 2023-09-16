using SchedulerAPI.Data.Models;

namespace SchedulerAPI.DTOs
{
    public class ApplicationUserDTO
    {
        public string UserId { get; set; }

    }

    public class JobDTO
    {
        public string JobId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public Priority Priority { get; set; }
        public JobStatus Status { get; set; }
        public string ProjectId { get; set; }
        public string UserId { get; set; }
    }

    public class ProjectDTO
    {
        public string ProjectId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string UserId { get; set; }
    }

    public class NotificationDTO
    {
        public string NotificationId { get; set; }
        public NotificationType Type { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsRead { get; set; }
        public string UserId { get; set; }
    }
    public static class DtoExtensions
    {
        public static ApplicationUser ToModel(this ApplicationUserDTO userDto)
        {
            return new ApplicationUser
            {
                Id = userDto.UserId,
            };
        }

        public static Job ToModel(this JobDTO jobDto)
        {
            return new Job
            {
                JobId = jobDto.JobId,
                Title = jobDto.Title,
                Description = jobDto.Description,
                DueDate = jobDto.DueDate,
                Priority = jobDto.Priority,
                Status = jobDto.Status,
                ProjectId = jobDto.ProjectId,
                UserId = jobDto.UserId,
            };
        }

        public static Project ToModel(this ProjectDTO projectDto)
        {
            return new Project
            {
                ProjectId = projectDto.ProjectId,
                Name = projectDto.Name,
                Description = projectDto.Description,
                UserId = projectDto.UserId,
            };
        }

        public static Notification ToModel(this NotificationDTO notificationDto)
        {
            return new Notification
            {
                NotificationId = notificationDto.NotificationId,
                Type = notificationDto.Type,
                Message = notificationDto.Message,
                Timestamp = notificationDto.Timestamp,
                IsRead = notificationDto.IsRead,
                UserId = notificationDto.UserId,
            };
        }

    }

}
