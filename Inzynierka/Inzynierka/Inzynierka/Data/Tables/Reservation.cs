using System.ComponentModel.DataAnnotations;

namespace Inzynierka.Data.Tables
{
    public class Reservation
    {
        [Display(Name = "Identyfikator rezerwacji")]
        public int Id { get; set; }
        [Display(Name = "Data rozpoczęcia")]
        public DateTime startDate { get; set; }
        [Display(Name = "Data zakończenia")]
        public DateTime endDate { get; set; }
        [Display(Name = "Status rezerwacji")]
        public string status { get; set; }

        // foreing key
        [Display(Name = "Czarter")]
        public Charters Charter { get; set; }
        [Display(Name = "Identyfikator czarteru")]
        public int CharterId { get; set; }

        [Display(Name = "Użytkownik")]
        public Users User { get; set; }
        [Display(Name = "Identyfikator użytkownika")]
        public Guid? UserId { get; set; }
    }
}
