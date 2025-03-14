using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectDashboard.Models;

namespace ProjectDashboard.Controllers;

[Authorize(Roles = "Admin")]
public class DashboardsController : Controller
{
  public IActionResult Index() => View();
}
