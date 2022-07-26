using Departments.Data;
using Departments.Models;
using Departments.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Departments.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private DepartmentViewModel _dvm;

        public DepartmentsController(ApplicationDbContext db)
        {
            _dvm = new DepartmentViewModel();
            _db = db;
        }
        public IActionResult Index()
        {
            _dvm.Departments = _db.Departments;
            return View(_dvm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(DepartmentViewModel departmentViewModel)
        {
            bool fine = true;
            if (departmentViewModel.CreateDepartment.Name == null)
            {
                ModelState.AddModelError("CreateDepartment.Name", "name can't be empty");
                fine = false;
            }
            else if (_db.Departments.FirstOrDefault(x => x.Name == departmentViewModel.CreateDepartment.Name) != null)
            {
                ModelState.AddModelError("CreateDepartment.Name", "name already exists");
                fine = false;
            }
            if (departmentViewModel.CreateDepartment.Description == null)
            {
                ModelState.AddModelError("CreateDepartment.Description", "Description can't be empty");
                fine = false;
            }
            if (departmentViewModel.CreateDepartment.Limit == null)
            {
                ModelState.AddModelError("CreateDepartment.Limit", "Limit can't be empty");
                fine = false;
            }
            if (fine)
            {
                _db.Departments.Add(departmentViewModel.CreateDepartment);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            departmentViewModel.Departments = _db.Departments;
            return View("Index", departmentViewModel);
        }

        //[HttpGet, Route("Index/{id:int}")]
        public JsonResult getDepartment(int id)
        {
            return new JsonResult(_db.Departments.Find(id));
        }

        public JsonResult getEmployees (int id)
        {
            return new JsonResult(_db.Users.Where(u => u.DepartmentId == id));
        }

        [HttpPost]
        public IActionResult Edit(DepartmentViewModel dvm)
        {
            var dep = dvm.EditDepartment;
            if (dep.Name == null || dep.Description == null || dep.Limit == null || dep.Limit <= 0)
            {
                dvm.Departments = _db.Departments;
                return View("Index", dvm);
            }
            var duplicate = _db.Departments.FirstOrDefault(d => d.Name == dep.Name);
            if (duplicate != null && duplicate.Id != dep.Id)
            {
                ModelState.AddModelError("EditDepartment.Name", "Name already exists");
                dvm.Departments = _db.Departments;
                return View("Index", dvm);
            }

            if (_db.Users.Where(u => u.DepartmentId == dep.Id).ToList().Count >= dep.Limit)
            {
                ModelState.AddModelError("EditDepartment.Limit", "Employees exceed limit ");
                dvm.Departments = _db.Departments;
                return View("Index", dvm);
            }

            _db.Entry(duplicate).State = EntityState.Detached;
            _db.Departments.Update(dep);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpPost]
        [Route("{id}")]
        public IActionResult Delete(int? id)
        {
            var dep = _db.Departments.Find(id);
            if(dep != null)
            {
                _db.Departments.Remove(dep);
                _db.SaveChanges();
            }
            DepartmentViewModel dvm = new DepartmentViewModel();
            dvm.Departments = _db.Departments;
            return View("Index", dvm) ;
        }
    }


}

