using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManagingTasksUsingEWS.Models;
using Microsoft.Exchange.WebServices.Data;

namespace ManagingTasksUsingEWS.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Action will create task and display all the tasks on UI
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(UserTasksModel model)
        {
            try
            {
                //Create TaskManager instance with user credentials.
                TaskManager taskManager = new TaskManager(model.User.TaskCreatorEmailID, model.User.Password);

                //Call create task method
                taskManager.CreateTask(model.NewTask.TaskTitle, model.NewTask.TaskMessage, model.NewTask.TaskStartDate);

                //Get all tasks
                model.Tasks = taskManager.GetAllTasks().Select(task => new TaskModel { TaskTitle = task.Subject, 
                                                                                        TaskMessage = task.Body.Text, 
                                                                                        TaskStartDate = task.DateTimeCreated }).ToList();
               
                return PartialView("Tasks", model);
            }
            catch
            {
                return PartialView("Tasks", model);
            }
        }
    }
}
