using System.ComponentModel.DataAnnotations;

namespace ProjectDashboard.Models
{
    public class Project
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime EndDate { get; set; }

        public ProjectStatus Status { get; set; } = ProjectStatus.Pending;
    }

    public enum ProjectStatus
    {
        Pending,
        InProgress,
        Completed
    }
}
