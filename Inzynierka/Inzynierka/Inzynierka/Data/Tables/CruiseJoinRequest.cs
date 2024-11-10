namespace Inzynierka.Data.Tables
{
    public class CruiseJoinRequest
    {
        public int Id { get; set; }
        public string status { get; set; }// RequestStatus
        public DateTime date { get; set; }// RequestDate

        // foreing key
        public Cruises Cruise { get; set; }
        public int CruiseId { get; set; }

        public Users User { get; set; }
        public Guid UserId { get; set; }

        public Users Capitan { get; set; }
        public Guid CapitanId { get; set; }
    }
}
