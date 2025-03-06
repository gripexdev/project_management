using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ProjectDashboard.Models;

namespace ProjectDashboard.Controllers;

public class DashboardsController : Controller
{
  public IActionResult Index() => View();
}
