using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectDashboard.Data;
using ProjectDashboard.Models;

namespace ProjectDashboard.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {

        private readonly AppDbContext _context;

        public EmployeeController(AppDbContext context)
        {
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




            return View();
        }

        [HttpPost]
        public IActionResult Add(Employee employee)
        {
            if (ModelState.IsValid)
            {
                _context.Employees.Add(employee);
                _context.SaveChanges();
                return Json(new { success = true, message = "Employee added successfully!" });
            }

            var errors = ModelState.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
            );

            return Json(new { success = false, message = "Failed to add employee. Please check the fields.", errors });
        }
    }
}
