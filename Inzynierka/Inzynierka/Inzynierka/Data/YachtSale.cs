using System.Data;

namespace Inzynierka.Data
{
    public class YachtSale
    {
        public int Id { get; set; }
        public DateTime date { get; set; }
        public int price { get; set; }
        public char currency { get; set; }//string
        public string location { get; set; }
        public string availabilityStatus { get; set; }//status

        // foreing key
        public int YachtId { get; set; }
        public Yachts Yacht { get; set; }

        public int BuyerUserId { get; set; }// kupujący
        public Users BuyerUser { get; set; }

        public int OwnerId { get; set; }
        public Users Owner { get; set; }
    }
}
