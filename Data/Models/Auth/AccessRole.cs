namespace Data.Models.Auth
{
    public class AccessRole
    {
        //  Table Properties
        public int AccessRoleId { get; set; }       //  Primary Key; Auto-Increment
        public string AccessRoleName { get; set; } = string.Empty;

        //  Navigation Properties
        public ICollection<UserInfoAccount> UsersInfoAccounts { get; set; } = new List<UserInfoAccount>();
    }
}
