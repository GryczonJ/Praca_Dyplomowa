namespace Inzynierka.Data.Tables
{
    public class Roles
    {
        public int Id { get; set; }
        public List<byte[]> certificates { get; set; }

        // foreing key

        // reference
        public List<Users> Users { get; set; }
        public List<Reports> Reports { get; set; }
    }
}
