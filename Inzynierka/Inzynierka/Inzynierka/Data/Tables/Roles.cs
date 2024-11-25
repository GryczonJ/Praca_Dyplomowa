using Microsoft.AspNetCore.Identity;
using System.Data;

namespace Inzynierka.Data.Tables
{
    public class Roles: IdentityRole<Guid>
    { 
        public List<byte[]>? certificates { get; set; }

        // foreing key

        // reference
        //public List<Users> Users { get; set; }
        public List<Reports> VeryficationReports { get; set; } = new List<Reports>();
        public LinkedList<Reports> SuspectReports { get; set; } = new LinkedList<Reports>();
    }
   /* public class Moderacja : Roles { 
        public int PoziomModeracji { get; set; } = 3; // Poziom uprawnień, np. 1 = niskie, 3 = wysokie
    }*/
}

