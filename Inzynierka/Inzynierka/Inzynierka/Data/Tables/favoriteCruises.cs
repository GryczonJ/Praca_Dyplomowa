namespace Inzynierka.Data.Tables
{
    public class FavoriteCruises
    {
        public Users User { get; set; }
        public Guid UserId { get; set; }
        public Cruises Cruise { get; set; }
        public int CruiseId { get; set; }
    }
}
