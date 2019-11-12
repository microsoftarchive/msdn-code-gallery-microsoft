using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace AccessSQLService.Model
{
    [DataContract]
    public class Article
    {
        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string Text { get; set; }
    }
}