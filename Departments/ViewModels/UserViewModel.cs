using Departments.Models;

namespace Departments.ViewModels
{
    public class UserViewModel
    {
        public IEnumerable<User> users { get; set; }

        public User CreateUser { get; set; }
        public User EditUser { get; set; }
    }
}
