using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inzynierka.Data.Tables
{
    public class Yachts
    {
        public int Id { get; set; }
        [Display(Name = "Nazwa jachtu")]
        public string name { get; set; }
        [Display(Name = "Opis jachtu")]
        public string description { get; set; }
        [Display(Name = "Typ jachtu")]
        public string type { get; set; }
        [Display(Name = "Producent")]
        public string manufacturer { get; set; }
        [Display(Name = "Model")]
        public string model { get; set; }
        [Display(Name = "Rok produkcji")]
        public DateTime year { get; set; }
        [Display(Name = "Długość (m)")]
        public double length { get; set; }
        [Display(Name = "Szerokość (m)")]
        public double width { get; set; }
        [Display(Name = "Załoga (osoby)")]
        public int crew { get; set; }// Załoga
        [Display(Name = "Liczba kabin")]
        public int cabins { get; set; }
        [Display(Name = "Liczba łóżek")]
        public int beds { get; set; }
        [Display(Name = "Liczba toalet")]
        public int toilets { get; set; }
        [Display(Name = "Liczba pryszniców")]
        public int showers { get; set; }
        [Display(Name = "Lokalizacja")]
        public string location { get; set; }
        [Display(Name = "Wyporność (kg)")]
        public int capacity { get; set; }// wyporność

        // foreing key
        [Display(Name = "Właściciel jachtu")]
        public Users Owner { get; set; }
        [Display(Name = "Identyfikator właściciela")]
        public Guid OwnerId { get; set; }

        [Display(Name = "Identyfikator obrazu")]
        public int? ImageId { get; set; }
        [Display(Name = "Obraz")]
        public Image? Image { get; set; }

        // reference
        [Display(Name = "Lista rejsów")]
        public List<Cruises> Cruises { get; set; } = new List<Cruises>(); // relation with Cruises table, one to many
        [Display(Name = "Lista czarterów")]
        public List<Charters> Charters { get; set; } = new List<Charters>(); // relation with Charters table, one to many
        [Display(Name = "Lista ofert sprzedaży")]
        public List<YachtSale> YachtSale { get; set; } = new List<YachtSale>();// relation with YachtSale table, one to many
        [Display(Name = "Lista raportów")]
        public List<Reports> Reports { get; set; }= new List<Reports>();// relation with Reports table, one to many
        [Display(Name = "Lista komentarzy")]
        public List<Comments> Comments { get; set; } = new List<Comments>();// relation with Comments table, one to many

        // Tymczasowa właściwość dla linku do obrazu
        [NotMapped]
        [Display(Name = "Link do obrazu")]
        public string ImageLink { get; set; }
    }
}
