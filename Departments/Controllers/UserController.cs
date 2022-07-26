using Departments.Data;
using Departments.Models;
using Departments.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Departments.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _db;
        private UserViewModel _uvm;

        public UserController(ApplicationDbContext db)
        {
            _db = db;
            _uvm = new UserViewModel();
        }

        public IActionResult Index()
        {
            _uvm.users = _db.Users;
            return View(_uvm);
        }

        [HttpPost]
        public IActionResult Create(UserViewModel uvm)
        {
            Department department = _db.Departments.FirstOrDefault(x => x.Name == uvm.CreateUser.Department.Name);
            if(uvm.CreateUser.Name == null || uvm.CreateUser.DateAdded == null || uvm.CreateUser.Department == null)
            {
                uvm.users = _db.Users;
                return View("Index", uvm);
            }
            if (department == null)
            {
                ModelState.AddModelError("CreateUser.Department.Name", "Department doesn't exist");
                uvm.users = _db.Users;
                return View("Index", uvm);
            }
            uvm.CreateUser.Department = department;
            _db.Users.Add(uvm.CreateUser);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
