using Inzynierka.Data.Tables;

namespace Inzynierka.Models
{
    public class UserRolesViewModel
    {
        // Lista ról
        public List<Roles> Roles { get; set; }

        // Lista użytkowników z przypisanymi rolami
        public List<UserRoleViewModel> UserRoles { get; set; }

    }
}
