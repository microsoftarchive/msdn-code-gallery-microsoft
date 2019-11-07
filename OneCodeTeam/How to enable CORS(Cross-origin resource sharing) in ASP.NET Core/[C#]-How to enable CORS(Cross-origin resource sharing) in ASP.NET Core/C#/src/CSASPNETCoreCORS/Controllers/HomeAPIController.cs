using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;

namespace CSASPNETCoreCORS.Controllers
{
    //[EnableCors("AllowSpecificOrigin")]
    [Route("api/[controller]")]

    public class HomeAPIController : Controller
    {
        [EnableCors("AllowSpecificOrigin")]
        [HttpGet]
        public string Get()
        {
            return "this message is from another origin";
        }

        [DisableCors]
        [HttpPost]
        public string Post()
        {
            return "this method can't cross origin";
        }
    }
}
