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
                    Password = model.Password, // Note: Store hashed password in production
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
    public async Task<IActionResult> Profile()
    {
        var user = await _userManager.GetUserAsync(HttpContext.User);
        //check roles
        var roles = await _userManager.GetRolesAsync(user);
        if (roles.Contains("Admin"))
        {
            ViewData["Role"] = "Admin";
            return View();
        }

        ViewData["Role"] = "User";
        return View();
    }


    [HttpGet]
    public async Task<IActionResult> AccessDenied()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login", "Account");
    }
}
