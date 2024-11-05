namespace Inzynierka.Data.Tables
{
    public class Image
    {
        public int Id { get; set; }
        public string link { get; set; }
        // foreing key
        public List<Users> Users { get; set; } = new List<Users>();// RequestUser
        public List<Yachts> Yachts { get; set; } = new List<Yachts>();// RequestYacht
    }
}
