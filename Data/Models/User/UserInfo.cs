namespace Data.Models.User
{
    // UsersInfo Table
    public class UserInfo
    {
        public int UserInfoId { get; set; }                                 // Primary Key        
        public string FirstName { get; set; } = string.Empty;               // Required; Max Length : 50
        public string? MiddleName { get; set; }                             // Optional; Max Length : 50
        public string LastName { get; set; } = string.Empty;                // Required; Max Length : 50
        public string? Suffix { get; set; }                                 // Optional; Max Length : 10
        public int Age { get; set; }                                        // Required
        public string Sex { get; set; } = string.Empty;                     // Required; Max Length : 10
        public int? BirthInfoId { get; set; }                               // Foreign Key to BirthInfo
        public int? AddressId { get; set; }                                 // Foreign Key to Address
        public string ContactNumber { get; set; } = string.Empty;           // Required; Field Length : 11
        public string TaxIdentificationNumber { get; set; } = string.Empty; // Required; Max Length : 12
        public string CivilStatus { get; set; } = string.Empty;             // Required; Max Length : 20
        public string Religion { get; set; } = string.Empty;                // Required; Max Length : 50


        /*  Navigation Properties   */
        public BirthInfo BirthInfo { get; set; } = null!;
        public Address Address { get; set; } = null!;
        public ICollection<CustomerAuth> CustomersAuth { get; set; } = null!; 
    }
}
