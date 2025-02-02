using System.ComponentModel.DataAnnotations;

namespace Inzynierka.Data.Tables
{
    public class Image
    {
        [Display(Name = "Identyfikator")]
        public int Id { get; set; }
        [Display(Name = "Link do obrazu")]
        public string link { get; set; }

       /* [Display(Name = "Zbanowany")]
        public bool banned { get; set; } = false;*/

        // foreing key
        [Display(Name = "Użytkownicy")]
        public List<Users> Users { get; set; } = new List<Users>();// RequestUser
        [Display(Name = "Jachty")]
        public List<Yachts> Yachts { get; set; } = new List<Yachts>();// RequestYacht
    }
}
