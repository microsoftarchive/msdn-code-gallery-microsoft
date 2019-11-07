using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PeoplePivotsCSOMWeb
{
    public class SocialPost
    {
        public DateTime CreatedDate { get; set; }
        public string Body { get; set; }
        public bool LikedByMe { get; set; }
    }
}