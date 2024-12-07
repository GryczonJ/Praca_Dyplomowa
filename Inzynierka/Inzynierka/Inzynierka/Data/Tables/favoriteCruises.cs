using System.ComponentModel.DataAnnotations;

namespace Inzynierka.Data.Tables
{
    public class FavoriteCruises
    {
        [Display(Name = "Użytkownik")]
        public Users User { get; set; }
        [Display(Name = "Identyfikator użytkownika")]
        public Guid UserId { get; set; }
        [Display(Name = "Rejs")]
        public Cruises Cruise { get; set; }
        [Display(Name = "Identyfikator rejsu")]
        public int CruiseId { get; set; }
    }
}
