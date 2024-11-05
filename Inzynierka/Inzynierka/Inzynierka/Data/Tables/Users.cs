namespace Inzynierka.Data.Tables
{
    public class Users
    {
        public int Id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public int    age { get; set; } = 0;
        public string name { get; set; }
        public string surname { get; set; }
        public string email { get; set; }
        public string phoneNumber { get; set; }
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
        public List<Comments> Comments { get; set; } = new List<Comments>(); // One user can have many comments>
        public List<Charters> Charters { get; set; } = new List<Charters>(); // One user can participate in many charters
        public List<CruiseJoinRequest> CruiseJoinRequests { get; set; } = new List<CruiseJoinRequest>(); // One user can have many join requests

        public List<Reservation> Reservation { get; set; } = new List<Reservation>(); // One user can have many reservations
        public List<Reports> Reports { get; set; } = new List<Reports>(); // One user can have many reports
        
        public List<favoriteCruises> favoriteCruises { get; set; } = new List<favoriteCruises>(); // One user can have many favorite cruises
        public List<favoriteYachtsForSale> favoriteYachtsForSale { get; set; } = new List<favoriteYachtsForSale>(); // One user can have many favorite yachts

        public List<Notifications> Notifications { get; set; } = new List<Notifications>(); // One user can have many notifications
        public List<YachtSale> YachtSale { get; set; } = new List<YachtSale>(); // One user can sell many yachts

    }
}
