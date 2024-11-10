using System.ComponentModel.DataAnnotations.Schema;

namespace Inzynierka.Data.Tables
{
    public class Charters
    {
        public int Id { get; set; }
        //start_date
        public DateTime startDate { get; set; }
        //end_date
        public DateTime endDate { get; set; }
        //price
        [Column(TypeName = "decimal(18, 2)")]
        public decimal price { get; set; }

        public string location { get; set; }
        public string description { get; set; }

        //currency
        public string currency { get; set; }
        //status
        public string status { get; set; }

        //foreing key
        public Yachts Yacht { get; set; }
        public int YachtId { get; set; }
        

        public Users Owner { get; set; }
        public Guid OwnerId { get; set; }

        //reference
        public List<Reservation> Reservations { get; set; } = new List<Reservation>(); // relation with Reservation table, one to many
        public List<Comments> Comments { get; set; } = new List<Comments>(); // relation with Comments table, one to many
        public List<Reports> Reports { get; set; } = new List<Reports>(); // relation with Reports table, one to many

    }
}
