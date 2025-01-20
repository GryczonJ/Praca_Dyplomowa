using System.ComponentModel.DataAnnotations;

namespace Inzynierka.Data.Tables
{
    public class CruiseJoinRequest
    {
        public int Id { get; set; }

        [Display(Name = "Status")]
        public RequestStatus status { get; set; }// RequestStatus
        [Display(Name = "Data zgłoszenia")]
        public DateTime date { get; set; }// RequestDate

        /*[Display(Name = "Zbanowany")]
        public bool banned { get; set; } = false;*/



        // foreing key
        [Display(Name = "Rejs")]
        public Cruises Cruise { get; set; }
        [Display(Name = "Identyfikator rejsu")]
        public int CruiseId { get; set; }

        [Display(Name = "Użytkownik")]
        public Users User { get; set; }
        [Display(Name = "Identyfikator użytkownika")]
        public Guid UserId { get; set; }

        [Display(Name = "Kapitan")]
        public Users Capitan { get; set; }
        [Display(Name = "Identyfikator kapitana")]
        public Guid CapitanId { get; set; }
    }
    // Enum for request status
    public enum RequestStatus
    {
        [Display(Name = "Oczekujące")]
        Pending,

        [Display(Name = "Zaakceptowane")]
        Accepted,

        [Display(Name = "Odrzucone")]
        Rejected
    }
}
