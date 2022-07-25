using Departments.Data;
using Departments.Models;
using Departments.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Departments.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private DepartmentViewModel _dvm;
        private string _depToEdit;

        public DepartmentsController(ApplicationDbContext db)
        {
            _dvm = new DepartmentViewModel(); 
            _db = db;
            _depToEdit = "";
        }
        public IActionResult Index()
        {
           _dvm.Departments = _db.Departments;
           return View(_dvm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(DepartmentViewModel departmentViewModel)
        {
            departmentViewModel.Departments = _db.Departments;
            if (ModelState.IsValid)
            {
                _db.Departments.Add(departmentViewModel.CreateDepartment);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            departmentViewModel.Departments = _db.Departments;
            return View(departmentViewModel);
        }

        public IActionResult Edit(string id)
        {
            var dep = _db.Departments.Where(x => x.Name == id).FirstOrDefault();
            _depToEdit = id;
            return PartialView("_EditDepartmentPartial", dep);
        }

        [HttpPost]
        IActionResult Edit(Department dep)
        {
            if(ModelState.IsValid && dep.Name == _depToEdit)
            {
                _db.Update(dep);
                _db.SaveChanges();
            }
            else if (ModelState.IsValid && dep.Name != _depToEdit)
            {
                _db.Remove(_db.Departments.Where(x => x.Name == dep.Name).FirstOrDefault());
                _db.Departments.Add(dep);
                _db.SaveChanges();
            }
            return PartialView("_EditDepartmentPartial", dep);
        }
    }
}
