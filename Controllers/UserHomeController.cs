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
            }

            // Pass the employee data to the view
            return View(userHomeViewModel);
        }


        [HttpGet]
        public async Task<IActionResult> Projects()
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
            else
            {
                userHomeViewModel.employee = employee;
                // userHomeViewModel.totalProjectsAssigned = employee.ProjectEmployees.Count();
                // userHomeViewModel.projects = employee.ProjectEmployees?.
                //                     Select(pe => pe.Project).ToList();

                return View(userHomeViewModel);
            }

        }
    }
}