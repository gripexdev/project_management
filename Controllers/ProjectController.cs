using Microsoft.AspNetCore.Mvc;
using ProjectDashboard.Models;
using ProjectDashboard.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization; // Required for Skip and Take methods

namespace ProjectDashboard.Controllers
{
    [Authorize]
    public class ProjectController : Controller
    {
        private readonly ILogger<ProjectController> _logger;
        private readonly AppDbContext _context;

        // Constructor accepting AppDbContext
        public ProjectController(ILogger<ProjectController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // Index action with pagination
        public IActionResult Index(int page = 1, int pageSize = 10)
        {
            // Calculate total count of projects
            var totalProjects = _context.Projects.Count();
            var totalPages = (int)Math.Ceiling(totalProjects / (double)pageSize);

            // Fetch paginated projects
            var projects = _context.Projects
                .OrderBy(p => p.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Create a ViewModel to pass data to the view 
            var model = new ProjectIndexViewModel
            {
                Projects = projects,
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize
            };

            return View(model);
        }

        // Add Project 
        [HttpPost]
        public IActionResult Add(Project project)
        {
            if (ModelState.IsValid)
            {
                _context.Projects.Add(project);
                _context.SaveChanges();
                return Json(new { success = true, message = "Project added successfully!" });
            }

            // Extract validation errors
            var errors = ModelState.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
            );

            return Json(new { success = false, message = "Failed to add project. Please check the fields.", errors });
        }

        // Delete Project
        [HttpPost]
        public IActionResult Delete(int id)
        {
            try
            {
                var project = _context.Projects.Find(id);
                if (project == null)
                {
                    return Json(new { success = false, message = "Project not found." });
                }

                _context.Projects.Remove(project);
                _context.SaveChanges();
                return Json(new { success = true, message = "Project deleted successfully!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting project.");
                return Json(new { success = false, message = "Error deleting project." });
            }
        }

        // Get Project by ID
        [HttpGet]
        public IActionResult GetProject(int id)
        {
            var project = _context.Projects.Find(id);
            if (project == null)
            {
                return Json(new { success = false, message = "Project not found." });
            }

            // Ensure Status is serialized as a string
            var projectDto = new
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                StartDate = project.StartDate.ToString("yyyy-MM-dd"),
                EndDate = project.EndDate.ToString("yyyy-MM-dd"),
                Status = project.Status.ToString() // Serialize as string
            };

            return Json(new { success = true, project = projectDto });
        }

        // Update Project
        [HttpPost]
        public IActionResult Update(Project project)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var existingProject = _context.Projects.Find(project.Id);
                    if (existingProject == null)
                    {
                        return Json(new { success = false, message = "Project not found." });
                    }

                    // Check if the data is unchanged
                    if (existingProject.Name == project.Name &&
                        existingProject.Description == project.Description &&
                        existingProject.StartDate == project.StartDate &&
                        existingProject.EndDate == project.EndDate &&
                        existingProject.Status == project.Status)
                    {
                        return Json(new { success = false, message = "No changes detected. Data is the same." });
                    }

                    // Update the existing project with new values
                    existingProject.Name = project.Name;
                    existingProject.Description = project.Description;
                    existingProject.StartDate = project.StartDate;
                    existingProject.EndDate = project.EndDate;
                    existingProject.Status = project.Status;

                    _context.SaveChanges();
                    return Json(new { success = true, message = "Project updated successfully!" });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating project.");
                    return Json(new { success = false, message = "Error updating project." });
                }
            }

            // Extract validation errors
            var errors = ModelState.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
            );

            return Json(new { success = false, message = "Failed to update project. Please check the fields.", errors });
        }

    }
}
