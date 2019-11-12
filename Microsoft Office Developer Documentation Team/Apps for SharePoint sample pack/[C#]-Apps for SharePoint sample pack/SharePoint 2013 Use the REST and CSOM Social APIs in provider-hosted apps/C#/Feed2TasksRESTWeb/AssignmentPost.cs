using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Feed2TasksRESTWeb
{
    public class AssignmentPost
    {
        public DateTime CreatedDate { get; set; }
        public string Body { get; set; }
        public string Requester { get; set; }
    }
}