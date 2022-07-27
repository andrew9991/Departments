using Departments.Data;
using Departments.Models;
using Departments.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

namespace Departments.Controllers
{
    public class UserController : Controller
    {
        private IWebHostEnvironment Environment;
        private readonly ApplicationDbContext _db;
        private UserViewModel _uvm;
        Dictionary<string, string> settings;


        public UserController(ApplicationDbContext db, IWebHostEnvironment environment)
        {
            _db = db;
            settings = new Dictionary<string, string>();
            _uvm = new UserViewModel(settings);
            Environment = environment;
        }

        private void getImages()
        {
            var provider = new PhysicalFileProvider(Environment.WebRootPath);
            var contents = provider.GetDirectoryContents(Path.Combine("uploads", ""));
            var objFiles = contents.OrderBy(m => m.LastModified);

            foreach (var item in objFiles.ToList())
            {
                _uvm.ImagesDict.Add(item.Name, item.Name);
            }
        }

        public IActionResult Index()
        {
            getImages();
            _uvm.users = _db.Users;
            return View(_uvm);
        }

        public JsonResult getEmployee(int id)
        {
            return new JsonResult(_db.Users.Find(id));
        }

        public JsonResult getDepartments()
        {
            return new JsonResult(_db.Departments);
        }


        [HttpPost]
        public IActionResult Create(UserViewModel uvm)
        {
            if(uvm.CreateUser.Name == null || uvm.CreateUser.DateAdded == null || uvm.CreateUser.Department == null)
            {
                uvm.users = _db.Users;
                return View("Index", uvm);
            }

            Department department = _db.Departments.FirstOrDefault(x => x.Name == uvm.CreateUser.Department.Name);
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

            string wwwPath = this.Environment.WebRootPath;
            string contentPath = this.Environment.ContentRootPath;

            string path = Path.Combine(this.Environment.WebRootPath, "uploads");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string fileName = DateTime.Now.ToString().GetHashCode().ToString("x") + '.' + uvm.Image.FileName.ToString().Split('.')[1];
            using (FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
            {
                uvm.Image.CopyTo(stream);
            }
            uvm.CreateUser.ProfilePicture = fileName;
            uvm.CreateUser.Department = department;
            uvm.CreateUser.Id = 0;
            //_db.Entry(department).State = EntityState.Detached;
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

            UserViewModel uvm = new UserViewModel(settings);
            uvm.users = _db.Users;
            return View("Index", uvm);
        }
    }
}
