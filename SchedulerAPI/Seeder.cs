using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SchedulerAPI.Data;
using SchedulerAPI.Data.Models;
namespace SchedulerAPI.Seeder
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using var context = new ApplicationDbContext(serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            context.Database.EnsureCreated();
            if (!context.Users.Any())
            {
                SeedRoles(roleManager).Wait();

                SeedUsers(userManager).Wait();

                SeedProjects(context);
                SeedJobs(context);
                SeedNotifications(context);


            }
        }

        private static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }
        }

        private static async Task SeedUsers(UserManager<ApplicationUser> userManager)
        {
            if (!userManager.Users.Any())
            {
                var adminUser = new ApplicationUser
                {
                    UserName = "admin@example.com",
                    Email = "admin@example.com",
                    // Add other properties as needed
                };

                await userManager.CreateAsync(adminUser, "AdminPassword123"); 

                // Assign the "Admin" role to one user
                await userManager.AddToRoleAsync(adminUser, "Admin");

                // Create 4 regular users
                for (int i = 1; i <= 4; i++)
                {
                    var user = new ApplicationUser
                    {
                        UserName = $"user{i}@example.com",
                        Email = $"user{i}@example.com",
                    };

                    await userManager.CreateAsync(user, "UserPassword123"); 
                }
            }
        }

        private static void SeedProjects(ApplicationDbContext context)
        {
            var userIds = context.Users.Select(u => u.Id).ToList();

            for (int i = 1; i <= 5; i++)
            {
                var project = new Project
                {
                    ProjectId = Guid.NewGuid().ToString(),
                    Name = $"Project {i}",
                    Description = $"Description for Project {i}",
                    UserId = userIds[i % userIds.Count], 
                };

                context.Projects.Add(project);

            }
            context.SaveChanges();
        }

        private static void SeedJobs(ApplicationDbContext context)
        {
            var userIds = context.Users.Select(u => u.Id).ToList();
            var projectIds = context.Projects.Select(p => p.ProjectId).ToList();

            for (int i = 1; i <= 5; i++)
            {
                var job = new Job
                {
                    JobId = Guid.NewGuid().ToString(),
                    Title = $"Job {i}",
                    Description = $"Description for Job {i}",
                    DueDate = DateTime.Now.AddDays(i),
                    Priority = Priority.Medium,
                    Status = JobStatus.Pending,
                    ProjectId = projectIds[i % projectIds.Count], 
                    UserId = userIds[i % userIds.Count], 
                };

                context.Jobs.Add(job);
            }
            context.SaveChanges();
        }

        private static void SeedNotifications(ApplicationDbContext context)
        {
            var userIds = context.Users.Select(u => u.Id).ToList();

            for (int i = 1; i <= 5; i++)
            {
                var notification = new Notification
                {
                    NotificationId = Guid.NewGuid().ToString(),
                    Type = NotificationType.DueDateReminder,
                    Message = $"Reminder {i}",
                    Timestamp = DateTime.Now.AddMinutes(i),
                    UserId = userIds[i % userIds.Count], 
                    IsRead = false,
                };

                context.Notifications.Add(notification);
            }
            context.SaveChanges();
        }
    }
}
