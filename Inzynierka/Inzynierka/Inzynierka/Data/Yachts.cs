namespace Inzynierka.Data
{
    public class Yachts
    {
        public int Id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string type { get; set; }
        public string manufacturer { get; set; }
        public string model { get; set; }
        public int year { get; set; }
        public int length { get; set; }
        public int width { get; set; }
        public int crew { get; set; }
        public int cabins { get; set; }
        public int beds { get; set; }
        public int toilets { get; set; }
        public int showers { get; set; }
        public int price { get; set; }
        public char currency { get; set; }//string
        public string location { get; set; }
        public string availabilityStatus  { get; set; }//status
        public List<byte[]> image { get; set; }

        // foreing key
        public int OwnerId { get; set; }
        public Users Owner { get; set; }

        public List<Cruises> Cruises { get; set; }
        public List<Charters> Charters { get; set; }
        public List<YachtSale> YachtSale { get; set; }
    }
}
