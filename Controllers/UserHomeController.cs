using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectDashboard.Data;
using ProjectDashboard.Models;

namespace ProjectDashboard.Controllers
{
    [Authorize(Roles = "User")]
    public class UserHomeController : Controller
    {

        private readonly UserManager<IdentityUser> _userManager;
        private readonly AppDbContext _context;

        public UserHomeController(UserManager<IdentityUser> userManager, AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
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
                .Include(e => e.ProjectEmployees) // Explicitly include the ProjectEmployees collection
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
    }
}