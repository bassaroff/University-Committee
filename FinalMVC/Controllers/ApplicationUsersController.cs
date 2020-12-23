using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FinalMVC.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace FinalMVC.Controllers
{
    public class ApplicationUsersController : Controller
    {
        private AdminController adminController = new AdminController();
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ApplicationUsers
        public ActionResult Index()
        {
           
            // In Startup iam creating first Admin Role and creating a default Admin User     
           /* if (adminController.isAdminUser())
            {*/
                return View(db.Users.ToList());
            /*}else
            {
                return View("Forbidden/403");
            }*/
                
        }

        // GET: ApplicationUsers/Details/5
        public ActionResult Details(string id)
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
            return View(applicationUser);
        }

        // GET: ApplicationUsers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ApplicationUsers/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в разделе https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FirstName,LastName,PictureUrl,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName")] ApplicationUser applicationUser)
        {
            if (ModelState.IsValid)
            {
                db.Users.Add(applicationUser);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(applicationUser);
        }

        // GET: ApplicationUsers/Edit/5
        public ActionResult Edit(string id)
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
            var MyCheckBoxList = new List<CheckBoxViewModel>();

            foreach (var item in Results)
            {
                MyCheckBoxList.Add(new CheckBoxViewModel { Id = item.SubjectID, Name = item.Name, Checked = item.Checked });
            }

            MyViewModel.Subjects = MyCheckBoxList;


            return View(MyViewModel);
        }

        // POST: ApplicationUsers/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в разделе https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UserViewModel user)
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
            return View(user);
        }

        // GET: ApplicationUsers/Delete/5
        public ActionResult Delete(string id)
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
            return View(applicationUser);
        }

        // POST: ApplicationUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            ApplicationUser applicationUser = db.Users.Find(id);
            db.Users.Remove(applicationUser);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
