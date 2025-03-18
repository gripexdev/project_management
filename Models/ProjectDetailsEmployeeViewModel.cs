using System.ComponentModel.DataAnnotations;

namespace ProjectDashboard.Models
{
    public class ProjectDetailsEmployeeViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? RoleInProject { get; set; }
        public DateTime JoinedDate { get; set; }
        public List<Task> Tasks { get; set; } = new List<Task>();
    }
}