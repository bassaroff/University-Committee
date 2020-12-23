using FinalMVC.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FinalMVC.Controllers
{
    public class FilesController : Controller
    {
        string conString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index(FileUpload model)
        {
            List<FileUpload> list = new List<FileUpload>();
            DataTable dtFiles = GetFileDetails();
            foreach (DataRow dr in dtFiles.Rows)
            {
                FileUpload file = new FileUpload
                {
                    UserId = @dr["USERID"].ToString(),
                    FileId = @dr["SQLID"].ToString(),
                    FileName = @dr["FILENAME"].ToString(),
                    FileUrl = @dr["FILEURL"].ToString()
                };
                ApplicationUser current = (ApplicationUser)Session["currentUser"];
                if (file.UserId.Equals(current.Id))
                {
                    list.Add(file);
                }
            }
            model.FileList = list;
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(HttpPostedFileBase files, string user_id, string type)
        {
            FileUpload model = new FileUpload();
            model.UserId = user_id;
            model.UserName = db.Users.Find(user_id).UserName;
            List<FileUpload> list = new List<FileUpload>();
            DataTable dtFiles = GetFileDetails();
            foreach (DataRow dr in dtFiles.Rows)
            {
                list.Add(new FileUpload
                {
                    UserId = @dr["USERID"].ToString(),
                    FileId = @dr["SQLID"].ToString(),
                    FileName = @dr["FILENAME"].ToString(),
                    FileUrl = @dr["FILEURL"].ToString(),
                    type = @dr["TYPE"].ToString()
                }) ; 
            }

            model.FileList = list;
            model.type = type;
            string path = "";
            var fileName = "";
            if (files != null)
            {
                var Extension = Path.GetExtension(files.FileName);
                switch (type)
                {
                    case "ent":
                        fileName = "ent-" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + Extension;
                        path = Path.Combine(Server.MapPath("~/Content/UploadedFiles"), fileName);
                        model.FileUrl = Url.Content(Path.Combine("~/Content/UploadedFiles/", fileName));
                        model.FileName = fileName;
                        break;
                    case "attestat":
                        fileName = "attestat-" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + Extension;
                        path = Path.Combine(Server.MapPath("~/Content/UploadedFiles"), fileName);
                        model.FileUrl = Url.Content(Path.Combine("~/Content/UploadedFiles/", fileName));
                        model.FileName = fileName;
                        break;
                    case "086":
                        fileName = "086-" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + Extension;
                        path = Path.Combine(Server.MapPath("~/Content/UploadedFiles"), fileName);
                        model.FileUrl = Url.Content(Path.Combine("~/Content/UploadedFiles/", fileName));
                        model.FileName = fileName;
                        break;
                    case "3x4":
                        fileName = "3x4-" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + Extension;
                        path = Path.Combine(Server.MapPath("~/Content/UploadedFiles"), fileName);
                        model.FileUrl = Url.Content(Path.Combine("~/Content/UploadedFiles/", fileName));
                        model.FileName = fileName;
                        break;
                }

                if (SaveFile(model))
                {
                    files.SaveAs(path);
                    TempData["AlertMessage"] = "Uploaded Successfully !!";
                    return RedirectToAction("Index", "Files");
                }
                else
                {
                    ModelState.AddModelError("", "Error In Add File. Please Try Again !!!");
                }
            }
            else
            {
                ModelState.AddModelError("", "Please Choose Correct File Type !!");
                return View(model);
            }
            return RedirectToAction("Index", "Files");
        }

        public DataTable GetFileDetails()
        {
            DataTable dtData = new DataTable();
            SqlConnection con = new SqlConnection(conString);
            con.Open();
            SqlCommand command = new SqlCommand("Select * From tblFileDetails", con);
            SqlDataAdapter da = new SqlDataAdapter(command);
            da.Fill(dtData);
            con.Close();
            return dtData;
        }

        private bool SaveFile(FileUpload model)
        {
            string strQry = "";

            foreach(var file in model.FileList)
            {
                if (file.type.Equals(model.type) && file.UserId.Equals(model.UserId)) 
                {
                    strQry = "DELETE FROM tblFileDetails WHERE SQLID=" + file.FileId;
                    SqlConnection conn = new SqlConnection(conString);
                    conn.Open();
                    SqlCommand comm = new SqlCommand(strQry, conn);
                    int result = comm.ExecuteNonQuery();
                    conn.Close();
                }
            }



            strQry = "INSERT INTO tblFileDetails (FileName,FileUrl,USERID,USERNAME,TYPE) VALUES('" +
                model.FileName + "','" + model.FileUrl + "','" + model.UserId + "','" + model.UserName + "','" + model.type + "')";
            SqlConnection con = new SqlConnection(conString);
            con.Open();
            SqlCommand command = new SqlCommand(strQry, con);
            int numResult = command.ExecuteNonQuery();
            con.Close();
            if (numResult > 0)
                return true;
            else
                return false;
        }

        public ActionResult DownloadFile(string filePath)
        {
            string fullName = Server.MapPath("~" + filePath);

            byte[] fileBytes = GetFile(fullName);
            return File(
                fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, filePath);
        }

        byte[] GetFile(string s)
        {
            System.IO.FileStream fs = System.IO.File.OpenRead(s);
            byte[] data = new byte[fs.Length];
            int br = fs.Read(data, 0, data.Length);
            if (br != fs.Length)
                throw new System.IO.IOException(s);
            return data;
        }
    }
}