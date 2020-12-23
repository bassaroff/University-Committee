using FinalMVC.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Image = FinalMVC.Models.Image;

namespace FinalMVC.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Profile (string id)
        {
             var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            Session["currentUser"] = UserManager.FindById(User.Identity.GetUserId());
            ViewBag.Message = "Your contact page.";
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            var Results = from sub in db.Subjects
                          select new
                          {
                              sub.SubjectID,
                              sub.Name,
                              Checked = ((from us in db.UsersToSubjects
                                          where (us.UserID == id) & (us.SubjectID == sub.SubjectID)
                                          select us).Count() > 0
                                          )
                          };

           
            var s = UserManager.GetRoles(user.Id);

            if (String.IsNullOrEmpty(user.PictureUrl))
            {
                user.PictureUrl = "~/Content/UserAvatars/default.png";
            }

            var MyViewModel = new UserViewModel();
            MyViewModel.UserId = id;
            MyViewModel.FirstName = user.FirstName;
            MyViewModel.LastName = user.LastName;
            MyViewModel.UserName = user.UserName;
            MyViewModel.Email = user.Email;
            MyViewModel.PictureUrl = user.PictureUrl;
            MyViewModel.Phone = user.PhoneNumber;
            MyViewModel.role = s[0].ToString();
            MyViewModel.Mailing = user.Mailing;
            MyViewModel.Accepted = user.Accepted;

            var MyCheckBoxList = new List<CheckBoxViewModel>();

            foreach (var item in Results)
            {
                MyCheckBoxList.Add(new CheckBoxViewModel { Id = item.SubjectID, Name = item.Name, Checked = item.Checked });
            }

            MyViewModel.Subjects = MyCheckBoxList;

            var profileViewModel = new ProfileViewModel();
            profileViewModel.user = MyViewModel;
            profileViewModel.fileUpload = new FileUpload();
            profileViewModel.userNews = db.News.Where(n => n.UserID == profileViewModel.user.UserId).ToList();

            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Name");
            return View(profileViewModel);
        }


        [HttpPost]
        
        public ActionResult AddAvatar(Image imageModel, string user_id)
        {
            try
            {
                if (imageModel.ImageFile!= null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(imageModel.ImageFile.FileName);
                    string extension = Path.GetExtension(imageModel.ImageFile.FileName);

                    fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    imageModel.ImagePath = "~/Content/UserAvatars/" + fileName;

                    fileName = Path.Combine(Server.MapPath("~/Content/UserAvatars/"), fileName);
                    imageModel.ImageFile.SaveAs(fileName);
                    ApplicationUser user = db.Users.Find(user_id);

                    user.PictureUrl = imageModel.ImagePath;
                    db.SaveChanges();
                }
                else
                {
                    return RedirectToAction("Profile", new { id = user_id });
                }
                
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            

            
            return RedirectToAction("Profile", new { id = user_id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProfile(UserViewModel user)
        {

            if (ModelState.IsValid)
            {
                var MyUser = db.Users.Find(user.UserId);

                MyUser.FirstName = user.FirstName;
                MyUser.LastName = user.LastName;
                MyUser.UserName = user.UserName;
                MyUser.Email = user.Email;
                MyUser.PictureUrl = user.PictureUrl;
                MyUser.PhoneNumber = user.Phone;
                MyUser.Mailing = user.Mailing;
                foreach (var item in db.UsersToSubjects)
                {
                    if (item.UserID == user.UserId)
                    {
                        db.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                    }
                }

                foreach (var item in user.Subjects)
                {
                    if (item.Checked)
                    {
                        db.UsersToSubjects.Add(new UserToSubject() { UserID = user.UserId, SubjectID = item.Id });
                    }
                }

                db.SaveChanges();
            }
            return RedirectToAction("Profile", new { id = user.UserId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddPost([Bind(Include = "NewsID,Title,ShortContent,Content,PictureURL,CategoryId,IsActive,IsInTop,UserID,PostedDate")] News news)
        {
            if (ModelState.IsValid)
            {
                
                var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                news.UserID = User.Identity.GetUserId();
                news.User = UserManager.FindById(User.Identity.GetUserId());
                news.PostedDate = DateTime.Now;
                db.News.Add(news);
                db.SaveChanges();
                return RedirectToAction("Profile", new { id = news.UserID });
            }

            return View("Index");
        }

        public ActionResult ActivateMailing(string id)
        {
            var user = db.Users.Find(id);
            user.Mailing = true;
            db.SaveChanges();
            return RedirectToAction("Profile", new { id = id});
        }
        public ActionResult DisableMailing(string id)
        {
            var user = db.Users.Find(id);
            user.Mailing = false;
            db.SaveChanges();
            return RedirectToAction("Profile", new { id = id });
        }

        public ActionResult Admission()
        {
            var events = db.Events.Where(a=>a.Date > DateTime.Now).OrderByDescending(a=>a.Date).ToList();

            return View(events);
        }
        
        public ActionResult News()
        {
            var news = db.News.Include(n => n.Comments).ToList();
            return View(news);
        }
       
        public JsonResult GetSearchingNews(string SearchBy)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var news = db.News
                .Where(n => n.IsActive==true)
                .Where(a => a.Title.Contains(SearchBy)|| SearchBy == "" )
                .Select(n => new {
                    UserId = n.UserID,
                    CategoryId = n.CategoryId,
                    NewsID = n.NewsID,
                    Category = n.Category.Name,
                    UserName = n.User.UserName,
                    Title = n.Title,
                    ShortContent = n.ShortContent,
                    PostedMonth = n.PostedDate.Month,
                    PostedDay = n.PostedDate.Day,
                    PostedYear = n.PostedDate.Year,
                    PictureURL = n.PictureURL,
                    IsActive = n.IsActive
                })
                .ToList();
          
            return Json(news, JsonRequestBehavior.AllowGet); 
        }
    }
}