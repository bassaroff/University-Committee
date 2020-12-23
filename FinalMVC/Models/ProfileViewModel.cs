using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinalMVC.Models
{
    public class ProfileViewModel
    {
        public UserViewModel user { get; set; }
        public News News { get; set; }
        public ICollection<News> userNews { get; set; }

        public FileUpload fileUpload { get; set; }
    }
}