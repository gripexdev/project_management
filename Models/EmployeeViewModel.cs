namespace ProjectDashboard.Models
{
    public class EmployeeIndexViewModel
    {
        public IEnumerable<Employee> Employees { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
    }
}