using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using TheatreCMS.Areas.Subscribers.Models;
using TheatreCMS.Models;


namespace TheatreCMS.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Role { get; set; } = "User";            // role of user for website

        //String of users favorite cast members
        //This string is a list of cast member id, separated by commas.
        public string FavoriteCastMembers { get; set; } = "";    
        public virtual Subscriber SubscriberPerson { get; set; }                        // associated subscriber record
        public virtual ICollection<SeasonManager> SeasonManagerPerson { get; set; }     // all season managers associated with user

        /* Need to find a way to explicitly match a CastMember's User account to their ApplicationUser object, 
        If an app-user becomes a Castmember, ensure that for ApplicationUser user "=" CastMember castMember,
        user.CastMemberPersonID = castMembe.CastMemberPersonID */
        //public virtual CastMember CastMemberUser { get; set; }
        public int CastMemberUserID { get; set; }

        [InverseProperty("Sender")]             //inverseproperty attribute is needed here becuase my Messages.cs has two nav property to this class
        public virtual ICollection<Message> SentMessages { get; set; }   //list of sent messages associated with user
        [InverseProperty("Recipient")]
        public virtual ICollection<Message> ReceivedMessages { get; set; }   //list of received messages associated with user

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

        public DbSet<ContentSection> ContentSections { get; set; }
        //public DbSet<CurrentProduction> CurrentProductions { get; set; }
        public DbSet<CastMember> CastMembers { get; set; }
        public DbSet<ProductionPhotos> ProductionPhotos { get; set; }

        /* We'll be using the fluent API to create a 0..1 to 0..1 relationship
         * between a CastMember and an ApplicationUser, since some cast-members 
         * won't have logins, and not all app-users will be cast members */
        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{

        //    modelBuilder.Entity<CastMember>()
        //        .HasOptional(u => u.CastMemberPerson)
        //        .WithRequired(p => p.CastMemberUser);

        //    modelBuilder.Entity<ApplicationUser>()
        //        .HasOptional(p => p.CastMemberUser)
        //        .WithRequired(u => u.CastMemberPerson);
        //}
        /* The above code doesn't jell with update-database due to 
         * "conflicting multiplicities," but I think it's close. 
         * The documentation on Fluent API and code first all seem to 
         * suggest that this kind of implicitly loaded zero-or-one-
         * to-zero-or-one relationship doesn't really exist... 
         * 
         * This code has been commented out along with the two commented 
         * virtual methods in CastMember and ApplicationUser, and should only
         * serve as a starting point for later stories that might want to 
         * tackle this further. */

        public DbSet<Part> Parts { get; set; }
        public DbSet<Production> Productions { get; set; }
        public DbSet<DisplayLinks> DisplayLinks { get; set; }
        public DbSet<DisplayInfo> DisplayInfo { get; set; }
        public DbSet<Sponsor> Sponsors { get; set; }
        public DbSet<Subscriber> Subscribers { get; set; }
        public DbSet<SeasonManager> SeasonManagers { get; set; }
        public DbSet<CalendarEvent> CalendarEvent { get; set; }
        public DbSet<Photo> Photo { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<Message> Messages { get; set; }
        public System.Data.Entity.DbSet<TheatreCMS.ViewModels.NewsletterListVm> NewsletterListVms { get; set; }

        public System.Data.Entity.DbSet<TheatreCMS.Models.Award> Awards { get; set; }
        public DbSet<SubscriptionPlan> SubscriptionPlan { get; set; }
    }

}