using SchedulerAPI;
using SchedulerAPI.Data.Models;
using SchedulerAPI.Repository;

public class NotificationService : IHostedService
{
    private Timer _timer;
    private JobRepository _jobRepository;
    private NotificationRepository _notificationRepository;
    private ProjectRepository _projectRepository;
    private MailingService _mailingService;

    public NotificationService(JobRepository jobRepository, NotificationRepository notificationRepository, ProjectRepository projectRepository, MailingService mailingService)
    {
        _jobRepository = jobRepository;
        _notificationRepository = notificationRepository;
        _projectRepository = projectRepository;
        _mailingService = mailingService;

    }
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
        return Task.CompletedTask;

    }

    async public void DoWork(object? state)
    {
        var jobsDueIn48HoursOrMarkedAsCompleted = await _jobRepository.GetJobsDueIn48HoursOrMarkedAsCompletedAsync();

        // send notification to user
        foreach (var job in jobsDueIn48HoursOrMarkedAsCompleted)
        {
            var message = job.Status == JobStatus.Completed ? $"Job {job.JobId} has been marked as completed" : $"Job {job.JobId} is due in 48 hours";
            if (job.Status == JobStatus.Completed && job.DueDate <= DateTime.Now.AddDays(2))
            {
                message = $"Job {job.JobId} is due in 48 hours and has been marked as completed";
            }
            var notification = new Notification
            {
                NotificationId = Guid.NewGuid().ToString(),
                UserId = job.UserId,
                Message = message,
                Timestamp = DateTime.Now
            };
            await _notificationRepository.AddAsync(notification);
            await _mailingService.SendEmailAsync(notification);
        }

    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Stopping");
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }
}