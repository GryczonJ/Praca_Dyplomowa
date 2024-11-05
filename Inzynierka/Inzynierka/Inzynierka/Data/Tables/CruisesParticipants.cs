using System.Security.Cryptography.X509Certificates;

namespace Inzynierka.Data.Tables
{
    public class CruisesParticipants
    {
        public Users Users { get; set; }
        public int UsersId { get; set; }
        public Cruises Cruises { get; set; }
        public int CruisesId { get; set; }
    }
}
