using CSOneDriveAccess;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CSLoginOnOffice365AndGetAccountInfo.Controllers
{
    public class HomeController : Controller
    {
        private const string ClientId = "%client Id%";
        private const string Secret = "%secret%";
        private const string CallbackUri = "callback uri";

        public OAuthAccess OfficeAccess
        {
            get
            {
                var officeAccess = Session["OfficeAccess"];
                if (officeAccess == null)
                {
                    officeAccess = new OAuthAccess(ClientId, Secret, CallbackUri);
                    Session["OfficeAccess"] = officeAccess;
                }
                return officeAccess as OAuthAccess;
            }
        }

        public async Task<ActionResult> Index()
        {
            //if user is not login, redirect to office 365 for authenticate
            if (string.IsNullOrEmpty(OfficeAccess.AccessCode))
            {
                string url = OfficeAccess.GetLoginUrl("onedrive.appfolder");

                return new RedirectResult(url);
            }

            //when user is authenticated get user account info
            ViewBag.UserInfo = await OfficeAccess.GetAccountInfoAsync();
            return View();
        }

        //when user complate authenticate, will be call back this url with a code
        public async Task<RedirectResult> OnAuthComplate(string code)
        {
            //get token by the code
            await OfficeAccess.RedeemTokensAsync(code);

            return new RedirectResult("Index");
        }

        //download user picture
        public async Task<ActionResult> UserPicture()
        {
            var btyes = await OfficeAccess.GetAccountPicture();
            return base.File(btyes, "image/jpeg");
        }
    }
}