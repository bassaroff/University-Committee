using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FinalMVC.Models
{
    public class UserViewModel
    {
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public string PictureUrl { get; set; }
        public string role { get; set; }
        public bool Mailing { get; set; }
        public bool Accepted { get; set; }
        public IEnumerable<FileUpload> FileUploads { get; set; }
        public List<CheckBoxViewModel> Subjects { get; set; }
    }
}