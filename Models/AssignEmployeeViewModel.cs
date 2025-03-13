using System.ComponentModel.DataAnnotations;

namespace ProjectDashboard.Models
{
    public class AssignEmployeeViewModel
    {
        public int ProjectId { get; set; }
        public int EmployeeId { get; set; }
        public string RoleInProject { get; set; }
    }
}