using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinalMVC.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<News> News { get; set; }
    }
}