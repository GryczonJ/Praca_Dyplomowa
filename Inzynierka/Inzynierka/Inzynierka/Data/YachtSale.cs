using System.Data;

namespace Inzynierka.Data
{
    public class YachtSale
    {
        public int Id { get; set; }
        public DateTime saleDate { get; set; } // Zmieniono nazwę na bardziej opisową
        public int price { get; set; }
        public string currency { get; set; } // Zmieniono na string
        public string location { get; set; }
        public string availabilityStatus { get; set; } // status dostępności
        public string transactionStatus { get; set; } = "Pending"; // Dodano status transakcji
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
