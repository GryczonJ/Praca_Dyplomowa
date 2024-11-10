namespace Inzynierka.Data.Tables
{
    public class FavoriteYachtsForSale
    {
        public Users User { get; set; }
        public Guid UserId { get; set; }
        public YachtSale YachtForSale { get; set; }
        public int YachtSaleId { get; set; }
    }
}
