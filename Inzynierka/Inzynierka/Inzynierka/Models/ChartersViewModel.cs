namespace Inzynierka.Models
{
    public class ChartersViewModel
    {
        public IEnumerable<Inzynierka.Data.Tables.Charters> MyCharters { get; set; }
        public IEnumerable<Inzynierka.Data.Tables.Charters> OtherCharters { get; set; }
    }
}
