using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjectDashboard.Data;
using ProjectDashboard.Models;

namespace ProjectDashboard.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public EmployeeController(AppDbContext context, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public IActionResult Index(int page = 1, int pageSize = 10)
        {
            var totalEmployees = _context.Employees.Count();
            var totalPages = (int)Math.Ceiling(totalEmployees / (double)pageSize);

            var employees = _context.Employees
                .OrderBy(e => e.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var model = new EmployeeIndexViewModel
            {
                Employees = employees,
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize
            };

            return View(model);
        }


        [HttpPost]
        public IActionResult Add(Employee employee)
        {
            if (ModelState.IsValid)
            {

                // Create IdentityUser
                var user = new IdentityUser { UserName = employee.Email, Email = employee.Email };
                var result = _userManager.CreateAsync(user, employee.Password);

                if (result.Succeeded)
                {
                    // Hash the password before saving to the Employee table
                    var hashedPassword = _userManager.PasswordHasher.HashPassword(user, employee.Password);

                    // Save the employee to the database
                    employee.Password = hashedPassword; // Store the hashed password
                    _context.Employees.Add(employee);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Index", new { success = true, message = "Employee added successfully!" });
                }

                _context.Employees.Add(employee);
                _context.SaveChanges();
                return Json(new { success = true, message = "Employee added successfully!" });
            }

            var errors = ModelState.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
            );

            return Json(new { success = false, errors });
        }


    }
}