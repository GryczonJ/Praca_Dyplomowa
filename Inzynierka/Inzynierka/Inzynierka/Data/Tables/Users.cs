namespace Inzynierka.Data.Tables
{
    public class Users
    {
        public int Id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public int? age { get; set; } = 0;
        public string name { get; set; }
        public string surname { get; set; }
        public string email { get; set; }
        public string phone_number { get; set; }
        public string about_me { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }

        // Foreign key to Role
        public int RoleId { get; set; }
        public Roles Role { get; set; }


        // Many-to-many relationship with Cruises
        public List<CruisesParticipants> CruisesParticipants { get; set; } = new List<CruisesParticipants>();// = new List<Cruises>(); // One user can participate in many cruises
        public List<Cruises> Cruises { get; set; } = new List<Cruises>(); // One user can participate in many cruises Capitan ID

        public List<Charters> Charters { get; set; } = new List<Charters>(); // One user can participate in many charters

        public List<YachtSale> YachtSale { get; set; } = new List<YachtSale>(); // One user can sell many yachts

        public List<Reservation> reservation { get; set; } = new List<Reservation>(); // One user can have many reservations
    }
}
