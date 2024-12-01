using Inzynierka.Data.Tables;

namespace Inzynierka.Models
{
    public class YachtsIndexViewModel
    {
        public IEnumerable<Yachts> UserYachts { get; set; }
        public IEnumerable<Yachts> OtherYachts { get; set; }
    }
}
