using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinalMVC.Models
{
    public class NewsDetailsView
    {
        public News news { get; set; }
        public ICollection<News> RelatedNews { get; set; }
        public ICollection<Comment> Comments { get; set; }
    }
}