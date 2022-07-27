using Departments.Models;
using System.ComponentModel;

namespace Departments.ViewModels
{
    public class UserViewModel
    {
        public IEnumerable<User> users { get; set; }

        public User CreateUser { get; set; }
        public User EditUser { get; set; }

        [DisplayName("Profile Picture")]
        public IFormFile Image { get; set; }
        
    }
}
