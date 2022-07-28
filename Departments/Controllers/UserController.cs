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
            _uvm = new UserViewModel();
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
            settings = _uvm.ImagesDict;
        }

        public IActionResult Index()
        {
            getImages();
            _uvm.users = _db.Users;
            return View(_uvm);
        }

        public IActionResult Activate()
        {
            getImages();
            _uvm.users = _db.Users.Where(u => u.IsActivated == true);
            return View(_uvm);
        }

        public JsonResult ActivateEmployee(int id)
        {
            var emp = _db.Users.FirstOrDefault(u => u.Id == id);
            if(emp != null)
            {
                emp.IsActivated = true;
                _db.Users.Update(emp);
                _db.SaveChanges();
            }
            return Json(new { redirectToUrl = Url.Action("Activate", "User") });
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
            bool fine = true;
            if(uvm.CreateUser.Name == null || uvm.CreateUser.DateAdded == null || uvm.CreateUser.Department == null)
            {
                fine = false;
            }

            Department department = _db.Departments.FirstOrDefault(x => x.Name == uvm.CreateUser.Department.Name);
            if (department == null)
            {
                ModelState.AddModelError("CreateUser.Department.Name", "Department doesn't exist");
                fine = false;
            }
            else if (_db.Users.Where(u => u.DepartmentId == department.Id).ToList().Count >= department.Limit)
            {
                ModelState.AddModelError("CreateUser.Department.Name", "Department is already full");
                fine = false;
            }

            if (!fine)
            {
                //uvm.ImagesDict = settings;
                //uvm.users = _db.Users;
                //return View("Index", uvm);
                getImages();
                _uvm.users = _db.Users;
                return View("Index", _uvm);
            }

            if (uvm.Image != null)
            {
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
            }
            //Page.ClientScript.RegisterStartupScript(this.GetType(), "CallMyFunction", "clss()", true);
            //ClientScript.RegisterStartupScript(GetType(), "Javascript", "javascript:FUNCTIONNAME(); ", true);
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
            bool fine = true;
            if (emp.Name == null || emp.DateAdded == null || emp.Department == null)
            {
                fine = false;
            }
            var exists = _db.Departments.FirstOrDefault(d => d.Name == emp.Department.Name);
            if (exists == null)
            {
                ModelState.AddModelError("EditUser.Department.Name", "Department doesn't exist");
                fine = false;
            }
            else if (_db.Users.Where(u => u.DepartmentId == exists.Id).ToList().Count >= exists.Limit)
            {
                ModelState.AddModelError("EditUser.Department.Name", "Department is already full");
                fine = false;
            }

            if (!fine)
            {
                uvm.users = _db.Users;
                uvm.ImagesDict = _uvm.ImagesDict;
                return View("Index", uvm);
            }
            var user = _db.Users.Find(emp.Id);
            if (uvm.Image != null)
            {
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
                emp.ProfilePicture = fileName;
            }
            else
            {
                emp.ProfilePicture = user.ProfilePicture;
            }
            emp.Department = exists;
            _db.Entry(exists).State = EntityState.Detached;
            _db.Entry(user).State = EntityState.Detached;
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
            uvm.ImagesDict = _uvm.ImagesDict;
            return View("Index", uvm);
        }
    }
}
