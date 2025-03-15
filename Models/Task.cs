using System.ComponentModel.DataAnnotations;

namespace ProjectDashboard.Models
{
    public class Task
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Task name is required")]
        public string TaskName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Task description is required")]
        public string TaskDescription { get; set; } = string.Empty;

        public TaskStatus Status { get; set; } = TaskStatus.Backlog;

        public DateTime StartDate { get; set; } = DateTime.UtcNow;

        public DateTime? EndDate { get; set; }

        public TaskPriority Priority { get; set; } = TaskPriority.Medium;

        [Range(0, 100, ErrorMessage = "Progress must be between 0 and 100")]
        public int Progress { get; set; } = 0;

        // Foreign keys
        public int ProjectId { get; set; }
        public Project Project { get; set; } = null!;

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; } = null!;
    }

    public enum TaskStatus
    {
        Backlog,
        Todo,
        InProgress,
        Completed
    }

    public enum TaskPriority
    {
        Low,
        Medium,
        High
    }
}