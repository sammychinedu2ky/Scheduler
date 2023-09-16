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
    public class JobController : ControllerBase
    {
        private readonly JobRepository _jobRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuthorizationService _authorizationService;
        private readonly MailingService _mailingService;
        private readonly NotificationRepository _notificationRepository;

        public JobController(NotificationRepository notificationRepository, MailingService mailingService, JobRepository jobRepository, UserManager<ApplicationUser> userManager, IAuthorizationService authorizationService)
        {
            _jobRepository = jobRepository;
            _userManager = userManager;
            _authorizationService = authorizationService;
            _mailingService = mailingService;
            _notificationRepository = notificationRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllJobs()
        {
            var jobs = User.IsInRole("Admin") ? await _jobRepository.GetAllAsync() : await _jobRepository.GetJobsByUserIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            return Ok(jobs.ToDtoList());
        }

        [HttpGet("{jobId}")]
        public async Task<IActionResult> GetJob(string jobId)
        {
            var job = await _jobRepository.GetByIdAsync(jobId);
            if (job == null)
            {
                return NotFound();
            }

            var requirement = new AccessRequirement(job.UserId);
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, job, requirement);
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }

            return Ok(job.ToDto());
        }

        [HttpGet("filter")]
        public async Task<IActionResult> GetJobsByStatusOrPriority([FromQuery] JobStatus? status, [FromQuery] Priority? priority)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var jobs = User.IsInRole("Admin") ? await _jobRepository.GetJobsByStatusOrPriorityAsync(status, priority) : await _jobRepository.GetUserJobsByStatusOrPriorityAsync(userId, status, priority);
            return Ok(jobs.ToDtoList());
        }

        [HttpGet("due-this-week")]
        [Authorize]
        public async Task<IActionResult> GetJobsDueThisWeek()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var jobs = User.IsInRole("Admin") ? await _jobRepository.GetJobsDueThisWeekAsync() : await _jobRepository.GetUserJobsDueThisWeekAsync(userId);
            return Ok(jobs.ToDtoList());
        }

        [HttpPost]
        public async Task<IActionResult> CreateJob(JobDTO jobDTO)
        {
            var job = jobDTO.ToModel();
            if (User.IsInRole("Admin") || job.UserId == User.Identity.Name)
            {
                await _jobRepository.AddAsync(job);
                var notification = new Notification
                {
                    NotificationId = Guid.NewGuid().ToString(),
                    UserId = job.UserId,
                    Message = "New Task Created",
                    Timestamp = DateTime.Now
                };
                await _notificationRepository.AddAsync(notification);
                await _mailingService.SendEmailAsync(notification);

                return CreatedAtAction(nameof(GetJob), new { jobId = job.JobId }, jobDTO);
            }

            return Forbid();
        }

        [HttpPut("{jobId}")]
        public async Task<IActionResult> UpdateJob(string jobId, JobDTO jobDTO)
        {
            var job = jobDTO.ToModel();
            if (jobId != job.JobId)
            {
                return BadRequest();
            }

            var existingJob = await _jobRepository.GetByIdAsync(jobId);
            if (existingJob == null)
            {
                return NotFound();
            }

            var requirement = new AccessRequirement(job.UserId);
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, existingJob, requirement);
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }

            if (User.IsInRole("Admin") || job.UserId == User.Identity.Name)
            {
                await _jobRepository.UpdateAsync(job);
                return NoContent();
            }

            return Forbid();
        }

        [HttpPut("{jobId}/assign-to-project/{projectId}")]
        public async Task<IActionResult> AssignJobToProject(string jobId, string projectId)
        {
            var job = await _jobRepository.GetByIdAsync(jobId);
            if (job == null)
            {
                return NotFound("Job not found.");
            }

            var requirement = new AccessRequirement(job.UserId);
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, job, requirement);
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }

            // Assign the job to the project
            job.ProjectId = projectId;
            await _jobRepository.UpdateAsync(job);

            return Ok("Job assigned to project.");
        }

        [HttpPut("{jobId}/remove-from-project")]
        [Authorize]
        public async Task<IActionResult> RemoveJobFromProject(string jobId)
        {
            var job = await _jobRepository.GetByIdAsync(jobId);
            if (job == null)
            {
                return NotFound("Job not found.");
            }

            var requirement = new AccessRequirement(job.UserId);
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, job, requirement);
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }


            job.ProjectId = null;
            await _jobRepository.UpdateAsync(job);

            return Ok("Job removed from project.");
        }


        [HttpDelete("{jobId}")]
        public async Task<IActionResult> DeleteJob(string jobId)
        {
            var job = await _jobRepository.GetByIdAsync(jobId);
            if (job == null)
            {
                return NotFound();
            }

            var requirement = new AccessRequirement(job.UserId);
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, job, requirement);
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }

            if (User.IsInRole("Admin") || job.UserId == User.Identity.Name)
            {
                await _jobRepository.DeleteAsync(jobId);
                return NoContent();
            }

            return Forbid();
        }
    }
}
