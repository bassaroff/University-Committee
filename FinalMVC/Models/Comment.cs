using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinalMVC.Models
{
    public class Comment
    {
        public int CommentId {get;set;}
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        public string Message { get; set; }

        public DateTime PostedDate { get; set; }

        public int NewsId { get; set; }
        public virtual News News { get; set; }
    }
}