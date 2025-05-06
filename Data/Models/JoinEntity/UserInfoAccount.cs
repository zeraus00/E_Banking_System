namespace Data.Models.JoinEntity
{
    public class UserInfoAccount
    {
        public int UserInfoId { get; set; }
        public int AccountId { get; set; }
        public int AccessRoleId { get; set; }
        public bool IsLinkedToOnlineAccount { get; set; } = false;

        public UserInfo UserInfo { get; set; } = null!;
        public Account Account { get; set; } = null!;

        public AccessRole AccessRole { get; set; } = null!;
    }
}
