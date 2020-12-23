using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinalMVC.Models
{
    public class AdminViewModel
    {
        public ICollection<News> News { get; set; }
        public ICollection<Subject> Subjects { get; set; }
        public ICollection<ApplicationUser> Users { get; set; }
        public ICollection<Event> Events { get; set; }
        public ICollection<Course> Courses { get; set; }

    }
}