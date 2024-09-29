namespace Inzynierka.Data
{
    public class Charters
    {
        public int Id { get; set; }
       //start_date
       public DateTime start_date { get; set; }
        //end_date
        public DateTime end_date { get; set; }
        //price
        public int price { get; set; }
        //currency
        public char currency { get; set; }//string
        //status
        public string status { get; set; }

        //foreing key
        public int YachtId { get; set; }
        public Yachts Yacht { get; set; }

        public int UserId { get; set; }
        public Users User { get; set; }

        public int OwnerId { get; set; }
        public Users Owner { get; set; }
    }
}
