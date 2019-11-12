using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace CSUsingEFDBFirstWithNetCore.Controllers
{
    public class StudentController : Controller
    {
        public IActionResult Index()
        {
            Models.TestNetCoreEFContext context = new Models.TestNetCoreEFContext();

            var StudentList = context.Student.ToList();

            return View(StudentList);
        }

        [HttpGet]
        public IActionResult StudentDetail(int id)
        {
            Models.TestNetCoreEFContext context = new Models.TestNetCoreEFContext();
            var entity = context.Student.FirstOrDefault(m => m.Id == id);

            return View(entity);
        }

        [HttpPost]
        public IActionResult StudentDetail(Models.Student entity)
        {
            Models.TestNetCoreEFContext context = new Models.TestNetCoreEFContext();
            context.Entry(entity).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            context.SaveChanges();

            return View(entity);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(Models.Student studentEntity)
        {
            Models.TestNetCoreEFContext context = new Models.TestNetCoreEFContext();
            context.Student.Add(studentEntity);
            context.SaveChanges();

            return Redirect($"./StudentDetail/{studentEntity.Id}");
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            Models.TestNetCoreEFContext context = new Models.TestNetCoreEFContext();
            var entity = context.Student.FirstOrDefault(m => m.Id == id);
            context.Student.Remove(entity);
            context.SaveChanges();

            return Redirect("/");
        }
    }
}
