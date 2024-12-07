using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.X509Certificates;

namespace Inzynierka.Data.Tables
{
    public class CruisesParticipants
    {
        [Display(Name = "Użytkownik")]
        public Users Users { get; set; }
        [Display(Name = "Identyfikator użytkownika")]
        public Guid UsersId { get; set; }
        [Display(Name = "Rejs")]
        public Cruises Cruises { get; set; }
        [Display(Name = "Identyfikator rejsu")]
        public int CruisesId { get; set; }
    }
}
