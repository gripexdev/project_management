using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectDashboard.Data;
using ProjectDashboard.Models;
using ProjectDashboard.Services;

namespace ProjectDashboard.Controllers
{
    [Authorize(Roles = "User")]
    public class UserHomeController : Controller
    {

        private readonly UserManager<IdentityUser> _userManager;
        private readonly AppDbContext _context;
        private readonly NotificationService _notificationService;

        public UserHomeController(UserManager<IdentityUser> userManager, AppDbContext context, NotificationService notificationService)
        {
            _userManager = userManager;
            _context = context;
            _notificationService = notificationService;
        }

        public async Task<IActionResult> Index()
        {
            UserHomeViewModel userHomeViewModel = new UserHomeViewModel();
            // Get the currently authenticated user
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user == null)
            {
                // Handle the case where the user is not authenticated
                TempData["Error"] = "You must be logged in to view this page.";
                return RedirectToAction("Login", "Account");
            }

            // Get the email of the authenticated user
            var userEmail = user.Email;

            // Find the employee with the same email
            var employee = await _context.Employees
                .Include(e => e.Tasks) // Include tasks assigned to the employee
                .Include(e => e.ProjectEmployees).ThenInclude(pe => pe.Project) // Include projects assigned to the employee
                .FirstOrDefaultAsync(e => e.Email == userEmail);
            if (employee == null)
            {
                // Handle the case where no employee is found with the user's email
                TempData["Error"] = "No employee record found for the current user.";
                return View();
            }
            else
            {
                userHomeViewModel.employee = employee;
                userHomeViewModel.totalProjectsAssigned = employee.ProjectEmployees.Count();
                userHomeViewModel.totalTasksAssigned = employee.Tasks.ToList().Count();
            }

            // Prepare events for the calendar
            var events = new List<object>();

            // Add task deadlines
            foreach (var task in employee.Tasks)
            {
                if (task.EndDate.HasValue)
                {
                    events.Add(new
                    {
                        title = $"Task: {task.TaskName}",
                        start = task.EndDate.Value.ToString("yyyy-MM-dd"),
                        color = "#FF5733" // Red for tasks
                    });
                }
            }

            // Add project end dates
            foreach (var projectEmployee in employee.ProjectEmployees)
            {
                if (projectEmployee.Project.EndDate != default(DateTime))
                {
                    events.Add(new
                    {
                        title = $"Project: {projectEmployee.Project.Name}",
                        start = projectEmployee.Project.EndDate.ToString("yyyy-MM-dd"),
                        color = "#3498DB" // Blue for projects
                    });
                }
            }

            // Pass data to the view
            userHomeViewModel.employee = employee;
            userHomeViewModel.calendarEvents = events;


            // pass data for stats
            userHomeViewModel.completedTasksCount = employee.Tasks.Count(t => t.Status == Models.TaskStatus.Completed);
            userHomeViewModel.pendingTasksCount = employee.Tasks.Count(t => t.Status == Models.TaskStatus.InProgress);
            userHomeViewModel.overdueTasksCount = employee.Tasks.Count(t => t.EndDate < DateTime.UtcNow && t.Status != Models.TaskStatus.Completed);
            userHomeViewModel.activeProjectsCount = employee.ProjectEmployees.Count(pe => pe.Project.Status == ProjectStatus.InProgress);
            userHomeViewModel.completedProjectsCount = employee.ProjectEmployees.Count(pe => pe.Project.Status == ProjectStatus.Completed);
            userHomeViewModel.inProgressProjectsCount = employee.ProjectEmployees.Count(pe => pe.Project.Status == ProjectStatus.InProgress);

            // Kanban data
            userHomeViewModel.tasks = employee.Tasks.ToList();

            // Pass the employee data to the view
            return View(userHomeViewModel);
        }


        [HttpGet]
        public async Task<IActionResult> Projects(int page = 1, int pageSize = 10)
        {
            UserHomeViewModel userHomeViewModel = new UserHomeViewModel();

            // Get the currently authenticated user
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user == null)
            {
                // Handle the case where the user is not authenticated
                TempData["Error"] = "You must be logged in to view this page.";
                return RedirectToAction("Login", "Account");
            }

            // Find the employee with the same email
            var employee = await _context.Employees
                .Include(e => e.ProjectEmployees) // Explicitly include the ProjectEmployees collection
                .FirstOrDefaultAsync(e => e.Email == user.Email);
            if (employee == null)
            {
                // Handle the case where no employee is found with the user's email
                TempData["Error"] = "No employee record found for the current user.";
                return RedirectToAction("Index", "UserHome");
            }
            // Query ProjectEmployees directly for the authenticated employee
            var projectEmployees = await _context.ProjectEmployees
                .Include(pe => pe.Project) // Include the related Project
                .Include(task => task.Project.Tasks)
                .Where(pe => pe.EmployeeId == employee.Id) // Filter by EmployeeId
                .ToListAsync();

            // Assign data to the ViewModel
            userHomeViewModel.employee = employee;
            userHomeViewModel.totalProjectsAssigned = projectEmployees.Count;

            // Extract the Project objects from the ProjectEmployees
            userHomeViewModel.projects = projectEmployees
                .Where(pe => pe.Project != null) // Ensure Project is not null
                .Select(pe => pe.Project)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // assign the current page, total pages and page size to the view model to implement pagination
            userHomeViewModel.CurrentPage = page;
            userHomeViewModel.TotalPages = (int)Math.Ceiling(projectEmployees.Count / (double)pageSize);
            userHomeViewModel.PageSize = pageSize;

            return View(userHomeViewModel);

        }


        [HttpPost]
        public async Task<IActionResult> updateTaskStatus([FromBody] UpdateTaskStatusRequest request)
        {
            if (request == null || request.TaskId <= 0 || string.IsNullOrWhiteSpace(request.Status))
            {
                return BadRequest("Invalid request data");
            }

            var task = await _context.Tasks.FindAsync(request.TaskId);
            if (task == null)
            {
                return NotFound("Task not found");
            }

            // Parse the status string into the TaskStatus enum
            if (Enum.TryParse(typeof(Models.TaskStatus), request.Status, true, out var parsedStatus) && parsedStatus != null)
            {
                task.Status = (Models.TaskStatus)parsedStatus;
                _context.Tasks.Update(task);
                await _context.SaveChangesAsync();
                return Ok(task);
            }

            return BadRequest("Invalid status value");
        }

        // Get unread notifications for the authenticated user
        [HttpGet]
        public async Task<IActionResult> GetNotifications()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Email == user.Email);

            if (employee == null)
            {
                return Json(new List<object>());
            }

            var notifications = await _notificationService.GetUnreadNotificationsAsync(employee.Id);

            return Json(notifications.Select(n => new
            {
                id = n.Id,
                message = n.Message,
                createdAt = n.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss") // Format date for frontend
            }));
        }

        // Mark a notification as read
        [HttpPost]
        public async Task<IActionResult> MarkNotificationAsRead(int id)
        {
            await _notificationService.MarkAsReadAsync(id);
            return Ok();
        }

        // Helper class for deserializing the request body
        public class UpdateTaskStatusRequest
        {
            public int TaskId { get; set; }
            public string Status { get; set; }
        }

    }
}