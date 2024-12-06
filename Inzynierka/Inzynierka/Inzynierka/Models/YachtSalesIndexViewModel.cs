using Inzynierka.Data.Tables;

namespace Inzynierka.Models
{
    public class YachtSalesIndexViewModel
    {
        public List<YachtSale> UserSales { get; set; }
        public List<YachtSale> OtherSales { get; set; }
    }
}
