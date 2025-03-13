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
        public async Task<IActionResult> Add(Employee employee)
        {
            if (ModelState.IsValid)
            {
                // Create IdentityUser
                var user = new IdentityUser { UserName = employee.Email, Email = employee.Email };
                var result = await _userManager.CreateAsync(user, employee.Password);

                if (!result.Succeeded)
                {
                    var errorMessages = result.Errors.Select(e => e.Description).ToList();
                    TempData["Error"] = "Failed to create user. Errors: " + string.Join(", ", errorMessages);
                    return RedirectToAction("Index");
                }

                // Assign role to the user
                // await _userManager.AddToRoleAsync(user, employee.Role);

                // Save the employee to the database
                await _context.Employees.AddAsync(employee);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Employee added successfully!";
                return RedirectToAction("Index");
            }

            // Handle model validation errors
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            TempData["Error"] = "Failed to add employee. Errors: " + string.Join(", ", errors);
            return RedirectToAction("Index");
        }


        // Delete Employee
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            TempData.Clear();
            TempData.Remove("Error");
            TempData.Remove("Success");
            try
            {
                var employee = await _context.Employees.FindAsync(id);
                if (employee == null)
                {
                    TempData["Error"] = "Employee not found.";
                    return RedirectToAction("Index");
                    // return Json(new { success = false, message = "Employee not found." });
                }

                //delete identity user
                var user = await _userManager.FindByEmailAsync(employee.Email);
                if (user != null)
                {
                    await _userManager.DeleteAsync(user);
                }


                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Employee deleted successfully!";
                return RedirectToAction("Index");
                // return Json(new { success = true, message = "Employee deleted successfully!" });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error deleting employee, " + ex.Message;
                return RedirectToAction("Index");
                // return Json(new { success = false, message = "Error deleting employee, " + ex.Message });
            }
        }


        // Update Employee
        [HttpPost]
        public async Task<IActionResult> Update(Employee employee)
        {
            TempData.Clear();
            TempData.Remove("Error");
            TempData.Remove("Success");

            if (ModelState.IsValid)
            {
                try
                {
                    var existingEmployee = await _context.Employees.FindAsync(employee.Id);
                    if (existingEmployee == null)
                    {
                        TempData["Error"] = "Employee not found.";
                        return RedirectToAction("Index");
                        // return RedirectToAction("Index", new { success = false, message = "Employee not found." });
                        // return Json(new { success = false, message = "Employee not found." });
                    }

                    // Check if the data is unchanged
                    if (existingEmployee.Name == employee.Name &&
                        existingEmployee.Cin == employee.Cin &&
                        existingEmployee.Email == employee.Email &&
                        existingEmployee.Role == employee.Role)
                    {
                        TempData["Error"] = "No changes detected. Data is the same.";
                        return RedirectToAction("Index");
                        // return RedirectToAction("Index", new { success = false, message = "No changes detected. Data is the same." });
                        // return Json(new { success = false, message = "No changes detected. Data is the same." });
                    }

                    //update identity user
                    var user = await _userManager.FindByEmailAsync(existingEmployee.Email);

                    if (user != null)
                    {
                        user.Email = employee.Email;
                        user.UserName = employee.Email;
                        await _userManager.UpdateAsync(user);
                        // Assign role to the user
                        // await _userManager.AddToRoleAsync(user, employee.Role);
                    }
                    else
                    {
                        TempData["Error"] = "User not found.";
                        return RedirectToAction("Index");
                        // return RedirectToAction("Index", new { success = false, message = "User not found." });

                    }

                    // Update the existing project with new values
                    existingEmployee.Name = employee.Name;
                    existingEmployee.Cin = employee.Cin;
                    existingEmployee.Email = employee.Email;
                    existingEmployee.Role = employee.Role;

                    await _context.SaveChangesAsync();



                    TempData["Success"] = "Employee updated successfully!";
                    return RedirectToAction("Index");
                    // return RedirectToAction("Index", new { success = true, message = "Employee updated successfully!" });

                    // return Json(new { success = true, message = "Employee updated successfully!" });
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Failed to update employee. Please check the fields.";
                    return RedirectToAction("Index");
                    // return RedirectToAction("Index", new { success = false, message = "Error updating employee." + ex.Message });

                    // return Json(new { success = false, message = "Error updating employee." + ex.Message });
                }
            }

            // Extract validation errors
            var errors = ModelState.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
            );

            TempData["Error"] = "Failed to update employee. Please check the fields.";
            return RedirectToAction("Index");
            // return RedirectToAction("Index", new { success = false, message = "Failed to update employee. Please check the fields.", errors });

            // return Json(new { success = false, message = "Failed to update employee. Please check the fields.", errors });
        }


    }
}
