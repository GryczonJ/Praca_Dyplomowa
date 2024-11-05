using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace Inzynierka.Data.Tables
{
    public class YachtSale
    {
        public int Id { get; set; }
        public DateTime saleDate { get; set; } // Zmieniono nazwę na bardziej opisową

        [Column(TypeName = "decimal(18, 2)")]
        public decimal price { get; set; }// SalePrice
        public string currency { get; set; } // waluta
        public string location { get; set; }
        public string availabilityStatus { get; set; } // status dostępności
        public string status { get; set; } = "Pending"; // Dodano status transakcji (Pending, Accepted, Rejected) transactionStatus
        public string notes { get; set; } // Dodatkowe uwagi do transakcji

        // foreing key
        public int YachtId { get; set; }
        public Yachts Yacht { get; set; }

        public int BuyerUserId { get; set; }// kupujący
        public Users BuyerUser { get; set; }

        public int OwnerId { get; set; }
        public Users Owner { get; set; }
    }
}
