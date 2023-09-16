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

    public class ProjectController : ControllerBase
    {
        private readonly ProjectRepository _projectRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuthorizationService _authorizationService;

        public ProjectController(ProjectRepository projectRepository, UserManager<ApplicationUser> userManager, IAuthorizationService authorizationService)
        {
            _projectRepository = projectRepository;
            _userManager = userManager;
            _authorizationService = authorizationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProjects()
        {
            var projects = User.IsInRole("Admin") ? await _projectRepository.GetAllAsync() : await _projectRepository.GetProjectsByUserIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            return Ok(projects.ToDtoList());
        }

        [HttpGet("{projectId}")]
        public async Task<IActionResult> GetProject(string projectId)
        {

            var project = await _projectRepository.GetByIdAsync(projectId);
            if (project == null)
            {
                return NotFound();
            }
            var requirement = new AccessRequirement(project.UserId);
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, project, requirement);
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }

            return Ok(project.ToDto());
        }

        [HttpPost]
        public async Task<IActionResult> CreateProject(ProjectDTO projectDTO)
        {
            var project = projectDTO.ToModel();
            if (User.IsInRole("Admin") || project.UserId == User.Identity.Name)
            {
                await _projectRepository.AddAsync(project);
                return CreatedAtAction(nameof(GetProject), new { projectId = project.ProjectId }, projectDTO);
            }

            return Forbid();
        }

        [HttpPut("{projectId}")]
        public async Task<IActionResult> UpdateProject(string projectId, ProjectDTO projectDTO)
        {
            var project = projectDTO.ToModel();
            if (projectId != project.ProjectId)
            {
                return BadRequest();
            }

            var existingProject = await _projectRepository.GetByIdAsync(projectId);
            if (existingProject == null)
            {
                return NotFound();
            }

            var requirement = new AccessRequirement(project.UserId);
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, existingProject, requirement);
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }

            if (User.IsInRole("Admin") || project.UserId == User.Identity.Name)
            {
                await _projectRepository.UpdateAsync(project);
                return NoContent();
            }

            return Forbid();
        }

        [HttpDelete("{projectId}")]
        public async Task<IActionResult> DeleteProject(string projectId)
        {
            var project = await _projectRepository.GetByIdAsync(projectId);
            if (project == null)
            {
                return NotFound();
            }

            var requirement = new AccessRequirement(project.UserId);
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, project, requirement);
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }

            if (User.IsInRole("Admin") || project.UserId == User.Identity.Name)
            {
                await _projectRepository.DeleteAsync(projectId);
                return NoContent();
            }

            return Forbid();
        }
    }
}
