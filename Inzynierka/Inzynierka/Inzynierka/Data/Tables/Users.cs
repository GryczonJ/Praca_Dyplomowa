using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Inzynierka.Data.Tables
{
    public class Users : IdentityUser<Guid>
    {
        
        [Display(Name = "Wiek")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? age { get; set; }
        [Display(Name = "Imię")]
        public string firstName { get; set; } = "Jan";
        [Display(Name = "Nazwisko")]
        public string? lastName { get; set;}
        [Display(Name = "Nazwisko")]
        public string surName { get; set; } = "Kowalski";
        [Display(Name = "Opis o mnie")]
        public string aboutMe { get; set; } = "Nazywam sie Jan Kowalski";
        
        [Display(Name = "Zbanowany")]
        public bool banned { get; set; } = false;

        [Display(Name = "Powód Bana")]
        public string reasonBan { get; set; } = "";

        [Display(Name = "Profil publiczny")]
        public bool Public { get; set; } = true;

        // Foreign key
        //public Roles Role { get; set; }
        //public Guid? RoleId { get; set; }
        [Display(Name = "Zdjęcie")]
        public Image? Photos { get; set; }
        [Display(Name = "Identyfikator zdjęcia")]
        public int? PhotosId { get; set; }

        //reference
        // Many-to-many relationship with Cruises
        [Display(Name = "Udział w rejsach")]
        public List<CruisesParticipants> CruisesParticipants { get; set; } = new List<CruisesParticipants>();// One user can participate in many cruises
        [Display(Name = "Rejsy użytkownika")]
        public List<Cruises> Cruises { get; set; } = new List<Cruises>(); // One user can participate in many cruises Capitan ID
        [Display(Name = "Komentarze utworzone przez użytkownika")]
        public List<Comments> CommentsAsCreator { get; set; } = new List<Comments>(); // One user can have many comments>
        [Display(Name = "Komentarze użytkownika")]
        public List<Comments> CommentsAsProfile { get; set; } = new List<Comments>(); // One user can have many comments>
        [Display(Name = "Czartery użytkownika")]
        public List<Charters> Charters { get; set; } = new List<Charters>(); // One user can participate in many charters
        [Display(Name = "Wnioski o dołączenie do rejsu")]
        public List<CruiseJoinRequest> CruiseJoinRequests { get; set; } = new List<CruiseJoinRequest>(); // One user can have many join requests
        [Display(Name = "Wnioski o dołączenie do rejsu jako kapitan")]
        public List<CruiseJoinRequest> CruiseJoinRequestsAsCapitan { get; set; } = new List<CruiseJoinRequest>(); // One user can have many join requests

        [Display(Name = "Rezerwacje użytkownika")]
        public List<Reservation> Reservation { get; set; } = new List<Reservation>(); // One user can have many reservations
        [Display(Name = "Raporty moderacyjne")]
        public List<Reports> ModeratorReports { get; set; } = new List<Reports>(); // One user can have many reports
        [Display(Name = "Raporty o podejrzanym użytkowniku")]
        public List<Reports> SuspectUserReports { get; set; } = new List<Reports>(); // One user can have many reports

        [Display(Name = "Moje zgłoszenia")]
        public List<Reports> CreatorReports { get; set; } = new List<Reports>(); // One user can have many reports

        [Display(Name = "Ulubione rejsy")]
        public List<FavoriteCruises> FavoriteCruises { get; set; } = new List<FavoriteCruises>(); // One user can have many favorite cruises
        [Display(Name = "Ulubione jachty na oferty")]
        public List<FavoriteYachtsForSale> FavoriteYachtsForSale { get; set; } = new List<FavoriteYachtsForSale>(); // One user can have many favorite yachts

        [Display(Name = "Powiadomienia użytkownika")]
        public List<Notifications> Notifications { get; set; } = new List<Notifications>(); // One user can have many notifications
        [Display(Name = "Sprzedaż jachtów (jako sprzedawca)")]
        public List<YachtSale> YachtSalesAsSeller { get; set; } = new List<YachtSale>(); // One user can sell many yachts
        [Display(Name = "Sprzedaż jachtów (jako kupujący)")]
        public List<YachtSale> YachtSalesAsBuyer { get; set; } = new List<YachtSale>(); // One user can sell many yachts
        [Display(Name = "Jachty użytkownika")]
        public List<Yachts> Yachts { get; set; } = new List<Yachts>(); // One user can have many yachts
    }
}
