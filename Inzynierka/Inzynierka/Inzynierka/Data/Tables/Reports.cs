using System.ComponentModel.DataAnnotations;

namespace Inzynierka.Data.Tables
{
    public class Reports
    {
        [Display(Name = "Identyfikator")]
        public int Id { get; set; }

        [Display(Name = "Status zgłoszenia")]
        public RequestStatus status { get; set; } = RequestStatus.Submitted;  // RequestStatus

        [Display(Name = "Data zgłoszenia")]
        public DateTime date { get; set; }// RequestDate DateOfCreation

        [Display(Name = "Wiadomość")]
        public string message { get; set; }// RequestMessage

        [Display(Name = "Notatki")]
        public string? note { get; set; }// RequestNote


        // foreing key
        [Display(Name = "Moderator")]
        public Users? Moderator { get; set; }
        [Display(Name = "Identyfikator moderatora")]
        public Guid? ModeratorId { get; set; }

        [Display(Name = "Twórca")]
        public Users? Creator { get; set; }
        [Display(Name = "Identyfikator twórcy")]
        public Guid? CreatorId { get; set; }

        [Display(Name = "Zgłoszony użytkownik")]
        public Users? SuspectUser { get; set; }
        [Display(Name = "Identyfikator zgłoszonego użytkownika")]
        public Guid? SuspectUserId { get; set; }


        [Display(Name = "Zgłoszony rejs")]
        public Cruises? SuspectCruise { get; set; }
        [Display(Name = "Identyfikator zgłoszonego rejsu")]
        public int? SuspectCruiseId { get; set; }


        [Display(Name = "Zgłoszony jacht")]
        public Yachts? SuspectYacht { get; set; }
        [Display(Name = "Identyfikator zgłoszonego jachtu")]
        public int? SuspectYachtId  { get; set; }


        [Display(Name = "Weryfikacja dokumentu")]
        public Roles? DocumentVerification { get; set; }
        [Display(Name = "Identyfikator weryfikacji dokumentu")]
        public Guid? DocumentVerificationId { get; set; }


        [Display(Name = "Zgłoszona sprzedaż jachtu")]
        public YachtSale? SuspectYachtSale { get; set; }
        [Display(Name = "Identyfikator zgłoszonej sprzedaży jachtu")]
        public int? SuspectYachtSaleId { get; set; }


        [Display(Name = "Zgłoszony czarter")]
        public Charters? SuspectCharter { get; set; }
        [Display(Name = "Identyfikator zgłoszonego czarteru")]
        public int? SuspectCharterId { get; set; }


        [Display(Name = "Zgłoszony komentarz")]
        public Comments? SuspectComment { get; set; }
        [Display(Name = "Identyfikator zgłoszonego komentarza")]
        public int? SuspectCommentId { get; set; }


        [Display(Name = "Zgłoszona rola")]
        public Roles? SuspectRole { get; set; }
        [Display(Name = "Identyfikator zgłoszonej roli")]
        public Guid? SuspectRoleId { get; set; }
        // reference

        public enum RequestStatus
        {
            [Display(Name = "Brak statusu")]
            None,

            [Display(Name = "Zgłoszono")]
            Submitted,

            [Display(Name = "W toku")]
            InProgress,

            [Display(Name = "Zakończono")]
            Completed,

            [Display(Name = "Odrzucono")]
            Rejected,

            [Display(Name = "Ponowne Rozpatrzenie")]
            Reconsidered
        }
    }
}
