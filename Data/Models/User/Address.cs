namespace E_BankingSystem.Data.Models.User
{
    using E_BankingSystem.Data.Models.Place;
    // Addresses Table
    public class Address
    {
        public int AddressId { get; set; }  // Primary Key
        public int HouseNumber { get; set; }
        public string? Street { get; set; }


        public int BarangayId { get; set; } // Foreign Key to Barangay
        public Barangay Barangay { get; set; } = null!; // Navigation Property
        

        public int CityId { get; set; } // Foreign Key to City
        public City City { get; set; } = null!; // Navigation Property
        

        public int ProvinceId { get; set; } // Foreign Key to Province
        public Province Province { get; set; } = null!; // Navigation Property

        
        public int RegionId { get; set; }   // Foreign Key to Region
        public Region Region { get; set; } = null!; // Navigation Property


        public int PostalCode { get; set; }
    }
}
