namespace Inzynierka.Data
{
    public class Users
    {
        public int Id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string email { get; set; }
        public string phone_number { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }


        public byte[] ProfilePicture { get; set; } // Pole do przechowywania zdjęcia

        // Foreign key to Role
        public int RoleId { get; set; }
        public Roles Role { get; set; }

        // Many-to-many relationship with Cruises
        public List<Cruises> Cruises { get; set; }// = new List<Cruises>(); // One user can participate in many cruises
        public List<Charters> Charters { get; set; }

        public List<YachtSale> YachtSale { get; set; }
    }
}
