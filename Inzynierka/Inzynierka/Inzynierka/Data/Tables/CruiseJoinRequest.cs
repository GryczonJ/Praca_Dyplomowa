namespace Inzynierka.Data.Tables
{
    public class CruiseJoinRequest
    {
        public int Id { get; set; }
        public string status { get; set; }// RequestStatus
        public DateTime date { get; set; }// RequestDate

        // foreing key
        public int CruiseId { get; set; }
        public Cruises Cruise { get; set; }

        public int UserId { get; set; }
        public Users User { get; set; }

        public int CapitanId { get; set; }
        public Users Capitan { get; set; }
    }
}
