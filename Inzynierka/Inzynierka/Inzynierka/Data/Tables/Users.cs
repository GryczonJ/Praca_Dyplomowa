using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Inzynierka.Data.Tables
{
    public class Users : IdentityUser<Guid>
    {
        public int    age { get; set; } = 0;
        public string surname { get; set; }
        public string aboutMe { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public bool banned { get; set; } = false;
        public bool Public { get; set; } = true;

        // Foreign key
        public Roles Role { get; set; }
        public int RoleId { get; set; } 
        public Image Photos { get; set; }
        public int PhotosId { get; set; }

        //reference
        // Many-to-many relationship with Cruises
        public List<CruisesParticipants> CruisesParticipants { get; set; } = new List<CruisesParticipants>();// One user can participate in many cruises
        public List<Cruises> Cruises { get; set; } = new List<Cruises>(); // One user can participate in many cruises Capitan ID
        public List<Comments> CommentsAsCreator { get; set; } = new List<Comments>(); // One user can have many comments>
        public List<Comments> CommentsAsProfile { get; set; } = new List<Comments>(); // One user can have many comments>
        public List<Charters> Charters { get; set; } = new List<Charters>(); // One user can participate in many charters
        public List<CruiseJoinRequest> CruiseJoinRequests { get; set; } = new List<CruiseJoinRequest>(); // One user can have many join requests
        public List<CruiseJoinRequest> CruiseJoinRequestsAsCapitan { get; set; } = new List<CruiseJoinRequest>(); // One user can have many join requests

        public List<Reservation> Reservation { get; set; } = new List<Reservation>(); // One user can have many reservations
        public List<Reports> ModeratorReports { get; set; } = new List<Reports>(); // One user can have many reports
        public List<Reports> SuspectUserReports { get; set; } = new List<Reports>(); // One user can have many reports

        public List<FavoriteCruises> FavoriteCruises { get; set; } = new List<FavoriteCruises>(); // One user can have many favorite cruises
        public List<FavoriteYachtsForSale> FavoriteYachtsForSale { get; set; } = new List<FavoriteYachtsForSale>(); // One user can have many favorite yachts

        public List<Notifications> Notifications { get; set; } = new List<Notifications>(); // One user can have many notifications
        public List<YachtSale> YachtSalesAsSeller { get; set; } = new List<YachtSale>(); // One user can sell many yachts
        public List<YachtSale> YachtSalesAsBuyer { get; set; } = new List<YachtSale>(); // One user can sell many yachts
        public List<Yachts> Yachts { get; set; } = new List<Yachts>(); // One user can have many yachts
    }
}
