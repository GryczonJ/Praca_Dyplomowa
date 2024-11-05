namespace Inzynierka.Data.Tables
{
    public class Reports
    {
        public int Id { get; set; }
        public string status { get; set; }// RequestStatus
        public DateTime date { get; set; }// RequestDate DateOfCreation
        public string message { get; set; }// RequestMessage
        public string note { get; set; }// RequestNote

        // foreing key
        public Users Moderator { get; set; }
        public int ModeratorId { get; set; }

        public Cruises SuspectCruise { get; set; }
        public int SuspectCruiseId { get; set; }

        public Users SuspectUser { get; set; }
        public int SuspectUserId { get; set; }

        public Yachts SuspectYacht { get; set; }
        public int SuspectYachtId  { get; set; }

        public Roles DocumentVerification { get; set; }
        public int DocumentVerificationId { get; set; }

        public YachtSale SuspectYachtSale { get; set; }
        public int SuspectYachtSaleId { get; set; }

        public Charters SuspectCharter { get; set; }
        public int SuspectCharterId { get; set; }
        public Comments SuspectComment { get; set; }
        public int SuspectCommentId { get; set; }

        public Roles SuspectRole { get; set; }
        public int SuspectRoleId { get; set; }

        // reference
        
    }
}
