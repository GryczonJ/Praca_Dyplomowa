using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inzynierka.Data.Tables
{
    public class Reservation
    {
        [Display(Name = "Identyfikator rezerwacji")]
        public int Id { get; set; }
        [Display(Name = "Data rozpoczęcia")]
        public DateOnly startDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Data zakończenia")]
        public DateOnly endDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]

        [Display(Name = "Status rezerwacji")]
        public StatusReservation Status { get; set; } = StatusReservation.Pending;


        // foreing key
        [Display(Name = "Czarter")]
        public Charters? Charter { get; set; }
        [Display(Name = "Identyfikator czarteru")]
        public int CharterId { get; set; }

        [Display(Name = "Użytkownik")]
        public Users? User { get; set; }
        [Display(Name = "Identyfikator użytkownika")]
        public Guid? UserId { get; set; }
    }
    public enum StatusReservation
        {
        [Display(Name = "Oczekujący")]
        Pending = 0,
        [Display(Name = "Zaakceptowany")]
        Accepted = 1 ,
        [Display(Name = "Odrzucony")]
        Rejected= 2
    }
}
