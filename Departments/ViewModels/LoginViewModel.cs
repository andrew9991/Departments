using System.ComponentModel;

namespace Departments.ViewModels
{
    public class LoginViewModel
    {
        [DisplayName("User Name")]
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
