using System.Collections;

namespace ProjectDashboard.Models{
    public class UserHomeViewModel{
        public Employee? employee;
        public int? totalProjectsAssigned;
        public List<Project>? projects;
    }
}