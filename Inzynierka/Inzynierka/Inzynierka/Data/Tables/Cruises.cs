using System.ComponentModel.DataAnnotations.Schema;

namespace Inzynierka.Data.Tables
{
    /**
     * Klasa reprezentująca rejsy 
     */
    public class Cruises
    {
        public int Id { get; set; }
        public string destination { get; set; }
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal price { get; set; }// price_per_person
        public string currency { get; set; }// waluta
        public string status { get; set; }
        public int max_participants { get; set; }

        // Foreign key to Yacht
        public int YachtId { get; set; }
        public Yachts Yacht { get; set; }


        // Many-to-many relationship with Users
        public List<CruisesParticipants> CruisesParticipants { get; set; } = new List<CruisesParticipants>(); // One cruise can have many users

        // One-to-many relationship with Capitan
        public int CapitanId { get; set; }
        public Users Capitan { get; set; }

        // Referncja
        public List<CruiseJoinRequest> CruiseJoinRequests { get; set; }
    }
}
