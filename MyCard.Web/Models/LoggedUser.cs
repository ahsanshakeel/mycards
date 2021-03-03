using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyCard.Web.Models
{
    public class LoggedUser
    {
        public string UserId { get; set; }

        public DateTime LastAccessTime { get; set; }

        public string CMSKey { get; set; }
    }

    public class FailedUser
    {
        public string UserId { get; set; }

        public DateTime LastAccessTime { get; set; }
       
        public int Count { get; set; }
    }
}