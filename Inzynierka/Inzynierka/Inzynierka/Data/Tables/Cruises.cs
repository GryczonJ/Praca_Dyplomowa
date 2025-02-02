using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inzynierka.Data.Tables
{
    /**
     * Klasa reprezentująca rejsy 
     */
    public class Cruises
    {
        public int Id { get; set; }

        [Display(Name = "Nazwa rejsu")]
        public string name { get; set; }

        [Display(Name = "Opis")]
        public string description { get; set; }

        [Display(Name = "Cel rejsu")]
        public string destination { get; set; }// cel rejsu

        [Display(Name = "Data rozpoczęcia")]
        public DateOnly start_date { get; set; }

        [Display(Name = "Data zakończenia")]
        public DateOnly end_date { get; set; }

        [Display(Name = "Cena za osobę")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal price { get; set; }// price_per_person

        [Display(Name = "Waluta")]
        public string currency { get; set; }// waluta

        [Display(Name = "Status")]
        public CruiseStatus? status { get; set; }

        [Display(Name = "Maksymalna liczba uczestników")]
        public int maxParticipants { get; set; }

/*        [Display(Name = "Zbanowany")]
        public bool banned { get; set; } = false;*/



        //Foreign key

        // Foreign key to Yacht
        [Display(Name = "Jacht")]
        public Yachts? Yacht { get; set; }

        [Display(Name = "Identyfikator jachtu")]
        public int? YachtId { get; set; }

        // One-to-many relationship with Capitan
        [Display(Name = "Kapitan")]
        public Users Capitan { get; set; }
        [Display(Name = "Identyfikator kapitana")]
        public Guid CapitanId { get; set; }

        // Referncja
        // Many-to-many relationship with Users
        [Display(Name = "Uczestnicy rejsu")]
        public List<CruisesParticipants> CruisesParticipants { get; set; } = new List<CruisesParticipants>(); // One cruise can have many users
        [Display(Name = "Zapytania o dołączenie")]
        public List<CruiseJoinRequest> CruiseJoinRequests { get; set; } = new List<CruiseJoinRequest>(); // One cruise can have many join requests
        [Display(Name = "Raporty")]
        public List<Reports> Reports { get; set; } = new List<Reports>();// One cruise can have many reports
        [Display(Name = "Komentarze")]
        public List<Comments> Comments { get; set; } = new List<Comments>();// One cruise can have many comments
        [Display(Name = "Ulubione rejsy")]
        public List<FavoriteCruises> FavoriteCruises { get; set; } = new List<FavoriteCruises>();// One cruise can have many favorite cruises
    }
    public enum CruiseStatus
    {
        [Display(Name = "Planowany")]
        Planned,

        [Display(Name = "W trakcie")]
        Ongoing,

        [Display(Name = "Zakończony")]
        Completed,

        [Display(Name = "Anulowany")]
        Cancelled
    }
}
