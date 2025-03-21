using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectDashboard.Data;
using ProjectDashboard.Models;
using Microsoft.EntityFrameworkCore;

namespace ProjectDashboard.Controllers;

[Authorize(Roles = "Admin")]
public class DashboardsController : Controller
{
    private readonly AppDbContext _context;

    public DashboardsController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        // Fetch data from the database
        var totalProjects = await _context.Projects.CountAsync();
        var totalTasks = await _context.Tasks.CountAsync();
        var completedTasks = await _context.Tasks.CountAsync(t => t.Status == Models.TaskStatus.Completed);
        var inProgressTasks = await _context.Tasks.CountAsync(t => t.Status == Models.TaskStatus.InProgress);
        var backlogTasks = await _context.Tasks.CountAsync(t => t.Status == Models.TaskStatus.Backlog);

        var totalEmployees = await _context.Employees.CountAsync();
        var totalUnassignedEmployees = await _context.Employees
            .CountAsync(e => !_context.ProjectEmployees.Any(pe => pe.EmployeeId == e.Id));

        // Create the ViewModel
        var viewModel = new DashboardViewModel
        {
            TotalProjects = totalProjects,
            TotalTasks = totalTasks,
            CompletedTasks = completedTasks,
            InProgressTasks = inProgressTasks,
            PendingTasks = backlogTasks,
            TotalEmployees = totalEmployees,
            TotalUnassignedEmployees = totalUnassignedEmployees
        };

        return View(viewModel);
    }
}