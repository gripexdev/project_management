using System.Collections;

namespace ProjectDashboard.Models
{
    public class UserHomeViewModel
    {
        public Employee? employee { get; set; }
        public int? totalProjectsAssigned { get; set; }
        public int? totalTasksAssigned { get; set; }
        public List<Project>? projects { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public List<object>? calendarEvents { get; set; } // For FullCalendar events
    }
}