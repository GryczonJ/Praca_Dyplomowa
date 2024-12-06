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
        public string currency { get; set; } // Waluta
        public string location { get; set; }
        //public string availabilityStatus { get; set; } // status dostępności
        public string status { get; set; } = "Pending"; // Dodano status transakcji (Pending, Accepted, Rejected) transactionStatus
        public string notes { get; set; } // Dodatkowe uwagi do transakcji

        // foreing key
        public int YachtId { get; set; }
        public Yachts Yacht { get; set; }

        public Guid? BuyerUserId { get; set; }// kupujący
        public Users? BuyerUser { get; set; }

        public Guid OwnerId { get; set; }
        public Users Owner { get; set; }

        // reference
        public List<Reports> Reports { get; set; } = new List<Reports>(); // relation with Reports table, one to many
        public List<Image> Images { get; set; } = new List<Image>(); // relation with Image table, one to many 
        public List<Comments> Comments { get; set; } = new List<Comments>(); // relation with Comments table, one to many
        public List<FavoriteYachtsForSale> FavoriteYachtsForSale { get; set; } = new List<FavoriteYachtsForSale>(); // relation with FavoriteYachtsForSale table, one to many
    }
}
