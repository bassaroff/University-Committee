using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinalMVC.Models
{
    public class Subject
    {
        public int SubjectID { get; set; }
        public string Name { get; set; }
        public int Credits { get; set; }
        public int Hours { get; set; }

        public virtual ICollection<UserToSubject> UsersToSubjects { get; set; }

    }
}