namespace Data.Models.User
{
    //  Religions Table
    public class Religion
    {
        /*  Table Properties    */
        public int ReligionId { get; private set; }                 //  Primary Key
        public string ReligionName { get; set; } = string.Empty;    //  Required; Max Length: 50

        /*  Navigation Properties   */
        public ICollection<UserInfo> UsersInfo { get; set; } = new List<UserInfo>();
    }
}
