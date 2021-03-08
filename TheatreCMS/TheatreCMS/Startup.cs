using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
using Microsoft.AspNet;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.IO;
using System.Web;
using TheatreCMS.Models;
using TheatreCMS.Controllers;
using System;
using System.Linq;
using System.Drawing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Ajax.Utilities;
using System.Web.Caching;
using System.Data.Entity;
using TheatreCMS.Areas.Subscribers.Models;

[assembly: OwinStartupAttribute(typeof(TheatreCMS.Startup))]
namespace TheatreCMS
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            SeedSubscriptionPlans();
            createRolesandUsers();
            SeedCastPhotos();
            SeedCastMembers();
            SeedProductions();
            SeedProductionPhotos();
            SeedParts();
            SeedAwards();
            SeedSponsors();
            // Call methods here that are defined below.
            SeedCalendarEvents();
            SeedDisplayLinks();
            SeedDisplayInfo();


            //Seed10Users(); //add 10 members to the user list. slows application load, use as needed.
        }


        private ApplicationDbContext context = new ApplicationDbContext();

        //Generate 10 Members for testing the UserList
        private void Seed10Users()
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            //names for fake users
            var fnames = new List<string> { "Ted", "Molly", "Carlos", "Herman", "Jessica", "Ryan", "Dmitri", "Katherine", "Pat", "Juanita" };
            var lnames = new List<string> { "Tevez", "Garcia", "Walker", "Lewis", "Lopez", "Hall", "Smith", "Young", "Bruce", "Perez" };

            //add a new user, 10 times
            for (int i = 0; i < 10; i++)
            {
                var user = new ApplicationUser();
                user.UserName = $"{fnames[i]}{lnames[i]}{i + 1337}";
                user.FirstName = $"{fnames[i]}";
                user.LastName = $"{lnames[i]}";
                user.Email = $"{fnames[i]}{i}@gmail.com";
                user.StreetAddress = "123 Fake St.";
                user.City = "Portland";
                user.State = "OR";
                user.ZipCode = $"9721{i}";
                user.Role = "Member";
                string userPWD = $"asdf1234{i}";

                //check if create worked, add to UserManager
                if (userManager.Create(user, userPWD).Succeeded)
                {
                    userManager.AddToRole(user.Id, "Member");
                }
            }
        }

        //create method for default roles and Admin users for login
        private void createRolesandUsers()
        {


            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            //create Admin role on startup if none exist

            if (!roleManager.RoleExists("Admin"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Admin";
                roleManager.Create(role);

                var user = new ApplicationUser();
                user.UserName = "test";
                user.FirstName = "Queen";
                user.LastName = "LaSneefa";
                user.Email = "test@gmail.com";
                user.StreetAddress = "420 Beefsteak Ln";
                user.City = "Atlantis";
                user.State = "OR";
                user.ZipCode = "97212";
                user.Role = "Admin";

                string userPWD = "Passw0rd!";

                var chkUser = userManager.Create(user, userPWD);
                if (chkUser.Succeeded)
                {
                    var result1 = userManager.AddToRole(user.Id, "Admin");
                }
            }

            //Creating Member role
            if (!roleManager.RoleExists("Member"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Member";
                roleManager.Create(role);
            }

            var memuser = new ApplicationUser()
            {
                UserName = "memberTest",
                FirstName = "God",
                LastName = "Skrilla",
                Email = "member.test@gmail.com",
                StreetAddress = "314 Pi Ct",
                City = "Detroit",
                //State could be Enum later
                State = "FL",
                ZipCode = "12345",
                Role = "Member"
            };

            string memuserPWD = "Ih@ve12cats";

            var memchkUser = userManager.Create(memuser, memuserPWD);
            if (memchkUser.Succeeded)
            {
                var result1 = userManager.AddToRole(memuser.Id, "Member");
            }

            //

            //Creating Subscriber role
            if (!roleManager.RoleExists("Subscriber"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Subscriber";
                roleManager.Create(role);
            }

            //Seeding subscriber with subscripiton plan "Gold"
            var subscriptionplan = context.SubscriptionPlan.FirstOrDefault(p => p.SubscriptionLevel == "Gold");

            var subUser = new ApplicationUser()
            {
                UserName = "subscriberTest",
                FirstName = "Pamela",
                LastName = "Flanderson",
                Email = "subscriber.test@gmail.com",
                StreetAddress = "100 100th St",
                City = "Shire",
                State = "MA",
                ZipCode = "54321",
                Role = "Subscriber",
                SubscriberPerson = new Areas.Subscribers.Models.Subscriber()
                {
                    CurrentSubscriber = true,
                    HasRenewed = false,
                    Newsletter = true,
                    RecentDonor = false,
                    SubscriptionPlan = subscriptionplan,
                    //Generating SubscriberPerson data for Subscribers so can edit/delete from UserList page
                }
            };

            string subUserPWD = "100100St!";

            var chkSubUser = userManager.Create(subUser, subUserPWD);
            if (chkSubUser.Succeeded)
            {
                var result1 = userManager.AddToRole(subUser.Id, "Subscriber");
            }

            //create User role on startup if none exist

            if (!roleManager.RoleExists("User"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "User";
                roleManager.Create(role);

                var user = new ApplicationUser();
                user.UserName = "schwifty";
                user.FirstName = "Rick";
                user.LastName = "Sanchez";
                user.Email = "wubbalubbbadubdub@gmail.com";
                user.StreetAddress = "123 Smith Ave";
                user.City = "Rickland";
                user.State = "WA";
                user.ZipCode = "98101";
                user.Role = "User";

                string userPWD = "!Squanchie12";

                var chkUser = userManager.Create(user, userPWD);
                if (chkUser.Succeeded)
                {
                    var result1 = userManager.AddToRole(user.Id, "User");
                }
            }
        }

        //Three Subscripiton Plans available and associated attribute values
        private void SeedSubscriptionPlans()
        {
            var subscriptions = new List<SubscriptionPlan>
            {
                new SubscriptionPlan
                {
                    SubscriptionLevel = "Platinum",
                    PricePerYear = 500,
                    NumberOfShows = 100,
                },
            
                new SubscriptionPlan
                {
                    SubscriptionLevel = "Gold",
                    PricePerYear = 300,
                    NumberOfShows = 10,
                },
            
                new SubscriptionPlan
                {
                    SubscriptionLevel = "Silver",
                    PricePerYear = 200,
                    NumberOfShows = 5,
                }
            };
            //Saving Subscripition Plans to model and assigment of values to corresponding attributes
            subscriptions.ForEach(subscription => context.SubscriptionPlan.AddOrUpdate(p => new { p.SubscriptionLevel, p.PricePerYear, p.NumberOfShows },subscription));
            context.SaveChanges();
        }

        // I created a new method to seed the castmember images. I placed it before seedCastMembers() in order for those objects to reference the Photo database table
        private void SeedCastPhotos()
        {
            var converter = new ImageConverter();
            // create images first
            string imagesRoot = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + @"\Content\Images");

            Image image1 = Image.FromFile(Path.Combine(imagesRoot, @"London_Bauman.png"));
            Image image2 = Image.FromFile(Path.Combine(imagesRoot, @"JacQuelle_Davis.jpg"));
            Image image3 = Image.FromFile(Path.Combine(imagesRoot, @"Adriana_Gantzer.jpg"));
            Image image4 = Image.FromFile(Path.Combine(imagesRoot, @"Clara_Liis_Hillier.jpg"));
            Image image5 = Image.FromFile(Path.Combine(imagesRoot, @"Kaia_Maarja_Hillier.jpg"));
            Image image6 = Image.FromFile(Path.Combine(imagesRoot, @"Heath_Hyun_Houghton.jpg"));
            Image image7 = Image.FromFile(Path.Combine(imagesRoot, @"Tom_Mounsey.jpg"));
            Image image8 = Image.FromFile(Path.Combine(imagesRoot, @"Devon_Roberts.jpg"));

            var photos = new List<Photo>
            {
               new Photo
                {
                    OriginalHeight = image1.Height,
                    OriginalWidth = image1.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image1, typeof(byte[])),
                    Title = "London Bauman"
                },
                new Photo
                {
                    OriginalHeight = image2.Height,
                    OriginalWidth = image2.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image2, typeof(byte[])),
                    Title = "Jacquelle Davis"
                },
                new Photo
                {
                    OriginalHeight = image3.Height,
                    OriginalWidth = image3.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image3, typeof(byte[])),
                    Title = "Adriana Gantzer"
                },
                new Photo
                {
                    OriginalHeight = image4.Height,
                    OriginalWidth = image4.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image4, typeof(byte[])),
                    Title = "Clara-Liis Hillier"
                },
                new Photo
                {
                    OriginalHeight = image5.Height,
                    OriginalWidth = image5.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image5, typeof(byte[])),
                    Title = "Kaia Maarja Hillier"
                },
                new Photo
                {
                    OriginalHeight = image6.Height,
                    OriginalWidth = image6.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image6, typeof(byte[])),
                    Title = "Heath Hyun Houghton"
                },
                new Photo
                {
                    OriginalHeight = image7.Height,
                    OriginalWidth = image7.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image7, typeof(byte[])),
                    Title = "Tom Mounsey"
                },
                new Photo
                {
                    OriginalHeight = image8.Height,
                    OriginalWidth = image8.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image8, typeof(byte[])),
                    Title = "Devon Roberts"
                }
            };
            photos.ForEach(Photo => context.Photo.AddOrUpdate(p => p.PhotoFile, Photo));
            context.SaveChanges();
        }



        //Seeding database with dummy CastMembers
        private void SeedCastMembers()
        {
            //Add photos of cast members
            //string imagesRoot = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + @"\Content\Images");

            var castMembers = new List<CastMember>
            {
                // For each cast member I replaced CastMember.Photo with CastMember.PhotoId and assigned it the value of the corresponding Photo.PhotoId 
                new CastMember{Name = "London Bauman", MainRole = Enum.PositionEnum.Actor,
                Bio = "London Bauman is an actor, writer, sound designer, and Theatre Vertigo company member. " +
                "As an artist, he is interested in immersive physical theatre, magical realism, and new collaborative works." +
                " Selected work includes the role of Torch in Beirut (The Steep & Thorny Way to Heaven), " +
                "Barnaby the Barkeep in the devised Western Melodrama Bang! (Action/Adventure’s pilot season) " +
                "and sound design / original compositions for The Romeo and Juliet Project (Enso Theatre Ensemble). " +
                "In August, London will be traveling to the Edinburgh Fringe Festival in Scotland as Robert in Chet Wilson’s new play Gayface.",
                PhotoId = context.Photo.Where(photo => photo.Title == "London Bauman").FirstOrDefault().PhotoId,
                CurrentMember = true,},

                new CastMember{Name = "Jacquelle Davis", MainRole = Enum.PositionEnum.Actor,
                Bio = "Jacquelle Davis is a proud Portland native and member of Theatre Vertigo. " +
                "She studied acting at Willamette University. Jacquelle performs regularly with her beloved improv group, " +
                "No Filter. Her favorite roles include Jane Fonda in That Pretty Pretty; " +
                "Or, The Rape Play, and Box Worker 2 in Box. Jacquelle loves puns and pickles..",
                PhotoId = context.Photo.Where(photo => photo.Title == "Jacquelle Davis").FirstOrDefault().PhotoId,
                CurrentMember = true, },

                new CastMember{Name = "Adriana Gantzer", MainRole = Enum.PositionEnum.Actor,
                Bio = "Adriana has been a huge fan of Theatre Vertigo for many years and feels so fortunate to become " +
                "a part of this incredible company. She has been acting on stage for over a decade and has been a " +
                "full-time voiceover actor for over 4 years. Some favorite past roles include: Andy in A Dark Sky " +
                "Full of Stars, Adriana’s Theatre Vertigo debut; Matilde in The Clean House, Germaine in Picasso at " +
                "the Lapin Agile, and Georgeanne in Five Women Wearing the Same Dress. In her four years in Portland " +
                "she has worked with Milagro, NORTHWEST THEATRE WORKSHOP, Mask & Mirror, and Twilight theaters, " +
                "and at Prospect Theater Project in her hometown of Modesto, CA.",
                PhotoId = context.Photo.Where(photo => photo.Title == "Adriana Gantzer").FirstOrDefault().PhotoId,
                CurrentMember = true, },

                new CastMember{Name = "Clara-Liis Hillier", MainRole = Enum.PositionEnum.Actor,
                Bio = "Clara-Liis is a graduate of Reed College. A proud company member of Theatre Vertigo, " +
                "she is also a past resident actor for Bag&Baggage. She was last seen in Caucasian Chalk Circle " +
                "at Shaking the Tree, Gyspy at Broadway Rose, Godspell at Lakewood Theater " +
                "(Drammy Award for Supporting Actress), world premiere Carnivora as Woodwoman " +
                "(Theatre Vertigo) and as the Wicked Witch of the West in The Wizard of Oz (NW Children's Theater). " +
                "Favorite roles: Graeae Sister in Up The Fall with PHAME, Wait Until Dark (Susan Hendrix) with NWCTC, " +
                "Our Country's Good (Liz Morden) and Julius Caesar (Casca) with Bag&Baggage; The Seagull (Masha) with " +
                "NWCTC. When she's not onstage, Clara-Liis works for Portland Center Stage at The Armory as their" +
                " Education & Community Programs Associate and teaches Dance and Theater for NW Children's Theater " +
                "and Riverdale High School. Thank you to Heath K. for his love and patience and Mom and " +
                "Kaia for their strength and inspiration. For Ted.",
                PhotoId = context.Photo.Where(photo => photo.Title == "Clara-Liis Hillier").FirstOrDefault().PhotoId,
                CurrentMember = true, },

                new CastMember{Name = "Kaia Maarja Hillier", MainRole = Enum.PositionEnum.Actor,
                Bio = "Kaia is a current Theatre Vertigo company member, a Board and Company member of the Pulp Stage, " +
                "a No Filter Improv Troupe member, and Costume Designer. Past acting credits include: Jessica in A Maze" +
                " (Theatre Vertigo); Nora in Assistance (Theatre Vertigo); and April in The Best of Everything (Bag &Baggage)." +
                " Kaia has had SO much fun performing with the new ensemble in readings that celebrate Vertigo's " +
                "rich artistic past. Thank you to everyone who has come out to support the new ensemble and to " +
                "helping keep Theatre Vertigo and the Shoebox thriving-we need these space to stay alive and " +
                "let our community grow and share their art. Much love to Mom, the Ensemble, the Associate Artists, " +
                "Clara, and JQ.",
                PhotoId = context.Photo.Where(photo => photo.Title == "Kaia Maarja Hillier").FirstOrDefault().PhotoId,
                CurrentMember = true, },

                new CastMember{Name = "Heath Hyun Houghton", MainRole = Enum.PositionEnum.Actor,
                Bio = "A Korean American actor, writer and director.  He previously appeared with Theatre Vertigo in Assistance;" +
                " other Portland credits include work with Imago Theatre, Portland Shakespeare Project, Broadway Rose Theatre" +
                ", and many more.  Exploring the relationships between the sciences and the arts is a focal point of his work" +
                " as a collaborator and educator.",
                PhotoId = context.Photo.Where(photo => photo.Title == "Heath Hyun Houghton").FirstOrDefault().PhotoId,
                CurrentMember = true, },

                new CastMember{Name = "Tom Mounsey", YearJoined= 2012, MainRole = Enum.PositionEnum.Actor,
                Bio = "Tom found a passion for theatre and performance in his late 20s thanks to a class at Portland" +
                " Actors Conservatory, and has been acting in and around Portland since his graduation in 2008. " +
                "You might have seen him on stage at places like defunkt theatre, Imago Theatre, Northwest Classical " +
                "Theatre Collaborative, Action/Adventure Theatre, Lakewood Center for the Arts, Clackamas Repertory " +
                "Theatre, and of course, Theatre Vertigo. Tom was a member of Theatre Vertigo from 2012 to 2017, " +
                "and is very excited to be back as part of this amazing company.",
                PhotoId = context.Photo.Where(photo => photo.Title == "Tom Mounsey").FirstOrDefault().PhotoId,
                CurrentMember = true, },

                new CastMember{Name = "Devon Roberts", MainRole = Enum.PositionEnum.Actor,
                Bio = "Devon Roberts is a born and raised Portland director and actor. He holds a BA of Theater Arts " +
                "from Portland State University and is an alumnus of the Orchard Project Core Company. He has worked" +
                " with local companies: Boom Arts, Fuse Theatre Ensemble, Portland Center Stage at The Armory and out" +
                " of state: such as The Civilians, Tectonic Theater Project, Pig Iron and at the Edinburgh Fringe Festival." +
                " When Devon isn’t working on and off stage, he can be found enjoying the local cuisine, or soaking up" +
                " the natural beauty of Oregon. Devon is thankful for the opportunity to join the Vertigo Ensemble!",
                PhotoId = context.Photo.Where(photo => photo.Title == "Devon Roberts").FirstOrDefault().PhotoId,
                CurrentMember = true, },

                };

            castMembers.ForEach(castMember => context.CastMembers.AddOrUpdate(c => c.Name, castMember));
            context.SaveChanges();
        }

        //Seeding database with dummy SeedAwards
        private void SeedAwards()
        {
            var awards = new List<Award>
            {
                #region SeedAwardsData
                new Award
                {
                    Year = 2017, Name = "Drammy", Type = AwardType.Finalist, Category = "Sound Design", Recipient = "Andrew Bray",
                          ProductionId = context.Productions.Where(prod => prod.Title == "Assistance").FirstOrDefault().ProductionId,
                          //CastMemberId = context.CastMembers.Where(p => p.Name == "London Bauman").FirstOrDefault().CastMemberID,



                },
                 new Award
                {
                    Year = 2016, Name = "Drammy", Type = AwardType.Award, Category = "Outstanding Achievement", Recipient = "Scenic Artist,Mindy Barker",
                          ProductionId = context.Productions.Where(prod => prod.Title == "The Drunken City").FirstOrDefault().ProductionId,

                },
                new Award
                {
                    Year = 2016, Name = "Drammy", Type = AwardType.Finalist, Category = "Sound Design", Recipient = "Richard E.Moore",
                          ProductionId = context.Productions.Where(prod => prod.Title == "The Drunken City").FirstOrDefault().ProductionId,
                },
                new Award
                {
                    Year = 2015, Name = "Drammy", Type = AwardType.Award, Category = "Ensemble in a Play",
                          ProductionId = context.Productions.Where(prod => prod.Title == "Bob: A Life in Five Acts").FirstOrDefault().ProductionId,
                },
                new Award
                {
                    Year = 2015, Name = "Drammy", Type = AwardType.Award, Category = "Direction", Recipient = "Matthew B.Zrebski",
                          ProductionId = context.Productions.Where(prod => prod.Title == "Bob: A Life in Five Acts").FirstOrDefault().ProductionId ,
                },
                new Award
                {
                    Year = 2015, Name = "Drammy", Type = AwardType.Finalist, Category = "Ensemble in a Play",
                          ProductionId = context.Productions.Where(prod => prod.Title == "The School for Lies").FirstOrDefault().ProductionId,
                },
                new Award
                {
                    Year = 2014, Name = "Drammy", Type = AwardType.Award, Category = "Sound Design", Recipient = "Annalise Albright Woods",
                          ProductionId = context.Productions.Where(prod => prod.Title == "pool (no water)").FirstOrDefault().ProductionId,
                },
                new Award
                {
                    Year = 2014, Name = "Drammy", Type = AwardType.Finalist, Category = "Ensemble in a play ",
                          ProductionId = context.Productions.Where(prod => prod.Title == "pool (no water)").FirstOrDefault().ProductionId,
                },
                new Award
                {
                    Year = 2013, Name = "Drammy", Type = AwardType.Award, Category = "Actress in a supporting role", Recipient ="Brooke Calcagno",
                          ProductionId = context.Productions.Where(prod => prod.Title == "Mother Courage & Her Children").FirstOrDefault().ProductionId,
                },
                new Award
                {
                    Year = 2013, Name = "Drammy", Type = AwardType.Award, Category = "Sound design", Recipient ="Richard Moore",
                          ProductionId = context.Productions.Where(prod => prod.Title == "The Velvet Sky").FirstOrDefault().ProductionId,
                },
                new Award
                {
                    Year = 2012, Name = "Drammy", Type = AwardType.Award, Category = "Actor in a land role", Recipient ="Mario Calcagno",
                          ProductionId = context.Productions.Where(prod => prod.Title == "The American Pilot").FirstOrDefault().ProductionId,
                },
                new Award
                {
                    Year = 2012, Name = "Drammy", Type = AwardType.Award, Category = "Sound design", Recipient ="Em Gustason",
                          ProductionId = context.Productions.Where(prod => prod.Title == "The American Pilot").FirstOrDefault().ProductionId,
                },
                 new Award
                {
                    Year = 2010, Name = "Drammy", Type = AwardType.Award, Category = "Supporting Actress", Recipient ="Amy Newman",
                          ProductionId = context.Productions.Where(prod => prod.Title == "God's Ear").FirstOrDefault().ProductionId,
                },
                new Award
                {
                    Year = 2009, Name = "Drammy", Type = AwardType.Award, Category = "Supporting Actor", Recipient ="Garland Lyons",
                          ProductionId = context.Productions.Where(prod => prod.Title == "Romance").FirstOrDefault().ProductionId,
                },
                new Award
                {
                    Year = 2008, Name = "Drammy", Type = AwardType.Award, Category = "Outstanding puppeteering", OtherInfo = "in collaboration with Tears of Joy Theatre",
                          ProductionId = context.Productions.Where(prod => prod.Title == "The Long Christmas Ride Home").FirstOrDefault().ProductionId,
                },
                new Award
                {
                    Year = 2007, Name = "Drammy", Type = AwardType.Award, Category = "Set design", Recipient ="Ben Plont",
                          ProductionId = context.Productions.Where(prod => prod.Title == "Escape from Happiness").FirstOrDefault().ProductionId,
                },
                new Award
                {
                    Year = 2001, Name = "Drammy", Type = AwardType.Award, Category = "Supporting Actress", Recipient ="Lorraine Bahr",
                          ProductionId = context.Productions.Where(prod => prod.Title == "Lion in the Streets").FirstOrDefault().ProductionId,
                },
                new Award
                {
                    Year = 2001, Name = "Drammy", Type = AwardType.Award, Category = "Supporting Actress", Recipient ="Andrea White",
                          ProductionId = context.Productions.Where(prod => prod.Title == "Hellcab").FirstOrDefault().ProductionId,
                },
                new Award
                {
                    Year = 2000, Name = "Drammy", Type = AwardType.Award, Category = "Supporting Actor", Recipient ="Ted Schulz",
                          ProductionId = context.Productions.Where(prod => prod.Title == "The Grey Zone").FirstOrDefault().ProductionId,
                },
                new Award
                {
                    Year = 2000, Name = "Drammy", Type = AwardType.Award, Category = "Outstanding Direction", Recipient ="Michael Griggs",
                          ProductionId = context.Productions.Where(prod => prod.Title == "The Grey Zone").FirstOrDefault().ProductionId,
                },
                #endregion
            };

            //awards.ForEach(award => context.Awards.AddOrUpdate(aw => aw.AwardId, award));

            awards.ForEach(award => context.Awards.AddOrUpdate(a => new { a.Year, a.Recipient, a.Type, a.Category }, award));
            context.SaveChanges();
        }

        //Seeding database with dummy Productions
        private void SeedProductions()
        {
            var productions = new List<Production>
            {
                #region SeedProductionsData
                new Production{Title = "Hamilton", Playwright = "Lin-Manuel Miranda", Description = "This is a musical inspired by the biography " +
                "Alexander Hamilton by historian Ron Chernow. This musical tells the story of American Founding Father Alexander Hamilton through music " +
                "that draws heavily from hip hop, as well as R&B, pop, soul, and traditional-style show tune. ", Runtime=90, OpeningDay = new DateTime(2020, 01, 01, 19, 30, 00),
                ClosingDay = new DateTime(2020, 02, 29, 19, 30, 00), ShowtimeEve = new DateTime(2020, 01, 02, 19, 30, 00) , ShowtimeMat = new DateTime(2020, 01, 02, 14, 30, 00),
                TicketLink = "ticketsforyou.com", Season = 23},

                new Production{Title = "Phantom of the Opera", Playwright = "Andrew Lloyd Webber & Charles Hart", Description = "Based on a French " +
                "novel of the same name by Gaston Leroux, its central plot revolves around a beautiful soprano, Christine Daae, who becomes the obesession " +
                "of a mysterious, disfigured musical genius living in the subterranean labyrinth beneath the Paris Opera House.", Runtime=90, OpeningDay = new DateTime(2019, 10, 01, 17, 30, 00),
                ClosingDay = new DateTime(2019, 11, 30, 17, 30, 00), ShowtimeEve = new DateTime(2019, 10, 04, 17, 30, 00), ShowtimeMat = new DateTime(2019, 10, 04, 12, 30, 00),
                TicketLink = "ticketsforyou.com", Season = 23},

                new Production{Title = "The Book of Mormon", Playwright = "Trey Parker, Robert Lopez, & Matt Stone", Description = "The Book of Mormon " +
                "follows two Latter-Day Saints missionaries as they attempt to preach the faith of the Church of Jesus Christ of Latter-Day Saints to the " +
                "inhabitants of a remote Ugandan village.", Runtime=90, OpeningDay = new DateTime(2021, 01, 01, 19, 30, 00), ClosingDay = new DateTime(2021, 02, 28, 19, 30, 00),
                ShowtimeEve = new DateTime(2021, 01, 02, 19, 30, 00), ShowtimeMat = new DateTime(2021, 01, 02, 14, 30, 00), TicketLink = "ticketsforyou.com", Season = 24},

                new Production{Title = "Wicked", Playwright = "Stephen Schwartz", Description = "This musical is told from the perspective of the witches of " +
                "the Land of Oz; its plot begins before and continues after Dorothy Gale arrives in Oz from Kansas, and includes several references to the 1939 film.", Runtime=90,
                OpeningDay = new DateTime(2020, 10, 01, 19, 30, 00), ClosingDay = new DateTime(2020, 11, 30, 19, 30, 00),
                ShowtimeEve = new DateTime(2020, 10, 01, 19, 30, 00), ShowtimeMat = new DateTime(2020, 10, 01, 14, 30, 00), TicketLink = "ticketsforyou.com", Season = 24},

                new Production{Title = "How to Succeed in Business Without Really Trying", Playwright = "Frank Loesser", Description = "This story concerns young, " +
                "ambitious J. Pierrepont Finch, who, with the help of the book How to Succeed in Business Without Really Trying, rises from window washer to chairman of " +
                "the board of the World Wide Wicket Company.", Runtime=90, OpeningDay = new DateTime(2020, 04, 01, 19, 30, 00), ClosingDay = new DateTime(2020, 05, 31, 19, 30, 00),
                ShowtimeEve = new DateTime(2020, 04, 01, 19, 30, 00), ShowtimeMat = new DateTime(2020, 04, 01, 14, 30, 00), TicketLink = "ticketsforyou.com", Season = 23},

                // productions for season 20
                new Production{Title = "Assistance", Playwright = "Leslye Headland", Description = "The small army of administrative assistants belonging to Daniel " +
                "Weisinger, a famous and powerful business mogul, are a mixed bag of motives: suave, aggressive Vince despises his boss; the underdogs, clumsy Heather " +
                "and bitter Justin, are devoted to their jobs; obnoxiously competent Jenny is eager and confident that she will surpass her co-workers. Nick and Nora flirt " +
                "and joke to distract themselves from their unhappiness. But each assistant shares a deep ambition, desire for status, and twisted allegiance to their abusive" +
                " boss, even in the face of volatile power plays, unreasonable demands, and physical injury. Leslye Headland’s biting, fast-paced comedy Assistance, part of " +
                "her “Seven Deadly Sins” play cycle, explores greed and power both professional and personal, and the terrible, if hilarious, toll they can take on those who " +
                "are caught up in their pursuit and practice.",
                OpeningDay = new DateTime(2012, 05, 30, 14, 30, 00), ClosingDay = new DateTime(2012, 08, 30, 19, 30, 00), ShowtimeEve = new DateTime(2020, 05, 30, 19, 30, 00),
                ShowtimeMat = new DateTime(2012, 05, 30, 15, 30, 00), TicketLink = "ticketsforyou.com", Season = 20, Runtime = 120},

                new Production{Title = "Carnivora", Playwright = "Matthew B. Zrebski", Description = "Some of the dangerous seductions in “Carnivora” come courtesy of a very" +
                " un-mythic, albeit charismatic, cult leader (played by Dunkin) whose obsession is yet another fear, of sorts, that Zrebski has addressed. “Texting the Sun,”" +
                " a 2010 piece for the OCT/Kaiser Permanente Educational Theatre Program, dealt with concerns about a generation growing up fully immersed in interactive " +
                "technology and social media. In “Carnivora,” add in suspicions about the egocentric dynamics of an all-pervasive celebrity culture, and you have a " +
                "semi-coherent vision of society on the brink. Serve that up with an unhealthy dose of messianism, to folks frustrated by weakening economic and social-value" +
                " structures around them, and a bizarre cult web snags even the well-educated and well-meaning Lorraine.",
                OpeningDay = new DateTime(2012, 09, 01, 14, 30, 00), ClosingDay = new DateTime(2012, 12, 30, 19, 30, 00), ShowtimeEve = new DateTime(2012, 09, 01, 19, 30, 00),
                ShowtimeMat = new DateTime(2012, 12, 30, 15, 30, 00), TicketLink = "ticketsforyou.com", Runtime = 120, Season = 20},

                new Production{Title = "A Maze", Playwright = "Rob Handel", Description = "There are two kinds of mazes - a fairy tale-like king explains to his queen in " +
                "Rob Handel's \"A Maze\" - the kind you try to get through from one side to the other, and the kind where you either try to get from the entrance to the " +
                "center or from the center to the exit. The confounding and stimulating beauty of Handel's \"Maze\" is that it manages to be all those at once, and something" +
                " altogether different - a maze in which three paths may intersect or even merge.",
                OpeningDay = new DateTime(2013, 01, 02, 14, 30, 00), ClosingDay = new DateTime(2013, 03, 30, 15, 30, 00), ShowtimeEve = new DateTime(2012, 01, 02, 19, 30, 00),
                ShowtimeMat = new DateTime(2013, 01, 02, 15, 30, 00), TicketLink = "ticketsforyou.com", Runtime = 120, Season = 20},

                // productions for season 19
                new Production{Title = "The Drunken City", Playwright = "Adam Bock", Description = "Off on the bar crawl to end all crawls, three twenty-something brides-to-be" +
                " find their lives going topsy-turvy when one of them begins to question her future after a chance encounter with a recently jilted handsome stranger. " +
                "The Drunken City is a wildly theatrical take on the mystique of marriage and the ever-shifting nature of love and identity in a city that never sleeps.",
                OpeningDay = new DateTime(2013, 05, 30, 14, 30, 00), ClosingDay = new DateTime(2013, 08, 30, 19, 30, 00), ShowtimeEve = new DateTime(2013, 05, 30, 19, 30, 00),
                ShowtimeMat = new DateTime(2013, 05, 30, 15, 30, 00), TicketLink = "ticketsforyou.com", Runtime = 120, Season = 19},

                new Production{Title = "I Want to Destroy You", Playwright = "Rob Handel", Description = "Life’s not going especially well for Harold, a sort-of-famous artist" +
                " who’s now teaching grad students at a richer-than-thou private college, and he really likes it, actually, but there are … problems. His ex-wife is out of " +
                "the picture somewhere – California, it sounds like – and his teenage daughter, Micki, comes to visit, prodding him for more of a relationship than he seems" +
                " willing to commit to. His roof’s got a bad leak, and he’s unfortunately seriously ticked off the roofer, Andy. He’s up for tenure, but his friend and mentor" +
                " Bob is in a hospital, dying, and the school dean, a crafty-smooth politico named Stephanie (everyone’s on a chummy first-name basis around here, even when " +
                "they’re decidedly not chums), seems strangely unsympathetic: downright threatening, you might say. Then there’s Mark, the weirdo grad student, who comes to" +
                " class to give a presentation on a conceptual piece, and in the process starts waving a handgun around. Which very much freaks out the other students, earnest" +
                " Ilich and leafy Leaf, and throws a serious scare into Harold, which is both completely understandable and a tad ironic, because, after all, the work that " +
                "made Harold famous and a prize catch for the richer-than-thou college in the first place was the performance piece where he had himself shot. With a rifle." +
                " What goes around, as they say, comes around. And on the other end of things, it looks scary.",
                OpeningDay = new DateTime(2013, 09, 01, 14, 30, 00), ClosingDay = new DateTime(2013, 12, 30, 19, 30, 00), ShowtimeEve = new DateTime(2013, 09, 01, 19, 30, 00),
                ShowtimeMat = new DateTime(2013, 09, 01, 15, 30, 00), TicketLink = "ticketsforyou.com", Runtime = 120, Season = 19, IsWorldPremiere= true},

                new Production{Title = "Love and Information", Playwright = "Caryl Churchill", Description = "Someone sneezes. Someone can’t get a signal. Someone won’t " +
                "answer the door. Someone put an elephant on the stairs. Someone’s not ready to talk. Someone is her brother’s mother. Someone hates irrational numbers. " +
                "Someone told the police. Someone got a message from the traffic light. Someone’s never felt like this before.\n In this fast moving kaleidoscope, more " +
                "than a hundred characters try to make sense of what they know.",
                OpeningDay = new DateTime(2014, 01, 02, 14, 30, 00), ClosingDay = new DateTime(2014, 03, 30, 19, 30, 00), ShowtimeEve = new DateTime(2014, 01, 02, 19, 30, 00),
                ShowtimeMat = new DateTime(2014, 01, 02, 15, 30, 00), TicketLink = "ticketsforyou.com", Runtime = 120, Season = 19},

                // productions for season 18
                new Production{Title = "Bob: A Life in Five Acts", Playwright = "Peter Sinn Nachtrieb", Description = "BOB chronicles the highly unusual life of Bob and his" +
                " lifelong quest to become a “Great Man.” Born and abandoned in the bathroom of a fast food restaurant, Bob energetically embarks on an epic journey across" +
                " America and encounters inspiring generosity, crushing hardships, blissful happiness, stunning coincidences, wrong turns, lucky breaks, true love and " +
                "heartbreaking loss. Along the way, Bob meets a myriad of fellow countrymen all struggling to find their own place in the hullaballoo of it all. Will Bob’s" +
                " real life ever be able to live up to his dream? BOB is a comedic exploration of American mythology and values, the treacherous pursuit of happiness, and" +
                " discovering what it means to be truly “great.\"",
                OpeningDay = new DateTime(2014, 05, 30, 14, 30, 00), ClosingDay = new DateTime(2014, 08, 30, 19, 30, 00), ShowtimeEve = new DateTime(2014, 05, 30, 19, 30, 00),
                ShowtimeMat = new DateTime(2014, 05, 30, 15, 30, 00), TicketLink = "ticketsforyou.com", Runtime = 120, Season = 18},

                new Production{Title = "Sexual Neuroses of Our Parents", Playwright = "(translated from Lukas Bärfuss) by Neil Blackadder", Description = "Dora has been " +
                "taking tranquilizers for years because otherwise her behavior was uncontrollably wild. The drugs have kept her in a semi-comatose state, and now Dora’s" +
                " mother wants to know what her daughter is really like and convinces their doctor to take Dora off the drugs. Dora develops an enormous hunger for life," +
                " demonstrates her own will, and, above all, discovers her sexuality—to a degree which far exceeds adult ideas about how she should live.",
                OpeningDay = new DateTime(2014, 09, 01, 14, 30, 00), ClosingDay = new DateTime(2014, 12, 30, 19, 30, 00), ShowtimeEve = new DateTime(2012, 09, 01, 19, 30, 00),
                ShowtimeMat = new DateTime(2014, 09, 01, 15, 30, 00), TicketLink = "ticketsforyou.com", Runtime = 120, Season = 18},

                new Production{Title = "The School for Lies", Playwright = "David Ives", Description = "Adapted from Le Misanthrope by Molière. It’s 1666 and the brightest," +
                " wittiest salon in Paris is that of Celimene, a beautiful young widow so known for her satiric tongue she’s being sued for it. Surrounded by shallow " +
                "suitors, whom she lives off of without surrendering to, Celimene has managed to evade love since her beloved husband died—until today, when Frank appears. " +
                "A traveler from England known for his own coruscating wit and acidic misanthropy, Frank turns Celimene’s world upside-down, taking on her suitors," +
                " matching her barb for barb, and teaching her how to live again. (Never mind that their love affair has been engineered by a couple of well-placed lies.) " +
                "This wild farce of furious tempo and stunning verbal display, all in very contemporary couplets, runs variations on Molière’s The Misanthrope, which " +
                "inspired it. Another incomparable romp from the brilliant author of All in the Timing.",
                OpeningDay = new DateTime(2015, 01, 02, 14, 30, 00), ClosingDay = new DateTime(2015, 03, 30, 19, 30, 00), ShowtimeEve = new DateTime(2015, 01, 02, 19, 30, 00),
                ShowtimeMat = new DateTime(2015, 01, 02, 15, 30, 00), TicketLink = "ticketsforyou.com", Runtime = 120, Season = 18},

                // productions for season 17
                new Production{Title = "Dr. Jekyll and Mr. Hyde", Playwright = "Jeffrey Hatcher", Description = "Some of the strongest scenes here are between Hyde and his" +
                " love interestt, a fiery chambermaid named Elizabeth. In a twist on the Jekyll and Hyde story added by playwright Jeffrey Hatcher, Elizabeth sees through" +
                " Hyde’s anger and falls hopelessly in love with him. Their romantic scenes let us see a new and highly relatable side of Jekyll’s alter ego as the character " +
                "struggles to overcome his fears to be with the person he loves. It helps that Wennstrom and Koerschgen, whose interactions are backed only by the sound of" +
                " a pounding heartbeat, have palpable, natural chemistry.",
                OpeningDay = new DateTime(2015, 05, 30, 14, 30, 00), ClosingDay = new DateTime(2015, 08, 30, 19, 30, 00), ShowtimeEve = new DateTime(2015, 05, 30, 19, 30, 00),
                ShowtimeMat = new DateTime(2015, 05, 30, 15, 30, 00), TicketLink = "ticketsforyou.com", Runtime = 120, Season = 17},

                new Production{Title = "The End of Sex", Playwright = "Craig Jessen", Description = "The story centers around Sam (Stephanie Cordell) who is the head of " +
                "one team in a research lab, attempting to find a safe cream or pill that will increase sexual activity for those having trouble in that area.  She is " +
                "aided by her able assistant, Zoe (Beth Thompson) and, together, are trying to be the first team to find that secret, so that they will get the fame and " +
                "monies due them.",
                OpeningDay = new DateTime(2015, 09, 01, 14, 30, 00), ClosingDay = new DateTime(2015, 12, 30, 19, 30, 00), ShowtimeEve = new DateTime(2015, 09, 01, 19, 30, 00),
                ShowtimeMat = new DateTime(2015, 09, 01, 15, 30, 00), TicketLink = "ticketsforyou.com", Runtime = 120, Season = 17},

                new Production{Title = "pool (no water)", Playwright = "Mark Ravenhill", Description = "A famous artist invites her old friends to her luxurious new home." +
                " For one night only, the group is back together. But celebrations come to an abrupt end when the host suffers an horrific accident. As the victim lies in" +
                " a coma, an almost unthinkable plan starts to take shape: could her suffering be their next work of art? Pool (No Water) is a visceral and shocking new" +
                " play about the fragility of friendship and the jealousy and resentment inspired by success.",
                OpeningDay = new DateTime(2016, 01, 02, 14, 30, 00), ClosingDay = new DateTime(2016, 03, 30, 19, 30, 00), ShowtimeEve = new DateTime(2016, 01, 02, 19, 30, 00),
                ShowtimeMat = new DateTime(2016, 01, 02, 15, 30, 00), TicketLink = "ticketsforyou.com", Runtime = 120, Season = 17},

                new Production{Title = "Anonymous Theatre:  The Crucible", Playwright = "Arthur Miller", Description = "Salem, Massachusetts—1692.  Rigid piety huddles on" +
                " the edge of the new world’s wilderness.\nIts inhabitants believe unquestioningly in their own sanctity, but in Arthur Miller’s edgy masterpiece, that very" +
                " belief will have poisonous consequences when a vengeful teenager accuses a rival of witchcraft—and then those accusations multiply to consume the entire village.",
                OpeningDay = new DateTime(2016, 04, 01, 14, 30, 00), ClosingDay = new DateTime(2016, 05, 29, 19, 30, 00), ShowtimeEve = new DateTime(2016, 04, 01, 19, 30, 00),
                ShowtimeMat = new DateTime(2016, 04, 01, 15, 30, 00), TicketLink = "ticketsforyou.com", Runtime = 120, Season = 17},

                // productions for season 16
                new Production{Title = "Mother Courage & Her Children", Playwright = "(translated from Bertolt Brecht) by Tony Kushner", Description = "The indomitable" +
                " Mother Courage follows one luckless army after another across a war-torn world in her canteen wagon. She’ll do anything to hold onto her money-making " +
                "wagon, even if it means the loss of her children.",
                OpeningDay = new DateTime(2016, 05, 30, 14, 30, 00), ClosingDay = new DateTime(2016, 08, 30, 19, 30, 00), ShowtimeEve = new DateTime(2020, 08, 30, 19, 30, 00),
                ShowtimeMat = new DateTime(2016, 05, 30, 15, 30, 00), TicketLink = "ticketsforyou.com", Runtime = 120, Season = 16},

                new Production{Title = "The Velvet Sky", Playwright = "Roberto Aguirre-Sacasa", Description = "Poor Bethany Palmer hasn’t slept in thirteen years. When her" +
                " husband, Warren, steals their son, Andrew, away in the middle of the night, her already fragile grip on reality starts to weaken—even as she sets off after" +
                " them on a nightmarish phantasmagoria through an urban dreamscape. Chronicling Bethany’s desperate flight, THE VELVET SKY is a dark fairytale for grown-ups," +
                " about the stories and lies adults tell children to keep them safe from the things that lurk in the dark. Things like the macabre Sandman, who is hungry to" +
                " steal the innocent gleam from young Andrew’s eyes…",
                OpeningDay = new DateTime(2016, 09, 01, 14, 30, 00), ClosingDay = new DateTime(2016, 12, 30, 19, 30, 00), ShowtimeEve = new DateTime(2016, 09, 01, 19, 30, 00),
                ShowtimeMat = new DateTime(2016, 09, 01, 15, 30, 00), TicketLink = "ticketsforyou.com", Runtime = 120, Season = 16},

                new Production{Title = "Aloha, Say the Pretty Girls", Playwright = "Naomi Iizuka", Description = "Aloha, Say the Pretty Girls is a quirky journey that " +
                "follows a bunch of twenty-somethings in their quest for love and identity. Strangers, friends, lovers, and acquaintances travel the globe from Alaska to" +
                " Hawaii and from New York City to Inner Borneo, exploring themes of migration, evolution, and interconnectivity. Babies, wild dogs, komodo dragons, and" +
                " hula dancers abound in this hilarious play about finding your tribe in a world gone haywire.",
                OpeningDay = new DateTime(2017, 01, 02, 14, 30, 00), ClosingDay = new DateTime(2017, 03, 30, 19, 30, 00), ShowtimeEve = new DateTime(2017, 01, 02, 19, 30, 00),
                ShowtimeMat = new DateTime(2017, 01, 02, 15, 30, 00), TicketLink = "ticketsforyou.com", Runtime = 120, Season = 16},

                new Production{Title = "Anonymous Theatre:  The Skin of Our Teeth", Playwright = "Thornton Wilder", Description = "This comedic masterpiece spans the " +
                "entirety of history, with one ordinary American family who lives through it all. Dad’s just invented the wheel, Cain is throwing rocks at the neighbor " +
                "kid, mammoths and dinosaurs lounge in the family room and mom frets about how to get all those animals on the boat two by two.\nThrough Ice Ages, biblical " +
                "floods and political conventions, the Antrobus family of Excelsior, New Jersey perseveres. With a giant cast and time-set across the ages, this theatrical" +
                " allegory captures the human spirit – of brilliance, idiocy and ultimately sweet survival.",
                OpeningDay = new DateTime(2017, 04, 01, 14, 30, 00), ClosingDay = new DateTime(2017, 05, 28, 19, 30, 00), ShowtimeEve = new DateTime(2017, 04, 01, 19, 30, 00),
                ShowtimeMat = new DateTime(2017, 04, 01, 15, 30, 00), TicketLink = "ticketsforyou.com", Runtime = 120, Season = 16},

                //productions for season 15

                new Production{Title = "Cloud 9", Playwright = "Caryl Churchill", Description = "Clive, A British colonial administrator, lives with his family, " +
                "a governess and servant during turbulent times in Africa. The natives are rioting and Mrs Saunders, a widow, comes to them to seek safety. "+
                "Her arrival is soon followed by Harry Bagley, an explorer. The governess Ellen, who reveals herself to be a lesbian, is forced into marriage " +
                "with Harry after his sexuality is discovered and condemned by Clive.",
                OpeningDay = new DateTime(2011, 06, 01, 14, 00, 00), ClosingDay = new DateTime(2011, 06, 30, 19, 00, 00), ShowtimeEve = new DateTime(2011, 06, 01, 19, 00, 00),
                ShowtimeMat = new DateTime(2011, 06, 01, 14, 00, 00), TicketLink = "ticketsforyou.com", Runtime = 165, Season = 15},

                new Production{Title = "Hunter Gatherers", Playwright = "Peter Sinn Nachtrieb", Description = "Pam and Richard are hosting their best friends, " +
                "Wendy and Tom, for an annual dinner get-together. An animal sacrifice kicks off the evening, followed by a little more sex, violence, deception, " +
                "wrestling, and dancing than at previous parties. A darkly comic evening where the line between civilized and primal man is blurred, " +
                "and where not everyone will survive long enough to enjoy the brownies for dessert." ,
                OpeningDay = new DateTime(2011, 08, 01, 14, 00, 00), ClosingDay = new DateTime(2011, 08, 30, 19, 00, 00), ShowtimeEve = new DateTime(2011, 08, 01, 19, 00, 00),
                ShowtimeMat = new DateTime(2011, 08, 01, 14, 00, 00), TicketLink = "ticketsforyou.com", Runtime =120, Season = 15},

                new Production{Title = "The American Pilot", Playwright = "David Greig", Description = "An American pilot has crash-landed in a distant country "+
                "rent by civil war. Since the Americans fund the country's oppressive government and the pilot has landed in rural, rebel territory, " +
                "he represents both a temptation and an opportunity." ,
                OpeningDay = new DateTime(2011, 10, 01, 14, 00, 00), ClosingDay = new DateTime(2011, 11, 01, 19, 00, 00), ShowtimeEve = new DateTime(2011, 10, 01, 19, 00, 00),
                ShowtimeMat = new DateTime(2011, 10, 01, 14, 00, 00), TicketLink = "ticketsforyou.com", Runtime = 90, Season = 15},

                new Production{Title = "Anonymous Theatre: The Good Doctor", Playwright = "Neil Simon", Description = "A combination of Neil Simon and Chekhov, "+
                "The Good Doctor, a comedy with music, is, by turns, charming, hilarious, sad, and touching. It centers around a writer, "+
                "who speaks to the audience and shares his writing with them, throughout one day. He presents them with a plethora of scenes: some are from his childhood, " +
                "others are his family and friends, and still others are his own life experiences. The audience meets a variety of characters, all of whom " +
                "are immediately relatable and strikingly human." ,
                OpeningDay = new DateTime(2011, 12, 01, 14, 00, 00), ClosingDay = new DateTime(2011, 12, 30, 19, 00, 00), ShowtimeEve = new DateTime(2011, 12, 01, 19, 00, 00),
                ShowtimeMat = new DateTime(2011, 12, 01, 14, 00, 00), TicketLink = "ticketsforyou.com", Runtime = 120, Season = 15},

                // productions for season 14

                new Production{Title = "Dead Man's Cell Phone", Playwright = "Sarah Ruhl", Description = "Dead Man's Cell Phone explores the paradox of modern " +
                "technology's ability to both unite and isolate people in the digital age. The play was awarded a Helen Hayes Award for Outstanding New Play.",
                OpeningDay = new DateTime(2010, 6, 01, 14, 00, 00), ClosingDay = new DateTime(2010, 6, 30, 14, 00, 00), ShowtimeEve = new DateTime(2010, 6, 01, 19, 00, 00),
                ShowtimeMat = new DateTime(2010, 6, 01, 14, 00, 00), TicketLink = "ticketsforyou.com", Runtime = 120, Season = 14},

                new Production{Title = "99 Ways to Fuck a Swan", Playwright = "Kim Rosenstock", Description = "A long, long time ago, Leda makes love to a swan. 3,000 years later, "+
                "Michelangelo paints a picture. 350 years later, Rudolph buys it. 130 years later, Dave and Fiona stand in a museum, gazing at what remains. " +
                "Set in a world of bizarre romantic obsessions and everyday ineptitude, 99 WAYS TO FUCK A SWAN explores the dark corners of desire and the eternal mysteries of love",
                OpeningDay = new DateTime(2010, 8, 01, 14, 00, 00), ClosingDay = new DateTime(2010, 8, 30, 14, 00, 00), ShowtimeEve = new DateTime(2010, 8, 01, 19, 00, 00),
                ShowtimeMat = new DateTime(2010, 6, 01, 14, 00, 00), TicketLink = "ticketsforyou.com", Runtime = 120, Season = 14},

                new Production{Title = "The Adding Machine", Playwright = "Elmer Rice", Description = "Elmer Rice’s The Adding Machine centers around the life, death, and afterlife, "+
                "of a man named Zero. He and his wife live in a society dominated by reverence for financial gain and opportunism, with an emphasis on morality and rigid determinations " +
                "of what is right and wrong. The day after Mrs. Zero successfully plants a seed of ambition in her husband, Mr. Zero is fired from his monotonous accounting job. " +
                "Having come in for the day hoping to ask for a raise for his consistent work, Zero’s spirits are crushed when his boss tells him he is being replaced by an adding machine.",
                OpeningDay = new DateTime(2010, 10, 01, 14, 00, 00), ClosingDay = new DateTime(2010, 10, 30, 14, 00, 00), ShowtimeEve = new DateTime(2010, 10, 01, 19, 00, 00),
                ShowtimeMat = new DateTime(2010, 10, 01, 14, 00, 00), TicketLink = "ticketsforyou.com", Runtime = 100, Season = 14},

                new Production{Title = "Anonymous Theatre: You Can't Take It with You", Playwright = "George S. Kaufman and Moss Hart", Description = "Family can do crazy things to people." +
                "And the Sycamore family is a little crazy to begin with. Wily Grandpa Vanderhof, heads the wacky Sycamore household and is also a leader of a happily eccentric gang " +
                "of snake collectors, cunning revolutionaries, ballet dancers and skyrocket makers. But when the youngest daughter brings her fiancé and his buttoned-up parents over " +
                "for dinner, that’s when the real fireworks start to fly.",
                OpeningDay = new DateTime(2011, 1, 01, 14, 00, 00), ClosingDay = new DateTime(2011, 1, 31, 14, 00, 00), ShowtimeEve = new DateTime(2011, 1, 01, 19, 00, 00),
                ShowtimeMat = new DateTime(2011, 1, 01, 14, 00, 00), TicketLink = "ticketsforyou.com", Runtime = 120, Season = 14},

                // productions for Season 13

                new Production{Title = "The Ruby Sunrise", Playwright = "Rinne Groff", Description = "Setting off from a farm in Indiana as a young girl Ruby struggles to turn " +
                "her dream of the first all-electrical television system into a reality. This play also jumps forward to a McCarthy-era New York TV studio where Ruby’s heirs fight over "+
                "how her story should be told.",
                OpeningDay = new DateTime(2009, 6, 01, 14, 00, 00), ClosingDay = new DateTime(2009, 6, 30, 14, 00, 00), ShowtimeEve = new DateTime(2009, 6, 01, 19, 00, 00),
                ShowtimeMat = new DateTime(2009, 6, 01, 14, 00, 00), TicketLink = "ticketsforyou.com", Runtime = 120, Season = 13},

                new Production{Title = "Boom", Playwright = "Peter Sinn Nachtrieb", Description = "Jules, a grad student in marine biology, and Jo, a journalism student, "+
                "meet one Saturday night in Jules’s small underground laboratory on a university campus, after Jo answers Jules’s online personal ad offering an encounter that promises "+
                "/“to change the course of the world./” During his research on a deserted tropical island, Jules discovers patterns among the behavior of fish that perdict the end of most earthly life. " +
                "He then turns his tiny lab/apartment into a place to wait out the disaster and begin remaking humanity.",
                OpeningDay = new DateTime(2009, 8, 01, 14, 00, 00), ClosingDay = new DateTime(2009, 8, 30, 14, 00, 00), ShowtimeEve = new DateTime(2009, 8, 01, 19, 00, 00),
                ShowtimeMat = new DateTime(2009, 8, 01, 14, 00, 00), TicketLink = "ticketsforyou.com", Runtime = 120, Season = 13},

                new Production{Title = "God's Ear", Playwright = "Jenny Schwartz", Description = "God's Ear follows a family in the aftermath of the tragic drowning of their son. " +
                "The characters try their hardest to reach out to one another, but socially prescribed behaviors and language, meant to help them, never quite do. Though not entirely " +
                "set in reality, God's Ear is honest and genuine in its exploration of grief. ",
                OpeningDay = new DateTime(2009, 10, 01, 14, 00, 00), ClosingDay = new DateTime(2009, 10, 30, 14, 00, 00), ShowtimeEve = new DateTime(2009, 10, 01, 19, 00, 00),
                ShowtimeMat = new DateTime(2009, 10, 01, 14, 00, 00), TicketLink = "ticketsforyou.com", Runtime = 120, Season = 13},

                new Production{Title = "Anonymous Theatre: Lend Me A Tenor", Playwright = "Ken Ludwig", Description = "World-renowned tenor Tito Merelli has signed on to play Otello " +
                "at a Cleveland opera company in the fall of 1934. He arrives late and, through a set of crazy circumstances, passes out after mixing wine with a huge dose of tranquilizers. "+
                "Believing that the divo is dead, the excitable opera manager taps his hapless assistant, an aspiring singer named Max, to suit up as the Moor and replace Merelli. "+
                "Meanwhile, the tenor’s jealous wife, his ambitious female co-star, Max’s young girlfriend and the flirtatious head of the opera guild are on the scene fighting "+
                "for the star’s attention.",
                OpeningDay = new DateTime(2010, 1, 01, 14, 00, 00), ClosingDay = new DateTime(2010, 1, 31, 14, 00, 00), ShowtimeEve = new DateTime(2010, 1, 01, 19, 00, 00),
                ShowtimeMat = new DateTime(2010, 1, 01, 14, 00, 00), TicketLink = "ticketsforyou.com", Runtime = 120, Season = 13},



                // productions for Season 12

                new Production{Title = "Pterodactyls", Playwright = "Nicky Silver", Description = "Pterodactyls is an absurdist black comedy about the demise of the Duncan family, "+
                "and, by extension, the species. Emma Duncan, a hypochondriac with memory problems, and her orphaned fiancé, Tommy, confront her mother, Grace, with the news of their "+
                "intended marriage. Disapproving at first, Grace acquiesces and puts Tommy to work as a maid",
                OpeningDay = new DateTime(2008, 6, 01, 14, 00, 00), ClosingDay = new DateTime(2008, 6, 30, 14, 00, 00), ShowtimeEve = new DateTime(2008, 6, 01, 19, 00, 00),
                ShowtimeMat = new DateTime(2008, 6, 01, 14, 00, 00), TicketLink = "ticketsforyou.com", Runtime = 120, Season = 12},

                new Production{Title = "Romance", Playwright = "David Mamet", Description = "Pulitzer Prize—winning playwright David Mamet's Romance is an uproarious, take-no-prisoners "+
                "courtroom comedy that gleefully lampoons everyone from lawyers and judges, to Arabs and Jews, to gays and chiropractors. It's hay fever season, and in a courtroom "+
                "a judge is popping antihistamines.",
                OpeningDay = new DateTime(2008, 8, 01, 14, 00, 00), ClosingDay = new DateTime(2008, 8, 30, 14, 00, 00), ShowtimeEve = new DateTime(2008, 8, 01, 19, 00, 00),
                ShowtimeMat = new DateTime(2008, 8, 01, 14, 00, 00), TicketLink = "ticketsforyou.com", Runtime = 120, Season = 12},

                new Production{Title = "Freakshow", Playwright = "Carson Kreitzer", Description = "At the turn of the last century, a traveling Freakshow grinds to a halt. "+
                "Things are changing. The anger of being stared at, trapped, caged, is at war with the comfort of knowing your place in the universe. But the growing defiance "+
                "of the freaks is no act. Will the Dog Faced Woman break her bonds from the show, collapsing the fragile bubble of sustaining interdependence? "+
                "Can the jaded Ringmaster, a profoundly broken man, find redemption through his love for the Woman With No Arms and No Legs? In this poetic, gritty world, "+
                "there may only be one way out.",
                OpeningDay = new DateTime(2008, 10, 01, 14, 00, 00), ClosingDay = new DateTime(2008, 10, 30, 14, 00, 00), ShowtimeEve = new DateTime(2008, 10, 01, 19, 00, 00),
                ShowtimeMat = new DateTime(2008, 10, 01, 14, 00, 00), TicketLink = "ticketsforyou.com", Runtime = 120, Season = 12},

                new Production{Title = "Anonymous Theatre: Macbeth", Playwright = "William Shakespeare", Description = "Macbeth is a Scottish general and the thane of Glamis "+
                "who is led to wicked thoughts by the prophecies of the three witches, especially after their prophecy that he will be made thane of Cawdor comes true. "+
                "Macbeth is a brave soldier and a powerful man, but he is not a virtuous one.",
                OpeningDay = new DateTime(2009, 1, 01, 14, 00, 00), ClosingDay = new DateTime(2009, 1, 31, 14, 00, 00), ShowtimeEve = new DateTime(2009, 1, 01, 19, 00, 00),
                ShowtimeMat = new DateTime(2009, 1, 01, 14, 00, 00), TicketLink = "ticketsforyou.com", Runtime = 120, Season = 12},


                // productions for Season 11

                new Production{Title = "A Doll's House  ", Playwright = "Henrik Ibsen", Description = "The play centres on an ordinary family—Torvald Helmer, a bank lawyer, "+
                "his wife Nora, and their three little children. Torvald supposes himself the ethical member of the family, while his wife assumes the role of the pretty " +
                "and irresponsible little woman in order to flatter him.",
                OpeningDay = new DateTime(2007, 6, 01, 14, 00, 00), ClosingDay = new DateTime(2007, 6, 30, 14, 00, 00), ShowtimeEve = new DateTime(2007, 6, 01, 19, 00, 00),
                ShowtimeMat = new DateTime(2007, 6, 01, 14, 00, 00), TicketLink = "ticketsforyou.com", Runtime = 120, Season = 11},

                new Production{Title = "Where's My Money? ", Playwright = "John Patrick Shanley", Description = "When Celeste, an out-of-work actor who's cheating on her boyfriend " +
                "with a married man, runs into Natalie, whom she hasn't seen in years, the two have some catching up to do. Natalie, an accountant married to a lawyer, "+
                "gives the impression of being very together and does not approve of Celeste's lifestyle.",
                OpeningDay = new DateTime(2007, 8, 01, 14, 00, 00), ClosingDay = new DateTime(2007, 8, 30, 14, 00, 00), ShowtimeEve = new DateTime(2007, 8, 01, 19, 00, 00),
                ShowtimeMat = new DateTime(2007, 8, 01, 14, 00, 00), TicketLink = "ticketsforyou.com", Runtime = 120, Season = 11},

                new Production{Title = "The Long Christmas Ride Home", Playwright = "Paula Vogel", Description = "The Long Christmas Ride Home dramatises a road trip by two parents " +
                "and their three young children to visit grandparents for the Christmas holiday, and the emotional turmoil that they undergo. A significant element of the production schema " +
                "is a Western, contemporary employment of bunraku, an ancient form of Japanese puppetry",
                OpeningDay = new DateTime(2007, 10, 01, 14, 00, 00), ClosingDay = new DateTime(2007, 10, 30, 14, 00, 00), ShowtimeEve = new DateTime(2007, 10, 01, 19, 00, 00),
                ShowtimeMat = new DateTime(2007, 10, 01, 14, 00, 00), TicketLink = "ticketsforyou.com", Runtime = 120, Season = 11},

                new Production{Title = "Anonymous Theatre: Rumors", Playwright = "Neil Simon", Description = "The play starts with Ken Gorman and his wife, Chris Gorman, " +
                "at the 10th anniversary party of Charlie Brock, the Deputy Mayor of New York, and his wife, Myra. Unfortunately, things are not going quite to plan. " +
                "All the kitchen staff are gone, Myra is missing, and Charlie has shot himself in the head",
                OpeningDay = new DateTime(2008, 1, 01, 14, 00, 00), ClosingDay = new DateTime(2008, 1, 31, 14, 00, 00), ShowtimeEve = new DateTime(2008, 1, 01, 19, 00, 00),
                ShowtimeMat = new DateTime(2008, 1, 01, 14, 00, 00), TicketLink = "ticketsforyou.com", Runtime = 120, Season = 11},



                // productions for Season 10
                new Production{Title = "Valparaiso", Playwright = "Don DeLillo", Description = "Valparaiso is Don DeLillo 's second play, in which a man suddenly "+
                "becomes famous following a mistake in the itinerary of an ordinary business trip which takes him to Valparaíso, Chile, instead of Valparaiso, Indiana. " +
                "The 1999 play, which incorporates live performance with video projection, looks at how the media has affected modern mankind.",
                OpeningDay = new DateTime(2006, 6, 01, 14, 00, 00), ClosingDay = new DateTime(2006, 6, 30, 14, 00, 00), ShowtimeEve = new DateTime(2006, 6, 01, 19, 00, 00),
                ShowtimeMat = new DateTime(2006, 6, 01, 14, 00, 00), TicketLink = "ticketsforyou.com", Runtime = 120, Season = 10},

                new Production{Title = "Escape from Happiness", Playwright = "George F. Walker", Description = "Escape from Happiness takes place in the kitchen of an old, "+
                "slightly rundown house in a not-so-classy section of a large city. It's home to Nora, a good-natured, slow-moving, fairly batty middle-aged woman; "+
                "her daughter Gail, who is tough, sensible, and a little high-strung; Gail's husband Junior, an affable but rather dim fellow. Also living here is Tom, "+
                "who is dying of some unspecified disease; Tom is, according to Nora, a stranger who looks exactly like (and coincidentally has the same name as) her husband, "+
                "who deserted the family ten years ago after trying to burn down the house",
                OpeningDay = new DateTime(2006, 8, 01, 14, 00, 00), ClosingDay = new DateTime(2006, 8, 30, 14, 00, 00), ShowtimeEve = new DateTime(2006, 8, 01, 19, 00, 00),
                ShowtimeMat = new DateTime(2006, 8, 01, 14, 00, 00), TicketLink = "ticketsforyou.com", Runtime = 120, Season = 10},

                new Production{Title = "Tango", Playwright = "Stawomir Mrozek", Description = "Tango is set in a non-defined time in the home of Stomil and Eleonora, "+
                "parents of Artur. The place is disorganized not only in the way it is furnished, but also through the complete lack of house rules and common values. "+
                "Everyone can do whatever they want. It seems that perplexity may be the word that describes their lives best. Artur’s attempts are bound to fail, "+
                "but despite that he tries to make rules and grant some things meaning. Artur revolts against his father's slovenliness and his mother’s double "+
                "standards of morality.",
                OpeningDay = new DateTime(2006, 10, 01, 14, 00, 00), ClosingDay = new DateTime(2006, 10, 30, 14, 00, 00), ShowtimeEve = new DateTime(2006, 10, 01, 19, 00, 00),
                ShowtimeMat = new DateTime(2006, 10, 01, 14, 00, 00), TicketLink = "ticketsforyou.com", Runtime = 120, Season = 10},

                new Production{Title = "Anonymous Theatre: A Funny Thing Happened on the Way to the Forum", Playwright = "Larry Gelbart and Burt Shevelove", Description = "Inspired "+
                "by the farces of the ancient Roman playwright Plautus, the musical tells the bawdy story of a slave named Pseudolus and his attempts to win his freedom by helping "+
                "his young master woo the girl next door. The plot displays many classic elements of farce, including puns, the slamming of doors, cases of mistaken identity, and "+
                "satirical comments on social class.",
                OpeningDay = new DateTime(2007, 1, 01, 14, 00, 00), ClosingDay = new DateTime(2007, 1, 31, 14, 00, 00), ShowtimeEve = new DateTime(2006, 1, 01, 19, 00, 00),
                ShowtimeMat = new DateTime(2007, 1, 01, 14, 00, 00), TicketLink = "ticketsforyou.com", Runtime = 120, Season = 10},

                //// productions for Season 9
                new Production{Title = "The Flu Season", Playwright = "Will Eno", Description = "A love story goes bad (really bad), a play gets written in painful fits and starts, " +
                "snow falls, it turns to slush. Maybe spring arrives. This is a play to remind us why sunsets make us sad, how nostalgia is like fog and why we live our lives as though " +
                "we are in mourning for them.",
                OpeningDay = new DateTime(2006, 5, 01, 14, 00, 00), ClosingDay = new DateTime(2006, 5, 30, 14, 00, 00), ShowtimeEve = new DateTime(2006, 5, 01, 19, 00, 00),
                ShowtimeMat = new DateTime(2006, 5, 01, 14, 00, 00), TicketLink = "ticketsforyou.com", Runtime = 120, Season = 9},

                new Production{Title = "Love of the Nightengale", Playwright = "Timberlake Wertenbaker", Description = "The Love of the Nightingale tells the tragic tale of Procne and " +
                "Philomele, daughters of Pandion, King of Athens. When the play opens Athens is under attack by a neighboring kingdom, but Procne and Philomele frolic innocently in the " +
                "garden, virtually unaffected by the horrors of war.",
                OpeningDay = new DateTime(2006, 3, 01, 14, 00, 00), ClosingDay = new DateTime(2006, 3, 30, 14, 00, 00), ShowtimeEve = new DateTime(2006, 3, 01, 19, 00, 00),
                ShowtimeMat = new DateTime(2006, 3, 01, 14, 00, 00), TicketLink = "ticketsforyou.com", Runtime = 120, Season = 9},

                new Production{Title = "Like I Say", Playwright = "Len Jenkin", Description = "Like I Say is 'two plays in one'. One story concerns a writer, a nurse, an artist " +
                "and two dangerous puppeteers stuck in a decrepid hotel positioned on 'the psychic edge of our world.' The second story is about Coconut Joe, a salesman who has " +
                "strayed terribly off-course. ",
                OpeningDay = new DateTime(2006, 1, 01, 14, 00, 00), ClosingDay = new DateTime(2006, 1, 30, 14, 00, 00), ShowtimeEve = new DateTime(2006, 1, 01, 19, 00, 00),
                ShowtimeMat = new DateTime(2006, 1, 01, 14, 00, 00), TicketLink = "ticketsforyou.com", Runtime = 120, Season = 9},

                new Production{Title = "Big Wheel Bingo", Playwright = "J. Von Stratton", Description = "Big Wheel Bingo is an exciting game show that is chock-full of comedy, music, " +
                "song, featuring the talents of special guest celebrity wheel spinners.",
                OpeningDay = new DateTime(2005, 11, 01, 14, 00, 00), ClosingDay = new DateTime(2005, 11, 30, 14, 00, 00), ShowtimeEve = new DateTime(2005, 11, 01, 19, 00, 00),
                ShowtimeMat = new DateTime(2005, 11, 01, 14, 00, 00), TicketLink = "ticketsforyou.com", Runtime = 120, Season = 9},

                new Production{Title = "The 24-Hour Plays", Playwright = "", Description = "The 24-Hour Plays® bring together creative communities to produce plays and musicals " +
                "that are written, rehearsed, and performed in 24 hours.",
                OpeningDay = new DateTime(2005, 9, 01, 14, 00, 00), ClosingDay = new DateTime(2005, 9, 30, 14, 00, 00), ShowtimeEve = new DateTime(2005, 9, 01, 19, 00, 00),
                ShowtimeMat = new DateTime(2005, 9, 01, 14, 00, 00), TicketLink = "ticketsforyou.com", Runtime = 120, Season = 9},

                new Production{Title = "Anonymous Theatre: The Learned Ladies", Playwright = "Moliére", Description = "Two young people, Henriette and Clitandre, are in love, but " +
                "in order to marry, they must overcome an obstacle: the attitude of Henriette's family. Her sensible father and uncle are in favour of the marriage; but unfortunately " +
                "her father is under the thumb of his wife, Philaminte. And Philaminte, supported by Henriette's aunt and sister, wishes her to marry Trissotin, a 'scholar' and mediocre " +
                "poet with lofty aspirations, who has these three women completely in his thrall. For these three ladies are 'learned'; their obsession in life is learning and culture " +
                "of the most pretentious kind, and Trissotin is their special protégé and the fixture of their literary salon.",
                OpeningDay = new DateTime(2005, 7, 01, 14, 00, 00), ClosingDay = new DateTime(2005, 7, 30, 14, 00, 00), ShowtimeEve = new DateTime(2005, 7, 01, 19, 00, 00),
                ShowtimeMat = new DateTime(2005, 7, 01, 14, 00, 00), TicketLink = "ticketsforyou.com", Runtime = 120, Season = 9},

                //// Productions for season 8
                new Production{Title = "A Lie of the Mind", Playwright = "Sam Shepard", Description = "Told in three acts set in Montana and California, the story alternates between " +
                "two families after a severe incident of spousal abuse leaves all their lives altered until the final collision at an isolated cabin. The two families are linked by the " +
                "marriage of Jake (son of Lorraine and brother of Sally and Frankie) and Beth (daughter of Baylor and Meg and sister of Mike). The play begins with Beth recuperating in " +
                "her parents' home after a hospitalization resulting from Jake's abuse. Exploring family dysfunction and the nature of love, the play follows Jake as he searches for meaning " +
                "after his relationship with Beth and her family as they struggle with Beth's brain damage.",
                OpeningDay = new DateTime(2005, 5, 01, 14, 00, 00), ClosingDay = new DateTime(2005, 5, 30, 14, 00, 00), ShowtimeEve = new DateTime(2005, 5, 01, 19, 00, 00),
                ShowtimeMat = new DateTime(2005, 5, 01, 14, 00, 00), TicketLink = "ticketsforyou.com", Runtime = 120, Season = 8},

                new Production{Title = "The Devil Inside", Playwright = "David Lindsay-Abaire", Description = "Gene leads an ordinary life: attending college, working in his mother’s " +
                "laundromat, and doing skateboard tricks for fun and profit. But his world is turned upside down on his 21st birthday when his mother, tough and determined Mrs. Slater, " +
                "reveals that his father was murdered in the Poconos 14 years ago, and now that Gene has come of age, it is his duty to avenge his father’s death.",
                OpeningDay = new DateTime(2005, 3, 01, 14, 00, 00), ClosingDay = new DateTime(2005, 3, 30, 14, 00, 00), ShowtimeEve = new DateTime(2005, 3, 01, 19, 00, 00),
                ShowtimeMat = new DateTime(2005, 3, 01, 14, 00, 00), TicketLink = "ticketsforyou.com", Runtime = 120, Season = 8},

                new Production{Title = "A Bright Room Called Day", Playwright = "Tony Kushner", Description = "Agnes, an actress in Weimar Germany, and her cadre of passionate, " +
                "progressive friends, are torn between protest, escape, and survival as the world they knew crumbles around them. Her story is interrupted by an American woman enraged " +
                "by the cruelty of the Reagan administration, and a new character, grappling with the anxiety, distraction, hope, and hopelessness of an artist facing the once unthinkable " +
                "rise of authoritarianism in modern America.",
                OpeningDay = new DateTime(2005, 1, 01, 14, 00, 00), ClosingDay = new DateTime(2005, 1, 30, 14, 00, 00), ShowtimeEve = new DateTime(2005, 1, 01, 19, 00, 00),
                ShowtimeMat = new DateTime(2005, 1, 01, 14, 00, 00), TicketLink = "ticketsforyou.com", Runtime = 120, Season = 8},

                new Production{Title = "Anonymous Theatre: Don't Drink the Water", Playwright = "David Merrick", Description = "A cascade of comedy and a solid hit on Broadway, " +
                "this affair takes place inside an American embassy behind the Iron Curtain. An American tourist – a caterer by trade – and his wife and daughter rush into the embassy " +
                "two steps ahead of the police who suspect them of spying and picture-taking. It’s not much of a refuge, for the ambassador is absent and his son, now in charge, has been " +
                "expelled from a dozen countries and the continent of Africa. Nevertheless, they carefully and frantically plot their escape, and the ambassador’s son and the caterer’s " +
                "daughter even have time to fall in love.",
                OpeningDay = new DateTime(2004, 11, 01, 14, 00, 00), ClosingDay = new DateTime(2004, 11, 30, 14, 00, 00), ShowtimeEve = new DateTime(2004, 11, 01, 19, 00, 00),
                ShowtimeMat = new DateTime(2004, 11, 01, 14, 00, 00), TicketLink = "ticketsforyou.com", Runtime = 120, Season = 8},

                //// Productions for season 7
                new Production{Title = "Death and the Maiden", Playwright = "Ariel Dorfman", Description = "A Latin American woman meets a man that she believes raped and tortured her " +
                "when her country was controlled by a dictatorship 15 years earlier. She turns the tables on the man, despite her husband's counseling otherwise, in Ariel Dorfman's " +
                "political thriller.",
                OpeningDay = new DateTime(2004, 5, 01, 14, 00, 00), ClosingDay = new DateTime(2004, 5, 30, 14, 00, 00), ShowtimeEve = new DateTime(2004, 5, 01, 19, 00, 00),
                ShowtimeMat = new DateTime(2004, 5, 01, 14, 00, 00), TicketLink = "ticketsforyou.com", Runtime = 120, Season = 7},

                new Production{Title = "Faust.Us.", Playwright = "Cherilyn Woo", Description = "A brilliant but frustrated academic, Faust has achieved all that she can with her knowledge. " +
                "Dissatisfied with what she has, she yearns for more in her life. One day she is visited by an unexpected visitor, Mephistopheles, who is full of surprises. Mephistopheles " +
                "makes a deal with her; spend the rest of her life with him, as he shows her a lifetime of everything she desires, in exchange for her soul in the beyond. Driven by the " +
                "freedom that awaits her outside of her dusty study, she agrees to the deal and is whisked away into a supernatural life of magic, thrill and adventure. After journeying " +
                "through a lifetime chasing after ambition, love and desire, will she ever find what she is looking for?",
                OpeningDay = new DateTime(2004, 3, 01, 14, 00, 00), ClosingDay = new DateTime(2004, 3, 30, 14, 00, 00), ShowtimeEve = new DateTime(2004, 3, 01, 19, 00, 00),
                ShowtimeMat = new DateTime(2004, 3, 01, 14, 00, 00), TicketLink = "ticketsforyou.com", Runtime = 120, Season = 7},

                new Production{Title = "Big Love", Playwright = "Charles Mee amd Tina Landau", Description = "Fifty brides flee their fifty grooms and seek refuge in an Italian villa in this modern " +
                "re-making of one of the world's oldest plays, The Danaids by Aeschylus. Mayhem ensues, complete with grooms in flight suits, women throwing themselves to the ground, " +
                "occasional pop songs and romantic dances - even a bride falling in love.",
                OpeningDay = new DateTime(2004, 1, 01, 14, 00, 00), ClosingDay = new DateTime(2004, 1, 30, 14, 00, 00), ShowtimeEve = new DateTime(2004, 1, 01, 19, 00, 00),
                ShowtimeMat = new DateTime(2004, 1, 01, 14, 00, 00), TicketLink = "ticketsforyou.com", Runtime = 120, Season = 7},

                new Production{Title = "The 24-Hour Plays", Playwright = "", Description = "The 24-Hour Plays® bring together creative communities to produce plays and musicals " +
                "that are written, rehearsed, and performed in 24 hours.",
                OpeningDay = new DateTime(2003, 11, 01, 14, 00, 00), ClosingDay = new DateTime(2003, 11, 30, 14, 00, 00), ShowtimeEve = new DateTime(2003, 11, 01, 19, 00, 00),
                ShowtimeMat = new DateTime(2003, 11, 01, 14, 00, 00), TicketLink = "ticketsforyou.com", Runtime = 120, Season = 7},

                new Production{Title = "Anonymous Theatre: Picasso at the Lapin Agile", Playwright = "Steve Martin", Description = "This long running Off Broadway absurdist comedy places Albert " +
                "Einstein and Pablo Picasso in a Parisian cafe in 1904, just before the renowned scientist transformed physics with his theory of relativity and the celebrated painter " +
                "set the art world afire with cubism. In his first comedy for the stage, the popular actor and screenwriter plays fast and loose with fact, fame, and fortune as these two " +
                "geniuses muse on the century’s achievements and prospects, as well as other fanciful topics, with infectious dizziness.",
                OpeningDay = new DateTime(2003, 9, 01, 14, 00, 00), ClosingDay = new DateTime(2003, 9, 30, 14, 00, 00), ShowtimeEve = new DateTime(2003, 9, 01, 19, 00, 00),
                ShowtimeMat = new DateTime(2003, 9, 01, 14, 00, 00), TicketLink = "ticketsforyou.com", Runtime = 120, Season = 7},

                //// Productions for Season 6
                new Production{Title = "Prague-Nosis!", Playwright = "Jeff Goode", Description = "Dick is hired by Czech film star Pleasure Hello to find an international jewel thief " +
                "and a man with no pants. Why is he not wearing any pants? Who’s really from the Czech Republic? And what does the hypnotist Zing The Amazing have to do with all this?",
                OpeningDay = new DateTime(2003, 5, 01, 14, 00, 00), ClosingDay = new DateTime(2003, 5, 30, 14, 00, 00), ShowtimeEve = new DateTime(2003, 5, 01, 19, 00, 00),
                ShowtimeMat = new DateTime(2003, 5, 01, 14, 00, 00), TicketLink = "ticketsforyou.com", Runtime = 120, Season = 6},

                new Production{Title = "Freedomland", Playwright = "Amy Freed", Description = "Ages ago, Noah and his wife took their kids to the amusement park 'Freedomland.' After " +
                "that trip, Noah's wife ran off and left him to raise the family. Now a retired professor of religion, Noah has married Claude, a sex therapist, and lives a secluded " +
                "life in the family farmhouse. Breaking this seclusion are Noah's two daughters and son who return home for an impromptu reunion.",
                OpeningDay = new DateTime(2003, 3, 01, 14, 00, 00), ClosingDay = new DateTime(2003, 3, 30, 14, 00, 00), ShowtimeEve = new DateTime(2003, 3, 01, 19, 00, 00),
                ShowtimeMat = new DateTime(2003, 3, 01, 14, 00, 00), TicketLink = "ticketsforyou.com", Runtime = 120, Season = 6},

                new Production{Title = "The 24-Hour Plays", Playwright = "", Description = "The 24-Hour Plays® bring together creative communities to produce plays and musicals " +
                "that are written, rehearsed, and performed in 24 hours.",
                OpeningDay = new DateTime(2003, 1, 01, 14, 00, 00), ClosingDay = new DateTime(2003, 11, 30, 14, 00, 00), ShowtimeEve = new DateTime(2003, 1, 01, 19, 00, 00),
                ShowtimeMat = new DateTime(2003, 1, 01, 14, 00, 00), TicketLink = "ticketsforyou.com", Runtime = 120, Season = 6},

                new Production{Title = "Anonymous Theatre: The Importance of Being Earnest", Playwright = "Oscar Wilde", Description = "Oscar Wilde's madcap farce about mistaken " +
                "identities, secret engagements, and lovers entanglements still delights readers more than a century after its 1895 publication and premiere performance. The rapid-fire " +
                "wit and eccentric characters of The Importance of Being Earnest have made it a mainstay of the high school curriculum for decades.",
                OpeningDay = new DateTime(2002, 8, 01, 14, 00, 00), ClosingDay = new DateTime(2002, 8, 30, 14, 00, 00), ShowtimeEve = new DateTime(2002, 8, 01, 19, 00, 00),
                ShowtimeMat = new DateTime(2002, 8, 01, 14, 00, 00), TicketLink = "ticketsforyou.com", Runtime = 120, Season = 6},

                //// Productions for Season 5
                new Production{Title = "Masterpieces", Playwright = "Sarah Daniels", Description = "Masterpieces is a fiery and uncompromising condemnation of pornography" +
                " and the objectification of women.",
                OpeningDay = new DateTime(2002, 5, 01, 14, 00, 00), ClosingDay = new DateTime(2002, 5, 31, 14, 00, 00), ShowtimeEve = new DateTime(2002, 5, 01, 19, 00, 00),
                ShowtimeMat = new DateTime(2002, 5, 01, 14, 00, 00), TicketLink = "ticketsforyou.com", Runtime = 210, Season = 5},

                 new Production{Title = "Rashomon", Playwright = "Fay Kanin and Michael Kanin", Description = "The wife of a Samurai officer is assaulted and her husband killed" +
                "by a roving bandit. Contradictory versions of what happened are reenacted at the trial by the bandit, the wife and the dead husband who speaks through a medium." +
                "  Each version is true in its fashion.",
                OpeningDay = new DateTime(2002, 2, 01, 14, 00, 00), ClosingDay = new DateTime(2002, 2, 28, 14, 00, 00), ShowtimeEve = new DateTime(2002, 2, 01, 19, 00, 00),
                ShowtimeMat = new DateTime(2002, 2, 01, 14, 00, 00), TicketLink = "ticketsforyou.com", Runtime = 210, Season = 5},

                new Production{Title = "The Physicists", Playwright = "Friedrich Dürrenmatt", Description = "A nurse has been murdered at Les Cerisiers, a private sanitarium"  +
                "run by the world-renowned psychiatrist, Dr. Mathilde von Zahnd. This is the second nurse killed in three months, and both have been murdered by residents" +
                "of the physicist ward.",
                OpeningDay = new DateTime(2001, 10, 01, 14, 00, 00), ClosingDay = new DateTime(2001, 10, 31, 14, 00, 00), ShowtimeEve = new DateTime(2001, 10, 01, 19, 00, 00),
                ShowtimeMat = new DateTime(2001, 10, 01, 14, 00, 00), TicketLink = "ticketsforyou.com", Runtime = 210, Season = 5},

                new Production{Title = "The 24-Hour Plays", Playwright = "", Description = "The 24-Hour Plays® bring together creative communities to produce plays and musicals " +
                "that are written, rehearsed, and performed in 24 hours.",
                OpeningDay = new DateTime(2001, 7, 01, 14, 00, 00), ClosingDay = new DateTime(2001, 7, 31, 14, 00, 00), ShowtimeEve = new DateTime(2001, 7, 01, 19, 00, 00),
                ShowtimeMat = new DateTime(2001, 7, 01, 14, 00, 00), TicketLink = "ticketsforyou.com", Runtime = 210, Season = 5},

                 //// Productions for Season 4
                new Production{Title = "The Baptism", Playwright = "Amiri Baraka", Description = "A boy comes to the church to be baptized, but his sins become a heated topic of" +
                " discussion, launching angry accusations and a violent end.",
                OpeningDay = new DateTime(2001, 5, 01, 14, 00, 00), ClosingDay = new DateTime(2001, 5, 31, 14, 00, 00), ShowtimeEve = new DateTime(2001, 5, 01, 19, 00, 00),
                ShowtimeMat = new DateTime(2001, 5, 01, 14, 00, 00), TicketLink = "ticketsforyou.com", Runtime = 210, Season = 4},

                new Production{Title = "Brutality of Fact", Playwright = "Keith Reddin", Description = "Val, a matriarch, comes to live with her daughter, Jackie." +
                "Jackie, recently divorced and having lost custody of her daughter, takes her mother in willingly but also enlists her in a crusade to convert everyone" +
                "to a Jehovah’s Witness.",
                OpeningDay = new DateTime(2001, 2, 01, 14, 00, 00), ClosingDay = new DateTime(2001, 2, 28, 14, 00, 00), ShowtimeEve = new DateTime(2001, 2, 01, 19, 00, 00),
                ShowtimeMat = new DateTime(2001, 2, 01, 14, 00, 00), TicketLink = "ticketsforyou.com", Runtime = 210, Season = 4},

                new Production{Title = "Poona the Fuckdog: And Other Plays for Children", Playwright = "Jeff Goode", Description = "Poona's adventures take her to the" +
                "Kingdom of Do (where nobody did) ruled by a powerful television set. She meets, among others, Suzy-Suzy Cyber Assassin, a thespian shrub, lost space aliens," +
                "and she even talks to God! Poona finally grows old and must tell her fabulous story to all you little kiddies.",
                OpeningDay = new DateTime(2000, 10, 01, 14, 00, 00), ClosingDay = new DateTime(2000, 10, 31, 14, 00, 00), ShowtimeEve = new DateTime(2000, 10, 01, 19, 00, 00),
                ShowtimeMat = new DateTime(2000, 10, 01, 14, 00, 00), TicketLink = "ticketsforyou.com", Runtime = 210, Season = 4},

                new Production{Title = "Popcorn", Playwright = "Ben Elton", Description = "A satirical comedy going on hostage drama going on heated moral debate about" +
                " violent movies, copycat crimes and taking responsibility.",
                OpeningDay = new DateTime(2000, 7, 01, 14, 00, 00), ClosingDay = new DateTime(2000, 7, 31, 14, 00, 00), ShowtimeEve = new DateTime(2000, 7, 01, 19, 00, 00),
                ShowtimeMat = new DateTime(2000, 7, 01, 14, 00, 00), TicketLink = "ticketsforyou.com", Runtime = 210, Season = 4},

                 //// Productions for Season 3
                new Production{Title = "Hellcab", Playwright = "Will Kern", Description = "Hellcab portrays the story of a cab driver during the longest night of his life" +
                "as he transports a bizarre and mysterious array of customers through the gritty streets of Chicago",
                OpeningDay = new DateTime(2000, 5, 01, 14, 00, 00), ClosingDay = new DateTime(2000, 5, 31, 14, 00, 00), ShowtimeEve = new DateTime(2000, 5, 01, 19, 00, 00),
                ShowtimeMat = new DateTime(2000, 5, 01, 14, 00, 00), TicketLink = "ticketsforyou.com", Runtime = 210, Season = 3},

                new Production{Title = "The Anger of Ernest and Ernestine", Playwright = "Leah Cherniak and Martha Ross", Description = " It is a series of related comic" +
                " sketches about marriage featuring two who exaggerate their love for one another before they marry and move into their dingy basement" +
                " apartment, only to find that their habits are incompatible. Ernest is obsessive compulsive; Ernestine is happy making and living in a mess.",
                OpeningDay = new DateTime(2000, 3, 01, 14, 00, 00), ClosingDay = new DateTime(2000, 3, 31, 14, 00, 00), ShowtimeEve = new DateTime(2000, 3, 01, 19, 00, 00),
                ShowtimeMat = new DateTime(2000, 3, 01, 14, 00, 00), TicketLink = "ticketsforyou.com", Runtime = 210, Season = 3},

                new Production{Title = "Lion in the Streets", Playwright = "Judith Thompson", Description = "Its central character is the ghost Isobel, a nine-year-old" +
                " Portuguese girl who is searching for her killer by observing and occasionally interacting with her neighbors seventeen years after her murder, revealing" +
                " their dark, horrific, emotional, and very private experiences.",
                OpeningDay = new DateTime(2000, 1, 01, 14, 00, 00), ClosingDay = new DateTime(2000, 1, 31, 14, 00, 00), ShowtimeEve = new DateTime(2000, 1, 01, 19, 00, 00),
                ShowtimeMat = new DateTime(2000, 1, 01, 14, 00, 00), TicketLink = "ticketsforyou.com", Runtime = 210, Season = 3},

                new Production{Title = "The King has Gone to Tenebrae", Playwright = "George Herman", Description = "",
                OpeningDay = new DateTime(1999, 9, 01, 14, 00, 00), ClosingDay = new DateTime(1999, 9, 30, 14, 00, 00), ShowtimeEve = new DateTime(1999, 9, 01, 19, 00, 00),
                ShowtimeMat = new DateTime(1999, 9, 01, 14, 00, 00), TicketLink = "ticketsforyou.com", Runtime = 210, Season = 3},

                new Production{Title = "The Grey Zone", Playwright = "Tim Blake Nelson", Description = "A small group of Sonderkommando, prisoners assigned to dispose of" +
                " the bodies of other dead prisoners, are plotting an insurrection that, they hope, will destroy at least one of the camp's four crematoria and gas chambers.",
                OpeningDay = new DateTime(1999, 6, 01, 14, 00, 00), ClosingDay = new DateTime(1999, 6, 30, 14, 00, 00), ShowtimeEve = new DateTime(1999, 6, 01, 19, 00, 00),
                ShowtimeMat = new DateTime(1999, 6, 01, 14, 00, 00), TicketLink = "ticketsforyou.com", Runtime = 210, Season = 3}, 
                
                //// Productions for Season 2
                new Production{Title = "This is a Play", Playwright = "Daniel MacIvor", Description = "A hilarious metaplay,  This Is A Play follows three actors who," +
                " while performing, reveal their own thoughts and motivations as they struggle through crazy stage directions and an unoriginal musical score.",
                OpeningDay = new DateTime(1999, 5, 01, 14, 00, 00), ClosingDay = new DateTime(1999, 5, 31, 14, 00, 00), ShowtimeEve = new DateTime(1999, 5, 01, 19, 00, 00),
                ShowtimeMat = new DateTime(1999, 5, 01, 14, 00, 00), TicketLink = "ticketsforyou.com", Runtime = 210, Season = 2},

                new Production{Title = "Criminal Genius", Playwright = "George F. Walker", Description = "A gut-busting black comedy in which nothing is beyond ridicule" +
                "– not even murder, arson or pornography.",
                OpeningDay = new DateTime(1999, 3, 01, 14, 00, 00), ClosingDay = new DateTime(1999, 3, 31, 14, 00, 00), ShowtimeEve = new DateTime(1999, 3, 01, 19, 00, 00),
                ShowtimeMat = new DateTime(1999, 3, 01, 14, 00, 00), TicketLink = "ticketsforyou.com", Runtime = 210, Season = 2},

                new Production{Title = "Never Swim Alone", Playwright = "Daniel MacIvor", Description = "A swift, funny satire about two Alpha-males and their ruthless competition for" +
                " the title of Top Dog. The play is structured as a surreal egotistic boxing match: Frank and Bill, two guys in dark suits and bad ties, square off in a 13" +
                " round Battle Royale of vicious undermining and one-upmanship.",
                OpeningDay = new DateTime(1999, 1, 01, 14, 00, 00), ClosingDay = new DateTime(1999, 1, 31, 14, 00, 00), ShowtimeEve = new DateTime(1999, 1, 01, 19, 00, 00),
                ShowtimeMat = new DateTime(1999, 1, 01, 14, 00, 00), TicketLink = "ticketsforyou.com", Runtime = 210, Season = 2},

                new Production{Title = "Two-Headed Roommate", Playwright = "", Description = "",
                OpeningDay = new DateTime(1998, 9, 01, 14, 00, 00), ClosingDay = new DateTime(1998, 9, 30, 14, 00, 00), ShowtimeEve = new DateTime(1998, 9, 01, 19, 00, 00),
                ShowtimeMat = new DateTime(1998, 9, 01, 14, 00, 00), TicketLink = "ticketsforyou.com", Runtime = 210, Season = 2},

                new Production{Title = "Larry and the Werewolf", Playwright = "Jeff Goode", Description = "Larry Fingers and his cigar-chomping, gun-toting protege Spike" +
                " are about to hit the big-time. But will their budding music career survive the spectre of scandal raised by the Lycanthrope from Larry's occult past?" +
                " Larry's not saying. But hotel detective (and erotic novelist) Dick Piston means to find some answers. Even if he has to write it himself.",
                OpeningDay = new DateTime(1998, 6, 01, 14, 00, 00), ClosingDay = new DateTime(1998, 6, 30, 14, 00, 00), ShowtimeEve = new DateTime(1998, 6, 01, 19, 00, 00),
                ShowtimeMat = new DateTime(1998, 6, 01, 14, 00, 00), TicketLink = "ticketsforyou.com", Runtime = 45, Season = 2},
                
                //// Productions for Season 1
                new Production{Title = "A Woman, Alone", Playwright = "Dario Fo and Franca Rame", Description = "Locked up in a flat, a solitary woman makes her confession." +
                "Amid domestic chores, dodgy phone calls, a sex-mad brother-in-law and a forever screaming baby she tells us the story of how her love for a young student led" +
                " to her imprisonment at the hands of her jealous husband.",
                OpeningDay = new DateTime(1998, 3, 01, 14, 00, 00), ClosingDay = new DateTime(1998, 3, 31, 14, 00, 00), ShowtimeEve = new DateTime(1998, 3, 01, 19, 00, 00),
                ShowtimeMat = new DateTime(1998, 3, 01, 14, 00, 00), TicketLink = "ticketsforyou.com", Runtime = 120, Season = 1},

                new Production{Title = "Mass Murder", Playwright = "", Description = "",
                OpeningDay = new DateTime(1997, 10, 01, 14, 00, 00), ClosingDay = new DateTime(1997, 10, 31, 14, 00, 00), ShowtimeEve = new DateTime(1997, 10, 01, 19, 00, 00),
                ShowtimeMat = new DateTime(1997, 10, 01, 14, 00, 00), TicketLink = "ticketsforyou.com", Runtime = 120, Season = 1, IsWorldPremiere= true},



                #endregion
            };

            productions.ForEach(Production => context.Productions.AddOrUpdate(d => new { d.Title, d.Season }, Production));
            context.SaveChanges();
        }


        private void SeedProductionPhotos()
        {
            var converter = new ImageConverter();
            // create images first
            string imagesRoot = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + @"\Content\Images");

            // Image Unavailable photo
            Image imageUnavailable = Image.FromFile(Path.Combine(imagesRoot, @"Unavailable.png"));

            Image image1 = Image.FromFile(Path.Combine(imagesRoot, @"hamilton1.png"));
            Image image2 = Image.FromFile(Path.Combine(imagesRoot, @"hamilton2.png"));
            Image image3 = Image.FromFile(Path.Combine(imagesRoot, @"phantom1.png"));
            Image image4 = Image.FromFile(Path.Combine(imagesRoot, @"phantom2.png"));
            Image image5 = Image.FromFile(Path.Combine(imagesRoot, @"bookofmormon1.png"));
            Image image6 = Image.FromFile(Path.Combine(imagesRoot, @"bookofmormon2.png"));
            Image image7 = Image.FromFile(Path.Combine(imagesRoot, @"wicked1.png"));
            Image image8 = Image.FromFile(Path.Combine(imagesRoot, @"wicked2.png"));
            Image image9 = Image.FromFile(Path.Combine(imagesRoot, @"howtosucceedinbusinesswithoutreallytrying.png"));
            Image image10 = Image.FromFile(Path.Combine(imagesRoot, @"howtosucceedinbusinesswithoutreallytrying2.png"));
            // images for season 20
            Image image11 = Image.FromFile(Path.Combine(imagesRoot, @"assistance.jpg"));
            Image image12 = Image.FromFile(Path.Combine(imagesRoot, @"carnivora.jpg"));
            Image image13 = Image.FromFile(Path.Combine(imagesRoot, @"maze.jpg"));
            // images for season 19
            Image image14 = Image.FromFile(Path.Combine(imagesRoot, @"drunkenCity.jpg"));
            Image image15 = Image.FromFile(Path.Combine(imagesRoot, @"destroy.jpg"));
            Image image16 = Image.FromFile(Path.Combine(imagesRoot, @"love_and_information.jpg"));
            // images for season 18
            Image image17 = Image.FromFile(Path.Combine(imagesRoot, @"bob5.jpg"));
            Image image18 = Image.FromFile(Path.Combine(imagesRoot, @"sexual.jpg"));
            Image image19 = Image.FromFile(Path.Combine(imagesRoot, @"school_for_lies.jpg"));
            // images for season 17
            Image image20 = Image.FromFile(Path.Combine(imagesRoot, @"Hyde.jpg"));
            Image image21 = Image.FromFile(Path.Combine(imagesRoot, @"endsex.jpg"));
            Image image22 = Image.FromFile(Path.Combine(imagesRoot, @"pool.jpg"));
            Image image23 = Image.FromFile(Path.Combine(imagesRoot, @"crucible.jpg"));
            // images for season 16
            Image image24 = Image.FromFile(Path.Combine(imagesRoot, @"mother.jpg"));
            Image image25 = Image.FromFile(Path.Combine(imagesRoot, @"velvet.jpg"));
            Image image26 = Image.FromFile(Path.Combine(imagesRoot, @"aloha.jpg"));
            Image image27 = Image.FromFile(Path.Combine(imagesRoot, @"The_skin_of_our_teeth.jpg"));
            // images for season 15
            Image image28 = Image.FromFile(Path.Combine(imagesRoot, @"cloud9.jpg"));
            Image image29 = Image.FromFile(Path.Combine(imagesRoot, @"huntergatherers.jpg"));
            Image image30 = Image.FromFile(Path.Combine(imagesRoot, @"americanpilot.jpg"));
            Image image31 = Image.FromFile(Path.Combine(imagesRoot, @"doctor.jpg"));
            //images for season 14
            Image image32 = Image.FromFile(Path.Combine(imagesRoot, @"deadmanscellphone.jpg"));
            Image image33 = Image.FromFile(Path.Combine(imagesRoot, @"99ways.jpg"));
            Image image34 = Image.FromFile(Path.Combine(imagesRoot, @"addingmachine.jpg"));
            Image image35 = Image.FromFile(Path.Combine(imagesRoot, @"youcanttakeitwithyou.jpg"));
            //images for season 13
            Image image36 = Image.FromFile(Path.Combine(imagesRoot, @"ruby.jpg"));
            Image image37 = Image.FromFile(Path.Combine(imagesRoot, @"boomjpg.jpg"));
            Image image38 = Image.FromFile(Path.Combine(imagesRoot, @"godsear.jpg"));
            Image image39 = Image.FromFile(Path.Combine(imagesRoot, @"lendmeatenor.jpg"));
            //images for season 12
            Image image40 = Image.FromFile(Path.Combine(imagesRoot, @"pterodactyls.jpg"));
            Image image41 = Image.FromFile(Path.Combine(imagesRoot, @"Romance.jpg"));
            Image image42 = Image.FromFile(Path.Combine(imagesRoot, @"freakshow.jpg"));
            Image image43 = Image.FromFile(Path.Combine(imagesRoot, @"Macbeth.jpg"));
            //images for season 11
            Image image44 = Image.FromFile(Path.Combine(imagesRoot, @"dollshouse.jpg"));
            Image image45 = Image.FromFile(Path.Combine(imagesRoot, @"wheresmymoneyjpg.jpg"));
            Image image46 = Image.FromFile(Path.Combine(imagesRoot, @"longchristmasridehome.jpg"));
            Image image47 = Image.FromFile(Path.Combine(imagesRoot, @"Rumors.jpg"));
            ////images for season 10
            Image image48 = Image.FromFile(Path.Combine(imagesRoot, @"Valparaiso.jpg"));
            Image image49 = Image.FromFile(Path.Combine(imagesRoot, @"EscapefromHappiness.jpg"));
            Image image50 = Image.FromFile(Path.Combine(imagesRoot, @"Tango.jpg"));
            Image image51 = Image.FromFile(Path.Combine(imagesRoot, @"Forum.jpg"));
            // images for season 9
            Image image52 = Image.FromFile(Path.Combine(imagesRoot, @"fluSeason.jpg"));
            Image image53 = Image.FromFile(Path.Combine(imagesRoot, @"loveNightengale.jpg"));
            Image image54 = Image.FromFile(Path.Combine(imagesRoot, @"likeISay.jpg"));
            Image image55 = Image.FromFile(Path.Combine(imagesRoot, @"bigWheelBingo.jpg"));
            Image image56 = Image.FromFile(Path.Combine(imagesRoot, @"24HourPlays.jpg"));
            Image image57 = Image.FromFile(Path.Combine(imagesRoot, @"learnedLadies.jpg"));
            // images for season 8
            Image image58 = Image.FromFile(Path.Combine(imagesRoot, @"lieOfTheMind.jpg"));
            Image image59 = Image.FromFile(Path.Combine(imagesRoot, @"theDevilInside.jpg"));
            Image image60 = Image.FromFile(Path.Combine(imagesRoot, @"brightRoomCalledDay.jpg"));
            Image image61 = Image.FromFile(Path.Combine(imagesRoot, @"dontDrinkTheWater.jpg"));
            // images for season 7
            Image image62 = Image.FromFile(Path.Combine(imagesRoot, @"deathAndTheMaiden.jpg"));
            Image image63 = Image.FromFile(Path.Combine(imagesRoot, @"faustus.jpg"));
            Image image64 = Image.FromFile(Path.Combine(imagesRoot, @"bigLove.jpg"));
            Image image65 = Image.FromFile(Path.Combine(imagesRoot, @"24HourPlays2.jpg"));
            Image image66 = Image.FromFile(Path.Combine(imagesRoot, @"picassoAtTheLapinAgile.jpg"));
            // images for season 6
            Image image67 = Image.FromFile(Path.Combine(imagesRoot, @"pragueNosis.jpg"));
            Image image68 = Image.FromFile(Path.Combine(imagesRoot, @"freedomland.jpg"));
            Image image69 = Image.FromFile(Path.Combine(imagesRoot, @"24HourPlays3.jpg"));
            Image image70 = Image.FromFile(Path.Combine(imagesRoot, @"importanceOfBeingEarnest.jpg"));
            // images for season 5
            Image image71 = Image.FromFile(Path.Combine(imagesRoot, @"masterpieces.jpg"));
            Image image72 = Image.FromFile(Path.Combine(imagesRoot, @"rashomon.jpg"));
            Image image73 = Image.FromFile(Path.Combine(imagesRoot, @"Physicists.jpeg"));
            Image image74 = Image.FromFile(Path.Combine(imagesRoot, @"24HourPlays4.jpg"));
            // images for season 4
            Image image75 = Image.FromFile(Path.Combine(imagesRoot, @"baptism.jpg"));
            Image image76 = Image.FromFile(Path.Combine(imagesRoot, @"brutalityOfFact.jpg"));
            Image image77 = Image.FromFile(Path.Combine(imagesRoot, @"poona.jpg"));
            Image image78 = Image.FromFile(Path.Combine(imagesRoot, @"popcorn.jpg"));
            // images for season 3
            Image image79 = Image.FromFile(Path.Combine(imagesRoot, @"hellcab.jpg"));
            Image image80 = Image.FromFile(Path.Combine(imagesRoot, @"angerErnestAndErnestine.jpg"));
            Image image81 = Image.FromFile(Path.Combine(imagesRoot, @"lionInTheStreets.jpg"));
            Image image82 = Image.FromFile(Path.Combine(imagesRoot, @"kingTenebrae.jpg"));
            Image image83 = Image.FromFile(Path.Combine(imagesRoot, @"grayZone.jpg"));
            // images for season 2
            Image image84 = Image.FromFile(Path.Combine(imagesRoot, @"thisIsAPlay.jpg"));
            Image image85 = Image.FromFile(Path.Combine(imagesRoot, @"criminalGenius.png"));
            Image image86 = Image.FromFile(Path.Combine(imagesRoot, @"neverSwimAlone.jpg"));
            Image image87 = Image.FromFile(Path.Combine(imagesRoot, @"twoHeadedRoommate.jpg"));
            Image image88 = Image.FromFile(Path.Combine(imagesRoot, @"larryTheWerewolf.jpg"));
            // images for season 1
            Image image89 = Image.FromFile(Path.Combine(imagesRoot, @"aWomanAlone.jpg"));
            Image image90 = Image.FromFile(Path.Combine(imagesRoot, @"massMurders.jpg"));



            var photos = new List<Photo>
            {
                new Photo
                {
                    OriginalHeight = imageUnavailable.Height,
                    OriginalWidth = imageUnavailable.Width,
                    PhotoFile = (byte[])converter.ConvertTo(imageUnavailable, typeof(byte[])),
                    Title = "Photo Unavailable"
                },
                new Photo
                {
                    OriginalHeight = image1.Height,
                    OriginalWidth = image1.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image1, typeof(byte[])),
                    Title = "Hamilton Image 1"
                },
                new Photo
                {
                    OriginalHeight = image2.Height,
                    OriginalWidth = image2.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image2, typeof(byte[])),
                    Title = "Hamilton Image 2"
                },
                new Photo
                {
                    OriginalHeight = image3.Height,
                    OriginalWidth = image3.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image3, typeof(byte[])),
                    Title = "Phantom Of The Opera Image 1"
                },
                new Photo
                {
                    OriginalHeight = image4.Height,
                    OriginalWidth = image4.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image4, typeof(byte[])),
                    Title = "Phantom Of The Opera Image 2"
                },
                new Photo
                {
                    OriginalHeight = image5.Height,
                    OriginalWidth = image5.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image5, typeof(byte[])),
                    Title = "The Book of Mormon Image 1"
                },
                new Photo
                {
                    OriginalHeight = image6.Height,
                    OriginalWidth = image6.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image6, typeof(byte[])),
                    Title = "The Book of Mormon Image 2"
                },
                new Photo
                {
                    OriginalHeight = image7.Height,
                    OriginalWidth = image7.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image7, typeof(byte[])),
                    Title = "Wicked Image 1"
                },
                new Photo
                {
                    OriginalHeight = image8.Height,
                    OriginalWidth = image8.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image8, typeof(byte[])),
                    Title = "Wicked Image 2"
                },
                new Photo
                {
                    OriginalHeight = image9.Height,
                    OriginalWidth = image9.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image9, typeof(byte[])),
                    Title = "How to Succeed in Business Without Really Trying Image 1"
                },
                new Photo
                {
                    OriginalHeight = image10.Height,
                    OriginalWidth = image10.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image10, typeof(byte[])),
                    Title = "How to Succeed in Business Without Really Trying Image 2"
                },
                // Production photos for season 20
                new Photo
                {
                    OriginalHeight = image11.Height,
                    OriginalWidth = image11.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image11, typeof(byte[])),
                    Title = "Assistance Image"
                },
                new Photo
                {
                    OriginalHeight = image12.Height,
                    OriginalWidth = image12.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image12, typeof(byte[])),
                    Title = "Carnivora Image"
                },
                new Photo
                {
                    OriginalHeight = image13.Height,
                    OriginalWidth = image13.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image13, typeof(byte[])),
                    Title = "A Maze Image"
                },
                // Production photos for season 19
                new Photo
                {
                    OriginalHeight = image14.Height,
                    OriginalWidth = image14.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image14, typeof(byte[])),
                    Title = "The Drunken City Image"
                },
                new Photo
                {
                    OriginalHeight = image15.Height,
                    OriginalWidth = image15.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image15, typeof(byte[])),
                    Title = "I Want to Destroy You Image"
                },
                new Photo
                {
                    OriginalHeight = image16.Height,
                    OriginalWidth = image16.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image16, typeof(byte[])),
                    Title = "Love and Information Image"
                },
                // Production photos for season 18
                new Photo
                {
                    OriginalHeight = image17.Height,
                    OriginalWidth = image17.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image17, typeof(byte[])),
                    Title = "Bob: A Life in Five Acts Image"
                },
                new Photo
                {
                    OriginalHeight = image18.Height,
                    OriginalWidth = image18.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image18, typeof(byte[])),
                    Title = "Sexual Neuroses of Our Parents Image"
                },
                new Photo
                {
                    OriginalHeight = image19.Height,
                    OriginalWidth = image19.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image19, typeof(byte[])),
                    Title = "The School for Lies Image"
                },
                // Production photos for season 17
                new Photo
                {
                    OriginalHeight = image20.Height,
                    OriginalWidth = image20.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image20, typeof(byte[])),
                    Title = "Dr. Jekyll and Mr. Hyde Image"
                },
                new Photo
                {
                    OriginalHeight = image21.Height,
                    OriginalWidth = image21.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image21, typeof(byte[])),
                    Title = "The End of Sex Image"
                },
                new Photo
                {
                    OriginalHeight = image22.Height,
                    OriginalWidth = image22.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image22, typeof(byte[])),
                    Title = "pool (no water) Image"
                },
                new Photo
                {
                    OriginalHeight = image23.Height,
                    OriginalWidth = image23.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image23, typeof(byte[])),
                    Title = "Anonymous Theatre:  The Crucible Image"
                },
                // Production photos for season 16
                new Photo
                {
                    OriginalHeight = image24.Height,
                    OriginalWidth = image24.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image24, typeof(byte[])),
                    Title = "Mother Courage & Her Children Image"
                },
                new Photo
                {
                    OriginalHeight = image25.Height,
                    OriginalWidth = image25.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image25, typeof(byte[])),
                    Title = "The Velvet Sky Image"
                },
                new Photo
                {
                    OriginalHeight = image26.Height,
                    OriginalWidth = image26.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image26, typeof(byte[])),
                    Title = "Aloha, Say the Pretty Girls Image"
                },
                new Photo
                {
                    OriginalHeight = image27.Height,
                    OriginalWidth = image27.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image27, typeof(byte[])),
                    Title = "Anonymous Theatre:  The Skin of Our Teeth Image"
                },
                 // Production photos for season 15
                new Photo
                {
                    OriginalHeight = image28.Height,
                    OriginalWidth = image28.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image28, typeof(byte[])),
                    Title = "Cloud 9"
                },
                new Photo
                {
                    OriginalHeight = image29.Height,
                    OriginalWidth = image29.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image29, typeof(byte[])),
                    Title = "Hunter Gatherers"
                },
                new Photo
                {
                    OriginalHeight = image30.Height,
                    OriginalWidth = image30.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image30, typeof(byte[])),
                    Title = "The American Pilot"
                },
                new Photo
                {
                    OriginalHeight = image31.Height,
                    OriginalWidth = image31.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image31, typeof(byte[])),
                    Title = "Anonymous Theatre: The Good Doctor"
                },
                      // Production photos for season 14
                new Photo
                {
                    OriginalHeight = image32.Height,
                    OriginalWidth = image32.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image32, typeof(byte[])),
                    Title = "Dead Man's Cell Phone"
                },
                new Photo
                {
                    OriginalHeight = image33.Height,
                    OriginalWidth = image33.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image33, typeof(byte[])),
                    Title = "99 Ways to Fuck a Swan"
                },
                new Photo
                {
                    OriginalHeight = image34.Height,
                    OriginalWidth = image34.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image34, typeof(byte[])),
                    Title = "The Adding Machine"
                },
                new Photo
                {
                    OriginalHeight = image35.Height,
                    OriginalWidth = image35.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image35, typeof(byte[])),
                    Title = "Anonymous Theatre: You Can't Take It with You"
                },

                // Production photos for season 13

                new Photo
                {
                    OriginalHeight = image36.Height,
                    OriginalWidth = image36.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image36, typeof(byte[])),
                    Title = "The Ruby Sunrise"
                },

                new Photo
                {
                    OriginalHeight = image37.Height,
                    OriginalWidth = image37.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image37, typeof(byte[])),
                    Title = "Boom"
                },

                new Photo
                {
                    OriginalHeight = image38.Height,
                    OriginalWidth = image38.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image38, typeof(byte[])),
                    Title = "God's Ear"
                },

                new Photo
                {
                    OriginalHeight = image39.Height,
                    OriginalWidth = image39.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image39, typeof(byte[])),
                    Title = "Anonymous Theatre: Lend Me a Tenor"
                },

                // Production photos for season 12

                new Photo
                {
                    OriginalHeight = image40.Height,
                    OriginalWidth = image40.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image40, typeof(byte[])),
                    Title = "Pterodactyls"
                },

                new Photo
                {
                    OriginalHeight = image41.Height,
                    OriginalWidth = image41.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image41, typeof(byte[])),
                    Title = "Romance"
                },

                new Photo
                {
                    OriginalHeight = image42.Height,
                    OriginalWidth = image42.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image42, typeof(byte[])),
                    Title = "Freakshow"
                },

                new Photo
                {
                    OriginalHeight = image43.Height,
                    OriginalWidth = image43.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image43, typeof(byte[])),
                    Title = "Anonymous Theatre: MacBeth"
                },

                // Production photos for season 11

                new Photo
                {
                    OriginalHeight = image44.Height,
                    OriginalWidth = image44.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image44, typeof(byte[])),
                    Title = "A Doll's House"
                },

                new Photo
                {
                    OriginalHeight = image45.Height,
                    OriginalWidth = image45.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image45, typeof(byte[])),
                    Title = "Where's My Money? "
                },

                new Photo
                {
                    OriginalHeight = image46.Height,
                    OriginalWidth = image46.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image46, typeof(byte[])),
                    Title = "The Long Christmas Ride Home"
                },

                new Photo
                {
                    OriginalHeight = image47.Height,
                    OriginalWidth = image47.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image47, typeof(byte[])),
                    Title = "Anonymous Theatre: Rumors"
                },
                // Production photos for season 10

                new Photo
                {
                    OriginalHeight = image48.Height,
                    OriginalWidth = image48.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image48, typeof(byte[])),
                    Title = "Valparaiso"
                },

                new Photo
                {
                    OriginalHeight = image49.Height,
                    OriginalWidth = image49.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image49, typeof(byte[])),
                    Title = "Escape from Happiness"
                },

                new Photo
                {
                    OriginalHeight = image50.Height,
                    OriginalWidth = image50.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image50, typeof(byte[])),
                    Title = "Tango"
                },

                new Photo
                {
                    OriginalHeight = image51.Height,
                    OriginalWidth = image51.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image51, typeof(byte[])),
                    Title = "Anonymous Theatre: A Funny Thing Happened on the Way to the Forum"
                },

                // Photos for season 9
                new Photo
                {
                    OriginalHeight = image52.Height,
                    OriginalWidth = image52.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image52, typeof(byte[])),
                    Title = "The Flu Season"
                },

                new Photo
                {
                    OriginalHeight = image53.Height,
                    OriginalWidth = image53.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image53, typeof(byte[])),
                    Title = "Love of the Nightengale"
                },

                new Photo
                {
                    OriginalHeight = image54.Height,
                    OriginalWidth = image54.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image54, typeof(byte[])),
                    Title = "Like I Say"
                },

                new Photo
                {
                    OriginalHeight = image55.Height,
                    OriginalWidth = image55.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image55, typeof(byte[])),
                    Title = "Big Wheel Bingo"
                },

                new Photo
                {
                    OriginalHeight = image56.Height,
                    OriginalWidth = image56.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image56, typeof(byte[])),
                    Title = "The 24-Hour Plays - Season 9"
                },

                new Photo
                {
                    OriginalHeight = image57.Height,
                    OriginalWidth = image57.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image57, typeof(byte[])),
                    Title = "Anonymous Theatre: The Learned Ladies"
                },

                new Photo
                {
                    OriginalHeight = image58.Height,
                    OriginalWidth = image58.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image58, typeof(byte[])),
                    Title = "A Lie of the Mind"
                },

                new Photo
                {
                    OriginalHeight = image59.Height,
                    OriginalWidth = image59.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image59, typeof(byte[])),
                    Title = "The Devil Inside"
                },

                new Photo
                {
                    OriginalHeight = image60.Height,
                    OriginalWidth = image60.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image60, typeof(byte[])),
                    Title = "A Bright Room Called Day"
                },

                new Photo
                {
                    OriginalHeight = image61.Height,
                    OriginalWidth = image61.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image61, typeof(byte[])),
                    Title = "Anonymous Theatre: Don't Drink the Water"
                },

                new Photo
                {
                    OriginalHeight = image62.Height,
                    OriginalWidth = image62.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image62, typeof(byte[])),
                    Title = "Death and the Maiden"
                },

                new Photo
                {
                    OriginalHeight = image63.Height,
                    OriginalWidth = image63.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image63, typeof(byte[])),
                    Title = "Faust.Us."
                },

                new Photo
                {
                    OriginalHeight = image64.Height,
                    OriginalWidth = image64.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image64, typeof(byte[])),
                    Title = "Big Love"
                },

                new Photo
                {
                    OriginalHeight = image65.Height,
                    OriginalWidth = image65.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image65, typeof(byte[])),
                    Title = "The 24-Hour Plays - Season 7"
                },

                new Photo
                {
                    OriginalHeight = image66.Height,
                    OriginalWidth = image66.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image66, typeof(byte[])),
                    Title = "Anonymous Theatre: Picasso at the Lapin Agile"
                },

                new Photo
                {
                    OriginalHeight = image67.Height,
                    OriginalWidth = image67.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image67, typeof(byte[])),
                    Title = "Prague-Nosis!"
                },

                new Photo
                {
                    OriginalHeight = image68.Height,
                    OriginalWidth = image68.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image68, typeof(byte[])),
                    Title = "Freedomland"
                },

                new Photo
                {
                    OriginalHeight = image69.Height,
                    OriginalWidth = image69.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image69, typeof(byte[])),
                    Title = "The 24-Hour Plays - Season 6"
                },

                new Photo
                {
                    OriginalHeight = image70.Height,
                    OriginalWidth = image70.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image70, typeof(byte[])),
                    Title = "Anonymous Theatre: The Importance of Being Earnest"
                },

                // Photos for season 5
                new Photo
                {
                    OriginalHeight = image71.Height,
                    OriginalWidth = image71.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image71, typeof(byte[])),
                    Title = "Masterpieces"
                },

                new Photo
                {
                    OriginalHeight = image72.Height,
                    OriginalWidth = image72.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image72, typeof(byte[])),
                    Title = "Rashomon"
                },

                new Photo
                {
                    OriginalHeight = image73.Height,
                    OriginalWidth = image73.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image73, typeof(byte[])),
                    Title = "The Physicists"
                },

                new Photo
                {
                    OriginalHeight = image74.Height,
                    OriginalWidth = image74.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image74, typeof(byte[])),
                    Title = "The 24-Hour Plays - Season 5"
                },

                // Photos for season 4
                new Photo
                {
                    OriginalHeight = image75.Height,
                    OriginalWidth = image75.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image75, typeof(byte[])),
                    Title = "The Baptism"
                },

                new Photo
                {
                    OriginalHeight = image76.Height,
                    OriginalWidth = image76.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image76, typeof(byte[])),
                    Title = "Brutality of Fact"
                },

                new Photo
                {
                    OriginalHeight = image77.Height,
                    OriginalWidth = image77.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image77, typeof(byte[])),
                    Title = "Poona the Fuckdog: And Other Plays for Children"
                },



                new Photo
                {
                    OriginalHeight = image78.Height,
                    OriginalWidth = image78.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image78, typeof(byte[])),
                    Title = "Popcorn"
                },

                // Photos for season 3
                new Photo
                {
                    OriginalHeight = image79.Height,
                    OriginalWidth = image79.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image79, typeof(byte[])),
                    Title = "Hellcab"
                },

                new Photo
                {
                    OriginalHeight = image80.Height,
                    OriginalWidth = image80.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image80, typeof(byte[])),
                    Title = "The Anger of Ernest and Ernestine"
                },

                new Photo
                {
                    OriginalHeight = image81.Height,
                    OriginalWidth = image81.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image81, typeof(byte[])),
                    Title = "Lion in the Streets"
                },

                new Photo
                {
                    OriginalHeight = image82.Height,
                    OriginalWidth = image82.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image82, typeof(byte[])),
                    Title = "The King has Gone to Tenebrae"
                },

                new Photo
                {
                    OriginalHeight = image83.Height,
                    OriginalWidth = image83.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image83, typeof(byte[])),
                    Title = "The Grey Zone"
                },

                // Photos for season 2
                new Photo
                {
                    OriginalHeight = image84.Height,
                    OriginalWidth = image84.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image84, typeof(byte[])),
                    Title = "This is a Play"
                },

                new Photo
                {
                    OriginalHeight = image85.Height,
                    OriginalWidth = image85.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image85, typeof(byte[])),
                    Title = "Criminal Genius"
                },

                new Photo
                {
                    OriginalHeight = image86.Height,
                    OriginalWidth = image86.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image86, typeof(byte[])),
                    Title = "Never Swim Alone"
                },

                new Photo
                {
                    OriginalHeight = image87.Height,
                    OriginalWidth = image87.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image87, typeof(byte[])),
                    Title = "Two-Headed Roommate"
                },

                new Photo
                {
                    OriginalHeight = image88.Height,
                    OriginalWidth = image88.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image88, typeof(byte[])),
                    Title = "Larry and the Werewolf"
                },

                // Photos for season 1
                new Photo
                {
                    OriginalHeight = image89.Height,
                    OriginalWidth = image89.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image89, typeof(byte[])),
                    Title = "A Woman, Alone"
                },

                new Photo
                {
                    OriginalHeight = image90.Height,
                    OriginalWidth = image90.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image90, typeof(byte[])),
                    Title = "Mass Murder"
                },
            };
            photos.ForEach(Photo => context.Photo.AddOrUpdate(p => p.PhotoFile, Photo));
            context.SaveChanges();




            var productionphoto = new List<ProductionPhotos>
            {
                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "Photo Unavailable").FirstOrDefault().PhotoId,
                    Title = "Photo Unavailable",
                    Description = "This photo is used when there are no photos to display or the photo does not load properly"
                },
                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "Hamilton Image 1").FirstOrDefault().PhotoId,
                    Title = "Hamilton",
                    Description = "Actors performing \"The Story Of Tonight\".",
                    Production = context.Productions.Where(name => name.Title == "Hamilton").FirstOrDefault()
                },
                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "Hamilton Image 2").FirstOrDefault().PhotoId,
                    Title = "Hamilton",
                    Description = "Hamilton Cover.",
                    Production = context.Productions.Where(name => name.Title == "Hamilton").FirstOrDefault()
                },
                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "Phantom Of The Opera Image 1").FirstOrDefault().PhotoId,
                    Title = "Phantom Of The Opera",
                    Description = "Phantom Of The Opera Cover.",
                    Production = context.Productions.Where(name => name.Title == "Phantom of the Opera").FirstOrDefault()
                },
                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "Phantom Of The Opera Image 2").FirstOrDefault().PhotoId,
                    Title = "Phantom Of The Opera",
                    Description = "The Phantom and Christine embrace.",
                    Production = context.Productions.Where(name => name.Title == "Phantom of the Opera").FirstOrDefault()
                },
                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "The Book of Mormon Image 1").FirstOrDefault().PhotoId,
                    Title = "The Book of Mormon",
                    Description = "Kevin Price main stage.",
                    Production = context.Productions.Where(name => name.Title == "The Book of Mormon").FirstOrDefault()
                },
                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "The Book of Mormon Image 2").FirstOrDefault().PhotoId,
                    Title = "The Book of Mormon",
                    Description = "General Butt-F******-Naked and Kevin Price singing together.",
                    Production = context.Productions.Where(name => name.Title == "The Book of Mormon").FirstOrDefault()
                },
                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "Wicked Image 1").FirstOrDefault().PhotoId,
                    Title = "Wicked",
                    Description = "Wicked Cover",
                    Production = context.Productions.Where(name => name.Title == "Wicked").FirstOrDefault()
                },
                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "Wicked Image 2").FirstOrDefault().PhotoId,
                    Title = "Wicked",
                    Description = "Elphaba Thropp in the spotlight.",
                    Production = context.Productions.Where(name => name.Title == "Wicked").FirstOrDefault()
                },
                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "How to Succeed in Business Without Really Trying Image 1").FirstOrDefault().PhotoId,
                    Title = "How to Succeed in Business Without Really Trying",
                    Description = "Smitty and the other secretaries read up on \"How to Hook a Tycoon and Take Him For All He’s Worth\".",
                    Production = context.Productions.Where(name => name.Title == "How to Succeed in Business Without Really Trying").FirstOrDefault()
                },
                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "How to Succeed in Business Without Really Trying Image 2").FirstOrDefault().PhotoId,
                    Title = "How to Succeed in Business Without Really Trying",
                    Description = "Daniel Radcliffe as Pierrepont Finch.",
                    Production = context.Productions.Where(name => name.Title == "How to Succeed in Business Without Really Trying").FirstOrDefault()
                },
                // Production photos for season 20
                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "Assistance Image").FirstOrDefault().PhotoId,
                    Title = "Assistance",
                    Description = "Assistance cover",
                    Production = context.Productions.Where(name => name.Title == "Assistance").FirstOrDefault()
                },
                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "Carnivora Image").FirstOrDefault().PhotoId,
                    Title = "Carnivora",
                    Description = "Carnivora cover",
                    Production = context.Productions.Where(name => name.Title == "Carnivora").FirstOrDefault()
                },
                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "A Maze Image").FirstOrDefault().PhotoId,
                    Title = "A Maze",
                    Description = "A Maze cover",
                    Production = context.Productions.Where(name => name.Title == "A Maze").FirstOrDefault()
                },
                // Production photos for season 19
                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "The Drunken City Image").FirstOrDefault().PhotoId,
                    Title = "The Drunken City",
                    Description = "The Drunken City cover",
                    Production = context.Productions.Where(name => name.Title == "The Drunken City").FirstOrDefault()
                },
                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "I Want to Destroy You Image").FirstOrDefault().PhotoId,
                    Title = "I Want to Destroy You",
                    Description = "I Want to Destroy You cover",
                    Production = context.Productions.Where(name => name.Title == "I Want to Destroy You").FirstOrDefault()
                },
                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "Love and Information Image").FirstOrDefault().PhotoId,
                    Title = "Love and Information",
                    Description = "Love and Information cover",
                    Production = context.Productions.Where(name => name.Title == "Love and Information").FirstOrDefault()
                },
                // Production photos for season 18
                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "Bob: A Life in Five Acts Image").FirstOrDefault().PhotoId,
                    Title = "Bob: A Life in Five Acts",
                    Description = "Bob: A Life in Five Acts cover",
                    Production = context.Productions.Where(name => name.Title == "Bob: A Life in Five Acts").FirstOrDefault()
                },
                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "Sexual Neuroses of Our Parents Image").FirstOrDefault().PhotoId,
                    Title = "Sexual Neuroses of Our Parents",
                    Description = "Sexual Neuroses of Our Parents cover",
                    Production = context.Productions.Where(name => name.Title == "Sexual Neuroses of Our Parents").FirstOrDefault()
                },
                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "The School for Lies Image").FirstOrDefault().PhotoId,
                    Title = "The School for Lies",
                    Description = "The School for Lies cover",
                    Production = context.Productions.Where(name => name.Title == "The School for Lies").FirstOrDefault()
                },
                // Production photos for season 17
                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "Dr. Jekyll and Mr. Hyde Image").FirstOrDefault().PhotoId,
                    Title = "Dr. Jekyll and Mr. Hyde",
                    Description = "Dr. Jekyll and Mr. Hyde cover",
                    Production = context.Productions.Where(name => name.Title == "Dr. Jekyll and Mr. Hyde").FirstOrDefault()
                },
                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "The End of Sex Image").FirstOrDefault().PhotoId,
                    Title = "The End of Sex",
                    Description = "The End of Sex cover",
                    Production = context.Productions.Where(name => name.Title == "The End of Sex").FirstOrDefault()
                },
                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "pool (no water) Image").FirstOrDefault().PhotoId,
                    Title = "pool (no water)",
                    Description = "pool (no water) cover",
                    Production = context.Productions.Where(name => name.Title == "pool (no water)").FirstOrDefault()
                },
                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "Anonymous Theatre:  The Crucible Image").FirstOrDefault().PhotoId,
                    Title = "Anonymous Theatre:  The Crucible",
                    Description = "Anonymous Theatre:  The Crucible cover",
                    Production = context.Productions.Where(name => name.Title == "Anonymous Theatre:  The Crucible").FirstOrDefault()
                },
                // Production photos for season 16
                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "Mother Courage & Her Children Image").FirstOrDefault().PhotoId,
                    Title = "Mother Courage & Her Children",
                    Description = "Mother Courage & Her Children cover",
                    Production = context.Productions.Where(name => name.Title == "Mother Courage & Her Children").FirstOrDefault()
                },
                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "The Velvet Sky Image").FirstOrDefault().PhotoId,
                    Title = "The Velvet Sky",
                    Description = "The Velvet Sky cover",
                    Production = context.Productions.Where(name => name.Title == "The Velvet Sky").FirstOrDefault()
                },
                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "Aloha, Say the Pretty Girls Image").FirstOrDefault().PhotoId,
                    Title = "Aloha, Say the Pretty Girls",
                    Description = "Aloha, Say the Pretty Girls cover",
                    Production = context.Productions.Where(name => name.Title == "Aloha, Say the Pretty Girls").FirstOrDefault()
                },
                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "Anonymous Theatre:  The Skin of Our Teeth Image").FirstOrDefault().PhotoId,
                    Title = "Anonymous Theatre:  The Skin of Our Teeth",
                    Description = "Anonymous Theatre:  The Skin of Our Teeth cover",
                    Production = context.Productions.Where(name => name.Title == "Anonymous Theatre:  The Skin of Our Teeth").FirstOrDefault()
                },
                 // Production photos for season 15

                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "Cloud 9").FirstOrDefault().PhotoId,
                    Title = "Cloud 9",
                    Description = "Cloud 9 cover",
                    Production = context.Productions.Where(name => name.Title == "Cloud 9").FirstOrDefault()
                },
                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "Hunter Gatherers").FirstOrDefault().PhotoId,
                    Title = "Hunter Gatherers",
                    Description = "Hunter Gatherers cover",
                    Production = context.Productions.Where(name => name.Title == "Hunter Gatherers").FirstOrDefault()
                },
                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "The American Pilot").FirstOrDefault().PhotoId,
                    Title = "The American Pilot",
                    Description = "The American Pilot cover",
                    Production = context.Productions.Where(name => name.Title == "The American Pilot").FirstOrDefault()
                },
                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "Anonymous Theatre: The Good Doctor").FirstOrDefault().PhotoId,
                    Title = "Anonymous Theatre: The Good Doctor",
                    Description = "Anonymous Theatre: The Good Doctor",
                    Production = context.Productions.Where(name => name.Title == "Anonymous Theatre: The Good Doctor").FirstOrDefault()
                },

                 // Production photos for season 14
                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "Dead Man's Cell Phone").FirstOrDefault().PhotoId,
                    Title = "Dead Man's Cell Phone",
                    Description = "Dead Man's Cell Phone",
                    Production = context.Productions.Where(name => name.Title == "Dead Man's Cell Phone").FirstOrDefault()
                },
                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "99 Ways to Fuck a Swan").FirstOrDefault().PhotoId,
                    Title = "99 Ways to Fuck a Swan",
                    Description = "99 Ways to Fuck a Swan cover",
                    Production = context.Productions.Where(name => name.Title == "99 Ways to Fuck a Swan").FirstOrDefault()
                },
                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "The Adding Machine").FirstOrDefault().PhotoId,
                    Title = "The Adding Machine",
                    Description = "The Adding Machine cover",
                    Production = context.Productions.Where(name => name.Title == "The Adding Machine").FirstOrDefault()
                },
                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "Anonymous Theatre: You Can't Take It with You").FirstOrDefault().PhotoId,
                    Title = "Anonymous Theatre: You Can't Take It with You",
                    Description = "Anonymous Theatre: You Can't Take It with You",
                    Production = context.Productions.Where(name => name.Title == "Anonymous Theatre: You Can't Take It with You").FirstOrDefault()
                },

                 // Production photos for season 13
                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "The Ruby Sunrise").FirstOrDefault().PhotoId,
                    Title = "The Ruby Sunrise",
                    Description = "The Ruby Sunrise",
                    Production = context.Productions.Where(name => name.Title == "The Ruby Sunrise").FirstOrDefault()
                },
                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "Boom").FirstOrDefault().PhotoId,
                    Title = "Boom",
                    Description = "Boom",
                    Production = context.Productions.Where(name => name.Title == "Boom").FirstOrDefault()
                },
                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "God's Ear").FirstOrDefault().PhotoId,
                    Title = "God's Ear",
                    Description = "God's Ear",
                    Production = context.Productions.Where(name => name.Title == "God's Ear").FirstOrDefault()
                },
                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "Anonymous Theatre: Lend Me a Tenor").FirstOrDefault().PhotoId,
                    Title = "Anonymous Theatre: Lend Me a Tenor",
                    Description = "Anonymous Theatre: Lend Me a Tenor",
                    Production = context.Productions.Where(name => name.Title == "Anonymous Theatre: Lend Me a Tenor").FirstOrDefault()
                },

                 // Production photos for season 12

                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "Pterodactyls").FirstOrDefault().PhotoId,
                    Title = "Pterodactyls",
                    Description = "Pterodactyls",
                    Production = context.Productions.Where(name => name.Title == "Pterodactyls").FirstOrDefault()
                },

                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "Romance").FirstOrDefault().PhotoId,
                    Title = "Romance",
                    Description = "Romance",
                    Production = context.Productions.Where(name => name.Title == "Romance").FirstOrDefault()
                },

                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "Freakshow").FirstOrDefault().PhotoId,
                    Title = "Freakshow",
                    Description = "Freakshow",
                    Production = context.Productions.Where(name => name.Title == "Freakshow").FirstOrDefault()
                },

                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "Anonymous Theatre: MacBeth").FirstOrDefault().PhotoId,
                    Title = "Anonymous Theatre: MacBeth",
                    Description = "Anonymous Theatre: MacBeth",
                    Production = context.Productions.Where(name => name.Title == "Anonymous Theatre: MacBeth").FirstOrDefault()
                },

                 // Production photos for season 11

                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "A Doll's House").FirstOrDefault().PhotoId,
                    Title = "A Doll's House",
                    Description = "A Doll's House",
                    Production = context.Productions.Where(name => name.Title == "A Doll's House").FirstOrDefault()
                },

                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "Where's My Money? ").FirstOrDefault().PhotoId,
                    Title = "Where's My Money? ",
                    Description = "Where's My Money? ",
                    Production = context.Productions.Where(name => name.Title == "Where's My Money? ").FirstOrDefault()
                },

                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "The Long Christmas Ride Home").FirstOrDefault().PhotoId,
                    Title = "The Long Christmas Ride Home",
                    Description = "The Long Christmas Ride Home",
                    Production = context.Productions.Where(name => name.Title == "The Long Christmas Ride Home").FirstOrDefault()
                },

                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "Anonymous Theatre: Rumors").FirstOrDefault().PhotoId,
                    Title = "Anonymous Theatre: Rumors",
                    Description = "Anonymous Theatre: Rumors",
                    Production = context.Productions.Where(name => name.Title == "Anonymous Theatre: Rumors").FirstOrDefault()
                },

                // Production photos for season 10

                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "Valparaiso").FirstOrDefault().PhotoId,
                    Title = "Valparaiso",
                    Description = "Valparaiso",
                    Production = context.Productions.Where(name => name.Title == "Valparaiso").FirstOrDefault()
                },

                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "Escape from Happiness").FirstOrDefault().PhotoId,
                    Title = "Escape from Happiness",
                    Description = "Escape from Happiness",
                    Production = context.Productions.Where(name => name.Title == "Escape from Happiness").FirstOrDefault()
                },

                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "Tango").FirstOrDefault().PhotoId,
                    Title = "Tango",
                    Description = "Tango",
                    Production = context.Productions.Where(name => name.Title == "Tango").FirstOrDefault()
                },

                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "Anonymous Theatre: A Funny Thing Happened on the Way to the Forum").FirstOrDefault().PhotoId,
                    Title = "Anonymous Theatre: A Funny Thing Happened on the Way to the Forum",
                    Description = "Anonymous Theatre: A Funny Thing Happened on the Way to the Forum",
                    Production = context.Productions.Where(name => name.Title == "Anonymous Theatre: A Funny Thing Happened on the Way to the Forum").FirstOrDefault()
                },

                // production photos for season 9
                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "The Flu Season").FirstOrDefault().PhotoId,
                    Title = "The Flu Season",
                    Description = "The Flu Season",
                    Production = context.Productions.Where(name => name.Title == "The Flu Season").FirstOrDefault()
                },

                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "Love of the Nightengale").FirstOrDefault().PhotoId,
                    Title = "Love of the Nightengale",
                    Description = "Love of the Nightengale",
                    Production = context.Productions.Where(name => name.Title == "Love of the Nightengale").FirstOrDefault()
                },

                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "Like I Say").FirstOrDefault().PhotoId,
                    Title = "Like I Say",
                    Description = "Like I Say",
                    Production = context.Productions.Where(name => name.Title == "Like I Say").FirstOrDefault()
                },

                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "Big Wheel Bingo").FirstOrDefault().PhotoId,
                    Title = "Big Wheel Bingo",
                    Description = "Big Wheel Bingo",
                    Production = context.Productions.Where(name => name.Title == "Big Wheel Bingo").FirstOrDefault()
                },

                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "The 24-Hour Plays - Season 9").FirstOrDefault().PhotoId,
                    Title = "The 24-Hour Plays - Season 9",
                    Description = "The 24-Hour Plays - Season 9",
                    Production = context.Productions.Where(name => name.Title == "The 24-Hour Plays" && name.Season == 9).FirstOrDefault()
                },

                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "Anonymous Theatre: The Learned Ladies").FirstOrDefault().PhotoId,
                    Title = "Anonymous Theatre: The Learned Ladies",
                    Description = "Anonymous Theatre: The Learned Ladies",
                    Production = context.Productions.Where(name => name.Title == "Anonymous Theatre: The Learned Ladies").FirstOrDefault()
                },

                // production photos for season 8
                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "A Lie of the Mind").FirstOrDefault().PhotoId,
                    Title = "A Lie of the Mind",
                    Description = "A Lie of the Mind",
                    Production = context.Productions.Where(name => name.Title == "A Lie of the Mind").FirstOrDefault()
                },

                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "The Devil Inside").FirstOrDefault().PhotoId,
                    Title = "The Devil Inside",
                    Description = "The Devil Inside",
                    Production = context.Productions.Where(name => name.Title == "The Devil Inside").FirstOrDefault()
                },

                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "A Bright Room Called Day").FirstOrDefault().PhotoId,
                    Title = "A Bright Room Called Day",
                    Description = "A Bright Room Called Day",
                    Production = context.Productions.Where(name => name.Title == "A Bright Room Called Day").FirstOrDefault()
                },

                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "Anonymous Theatre: Don't Drink the Water").FirstOrDefault().PhotoId,
                    Title = "Anonymous Theatre: Don't Drink the Water",
                    Description = "Anonymous Theatre: Don't Drink the Water",
                    Production = context.Productions.Where(name => name.Title == "Anonymous Theatre: Don't Drink the Water").FirstOrDefault()
                },

                // production photos for season 7
                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "Death and the Maiden").FirstOrDefault().PhotoId,
                    Title = "Death and the Maiden",
                    Description = "Death and the Maiden",
                    Production = context.Productions.Where(name => name.Title == "Death and the Maiden").FirstOrDefault()
                },

                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "Faust.Us.").FirstOrDefault().PhotoId,
                    Title = "Faust.Us.",
                    Description = "Faust.Us.",
                    Production = context.Productions.Where(name => name.Title == "Faust.Us.").FirstOrDefault()
                },

                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "Big Love").FirstOrDefault().PhotoId,
                    Title = "Big Love",
                    Description = "Big Love",
                    Production = context.Productions.Where(name => name.Title == "Big Love").FirstOrDefault()
                },

                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "The 24-Hour Plays - Season 7").FirstOrDefault().PhotoId,
                    Title = "The 24-Hour Plays - Season 7",
                    Description = "The 24-Hour Plays - Season 7",
                    Production = context.Productions.Where(name => name.Title == "The 24-Hour Plays" && name.Season == 7).FirstOrDefault()
                },

                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "Anonymous Theatre: Picasso at the Lapin Agile").FirstOrDefault().PhotoId,
                    Title = "Anonymous Theatre: Picasso at the Lapin Agile",
                    Description = "Anonymous Theatre: Picasso at the Lapin Agile",
                    Production = context.Productions.Where(name => name.Title == "Anonymous Theatre: Picasso at the Lapin Agile").FirstOrDefault()
                },

                // production photos for season 6
                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "Prague-Nosis!").FirstOrDefault().PhotoId,
                    Title = "Prague-Nosis!",
                    Description = "Prague-Nosis!",
                    Production = context.Productions.Where(name => name.Title == "Prague-Nosis!").FirstOrDefault()
                },

                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "Freedomland").FirstOrDefault().PhotoId,
                    Title = "Freedomland",
                    Description = "Freedomland",
                    Production = context.Productions.Where(name => name.Title == "Freedomland").FirstOrDefault()
                },

                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "The 24-Hour Plays - Season 6").FirstOrDefault().PhotoId,
                    Title = "The 24-Hour Plays - Season 6",
                    Description = "The 24-Hour Plays - Season 6",
                    Production = context.Productions.Where(name => name.Title == "The 24-Hour Plays").Where(name => name.Season == 6).FirstOrDefault()
                },

                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "Anonymous Theatre: The Importance of Being Earnest").FirstOrDefault().PhotoId,
                    Title = "Anonymous Theatre: The Importance of Being Earnest",
                    Description = "Anonymous Theatre: The Importance of Being Earnest",
                    Production = context.Productions.Where(name => name.Title == "Anonymous Theatre: The Importance of Being Earnest").FirstOrDefault()
                },

                // production photos for season 5
                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "Masterpieces").FirstOrDefault().PhotoId,
                    Title = "Masterpieces",
                    Description = "Masterpieces",
                    Production = context.Productions.Where(name => name.Title == "Masterpieces").FirstOrDefault()
                },

                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "Rashomon").FirstOrDefault().PhotoId,
                    Title = "Rashomon",
                    Description = "Rashomon",
                    Production = context.Productions.Where(name => name.Title == "Rashomon").FirstOrDefault()
                },

                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "The Physicists").FirstOrDefault().PhotoId,
                    Title = "The Physicists",
                    Description = "The Physicists",
                    Production = context.Productions.Where(name => name.Title == "The Physicists").FirstOrDefault()
                },

                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "The 24-Hour Plays - Season 5").FirstOrDefault().PhotoId,
                    Title = "The 24-Hour Plays - Season 5",
                    Description = "The 24-Hour Plays - Season 5",
                    Production = context.Productions.Where(name => name.Title == "The 24-Hour Plays" && name.Season == 5).FirstOrDefault()
                },

                // production photos for season 4

                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "The Baptism").FirstOrDefault().PhotoId,
                    Title = "The Baptism",
                    Description = "The Baptism",
                    Production = context.Productions.Where(name => name.Title == "The Baptism").FirstOrDefault()
                },

                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "Brutality of Fact").FirstOrDefault().PhotoId,
                    Title = "Brutality of Fact",
                    Description = "Brutality of Fact",
                    Production = context.Productions.Where(name => name.Title == "Brutality of Fact").FirstOrDefault()
                },

                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "Poona the Fuckdog: And Other Plays for Children").FirstOrDefault().PhotoId,
                    Title = "Poona the Fuckdog: And Other Plays for Children",
                    Description = "Poona the Fuckdog: And Other Plays for Children",
                    Production = context.Productions.Where(name => name.Title == "Poona the Fuckdog: And Other Plays for Children").FirstOrDefault()
                },

                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "Popcorn").FirstOrDefault().PhotoId,
                    Title = "Popcorn",
                    Description = "Popcorn",
                    Production = context.Productions.Where(name => name.Title == "Popcorn").FirstOrDefault()
                },

                // production photos for season 3

                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "Hellcab").FirstOrDefault().PhotoId,
                    Title = "Hellcab",
                    Description = "Hellcab",
                    Production = context.Productions.Where(name => name.Title == "Hellcab").FirstOrDefault()
                },

                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "The Anger of Ernest and Ernestine").FirstOrDefault().PhotoId,
                    Title = "The Anger of Ernest and Ernestine",
                    Description = "The Anger of Ernest and Ernestine",
                    Production = context.Productions.Where(name => name.Title == "The Anger of Ernest and Ernestine").FirstOrDefault()
                },

                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "Lion in the Streets").FirstOrDefault().PhotoId,
                    Title = "Lion in the Streets",
                    Description = "Lion in the Streets",
                    Production = context.Productions.Where(name => name.Title == "Lion in the Streets").FirstOrDefault()
                },

                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "The King has Gone to Tenebrae").FirstOrDefault().PhotoId,
                    Title = "The King has Gone to Tenebrae",
                    Description = "The King has Gone to Tenebrae",
                    Production = context.Productions.Where(name => name.Title == "The King has Gone to Tenebrae").FirstOrDefault()
                },

                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "The Grey Zone").FirstOrDefault().PhotoId,
                    Title = "The Grey Zone",
                    Description = "The Grey Zone",
                    Production = context.Productions.Where(name => name.Title == "The Grey Zone").FirstOrDefault()
                },

                // production photos for season 2

                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "This is a Play").FirstOrDefault().PhotoId,
                    Title = "This is a Play",
                    Description = "This is a Play",
                    Production = context.Productions.Where(name => name.Title == "This is a Play").FirstOrDefault()
                },

                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "Criminal Genius").FirstOrDefault().PhotoId,
                    Title = "Criminal Genius",
                    Description = "Criminal Genius",
                    Production = context.Productions.Where(name => name.Title == "Criminal Genius").FirstOrDefault()
                },

                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "Never Swim Alone").FirstOrDefault().PhotoId,
                    Title = "Never Swim Alone",
                    Description = "Never Swim Alone",
                    Production = context.Productions.Where(name => name.Title == "Never Swim Alone").FirstOrDefault()
                },

                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "Two-Headed Roommate").FirstOrDefault().PhotoId,
                    Title = "Two-Headed Roommate",
                    Description = "Two-Headed Roommate",
                    Production = context.Productions.Where(name => name.Title == "Two-Headed Roommate").FirstOrDefault()
                },

                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "Larry and the Werewolf").FirstOrDefault().PhotoId,
                    Title = "Larry and the Werewolf",
                    Description = "Larry and the Werewolf",
                    Production = context.Productions.Where(name => name.Title == "Larry and the Werewolf").FirstOrDefault()
                },

                // production photos for season 1

                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "A Woman, Alone").FirstOrDefault().PhotoId,
                    Title = "A Woman, Alone",
                    Description = "A Woman, Alone",
                    Production = context.Productions.Where(name => name.Title == "A Woman, Alone").FirstOrDefault()
                },

                new ProductionPhotos
                {
                    PhotoId = context.Photo.Where(photo => photo.Title == "Mass Murder").FirstOrDefault().PhotoId,
                    Title = "Mass Murder",
                    Description = "Mass Murder",
                    Production = context.Productions.Where(name => name.Title == "Mass Murder").FirstOrDefault()
                },
            };
            productionphoto.ForEach(prodphoto => context.ProductionPhotos.AddOrUpdate(p => p.PhotoId, prodphoto));
            context.SaveChanges();

            // get all production titles
            // foreach production title in production titles
            // get production
            // assign production's default photo to a production photo that contains that title



            var productions = context.Productions.ToList();
            var productionPhotos = context.ProductionPhotos.ToList();
            foreach (var production in productions)
            {
                production.DefaultPhoto = productionPhotos.Where(productionPhoto => productionPhoto.Production == production).FirstOrDefault();
            }
            context.SaveChanges();
        }

        //Seeding database with dummy Parts
        private void SeedParts()
        {

            var parts = new List<Part>
            {
                new Part{
                    Production = context.Productions.Where(p => p.Title == "Hamilton").FirstOrDefault(),
                    Character="Alexander Hamilton",
                    Type=Enum.PositionEnum.Actor,
                    Person= context.CastMembers.Where(c => c.Name == "London Bauman").FirstOrDefault(),
                    Details="The ten-dollar Founding Father without a father, Alexander Hamilton starts out as a penniless immigrant but " +
                    "rises up in the ranks and becomes an aide to George Washington himself. After the American Revolution, he becomes one of " +
                    "the most prominent politicians of the young United States, creating USA's financial system, writing in defense of the " +
                    "Constitution and founding the Federalist Party."},

                new Part{
                    Production= context.Productions.Where(p => p.Title == "Hamilton").FirstOrDefault(),
                    Character="Devon Roberts",
                    Type=Enum.PositionEnum.Director,
                    Person= context.CastMembers.Where(c => c.Name == "Devon Roberts").FirstOrDefault(),
                    Details="The director manages the creative aspects of the production. They direct the making of a film by visualizing the " +
                    "script while guiding the actors and technical crew to capture the vision for the screen. They control the film's dramatic and " +
                    "artistic aspects"},

                new Part{
                    Production= context.Productions.Where(p => p.Title == "Phantom of the Opera").FirstOrDefault(),
                    Character="Christine Daaé",
                    Type=Enum.PositionEnum.Actor,
                    Person= context.CastMembers.Where(c => c.Name == "Jacquelle Davis").FirstOrDefault(),
                    Details="A young Swedish soprano who becomes torn between her loyalty for her mentor Erik, and her love for her childhood friend " +
                    "Raoul de Chagny."},

                new Part{
                    Production= context.Productions.Where(p => p.Title == "Phantom of the Opera").FirstOrDefault(),
                    Character="Erik",
                    Type=Enum.PositionEnum.Actor,
                    Person= context.CastMembers.Where(c => c.Name == "Tom Mounsey").FirstOrDefault(),
                    Details="Known as “P. of the Opera,” “the ghost,” “the Voice” and “the Master of the Traps,” Erik is the antagonist " +
                    "of the novel and a tragic, violent, and ultimately mysterious figure. Although the narrator asserts that Erik is a " +
                    "human being, he displays characteristics that suggest he might be more supernatural than purely human: his " +
                    "appearance as a skeleton covered in rotten skin, his extraordinary singing abilities, and his capacity for " +
                    "ventriloquism, which allows him to project his voice anywhere he pleases, making it seem as though he is in various" +
                    " places at once."},

                new Part{
                    Production= context.Productions.Where(p => p.Title == "Phantom of the Opera").FirstOrDefault(),
                    Character="Devon Roberts",
                    Type=Enum.PositionEnum.Director,
                    Person= context.CastMembers.Where(c => c.Name == "Devon Roberts").FirstOrDefault(),
                    Details="The director manages the creative aspects of the production. They direct the making of a film by visualizing the " +
                    "script while guiding the actors and technical crew to capture the vision for the screen. They control the film's dramatic and " +
                    "artistic aspects"},

                new Part{
                    Production= context.Productions.Where(p => p.Title == "The Book of Mormon").FirstOrDefault(),
                    Character="Arnold Cunningham",
                    Type=Enum.PositionEnum.Actor,
                    Person= context.CastMembers.Where(c => c.Name == "Heath Hyun Houghton").FirstOrDefault(),
                    Details="Elder Arnold Cunningham is an enthusiastic but childish young Mormon who's excited to go out and proselytize with his " +
                    "new best friend... even though he hasn't actually read the Book of Mormon. He also has a very active imagination - that is to say, " +
                    "he lies a lot. This causes problems in the remote Ugandan village they've been assigned to."},

                new Part{
                    Production= context.Productions.Where(p => p.Title == "The Book of Mormon").FirstOrDefault(),
                    Character="Devon Roberts",
                    Type=Enum.PositionEnum.Director,
                    Person= context.CastMembers.Where(c => c.Name == "Devon Roberts").FirstOrDefault(),
                    Details="The director manages the creative aspects of the production. They direct the making of a film by visualizing the " +
                    "script while guiding the actors and technical crew to capture the vision for the screen. They control the film's dramatic and " +
                    "artistic aspects"},

                new Part{
                    Production= context.Productions.Where(p => p.Title == "Wicked").FirstOrDefault(),
                    Character="Glinda the Good Witch",
                    Type=Enum.PositionEnum.Actor,
                    Person= context.CastMembers.Where(c => c.Name == "Adriana Gantzer").FirstOrDefault(),
                    Details="Glinda is a very bubbly, perky, and popular girl. Unlike in The Wonderful Wizard of Oz she has blonde hair instead of red " +
                    "hair and a blue dress instead of a light pink dress."},

                new Part{
                    Production= context.Productions.Where(p => p.Title == "Wicked").FirstOrDefault(),
                    Character="Devon Roberts",
                    Type=Enum.PositionEnum.Director,
                    Person= context.CastMembers.Where(c => c.Name == "Devon Roberts").FirstOrDefault(),
                    Details="The director manages the creative aspects of the production. They direct the making of a film by visualizing the " +
                    "script while guiding the actors and technical crew to capture the vision for the screen. They control the film's dramatic and " +
                    "artistic aspects"},

                new Part{
                    Production= context.Productions.Where(p => p.Title == "How to Succeed in Business Without Really Trying").FirstOrDefault(),
                    Character="J. Pierrepont Finch",
                    Type=Enum.PositionEnum.Actor,
                    Person= context.CastMembers.Where(c => c.Name == "Tom Mounsey").FirstOrDefault(),
                    Details="Our story's protagonist. An irrepressible, clear-eyed, almost puckish hero, he is a window washer who applies for a job at " +
                    "the World Wide Wicket Company and attempts to climb the 'ladder of success' using instruction from his little book called How to Succeed " +
                    "in Business Without Really Trying"},

                new Part{

                    Production= context.Productions.Where(p => p.Title == "How to Succeed in Business Without Really Trying").FirstOrDefault(),
                    Character="Devon Roberts",
                    Type=Enum.PositionEnum.Director,
                    Person= context.CastMembers.Where(c => c.Name == "Devon Roberts").FirstOrDefault(),
                    Details="The director manages the creative aspects of the production. They direct the making of a film by visualizing the " +
                    "script while guiding the actors and technical crew to capture the vision for the screen. They control the film's dramatic and " +
                    "artistic aspects"},
            };

            parts.ForEach(x => // iterate through the list Parts
            {
                var tempPart = context.Parts.Where(p => p.Production.Title == x.Production.Title && p.Character == x.Character && p.Type == x.Type).FirstOrDefault(); // Where the production title, Character, and part Type match it will return the query data or null if it doesn't exist
                if (tempPart != null) // where it does not return null
                {
                    x.PartID = tempPart.PartID; // update the partID with the ID assigned to tempPart
                }
                context.Parts.AddOrUpdate(p => p.PartID, x); // runs the addorupdate- if tempPart returns null a new record will be added if it returned not null it will update based off of the PartID assigned in the if statement
            });
            context.SaveChanges();
        }



        //Created a new method to create Sponsor photos, convert Sponsor photos to bytes, add Sponsor photos to Photos table, and seed the Sponsors 
        //with the 5 existing sponsors from the live website.
        private void SeedSponsors()
        {
            //Create images
            string imagesRoot = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + @"\Content\Images");

            Image image1 = Image.FromFile(Path.Combine(imagesRoot, @"Ellyn-Bye-Logo.png"));
            Image image2 = Image.FromFile(Path.Combine(imagesRoot, @"LogoRoundColor.png"));
            Image image3 = Image.FromFile(Path.Combine(imagesRoot, @"Ninkasi-white-text.png"));
            Image image4 = Image.FromFile(Path.Combine(imagesRoot, @"OCF-logo-in-white-with-tagline-lg.png"));
            Image image5 = Image.FromFile(Path.Combine(imagesRoot, @"RACC2.png"));

            //Convert images
            var converter = new ImageConverter();

            var photos = new List<Photo>
            {
                new Photo
                {
                    OriginalHeight = image1.Height,
                    OriginalWidth = image1.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image1, typeof(byte[])),
                    Title = "Ellyn Bye"
                },
                new Photo
                {
                    OriginalHeight = image2.Height,
                    OriginalWidth = image2.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image2, typeof(byte[])),
                    Title = "Cider Riot!"
                },
                new Photo
                {
                    OriginalHeight = image3.Height,
                    OriginalWidth = image3.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image3, typeof(byte[])),
                    Title = "Ninkasi Brewing"
                },
                new Photo
                {
                    OriginalHeight = image4.Height,
                    OriginalWidth = image4.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image4, typeof(byte[])),
                    Title = "The Oregon Community Foundation"
                },
                new Photo
                {
                    OriginalHeight = image5.Height,
                    OriginalWidth = image5.Width,
                    PhotoFile = (byte[])converter.ConvertTo(image5, typeof(byte[])),
                    Title = "Regional Arts & Culture Council"
                },
            };

            //Add photos to Photo table
            photos.ForEach(photo => context.Photo.AddOrUpdate(c => c.PhotoFile, photo));
            context.SaveChanges();

            //Create sponsors
            var sponsor = new List<Sponsor>
            {

                new Sponsor{ Name = "Ellyn Bye", PhotoId = context.Photo.Where(photo => photo.Title == "Ellyn Bye").FirstOrDefault().PhotoId,
               /* Height = image1.Height, Width = image1.Width,*/ Current = true, Link = "", },

                new Sponsor{ Name = "Cider Riot!", PhotoId = context.Photo.Where(photo => photo.Title == "Cider Riot!").FirstOrDefault().PhotoId,
              /*  Height = image2.Height, Width = image2.Width, */Current = true, Link = "https://www.ciderriot.com", },

                new Sponsor{ Name = "Ninkasi Brewing", PhotoId = context.Photo.Where(photo => photo.Title == "Ninkasi Brewing").FirstOrDefault().PhotoId,
               /* Height = image3.Height, Width = image3.Width, */Current = true, Link = "https://ninkasibrewing.com", },

                new Sponsor{ Name = "The Oregon Community Foundation",
                PhotoId = context.Photo.Where(photo => photo.Title == "The Oregon Community Foundation").FirstOrDefault().PhotoId,
                /*Height = image4.Height, Width = image4.Width,*/Current = true, Link = "https://oregoncf.org", },

                new Sponsor{ Name = "Regional Arts & Culture Council",
                PhotoId = context.Photo.Where(photo => photo.Title == "Regional Arts & Culture Council").FirstOrDefault().PhotoId,
                /*Height = image5.Height, Width = image5.Width,*/ Current = true,
                Link = "https://racc.org", },

            };

            sponsor.ForEach(Sponsor => context.Sponsors.AddOrUpdate(c => c.Name, Sponsor));
            context.SaveChanges();
        }


        /* Method to seed the calendar index view with matinee events, evening events, and rental events. */
        private void SeedCalendarEvents()
        {
            var matineeCalendarEvents = new List<CalendarEvent>
            {
                new CalendarEvent
                {
                    Title = "Hamilton",
                    /* Using DateTime.Now.Year and DateTime.Now.Month will automatically recreate these same events for each
                     * year and month keeping it on the same relative day across different months by calling the FirstFridayOfMonth() method. */
			        StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, FirstFridayOfMonth(), 12, 00, 00),
                    EndDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, FirstFridayOfMonth(), 13, 30, 00),
                    TicketsAvailable = 25,
                    Color = "#db1a11",
                    AllDay = false,
                    ProductionId = context.Productions.Where(p => p.Title == "Hamilton").FirstOrDefault().ProductionId,
                },
                new CalendarEvent
                {
                    Title = "Phantom of the Opera",
                    StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, SecondMondayOfMonth(), 12, 00, 00),
                    EndDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, SecondMondayOfMonth(), 13, 30, 00),
                    TicketsAvailable = 40,
                    Color = "#db1a11",
                    AllDay = false,
                    ProductionId = context.Productions.Where(p => p.Title == "Phantom of the Opera").FirstOrDefault().ProductionId,
                }
            };
            /* Iterate through the list. */
            matineeCalendarEvents.ForEach(CalendarEvent =>
            {
                /* Where the calendar event title and start date match it will return the query data or null if it doesn't exist. */
                var tempMatEvent = context.CalendarEvent.Where(c => c.Title == CalendarEvent.Title && c.StartDate == CalendarEvent.StartDate).FirstOrDefault();
                /* If it doesn't return null */
                if (tempMatEvent != null)
                {
                    /* Update the calendar event EventId with the Id assigned to tempMatEvent */
                    CalendarEvent.EventId = tempMatEvent.EventId;
                }
                /* Runs AddOrUpdate. If tempMatEvent returns null a new record will be added otherwise if it returns not null it will update based off 
                 * of the EventID assigned in the if statement. */
                context.CalendarEvent.AddOrUpdate(c => c.EventId, CalendarEvent);
            });
            context.SaveChanges();

            var eveningCalendarEvents = new List<CalendarEvent>
            {
                new CalendarEvent
                {
                    Title = "Hamilton",
                    StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, FirstFridayOfMonth(), 20, 00, 00),
                    EndDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, FirstFridayOfMonth(), 21, 30, 00),
                    TicketsAvailable = 50,
                    Color = "#db1a11",
                    AllDay = false,
                    ProductionId = context.Productions.Where(p => p.Title == "Hamilton").FirstOrDefault().ProductionId,
                },
                 new CalendarEvent
                {
                    Title = "Phantom of the Opera",
                    StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, SecondMondayOfMonth(), 20, 00, 00),
                    EndDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, SecondMondayOfMonth(), 21, 30, 00),
                    TicketsAvailable = 20,
                    Color = "#db1a11",
                    AllDay = false,
                    ProductionId = context.Productions.Where(p => p.Title == "Phantom of the Opera").FirstOrDefault().ProductionId,
                }
            };
            eveningCalendarEvents.ForEach(CalendarEvent =>
            {
                var tempEveningEvent = context.CalendarEvent.Where(c => c.Title == CalendarEvent.Title && c.StartDate == CalendarEvent.StartDate).FirstOrDefault();
                if (tempEveningEvent != null)
                {
                    CalendarEvent.EventId = tempEveningEvent.EventId;
                }
                context.CalendarEvent.AddOrUpdate(c => c.EventId, CalendarEvent);
            });
            context.SaveChanges();

            var startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, ThirdWedOfMonth(), 11, 00, 00);
            var startDate2 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, SecondFridayOfMonth(), 10, 00, 00);
            var rentalEvents = new List<CalendarEvent>
            {
                new CalendarEvent
                {
                    Title = "Private Event",
                    StartDate = startDate,
                    EndDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, ThirdWedOfMonth(), 12, 30, 00),
                    Color = "#4287f5",
                    AllDay = false,
                },
                new CalendarEvent
                {
                    Title = "Private Event",
                    StartDate = startDate2,
                    EndDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, SecondFridayOfMonth(), 11, 30, 00),
                    Color = "#4287f5",
                    AllDay = false,
                }
            };
            rentalEvents.ForEach(CalendarEvent =>
            {
                var tempRental = context.CalendarEvent.Where(c => c.Title == CalendarEvent.Title && c.StartDate == CalendarEvent.StartDate).FirstOrDefault();
                if (tempRental != null)
                {
                    CalendarEvent.EventId = tempRental.EventId;
                }
                context.CalendarEvent.AddOrUpdate(c => c.EventId, CalendarEvent);
            });
            context.SaveChanges();
        }

        /* Method to get the first Friday of a given month and year. */
        public static int FirstFridayOfMonth()
        {
            /* Creates a new DateTime that reflects the firstFriday of the current year and month. */
            DateTime firstFriday = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            /* While the firstFriday.DayOfWeek is not Friday... */
            while (firstFriday.DayOfWeek != DayOfWeek.Friday)
            {
                /* ...add a day to firstFriday to update DateTime.Day. */
                firstFriday = firstFriday.AddDays(1);
            }
            return firstFriday.Day;
        }

        /* Method to get the second Monday of a given month and year. */
        public static int SecondMondayOfMonth()
        {
            /* Creates a new DateTime that reflects the secondMonday of the current year and month. */
            DateTime secondMonday = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            /* While secondMonday.DayOfWeek is not Monday... */
            while (secondMonday.DayOfWeek != DayOfWeek.Monday)
            {
                /* ...add a day to secondMonday to update the DateTime.Day. */
                secondMonday = secondMonday.AddDays(1);
            }
            /* Add 7 days to secondMonday, then return the day. */
            return secondMonday.AddDays(7).Day;
        }

        /* Method to get the 3rd Wednesday of a given month and year. */
        public static int ThirdWedOfMonth()
        {
            DateTime thirdWed = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            while (thirdWed.DayOfWeek != DayOfWeek.Wednesday)
            {
                thirdWed = thirdWed.AddDays(1);
            }
            return thirdWed.AddDays(14).Day;
        }

        /* Method to get the second Friday of a given month and year. */
        public static int SecondFridayOfMonth()
        {
            DateTime secondFriday = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            while (secondFriday.DayOfWeek != DayOfWeek.Friday)
            {
                secondFriday = secondFriday.AddDays(1);
            }
            return secondFriday.AddDays(7).Day;
        }

        /* Method to Seed the DisplayLink table in the database. */
        /* Naming convention for the Seeded info is as follows: [Page] - [Section Description] - [Element Description] */
        private void SeedDisplayLinks()
        {
            var displayLink = new List<DisplayLinks>
            {
                new DisplayLinks
                {
                    Name = "Home About - Buttons - Donate",
                    Text = "DONATE TO THEATRE VERTIGO",
                    Link = "https://app.arts-people.com/index.php?donation=verti"
                },
                new DisplayLinks
                {
                    Name = "Home About - Buttons - Ticketing",
                    Text = "TICKETING",
                    Link = "https://app.arts-people.com/index.php?ticketing=verti"
                },
                new DisplayLinks
                {
                    Name = "Home About - Dining - Deadshot",
                    Text = "Deadshot",
                    Link = "https://www.deadshotpdx.com/"
                },
                new DisplayLinks
                {
                    Name = "Home About - Dining - Firkin Tavern",
                    Text = "Firkin Tavern",
                    Link = "https://www.thefirkintavern.com/"
                },
                new DisplayLinks
                {
                    Name = "Home About - Dining - Food Fork and Drink",
                    Text = "Food Fork and Drink",
                    Link = "https://www.fordfoodanddrink.com/"
                },
                new DisplayLinks
                {
                    Name = "Home About - Dining - Double Dragon",
                    Text = "Double Dragon",
                    Link = "https://www.doubledragonpdx.com/"
                },
                new DisplayLinks
                {
                    Name = "Home About - Dining - Cellar Door Coffee Roasters",
                    Text = "Cellar Door Coffee Roasters",
                    Link = "https://www.cellardoorcoffee.com/"
                }
            };
            displayLink.ForEach(displayLinks => context.DisplayLinks.AddOrUpdate(d => new { d.Name, d.Text, d.Link }, displayLinks));
            context.SaveChanges();
        }

        protected void SeedDisplayInfo()
        {
            //var topcontent = new DisplayInfo();
            //topcontent.Title = "Archive Page-title";
            //topcontent.TextContent = "Theatre Vertigo Archive";

            //var history = new DisplayInfo();
            //history.Title = "History";
            //history.TextContent = "In 1997, Theatre Vertigo was founded by Paul Floding, " +
            //    "Nanette Pettit and Jeff Meyers.  Since then, Theatre Vertigo has performed " +
            //    "in numerous spaces including The Russell Street Theater, The Electric Company," +
            //    " Theater!Theatre!, and their current home, The Shoebox Theater.  From 2003 to " +
            //    "2014, Theatre Vertigo produced Anonymous Theatre as a summer fundraiser in " +
            //    "collaboration with The Anonymous Theatre Company.Other past collaborations include " +
            //    "defunkt theatre, Stark Raving Theater,and Tears of Joy Theatre.  \n\nTheatre Vertigo " +
            //    "has worked on world premieres including Faust.Us by Joseph Fisher, 99 Ways to " +
            //    "Fuck a Swan by Kim Rosenstock, and The End of Sex by Craig Jessen.  In 2016, Theatre " +
            //    "Vertigo produced its first officially commissioned work from a playwright, I Want To " +
            //    "Destroy You, by Rob Handel.";
            //var mission = new DisplayInfo();
            //mission.Title = "Mission Statement";
            //mission.TextContent = "Theatre Vertigo’s mission is to engage audiences through compelling, " +
            //    "ensemble - driven theatre with a focus on producing and developing new and rarely seen works.";

            //context.DisplayInfo.AddOrUpdate(d => new { d.Title, d.TextContent }, topcontent, history, mission);


            //Commented out previous seed method and re-built to be able to separate the "History" and "Mission Statement" contents. 
            //Let the previous format above with the new information if we need to revert back to that method. 
            var info = new List<DisplayInfo>() //re-formatted the seed method so it will be easier to add information later if needed
            {
                new DisplayInfo
                {
                    Title = "Archive Page-title",
                    TextContent = "Theatre Vertigo Archive"
                },

                new DisplayInfo
                {
                    Title = "Archive Page-content",
                    TextContent = "In 1997, Theatre Vertigo was founded by Paul Floding, Nanette Pettit and Jeff Meyers.  " +
                    "Since then, Theatre Vertigo has performed in numerous spaces including The Russell Street Theater, " +
                    "The Electric Company, Theater!Theatre!, and their current home, The Shoebox Theater." +
                    "From 2003 to 2014, Theatre Vertigo produced Anonymous Theatre as a summer fundraiser in collaboration " +
                    "with The Anonymous Theatre Company. Other past collaborations include defunkt theatre, " +
                    "Stark Raving Theater, and Tears of Joy Theatre." +
                    "<br />" +
                    "<br />" +
                    "Theatre Vertigo has worked on world premieres including <i>Faust</i>. <i>Us</i> by Joseph Fisher, " +
                    "<i>99 Ways to Fuck a Swan</i> by Kim Rosenstock, and <i>The End of Sex</i> by Craig Jessen." +
                    "In 2016, Theatre Vertigo produced its first officially commissioned work from a playwright, <i>I Want " +
                    "To Destroy You</i>, by Rob Handel." 
                    //For the Archive content page - same as the content in "History" DisplayInfo, but decided to leave it in case
                    //the theater wants to change this content on the archive page.
                },

                new DisplayInfo
                {
                    Title = "History",
                    TextContent = "In 1997, Theatre Vertigo was founded by Paul Floding, Nanette Pettit and Jeff Meyers.  " +
                    "Since then, Theatre Vertigo has performed in numerous spaces including The Russell Street Theater, " +
                    "The Electric Company, Theater!Theatre!, and their current home, The Shoebox Theater." +
                    "From 2003 to 2014, Theatre Vertigo produced Anonymous Theatre as a summer fundraiser in collaboration " +
                    "with The Anonymous Theatre Company. Other past collaborations include defunkt theatre, " +
                    "Stark Raving Theater, and Tears of Joy Theatre." +
                    "<br />" +
                    "<br />" +
                    "Theatre Vertigo has worked on world premieres including <i>Faust</i>. <i>Us</i> by Joseph Fisher, " +
                    "<i>99 Ways to Fuck a Swan</i> by Kim Rosenstock, and <i>The End of Sex</i> by Craig Jessen." +
                    "In 2016, Theatre Vertigo produced its first officially commissioned work from a playwright, <i>I Want " +
                    "To Destroy You</i>, by Rob Handel."
                },

                new DisplayInfo
                {
                    Title = "Mission",
                    TextContent = "Theatre Vertigo’s mission is to engage audiences through compelling, " +
                    "ensemble - driven theatre with a focus on producing and developing new and rarely seen works."
                }
            };
            info.ForEach(infos => context.DisplayInfo.AddOrUpdate(x => new { x.Title, x.TextContent }, infos));
            context.SaveChanges();
        }

    }
}
