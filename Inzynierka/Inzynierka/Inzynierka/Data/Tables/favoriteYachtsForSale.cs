using System.ComponentModel.DataAnnotations;

namespace Inzynierka.Data.Tables
{
    public class FavoriteYachtsForSale
    {
        [Display(Name = "Użytkownik")]
        public Users User { get; set; }
        [Display(Name = "Identyfikator użytkownika")]
        public Guid UserId { get; set; }
        [Display(Name = "Jacht na sprzedaż")]
        public YachtSale YachtForSale { get; set; }
        [Display(Name = "Identyfikator jachtu na sprzedaż")]
        public int YachtSaleId { get; set; }
    }
}
