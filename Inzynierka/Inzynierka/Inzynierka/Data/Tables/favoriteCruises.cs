namespace Inzynierka.Data.Tables
{
    public class favoriteCruises
    {
        public Users User { get; set; }
        public int UserId { get; set; }
        public Cruises Cruise { get; set; }
        public int CruiseId { get; set; }
    }
}
