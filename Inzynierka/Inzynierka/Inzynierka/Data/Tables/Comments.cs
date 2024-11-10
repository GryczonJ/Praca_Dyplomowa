namespace Inzynierka.Data.Tables
{
    public class Comments
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public DateTime CreateDate { get; set; }
        public int Rating { get; set; }

        // foreing key
        public Users Creator { get; set; }
        public Guid CreatorId { get; set; }

        public Users Profile { get; set; }
        public Guid ProfileId { get; set; }

        public Charters Charter { get; set; }
        public int CharterId { get; set; }

        public Cruises Cruises { get; set; }
        public int CruisesId { get; set; }

        public Yachts Yachts { get; set; }
        public int YachtsId { get; set; }
        
        // reference

        // foreing key
        public List<Reports> Reports { get; set; } = new List<Reports>();

    }
}
