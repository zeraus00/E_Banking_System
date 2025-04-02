namespace E_BankingSystem.Data.Models.User
{
    // Addresses Table
    public class Address
    {
        public int AddressId { get; set; }  // Primary Key
        public string House { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;


        public int BarangayId { get; set; } // Foreign Key to Barangay
        public Barangay Barangay { get; set; } = null!; // Navigation Property
        

        public int CityId { get; set; } // Foreign Key to City
        public City City { get; set; } = null!; // Navigation Property
        

        public int ProvinceId { get; set; } // Foreign Key to Province
        public Province Province { get; set; } = null!; // Navigation Property

        
        public int RegionId { get; set; }   // Foreign Key to Region
        public Region Region { get; set; } = null!; // Navigation Property


        public int PostalCode { get; set; }

        public ICollection<UserInfo> UserInfos { get; set; } = null!; // Navigation Property

    }
}
