using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Transactions;

namespace Inzynierka.Data.Tables
{
    public class YachtSale
    {
        public int Id { get; set; }

        [Display(Name = "Data sprzedaży")]
        public DateTime saleDate { get; set; } // Zmieniono nazwę na bardziej opisową

        [Column(TypeName = "decimal(18, 2)")]
        [Display(Name = "Cena sprzedaży")]
        public decimal price { get; set; } // Cena sprzedaży

        [Display(Name = "Waluta")]
        public string currency { get; set; } // Waluta

        [Display(Name = "Lokalizacja")]
        public string location { get; set; }

        [Display(Name = "Status transakcji")]
        public TransactionStatus status { get; set; } = TransactionStatus.Pending; // Status transakcji jako enum

        [Display(Name = "Uwagi")]
        public string notes { get; set; } // Uwagi do transakcji

        [Display(Name = "Zbanowany")]
        public bool banned { get; set; } = false;



        // Klucz obcy
        [Display(Name = "Identyfikator jachtu")]
        public int YachtId { get; set; }

        [Display(Name = "Jacht")]
        public Yachts Yacht { get; set; }



        [Display(Name = "Identyfikator kupującego")]
        public Guid? BuyerUserId { get; set; } // Kupujący

        [Display(Name = "Kupujący")]
        public Users? BuyerUser { get; set; }



        [Display(Name = "Identyfikator właściciela")]
        public Guid OwnerId { get; set; }

        [Display(Name = "Właściciel")]
        public Users Owner { get; set; }



        // Relacje
        [Display(Name = "Raporty")]
        public List<Reports> Reports { get; set; } = new List<Reports>();


        [Display(Name = "Obrazy")]
        public List<Image> Images { get; set; } = new List<Image>();


        [Display(Name = "Komentarze")]
        public List<Comments> Comments { get; set; } = new List<Comments>();


        [Display(Name = "Ulubione oferty sprzedaży")]
        public List<FavoriteYachtsForSale> FavoriteYachtsForSale { get; set; } = new List<FavoriteYachtsForSale>();
    }
    public enum TransactionStatus
    {
        Pending,   // Oczekujący
        Accepted,  // Zaakceptowany
        Rejected   // Odrzucony
    }
}
