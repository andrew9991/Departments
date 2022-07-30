using Departments.Data;
using Departments.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Web.Helpers;

namespace Departments.Controllers
{
    public class LoginController : Controller
    {
        private readonly ApplicationDbContext _db;

        public LoginController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel lvm)
        {
            var user = _db.Users.FirstOrDefault(u => u.userName == lvm.UserName && u.Password == lvm.Password);
            if(user == null)
            {
                return View("Error");
            }

            LoggedInViewModel userModel = new LoggedInViewModel()
            {
                UserId = user.Id,
                Name = user.Name,
                UserName = user.userName,
                ProfilePicture = user.ProfilePicture
            };
            string userData = JsonConvert.SerializeObject(userModel);
            
            //var authTicket = new FormsAuthenticationTicket
            //                (
            //                1, loginView.UserName, DateTime.Now, DateTime.Now.AddMinutes(15), false, userData
            //                );
            return RedirectToAction("Index","Home");
        }
    }
}
