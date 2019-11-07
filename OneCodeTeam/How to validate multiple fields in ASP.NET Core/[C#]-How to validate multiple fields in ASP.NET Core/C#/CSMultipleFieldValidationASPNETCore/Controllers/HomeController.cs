using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CSMultipleFieldValidationASPNETCore.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CSMultipleFieldValidationASPNETCore.Controllers
{
    public class HomeController : Controller
    {
        private static List<Schedule> _schedules = new List<Schedule>();

        [HttpGet]
        public IActionResult Index()
        {
            Schedule model = new Schedule { StartTime = DateTime.Now, EndTime = DateTime.Now };
            ViewBag.Schedules = _schedules;
            return View(model);
        }

        [HttpPost]
        public IActionResult Index(Schedule model)
        {
            ViewBag.Schedules = _schedules;
            if (ModelState.IsValid)
            {
                _schedules.Add(model);
                Schedule newModel = new Schedule { StartTime = DateTime.Now, EndTime = DateTime.Now };
                return View(newModel);
            }
            else {
                return View(model);
            }
        }
    }
}
