using System.ComponentModel.DataAnnotations;

namespace ProjectDashboard.Models
{
    public class Employee
    {

        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(50, ErrorMessage = "Name is too long")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "CIN is required")]
        [StringLength(10, ErrorMessage = "CIN must be 10 characters")]
        public string Cin { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = string.Empty;

        public byte[]? Img { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
        public string Password { get; set; } = string.Empty;

        public string Role { get; set; } = "User"; //normal user by default

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<ProjectEmployee> ProjectEmployees { get; set; } = new List<ProjectEmployee>();
    }
}
