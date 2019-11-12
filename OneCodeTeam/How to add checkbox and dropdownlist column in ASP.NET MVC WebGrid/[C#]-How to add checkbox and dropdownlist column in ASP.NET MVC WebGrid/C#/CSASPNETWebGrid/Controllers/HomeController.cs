using CSASPNETWebGrid.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace CSASPNETWebGrid.Controllers
{
    public class HomeController : Controller
    {
        PersonService _service;

        public HomeController()
        {
            _service = new PersonService();
        }

        public ActionResult Index()
        {
            return View(_service.GetPersons());
        }

        public ActionResult About()
        {

            return View();
        }

        public ActionResult Contact()
        {

            return View();
        }


        [HttpPost]
        public ActionResult Index(int[] selectedRows)
        {
            return View(_service.GetPersons());
        }

        
    }
}
