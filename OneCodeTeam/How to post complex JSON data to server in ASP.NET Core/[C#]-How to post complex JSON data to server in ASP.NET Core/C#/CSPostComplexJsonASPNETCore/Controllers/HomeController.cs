using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CSPostComplexJsonASPNETCore.Models;

namespace CSPostComplexJsonASPNETCore.Controllers
{
    public class HomeController : Controller
    {
        private static List<ItemGroup> _datas = new List<ItemGroup>();

        public IActionResult Index()
        {
            return View(_datas);
        }

        [HttpPost]
        public JsonResult PostJson([FromBody] ItemGroup data)
        {
            if (data != null)
            {
                _datas.Add(data);
            }

            return Json(new
            {
                state = 0,
                msg = string.Empty
            });
        }
    }
}