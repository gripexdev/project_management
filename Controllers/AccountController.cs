using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjectDashboard.Data;
using ProjectDashboard.Models;

public class AccountController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;

    private readonly AppDbContext _context;

    public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, AppDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(Employee model)
    {
        if (ModelState.IsValid)
        {
            // Create a new IdentityUser (or Employee if using custom user class)
            var user = new IdentityUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                //assing default rolee User to the new use
                await _userManager.AddToRoleAsync(user, "User");

                // Optionally, add additional fields to the Employee table
                var employee = new Employee
                {
                    Name = model.Name,
                    Cin = model.Cin,
                    Email = model.Email,
                    Password = model.Password,
                    Role = "User", // Default role
                    CreatedAt = DateTime.UtcNow
                };

                // Save the employee to the database (if using a separate Employee table)
                _context.Employees.Add(employee);
                _context.SaveChanges();

                // Sign in the user
                await _signInManager.SignInAsync(user, isPersistent: false);
                // return RedirectToAction("Index", "Home");
                return RedirectToAction("Index", "UserHome");
            }

            // Add errors to ModelState
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        return View(model);
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginView model)
    {
        if (ModelState.IsValid)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                //check if user is admin or user
                var user = await _userManager.FindByEmailAsync(model.Email);
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains("Admin"))
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return RedirectToAction("Index", "UserHome");
                }
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        }

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public async Task<IActionResult> AccessDenied()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login", "Account");
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Profile()
    {
        //get user
        var user = await _userManager.GetUserAsync(HttpContext.User);

        //get employee
        var employee = _context.Employees.Where(e => e.Email == user.Email).FirstOrDefault();

        //check roles
        var roles = await _userManager.GetRolesAsync(user);
        if (roles.Contains("Admin"))
        {
            ViewData["Role"] = "Admin";
            return View();
        }

        ViewData["Role"] = "User";
        return View(employee);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Profile(Employee model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                // Get user
                var user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    TempData["Error"] = "User not found!";
                    return View(model);
                }

                // Get employee
                var employee = _context.Employees.FirstOrDefault(e => e.Email == user.Email);
                if (employee == null)
                {
                    TempData["Error"] = "Employee not found!";
                    return View(model);
                }

                // Validate email uniqueness
                if (!string.Equals(employee.Email, model.Email, StringComparison.OrdinalIgnoreCase))
                {
                    var emailExists = await _userManager.FindByEmailAsync(model.Email);
                    if (emailExists != null && emailExists.Id != user.Id)
                    {
                        ModelState.AddModelError("Email", "This email is already in use.");
                        return View(model);
                    }
                }

                // Update employee
                employee.Name = model.Name;
                employee.Cin = model.Cin;
                employee.Email = model.Email;

                // Update user
                var setEmailResult = await _userManager.SetEmailAsync(user, model.Email);
                var setUserNameResult = await _userManager.SetUserNameAsync(user, model.Email);

                if (!setEmailResult.Succeeded || !setUserNameResult.Succeeded)
                {
                    TempData["Error"] = "An error occurred while updating your account!";
                    return View(model);
                }

                // Save changes
                _context.Employees.Update(employee);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Account Updated Successfully!";
                return View(employee);
            }
            catch (Exception e)
            {
                TempData["Error"] = "An error occurred while updating your account!";
                return View(model);
            }
        }

        return View(model);
    }
}
