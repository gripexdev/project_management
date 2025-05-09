namespace ProjectDashboard.Models
{
    public class ProjectEmployee
    {
        public int ProjectId { get; set; }

        public Project Project { get; set; } // removed 'required' keyword

        public int EmployeeId { get; set; }

        public Employee Employee { get; set; } // removed 'required' keyword

        public string? RoleInProject { get; set; } //"Manager", "Developer"

        //public string? Task {get; set; } = string.Empty;

        public DateTime JoinedDate { get; set; } = DateTime.UtcNow;

        //public DateTime? EndDate { get; set; }

    }
}
