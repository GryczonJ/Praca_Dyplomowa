using System.ComponentModel.DataAnnotations;

namespace Inzynierka.Data.Tables
{
    public class Comments
    {
        public int Id { get; set; }
        [Display(Name = "Wiadomość")]
        public string Message { get; set; }
        [Display(Name = "Data utworzenia")]
        public DateTime CreateDate { get; set; }
        [Display(Name = "Ocena")]
        public int Rating { get; set; }

        // foreing key
        [Display(Name = "Twórca")]
        public Users Creator { get; set; }
        [Display(Name = "Identyfikator twórcy")]
        public Guid CreatorId { get; set; }

        [Display(Name = "Profil")]
        public Users? Profile { get; set; }
        [Display(Name = "Identyfikator profilu")]
        public Guid? ProfileId { get; set; }

        [Display(Name = "Czarter")]
        public Charters? Charter { get; set; }
        [Display(Name = "Identyfikator czarteru")]
        public int? CharterId { get; set; }

        [Display(Name = "Rejs")]
        public Cruises? Cruises { get; set; }
        [Display(Name = "Identyfikator rejsu")]
        public int? CruisesId { get; set; }

        [Display(Name = "Jacht")]
        public Yachts? Yachts { get; set; }
        [Display(Name = "Identyfikator jachtu")]
        public int? YachtsId { get; set; }

        // reference

        // foreing key
        [Display(Name = "Komentarze")]
        public List<Reports> Reports { get; set; } = new List<Reports>();

    }
}
