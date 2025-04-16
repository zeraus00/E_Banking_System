namespace Data.Models.User
{
    public class Name
    {
        /*  Table Properties    */
        public int NameId { get; private set; }     //  Primary key
        public string FirstName { get; set; } = string.Empty;   // Required: MaxLength = 50;
        public string? MiddleName { get; set; }                 // Optional: MaxLength = 50;
        public string LastName { get; set; } = string.Empty;    // Required: MaxLength = 50;
        public string? Suffix { get; set; }                     // Optional: MaxLength = 10;

        /*  Navigation Properties   */
        public UserInfo UserInUsersInfo = null!;
        public ICollection<UserInfo> FatherInUsersInfo = new List<UserInfo>();
        public ICollection<UserInfo> MotherInUsersInfo = new List<UserInfo>();
        public ICollection<UserInfo> BeneficiaryInAccounts = new List<UserInfo>();
    }
}
