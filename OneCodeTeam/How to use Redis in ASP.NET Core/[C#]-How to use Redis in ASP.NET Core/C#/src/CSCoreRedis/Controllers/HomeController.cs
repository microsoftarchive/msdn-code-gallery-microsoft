using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace CSCoreRedis.Controllers
{
    public class HomeController : Controller
    {

        private static Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            return ConnectionMultiplexer.Connect("localhost,abortConnect=false");
        });

        public static ConnectionMultiplexer Connection
        {
            get
            {
                return lazyConnection.Value;
            }
        }
        public IDatabase db { get; set; }

        /// <summary>
        /// Key name of the list in the Redis database.
        /// </summary>
        public static string ListKeyName = "MessageList";

        public HomeController()
        {
            db = Connection.GetDatabase();
            if (db.IsConnected(ListKeyName) && (!db.KeyExists(ListKeyName) || !db.KeyType(ListKeyName).Equals(RedisType.List)))
            {
                //Add sample data.
                db.KeyDelete(ListKeyName);
                //Push data from the left
                db.ListLeftPush(ListKeyName, "TestMsg1");
                db.ListLeftPush(ListKeyName, "TestMsg2");
                db.ListLeftPush(ListKeyName, "TestMsg3");
                db.ListLeftPush(ListKeyName, "TestMsg4");
            }
        }

        public IActionResult Index()
        {
            //Get the latest 5 messages.
            if (db.IsConnected(ListKeyName))
            {
                //get 5 items from the left
                List<string> messageList = db.ListRange(ListKeyName,0,4).Select(o => (string)o).ToList();
                ViewData["MessageList"] = messageList;
                return View(messageList);
            }
            else
            {
                ViewData["Error"] = "Multiplexer not connected";
                return View();
            }
        }

        [HttpPost]
        public ActionResult SendMessage(string message)
        {
            //Add message to the list from left
            if (db.IsConnected(ListKeyName))
            {
                db.ListLeftPush(ListKeyName, message);
            }
            return RedirectToAction("Index");
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
