namespace Inzynierka.Data.Tables
{
    public class Notifications
    {
        public int Id { get; set; }
        public string status { get; set; }// RequestStatus
        public DateTime CreateDate { get; set; }// RequestDate
        public string message { get; set; }// RequestMessage
        public DataTime ReadDate { get; set; }// RequestDate

        // foreing key
        public Users User { get; set; }// RequestUser
        public int UserId { get; set; }
    }
}
