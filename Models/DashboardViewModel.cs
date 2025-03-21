namespace ProjectDashboard.Models
{
    public class DashboardViewModel
    {
        public int TotalProjects { get; set; }
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int InProgressTasks { get; set; }
        public int PendingTasks { get; set; }
        public int TotalEmployees { get; set; }
        public int TotalUnassignedEmployees { get; set; }

        // Dynamic percentages (rounded to the nearest integer)
        public int CompletedTasksPercentage => TotalTasks == 0 ? 0 : (int)Math.Round((decimal)CompletedTasks / TotalTasks * 100);
        public int InProgressTasksPercentage => TotalTasks == 0 ? 0 : (int)Math.Round((decimal)InProgressTasks / TotalTasks * 100);
        public int TaskCompletionPercentage => TotalTasks == 0 ? 0 : (int)Math.Round((decimal)(CompletedTasks + InProgressTasks) / TotalTasks * 100);
        public int UnassignedEmployeesPercentage => TotalEmployees == 0 ? 0 : (int)Math.Round((decimal)TotalUnassignedEmployees / TotalEmployees * 100);
        public int EmployeesGrowthPercentage => TotalEmployees == 0 ? 0 : (int)Math.Round((decimal)(TotalEmployees - TotalUnassignedEmployees) / TotalEmployees * 100);
    }
}