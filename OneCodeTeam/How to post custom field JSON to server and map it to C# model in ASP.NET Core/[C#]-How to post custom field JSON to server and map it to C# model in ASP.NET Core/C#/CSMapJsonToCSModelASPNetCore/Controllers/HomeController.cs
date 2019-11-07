using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using CSMapJsonToCSModelASPNetCore.Model;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CSMapJsonToCSModelASPNetCore.Controllers
{
    public class HomeController : Controller
    {
        private static List<Employee> _employees = new List<Employee>();

        [HttpGet]
        public IActionResult Index()
        {
            return View(_employees);
        }

        [HttpPost]
        public JsonResult Index([FromBody]Employee employee)
        {
            if (employee != null) {
                employee.EmployeeId = Guid.NewGuid();
                _employees.Add(employee);
            }

            return Json(new {
                state = 0,
                msg = string.Empty
            });
        }
    }
}
