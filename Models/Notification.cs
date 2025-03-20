using System.ComponentModel.DataAnnotations;

namespace ProjectDashboard.Models
{
    public class Notification
    {
        public int Id { get; set; }

        [Required]
        public int EmployeeId { get; set; } // Foreign key to Employee

        [Required]
        public string Message { get; set; }

        public bool IsRead { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public Employee Employee { get; set; }
    }
}
