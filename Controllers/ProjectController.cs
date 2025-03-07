using Microsoft.AspNetCore.Mvc;
using ProjectDashboard.Models;
using ProjectDashboard.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization; // Required for Skip and Take methods

namespace ProjectDashboard.Controllers
{
    [Authorize]
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
    }
}
