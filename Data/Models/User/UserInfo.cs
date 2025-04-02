namespace E_BankingSystem.Data.Models.User
{
    // UsersInfo Table
    public class UserInfo
    {
        public int UserInfoId { get; set; } // Primary Key        
        public string FirstName { get; set; } = string.Empty;   // Required
        public string? MiddleName { get; set; } 
        public string LastName { get; set; } = string.Empty;    // Required
        public string? Suffix { get; set; }
        public int Age { get; set; }    // Required
        public char Sex { get; set; }   // Required


        public int BirthInfoId { get; set; } // Foreign Key to BirthInfo
        public BirthInfo BirthInfo { get; set; } = null!;   // Navigation Property


        public int AddressId { get; set; }  // Foreign Key to Address
        public Address Address { get; set; } = null!;   // Navigation Property


        public string ContactNumber { get; set; } = string.Empty;   // Required
        public string TaxIdentificationNumber { get; set; } = string.Empty; // Required
        public string CivilStatus { get; set; } = string.Empty; // Required
        public string Religion { get; set; } = string.Empty;    // Required


        public ICollection<CustomerAuth> CustomersAuth { get; set; } = null!; // Navigation Property
    }
}
