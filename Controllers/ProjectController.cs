using Microsoft.AspNetCore.Mvc;
using ProjectDashboard.Models;
using ProjectDashboard.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization; // Required for Skip and Take methods

namespace ProjectDashboard.Controllers
{
    [Authorize(Roles = "Admin")]
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

        // Fetch Employees Not Assigned to the Project
        [HttpGet]
        public IActionResult GetUnassignedEmployees(int projectId)
        {
            try
            {
                // TODO: Implement toast info message if no employees are available
                // Get IDs of employees already assigned to the project
                var assignedEmployeeIds = _context.ProjectEmployees
                    .Where(pe => pe.ProjectId == projectId)
                    .Select(pe => pe.EmployeeId)
                    .ToList();

                // Fetch unassigned employees (those not in the assigned list)
                var unassignedEmployees = _context.Employees
                    .Where(e => !assignedEmployeeIds.Contains(e.Id))
                    .Select(e => new { e.Id, e.Name }) // Return only Id and Name
                    .ToList();

                return Json(new { success = true, employees = unassignedEmployees });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching unassigned employees.");
                return Json(new { success = false, message = "Error fetching unassigned employees." });
            }
        }

        // Assign Employee to Project with Role
        [HttpPost]
        public IActionResult AssignEmployeeToProject([FromBody] AssignEmployeeViewModel model)
        {
            if (model == null)
            {
                return Json(new { success = false, message = "Invalid data." });
            }

            try
            {
                // Validate input
                if (model.ProjectId <= 0 || model.EmployeeId <= 0)
                {
                    return Json(new { success = false, message = "Invalid project or employee ID." });
                }

                if (string.IsNullOrWhiteSpace(model.RoleInProject))
                {
                    return Json(new { success = false, message = "Role in project is required." });
                }

                // Check if project and employee exist
                var project = _context.Projects.Find(model.ProjectId);
                var employee = _context.Employees.Find(model.EmployeeId);

                if (project == null)
                {
                    return Json(new { success = false, message = $"Project with ID {model.ProjectId} not found." });
                }

                if (employee == null)
                {
                    return Json(new { success = false, message = $"Employee with ID {model.EmployeeId} not found." });
                }

                // Check if the employee is already assigned to the project
                var isAlreadyAssigned = _context.ProjectEmployees
                    .Any(pe => pe.ProjectId == model.ProjectId && pe.EmployeeId == model.EmployeeId);

                if (isAlreadyAssigned)
                {
                    return Json(new { success = false, message = "Employee is already assigned to this project." });
                }

                // Create the ProjectEmployee relationship
                var projectEmployee = new ProjectEmployee
                {
                    ProjectId = model.ProjectId,
                    Project = project,
                    EmployeeId = model.EmployeeId,
                    Employee = employee,
                    RoleInProject = model.RoleInProject,
                    JoinedDate = DateTime.UtcNow
                };

                _context.ProjectEmployees.Add(projectEmployee);
                _context.SaveChanges();

                return Json(new { success = true, message = "Employee assigned successfully!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning employee to project.");
                return Json(new { success = false, message = $"Error assigning employee {model.EmployeeId} to project {model.ProjectId}. {model.RoleInProject}" });
            }
        }

    }
}
