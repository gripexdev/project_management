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

        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly AppDbContext _context;

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

            //fetch
            var employees = _context.Employees
                  .OrderBy(e => e.Id)
                  .Skip((page - 1) * pageSize)
                  .Take(pageSize)
                  .ToList();

            //pass data to view
            ViewBag.totalPages = totalPages;
            ViewBag.currentPage = page;
            ViewBag.pageSize = pageSize;

            return View(employees);
        }

        [HttpPost]
        public async Task<IActionResult> Add(Employee employee)
        {
            if (ModelState.IsValid)
            {
                // Create IdentityUser
                var user = new IdentityUser { UserName = employee.Email, Email = employee.Email };
                var result = await _userManager.CreateAsync(user, employee.Password);

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

                // Add Identity errors to ModelState
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return RedirectToAction("Index", new { success = false, message = "Failed to add employee. Please check the fields.", errors });
        }

        [HttpPost]
        public async Task<IActionResult> Update(Employee employee, int id)
        {
            if (ModelState.IsValid)
            {
                var existingEmployee = await _context.Employees.FindAsync(id);
                if (existingEmployee == null)
                {
                    return RedirectToAction("Index", new { success = false, message = "Employee not found!" });
                }

                // Update IdentityUser if email or password changes
                var user = await _userManager.FindByEmailAsync(existingEmployee.Email);
                if (user != null)
                {
                    if (existingEmployee.Email != employee.Email)
                    {
                        user.UserName = employee.Email;
                        user.Email = employee.Email;
                        await _userManager.UpdateAsync(user);
                    }

                    if (existingEmployee.Password != employee.Password)
                    {
                        var hashedPassword = _userManager.PasswordHasher.HashPassword(user, employee.Password);
                        existingEmployee.Password = hashedPassword; // Update hashed password
                        await _userManager.RemovePasswordAsync(user);
                        await _userManager.AddPasswordAsync(user, employee.Password);
                    }
                }

                // Update employee details
                existingEmployee.Name = employee.Name;
                existingEmployee.Cin = employee.Cin;
                existingEmployee.Email = employee.Email;
                existingEmployee.Role = employee.Role;

                _context.Employees.Update(existingEmployee);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", new { success = true, message = "Employee updated successfully!" });
            }

            // If something failed
            return RedirectToAction("Index", new { success = false, message = "Failed to update employee. Please check the fields." });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return RedirectToAction("Index", new { success = false, message = "Employee not found!" });
            }

            // Delete IdentityUser
            var user = await _userManager.FindByEmailAsync(employee.Email);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }

            // Delete the employee
            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", new { success = true, message = "Employee deleted successfully!" });
        }
    }
}
