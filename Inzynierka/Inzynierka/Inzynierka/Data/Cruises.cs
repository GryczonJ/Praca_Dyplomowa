namespace Inzynierka.Data
{
    public class Cruises
    {
        public int Id { get; set; }
        public string destination { get; set; }
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }
        public int price { get; set; }
        public char currency { get; set; }//string
        public string status { get; set; }
        public int max_participants { get; set; }

        // Foreign key to Yacht
        public int YachtId { get; set; }
        public Yachts Yacht { get; set; }


        // Many-to-many relationship with Users
        public List<Users> Users { get; set; }// = new List<Users>(); // One cruise can have many users

        // One-to-many relationship with Capitan
        public int CapitanId { get; set; }
        public Users Capitan { get; set; }

        // Referncja
        public List<CruiseJoinRequest> CruiseJoinRequests { get; set; }
    }
}
