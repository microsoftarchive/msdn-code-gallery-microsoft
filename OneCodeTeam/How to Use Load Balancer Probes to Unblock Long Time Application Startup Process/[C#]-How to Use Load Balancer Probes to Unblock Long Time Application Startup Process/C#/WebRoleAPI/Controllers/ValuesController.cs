using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebRoleAPI.Models;

namespace WebRoleAPI.Controllers
{
    [Authorize]
    public class ValuesController : ApiController
    {
        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [AllowAnonymous]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }

        [AllowAnonymous]
        [Route("api/values/ping")]
        [AcceptVerbs("Get")]
        public HttpResponseMessage Ping()
        {
            if (InitializedModel.Instance != null)
            {
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            else
            {
                // use 503 as service note ready
                // you can choose your own return code according to your business requirement
                return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable);
            }
        }

    }
}
