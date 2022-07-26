using Departments.Models;

namespace Departments.ViewModels
{
    public class DepartmentViewModel
    {
        public IEnumerable<Department> Departments { get; set; }

        public Department CreateDepartment { get; set; }
        public Department EditDepartment { get; set; }


    }
}
