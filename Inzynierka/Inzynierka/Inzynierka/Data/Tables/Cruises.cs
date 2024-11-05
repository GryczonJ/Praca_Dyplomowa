using System.ComponentModel.DataAnnotations.Schema;

namespace Inzynierka.Data.Tables
{
    /**
     * Klasa reprezentująca rejsy 
     */
    public class Cruises
    {
        public int Id { get; set; }
        public string name { get; set; }
        public string destination { get; set; }
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal price { get; set; }// price_per_person
        public string currency { get; set; }// waluta
        public string status { get; set; }
        public int maxParticipants { get; set; }

        //Foreign key

        // Foreign key to Yacht
        public Yachts Yacht { get; set; }
        public int YachtId { get; set; }

        // One-to-many relationship with Capitan
        public Users Capitan { get; set; }
        public int CapitanId { get; set; }
       
        // Referncja
        // Many-to-many relationship with Users
        public List<CruisesParticipants> CruisesParticipants { get; set; } = new List<CruisesParticipants>(); // One cruise can have many users
        public List<CruiseJoinRequest> CruiseJoinRequests { get; set; } = new List<CruiseJoinRequest>(); // One cruise can have many join requests
        public List<Reports> Reports { get; set; } = new List<Reports>();// One cruise can have many reports
        public List<Comments> Comments { get; set; } = new List<Comments>();// One cruise can have many comments
        public List<favoriteCruises> favoriteCruises { get; set; } = new List<favoriteCruises>();// One cruise can have many favorite cruises
    }
}
