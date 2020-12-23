using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinalMVC.Models
{
    public class UserToSubject
    {
        public int UserToSubjectID { get; set; }
        public string UserID { get; set; }
        public int SubjectID { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual Subject Subject { get; set; }
    }
}