using System.ComponentModel.DataAnnotations;

namespace Inzynierka.Data.Tables
{
    public class Notifications
    {
        [Display(Name = "Identyfikator")]
        public int Id { get; set; }
        [Display(Name = "Status powiadomienia")]
        public string status { get; set; }// RequestStatus
        [Display(Name = "Data utworzenia")]
        public DateTime CreateDate { get; set; }// RequestDate
        [Display(Name = "Wiadomość")]
        public string message { get; set; }// RequestMessage
        [Display(Name = "Data przeczytania")]
        public DateTime? ReadDate { get; set; }// RequestDate

        // foreing key
        [Display(Name = "Użytkownik")]
        public Users User { get; set; }// RequestUser
        [Display(Name = "Identyfikator użytkownika")]
        public Guid UserId { get; set; }
    }
}
