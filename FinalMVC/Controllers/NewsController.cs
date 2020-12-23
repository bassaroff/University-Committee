using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FinalMVC.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace FinalMVC.Controllers
{
    public class NewsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult NewsDetails(int id)
        {
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));

            NewsDetailsView newsDetails = new NewsDetailsView();
            newsDetails.news = db.News.Find(id);
            newsDetails.RelatedNews = db.News.Where(n => n.NewsID != id).ToList();
            newsDetails.Comments = db.Comments.Include(c => c.News).Include(c => c.User).Where(c => c.NewsId == newsDetails.news.NewsID).OrderByDescending(c=>c.PostedDate).ToList();
            if (Session["currentUser"] == null)
            {
                Session["currentUser"] = UserManager.FindById(User.Identity.GetUserId());
            }

            return View(newsDetails);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddComment(int NewsId, string Comment, string UserId)
        {
            Comment comment = new Comment();
            comment.NewsId = NewsId;
            comment.News = db.News.Find(NewsId);
            comment.UserId = UserId;
            comment.User = db.Users.Find(UserId);
            comment.PostedDate = DateTime.Now;
            comment.Message = Comment;
            db.Comments.Add(comment);
            db.SaveChanges();

            return RedirectToAction("NewsDetails", new { id = NewsId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditComment(int comment_id, string comment)
        {

            var comToEdit = db.Comments.Find(comment_id);
            comToEdit.Message = comment;
            db.SaveChanges();
            return RedirectToAction("NewsDetails", new { id = comToEdit.NewsId });
        }

        public ActionResult DeleteComment(int com_id)
        {
            var comment = db.Comments.Find(com_id);

            db.Comments.Remove(comment);
            db.SaveChanges();

            return RedirectToAction("NewsDetails", new { id = comment.NewsId });
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
