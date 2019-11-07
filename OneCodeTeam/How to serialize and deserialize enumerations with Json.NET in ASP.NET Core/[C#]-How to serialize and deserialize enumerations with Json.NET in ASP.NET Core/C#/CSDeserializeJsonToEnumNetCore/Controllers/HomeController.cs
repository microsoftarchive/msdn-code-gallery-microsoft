using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CSDeserializeJsonToEnumNetCore.Model;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CSDeserializeJsonToEnumNetCore.Controllers
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
        public IActionResult Index(Employee employee)
        {
            if (employee != null)
            {
                _employees.Add(employee);
            }

            return View(_employees);
        }
    }
}