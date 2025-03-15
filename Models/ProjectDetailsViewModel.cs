using System.ComponentModel.DataAnnotations;

namespace ProjectDashboard.Models
{
    public class ProjectDetailsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public ProjectStatus Status { get; set; }
        public List<ProjectDetailsEmployeeViewModel> AssignedEmployees { get; set; } = new List<ProjectDetailsEmployeeViewModel>();
    }
}