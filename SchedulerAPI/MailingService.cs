using Microsoft.AspNetCore.Identity;
using SchedulerAPI.Data.Models;

namespace SchedulerAPI
{
    public class MailingService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public MailingService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        async public Task SendEmailAsync(Notification notification)
        {
            // simulate sending mail gotten from notification
            // obtain users detail from notification
            var user = await _userManager.FindByIdAsync(notification.UserId);
            // send mail to user
            Console.WriteLine($"Sending mail to {user.Email}");

        }
    }
}
