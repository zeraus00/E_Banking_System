namespace Data.Models.Authentication
{
    // Roles Table
    public class Role
    {
        /*  Properties  */
        public int RoleId { get; private set; }                 //  Primary Key
        public string RoleName { get; set; } = string.Empty;    //  Required; Administrator, User, Employee

        /*  Navigation Properties  */
        public ICollection<UserAuth> UsersAuth { get; set; } = new List<UserAuth>();

    }
}
