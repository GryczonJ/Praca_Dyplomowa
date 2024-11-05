namespace Inzynierka.Data.Tables
{
    public class favoriteYachtsForSale
    {
        public Users User { get; set; }
        public int UserId { get; set; }
        public YachtSale YachtForSale { get; set; }
        public int YachtSaleId { get; set; }
    }
}
