namespace E_BankingSystem.Data.Models.User
{
    using E_BankingSystem.Data.Models.Authentication;
    using E_BankingSystem.Data.Models.Place;

    // BirthsInfo Table
    public class BirthInfo
    {
        public int BirthInfoId { get; set; }    // Primary Key

        
        public int UserId { get; set; } // Foreign Key to User
        public AccountAuth User { get; set; } = null!;


        public DateTime BirthDate { get; set; } // Required


        public int CityId { get; set; } // Foreign Key to City
        public City City { get; set; } = null!; // Navigation Property

        
        public int ProvinceId { get; set; } // Foreign Key to Province
        public Province Province { get; set; } = null!; // Navigation Property

        
        public int RegionId { get; set; }   // Foreign Key to Region
        public Region Region { get; set; } = null!; // Navigation Property
    }
}
