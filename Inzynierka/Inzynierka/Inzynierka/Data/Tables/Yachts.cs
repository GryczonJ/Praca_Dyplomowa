namespace Inzynierka.Data.Tables
{
    public class Yachts
    {
        public int Id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string type { get; set; }
        public string manufacturer { get; set; }
        public string model { get; set; }
        public DateTime year { get; set; }
        public double length { get; set; }
        public double width { get; set; }
        public int crew { get; set; }// Załoga
        public int cabins { get; set; }
        public int beds { get; set; }
        public int toilets { get; set; }
        public int showers { get; set; }
        public string location { get; set; }
        public int capacity { get; set; }// wyporność

        // foreing key
        public Users Owner { get; set; }
        public Guid OwnerId { get; set; }
        
        public Image Image { get; set; }
        public int ImageId { get; set; }

        // reference
        public List<Cruises> Cruises { get; set; } = new List<Cruises>(); // relation with Cruises table, one to many
        public List<Charters> Charters { get; set; } = new List<Charters>(); // relation with Charters table, one to many
        public List<YachtSale> YachtSale { get; set; } = new List<YachtSale>();// relation with YachtSale table, one to many
        public List<Reports> Reports { get; set; }= new List<Reports>();// relation with Reports table, one to many
        public List<Comments> Comments { get; set; } = new List<Comments>();// relation with Comments table, one to many
    }
}
