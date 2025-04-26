namespace Data.Models.Finance
{
    public class AccountStatusType
    {
        /*  Table Properties    */
        public int AccountStatusTypeId { get; set; }
        public string AccountStatusTypeName { get; set; } = string.Empty;

        /*  Navigation Properties   */
        public ICollection<Account> Accounts { get; set; } = new List<Account>();
    }
}
