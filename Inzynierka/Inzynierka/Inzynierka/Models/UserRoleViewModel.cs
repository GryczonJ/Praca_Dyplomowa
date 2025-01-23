namespace Inzynierka.Models
{
    public class UserRoleViewModel
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public Guid RoleId { get; set; }
        public string RoleName { get; set; }
    }
}
