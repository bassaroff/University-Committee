using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FinalMVC.Models
{
    public class News
    {
        public int NewsID { get; set; }
        public string Title { get; set; }
        public string ShortContent { get; set; }
        public string Content { get; set; }
        public string PictureURL { get; set; }
        public string UserID { get; set; }
        public virtual ApplicationUser User { get; set; }

        [Display(Name = "Category")]
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
        public bool IsActive { get; set; }
        public bool IsInTop { get; set; }
        public DateTime PostedDate { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }
    }
}