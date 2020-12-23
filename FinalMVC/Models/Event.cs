using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace FinalMVC.Models
{
    public class Event
    {
        public int EventID { get; set; }
        public string Title { get; set; }
        [Display(Name = "Content")]
        public string Content { get; set; }
        public bool isMaster { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddThh:mm}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
    }
}