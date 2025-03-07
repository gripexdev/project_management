namespace ProjectDashboard.Models
{
    public class ProjectEmployee
    {
        public int ProjectId { get; set; }

        public required Project Project { get; set; }

        public int EmployeeId { get; set; }

        public required Employee Employee { get; set; }

        public string? RoleInProject { get; set; } //"Manager", "Developer"

        public string? Task {get; set; } = string.Empty;

        public DateTime JoinedDate { get; set; } = DateTime.UtcNow;

        public DateTime? EndDate { get; set; }


    }
}
