using Departments.Data;
using Departments.Models;
using Departments.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        public JsonResult getEmployee(int id)
        {
            return new JsonResult(_db.Users.Find(id));
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
            else if (_db.Users.Where(u => u.DepartmentId == department.Id).ToList().Count >= department.Limit)
            {
                ModelState.AddModelError("CreateUser.Department.Name", "Department is already full");
                uvm.users = _db.Users;
                return View("Index", uvm);
            }
            uvm.CreateUser.Department = department;
            _db.Entry(department).State = EntityState.Detached;
            _db.Users.Add(uvm.CreateUser);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Edit(UserViewModel uvm)
        {
            var emp = uvm.EditUser;
            if (emp.Name == null || emp.DateAdded == null || emp.Department == null)
            {
                uvm.users = _db.Users;
                return View("Index", uvm);
            }
            var exists = _db.Departments.FirstOrDefault(d => d.Name == emp.Department.Name);
            if (exists == null)
            {
                ModelState.AddModelError("EditUser.Department.Name", "Department doesn't exist");
                uvm.users = _db.Users;
                return View("Index", uvm);
            }
            else if (_db.Users.Where(u => u.DepartmentId == exists.Id).ToList().Count >= exists.Limit)
            {
                ModelState.AddModelError("EditUser.Department.Name", "Department is already full");
                uvm.users = _db.Users;
                return View("Index", uvm);
            }
            emp.Department = exists;
            _db.Entry(exists).State = EntityState.Detached;
            _db.Users.Update(emp);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Route("DeleteEmp/{id}")]
        public IActionResult Delete(int? id)
        {
            var emp = _db.Users.Find(id);
            if (emp != null)
            {
                _db.Users.Remove(emp);
                _db.SaveChanges();
            }
            UserViewModel uvm = new UserViewModel();
            uvm.users = _db.Users;
            return View("Index", uvm);
        }
    }
}
