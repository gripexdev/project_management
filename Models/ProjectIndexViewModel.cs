namespace ProjectDashboard.Models
{
    public class ProjectIndexViewModel
    {
        public IEnumerable<Project> Projects { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
    }
}
