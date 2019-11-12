using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ManagingTasksUsingEWS.Models
{
    /// <summary>
    /// Task creator credentials.
    /// </summary>
    public class UserModel
    {
        public string TaskCreatorEmailID { get; set; }
        public string Password { get; set; }

    }

    /// <summary>
    /// Task entity along with attributes.
    /// </summary>
    public class TaskModel
    {
        public string TaskTitle { get; set; }
        public string TaskMessage { get; set; }
        public DateTime TaskStartDate { get; set; }
    }

    /// <summary>
    /// User's task collection along with user information.
    /// </summary>
    public class UserTasksModel
    {
        public UserModel User { get; set; }
        public TaskModel NewTask { get; set; }
        public List<TaskModel> Tasks { get; set; }
    }

}