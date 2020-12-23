using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace FinalMVC.Models
{
    // В профиль пользователя можно добавить дополнительные данные, если указать больше свойств для класса ApplicationUser. Подробности см. на странице https://go.microsoft.com/fwlink/?LinkID=317594.
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PictureUrl { get; set; }
        public bool Mailing { get; set; }
        public bool Accepted { get; set; }
        public virtual ICollection<News> News { get; set; }
        public virtual ICollection<UserToSubject> UsersToSubjects { get; set; }


        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Обратите внимание, что authenticationType должен совпадать с типом, определенным в CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Здесь добавьте утверждения пользователя
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public System.Data.Entity.DbSet<FinalMVC.Models.Event> Events { get; set; }

        public System.Data.Entity.DbSet<FinalMVC.Models.Subject> Subjects { get; set; }

        public System.Data.Entity.DbSet<FinalMVC.Models.Course> Courses { get; set; }

        public System.Data.Entity.DbSet<FinalMVC.Models.News> News { get; set; }

        public System.Data.Entity.DbSet<FinalMVC.Models.UserToSubject> UsersToSubjects { get; set; }

        public System.Data.Entity.DbSet<FinalMVC.Models.Category> Categories { get; set; }

        public System.Data.Entity.DbSet<FinalMVC.Models.Comment> Comments { get; set; }



        /*public System.Data.Entity.DbSet<FinalMVC.Models.ApplicationUser> ApplicationUsers { get; set; }*/
    }
}