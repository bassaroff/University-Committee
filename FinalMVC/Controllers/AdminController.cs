using FinalMVC.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace FinalMVC.Controllers
{
    public class AdminController : Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin
        public ActionResult Index()
        { 
            if (isAdminUser())
            {
                AdminViewModel adminViewModel = new AdminViewModel();

                adminViewModel.Courses = db.Courses.ToList();
                adminViewModel.Events = db.Events.ToList();
                adminViewModel.Users = db.Users.ToList();
                adminViewModel.Subjects = db.Subjects.ToList();
                adminViewModel.News = db.News.Include(n => n.Category).ToList();
                return View(adminViewModel);
            }
            else
            {
                return View("403");
            }
        }

        public ActionResult UserDetails(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser user = db.Users.Find(id);
            var Results = from s in db.Subjects
                          select new
                          {
                              s.SubjectID,
                              s.Name,
                              Checked = ((from us in db.UsersToSubjects
                                          where (us.UserID == id) & (us.SubjectID == s.SubjectID)
                                          select us).Count() > 0
                                          )
                          };
            var MyViewModel = new UserViewModel();
            MyViewModel.UserId = id;
            MyViewModel.FirstName = user.FirstName;
            MyViewModel.LastName = user.LastName;
            MyViewModel.UserName = user.UserName;
            MyViewModel.Email = user.Email;
            MyViewModel.PictureUrl = user.PictureUrl;
            MyViewModel.Phone = user.PhoneNumber;
            MyViewModel.Accepted = user.Accepted;
            var MyCheckBoxList = new List<CheckBoxViewModel>();

            foreach (var item in Results)
            {
                MyCheckBoxList.Add(new CheckBoxViewModel { Id = item.SubjectID, Name = item.Name, Checked = item.Checked });
            }

            MyViewModel.Subjects = MyCheckBoxList;

            
            MyViewModel.FileUploads = GetUserUploads(id);

            if (user == null)
            {
                return HttpNotFound();
            }
            return PartialView(MyViewModel);
        }

        public ActionResult DisableStatus(string id)
        {
            ApplicationUser user = db.Users.Find(id);
            user.Accepted = false;
            db.SaveChanges();

            return RedirectToAction("UserDetails", new { id = id });
        }

        public ActionResult ActivateStatus(string id)
        {
            ApplicationUser user = db.Users.Find(id);
            user.Accepted = true;
            db.SaveChanges();

            return RedirectToAction("UserDetails", new { id = id });
        }
        public ActionResult EditUser(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            /*ViewBag.RoleID = new SelectList(db.Roles, "RoleID", "Name", user.RoleID);*/
            var Results = from s in db.Subjects
                          select new
                          {
                              s.SubjectID,
                              s.Name,
                              Checked = ((from us in db.UsersToSubjects
                                          where (us.UserID == id) & (us.SubjectID == s.SubjectID)
                                          select us).Count() > 0
                                          )
                          };
            var MyViewModel = new UserViewModel();
            MyViewModel.UserId = id;
            MyViewModel.FirstName = user.FirstName;
            MyViewModel.LastName = user.LastName;
            MyViewModel.UserName = user.UserName;
            MyViewModel.Email = user.Email;
            MyViewModel.PictureUrl = user.PictureUrl;
            MyViewModel.Phone = user.PhoneNumber;
            MyViewModel.Accepted = user.Accepted;
            var MyCheckBoxList = new List<CheckBoxViewModel>();

            foreach (var item in Results)
            {
                MyCheckBoxList.Add(new CheckBoxViewModel { Id = item.SubjectID, Name = item.Name, Checked = item.Checked });
            }

            MyViewModel.Subjects = MyCheckBoxList;


            return PartialView(MyViewModel);
        }

        public List<FileUpload> GetUserUploads(string user_id)
        {
            FilesController filesController = new FilesController();

            List<FileUpload> list = new List<FileUpload>();

            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));

            DataTable dtFiles = filesController.GetFileDetails();

            foreach (DataRow dr in dtFiles.Rows)
            {
                FileUpload file = new FileUpload
                {
                    type = @dr["TYPE"].ToString(),
                    UserId = @dr["USERID"].ToString(),
                    FileId = @dr["SQLID"].ToString(),
                    FileName = @dr["FILENAME"].ToString(),
                    FileUrl = @dr["FILEURL"].ToString()
                };
                ApplicationUser current = UserManager.FindById(user_id);
                if (file.UserId.Equals(current.Id))
                {
                    list.Add(file);
                }
            }
            return list;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUser(UserViewModel user)
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
                return RedirectToAction("Index");
            }
            /* ViewBag.RoleID = new SelectList(db.Roles, "RoleID", "Name", user.RoleID);*/
            return PartialView(user);
        }

        public ActionResult DeleteUser(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser applicationUser = db.Users.Find(id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            return PartialView(applicationUser);
        }

        // POST: ApplicationUsers/Delete/5
        [HttpPost, ActionName("DeleteUser")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmedUser(string id)
        {
            ApplicationUser applicationUser = db.Users.Find(id);
            db.Users.Remove(applicationUser);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult CreateSubject()
        {
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateSubject([Bind(Include = "SubjectID,Name,Credits,Hours")] Subject subject)
        {
            if (ModelState.IsValid)
            {
                db.Subjects.Add(subject);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return PartialView(subject);
        }


        public async Task<ActionResult> EditSubject(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Subject subject = await db.Subjects.FindAsync(id);
            if (subject == null)
            {
                return HttpNotFound();
            }
            return PartialView(subject);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditSubject([Bind(Include = "SubjectID,Name,Credits,Hours")] Subject subject)
        {
            if (ModelState.IsValid)
            {
                db.Entry(subject).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return PartialView(subject);
        }

        public async Task<ActionResult> SubjectDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Subject subject = await db.Subjects.FindAsync(id);
            if (subject == null)
            {
                return HttpNotFound();
            }
            return PartialView(subject);
        }

        public async Task<ActionResult> DeleteSubject(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Subject subject = await db.Subjects.FindAsync(id);
            if (subject == null)
            {
                return HttpNotFound();
            }
            return PartialView(subject);
        }

        // POST: Subjects/Delete/5
        [HttpPost, ActionName("DeleteSubject")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmedSubject(int id)
        {
            Subject subject = await db.Subjects.FindAsync(id);
            db.Subjects.Remove(subject);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }


        public async Task<ActionResult> NewsDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            News news = await db.News.FindAsync(id);
            if (news == null)
            {
                return HttpNotFound();
            }
            return PartialView(news);
        }

        public ActionResult CreateNews()
        {
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Name");
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateNews([Bind(Include = "NewsID,Title,ShortContent,Content,PictureURL,CategoryId,IsActive,IsInTop,UserID,PostedDate")] News news)
        {
            if (ModelState.IsValid)
            {
                ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Name", news.CategoryId);
                var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                news.UserID = User.Identity.GetUserId();
                news.User = UserManager.FindById(User.Identity.GetUserId());
                news.PostedDate = DateTime.Now;
                db.News.Add(news);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(news);
        }

        public async Task<ActionResult> EditNews(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            News news = await db.News.FindAsync(id);
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Name", news.CategoryId);
            if (news == null)
            {
                return HttpNotFound();
            }
            return PartialView(news);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditNews([Bind(Include = "NewsID,Title,ShortContent,Content,PictureURL,CategoryId,IsActive,IsInTop,UserID,PostedDate")] News news)
        {
            if (ModelState.IsValid)
            {
                ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Name", news.CategoryId);
                db.Entry(news).State = EntityState.Modified;
                await db.SaveChangesAsync();

                if (news.IsActive == true)
                {
                    var users = db.Users.ToList();
                    foreach(var user in users)
                    {
                        if (user.Mailing)
                        {
                            MailMessage mm = new MailMessage("testemailforsite110@gmail.com", user.Email);
                            mm.Subject = news.Title;
                            mm.Body = "<!DOCTYPE html> " +
                                "<html xmlns=\"http://www.w3.org/1999/xhtml\">" +
                                "<head>" +
                                    "<title>Email</title>" +
                                "</head>" +
                                "<body style=\"font-family:'Century Gothic'\">" +
                                "<img src='" + news.PictureURL + "'>" + 
                                    "<h1 style=\"text-align:center;\"> " + news.Title + "</h1>" +
                                    "<h2 style=\"font-size:14px;\">" +
                                        "Short content : " + news.ShortContent +
                                    "</h2>" +
                                    "<hr />" +
                                    "<h3>Do you wanna know more - click here: <a href='https://localhost:44399/News/NewsDetails/" + news.NewsID + "'>Here</a>" +
                            "</body>" +
                                "</html>";
                            mm.IsBodyHtml = true;


                            SmtpClient smtp = new SmtpClient();
                            smtp.Host = "smtp.gmail.com";
                            smtp.Port = 587;
                            smtp.EnableSsl = true;
                            /*testemailforsite110@gmail.com*/
                            /*110qwerty*/

                            NetworkCredential nc = new NetworkCredential("testemailforsite110@gmail.com", "110qwerty");
                            smtp.UseDefaultCredentials = true;
                            smtp.Credentials = nc;
                            smtp.Send(mm);
                        }
                    }

                }

                return RedirectToAction("Index");
            }
            return PartialView(news);
        }

        public async Task<ActionResult> DeleteNews(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            News news = await db.News.FindAsync(id);
            if (news == null)
            {
                return HttpNotFound();
            }
            return PartialView(news);
        }

        // POST: News/Delete/5
        [HttpPost, ActionName("DeleteNews")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmedNews(int id)
        {
            News news = await db.News.FindAsync(id);
            db.News.Remove(news);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }


        public async Task<ActionResult> EventDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = await db.Events.FindAsync(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            return PartialView(@event);
        }
        public ActionResult CreateEvent()
        {
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateEvent([Bind(Include = "EventID,Title,content,isMaster,Date")] Event @event)
        {
            if (ModelState.IsValid)
            {
                db.Events.Add(@event);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return PartialView(@event);
        }

        public async Task<ActionResult> EditEvent(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = await db.Events.FindAsync(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            return PartialView(@event);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditEvent([Bind(Include = "EventID,Title,isMaster, content,Date")] Event @event)
        {
            if (ModelState.IsValid)
            {
                db.Entry(@event).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return PartialView(@event);
        }

        // GET: Events/Delete/5
        public async Task<ActionResult> DeleteEvent(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = await db.Events.FindAsync(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            return PartialView(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("DeleteEvent")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmedEvent(int id)
        {
            Event @event = await db.Events.FindAsync(id);
            db.Events.Remove(@event);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> CourseDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = await db.Courses.FindAsync(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return PartialView(course);
        }

        public ActionResult CreateCourse()
        {
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateCourse([Bind(Include = "CourseID,Name,Description,Price,Hours")] Course course)
        {
            if (ModelState.IsValid)
            {
                db.Courses.Add(course);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return PartialView(course);
        }

        public async Task<ActionResult> EditCourse(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = await db.Courses.FindAsync(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return PartialView(course);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditCourse([Bind(Include = "CourseID,Name,Description,Price,Hours")] Course course)
        {
            if (ModelState.IsValid)
            {
                db.Entry(course).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return PartialView(course);
        }

        public async Task<ActionResult> DeleteCourse(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = await db.Courses.FindAsync(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return PartialView(course);
        }

        [HttpPost, ActionName("DeleteCourse")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmedCourse(int id)
        {
            Course course = await db.Courses.FindAsync(id);
            db.Courses.Remove(course);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }


        public Boolean isAdminUser()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity;
                ApplicationDbContext context = new ApplicationDbContext();
                var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
                var s = UserManager.GetRoles(user.GetUserId());

                if (s[0].ToString() == "admin")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }
    }
}