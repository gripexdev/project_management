using System.Collections;

namespace ProjectDashboard.Models
{
    public class UserHomeViewModel
    {
        public Employee? employee { get; set; }
        public int? totalProjectsAssigned { get; set; }
        public int? totalTasksAssigned { get; set; }
        public int completedTasksCount { get; set; }
        public int pendingTasksCount { get; set; }
        public int overdueTasksCount { get; set; }
        public int activeProjectsCount { get; set; }
        public int completedProjectsCount { get; set; }
        public List<Project>? projects { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public List<object>? calendarEvents { get; set; } // For FullCalendar events
        public List<Task>? tasks { get; set; }
    }
}