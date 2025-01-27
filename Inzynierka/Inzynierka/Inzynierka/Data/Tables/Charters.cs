using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inzynierka.Data.Tables
{
    public class Charters
    {
        public int Id { get; set; }
        //start_date
        [Display(Name = "Data rozpoczęcia")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateOnly startDate { get; set; }
        //end_date
        [Display(Name = "Data zakończenia")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateOnly endDate { get; set; }
  
        [Display(Name = "Cena")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal price { get; set; }
        
        [Display(Name = "Lokalizacja")]
        public string location { get; set; }
        [Display(Name = "Opis")]
        public string description { get; set; }

        [Display(Name = "Waluta")]
        [StringLength(3, ErrorMessage = "Kod waluty powinien mieć maksymalnie 3 znaki.")]
        public string currency { get; set; }
        
        [Display(Name = "Status")]
        public CharterStatus status { get; set; }

        [Display(Name = "Zbanowany")]
        public bool banned { get; set; } = false;




        //foreing key
        [Display(Name = "Jacht")]
        public Yachts? Yacht { get; set; }
        [ForeignKey(nameof(Yacht))]
        [Display(Name = "Identyfikator jachtu")]
        public int? YachtId { get; set; }

        [Display(Name = "Właściciel")]
        public Users Owner { get; set; }
        [Display(Name = "Identyfikator właściciela")]
        public Guid OwnerId { get; set; }

        //reference
        [Display(Name = "Rezerwacje")]
        public List<Reservation> Reservations { get; set; } = new List<Reservation>(); // relation with Reservation table, one to many
        [Display(Name = "Komentarze")]
        public List<Comments> Comments { get; set; } = new List<Comments>(); // relation with Comments table, one to many
        [Display(Name = "Raporty")]
        public List<Reports> Reports { get; set; } = new List<Reports>(); // relation with Reports table, one to many

    }
    public enum CharterStatus
    {
        [Display(Name = "Planowane")]
        Planowane,

        [Display(Name = "W trakcie")]
        WTrakcie,

        [Display(Name = "Zakończone")]
        Zakonczone
    }
}
