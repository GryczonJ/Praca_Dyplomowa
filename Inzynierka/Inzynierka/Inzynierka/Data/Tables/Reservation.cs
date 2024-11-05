namespace Inzynierka.Data.Tables
{
    public class Reservation
    {
        public int Id { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public string status { get; set; }

        // foreing key
        public Charters Charter { get; set; }
        public int CharterId { get; set; }
        public Users User { get; set; }
        public int UserId { get; set; }
    }
}
