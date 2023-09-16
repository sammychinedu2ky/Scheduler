using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SchedulerAPI.Data.Models;
using SchedulerAPI.DTOs;
using SchedulerAPI.Repository;
using System.Security.Claims;
namespace SchedulerAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly NotificationRepository _notificationRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuthorizationService _authorizationService;

        public NotificationController(NotificationRepository notificationRepository, UserManager<ApplicationUser> userManager, IAuthorizationService authorizationService)
        {
            _notificationRepository = notificationRepository;
            _userManager = userManager;
            _authorizationService = authorizationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllNotifications()
        {
            var notifications = User.IsInRole("Admin") ? await _notificationRepository.GetAllAsync() : await _notificationRepository.GetNotificationsByUserIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            return Ok(notifications.ToDtoList());
        }

        [HttpGet("{notificationId}")]
        public async Task<IActionResult> GetNotification(string notificationId)
        {
            var notification = await _notificationRepository.GetByIdAsync(notificationId);
            if (notification == null)
            {
                return NotFound();
            }

            var requirement = new AccessRequirement(notification.UserId);
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, notification, requirement);
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }

            return Ok(notification.ToDto());
        }

        [HttpPost]
        public async Task<IActionResult> CreateNotification(NotificationDTO notificationDTO)
        {
            var notification = notificationDTO.ToModel();
            if (User.IsInRole("Admin") || notification.UserId == User.Identity.Name)
            {
                await _notificationRepository.AddAsync(notification);
                return CreatedAtAction(nameof(GetNotification), new { notificationId = notification.NotificationId }, notificationDTO);
            }

            return Forbid();
        }

        [HttpPut("{notificationId}")]
        public async Task<IActionResult> UpdateNotification(string notificationId, NotificationDTO notificationDTO)
        {
            var notification = notificationDTO.ToModel();
            if (notificationId != notification.NotificationId)
            {
                return BadRequest();
            }

            var existingNotification = await _notificationRepository.GetByIdAsync(notificationId);
            if (existingNotification == null)
            {
                return NotFound();
            }

            var requirement = new AccessRequirement(existingNotification.UserId);
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, existingNotification, requirement);
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }

            if (User.IsInRole("Admin") || notification.UserId == User.Identity.Name)
            {
                await _notificationRepository.UpdateAsync(notification);
                return NoContent();
            }

            return Forbid();
        }

        [HttpPut("{notificationId}/mark")]
        [Authorize]
        public async Task<IActionResult> MarkNotification(string notificationId, [FromQuery] bool markAsRead)
        {
            var notification = await _notificationRepository.GetByIdAsync(notificationId);
            if (notification == null)
            {
                return NotFound("Notification not found.");
            }

            // Check if the user is authorized to modify the notification

            var requirement = new AccessRequirement(notification.UserId);
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, notification, requirement);
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }

            if (markAsRead)
            {
                // Mark the notification as read
                notification.IsRead = true;
            }
            else
            {
                // Mark the notification as unread
                notification.IsRead = false;
            }

            notification.Timestamp = DateTime.UtcNow; // Update timestamp if needed
            await _notificationRepository.UpdateAsync(notification);

            return Ok("Notification marked.");
        }


        [HttpDelete("{notificationId}")]
        public async Task<IActionResult> DeleteNotification(string notificationId)
        {
            var notification = await _notificationRepository.GetByIdAsync(notificationId);
            if (notification == null)
            {
                return NotFound();
            }

            var requirement = new AccessRequirement(notification.UserId);
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, notification, requirement);
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }

            if (User.IsInRole("Admin") || notification.UserId == User.Identity.Name)
            {
                await _notificationRepository.DeleteAsync(notificationId);
                return NoContent();
            }

            return Forbid();
        }
    }
}
