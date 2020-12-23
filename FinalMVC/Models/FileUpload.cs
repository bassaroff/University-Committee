using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace FinalMVC.Models
{
    public class FileUpload
    {
        public string FileId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string type { get; set; }
        public string FileName { get; set; }
        public string FileUrl { get; set; }
        public IEnumerable<FileUpload> FileList { get; set; }
    }
}