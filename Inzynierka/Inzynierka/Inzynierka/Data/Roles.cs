namespace Inzynierka.Data
{
    public class Roles
    {
        public int Id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public List<byte[]> certificates { get; set; }

        // reference
        public List<Users> Users { get; set; }

    }
}
