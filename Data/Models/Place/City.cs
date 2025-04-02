namespace E_BankingSystem.Data.Models.Place
{
    // Cities Table
    public class City
    {
        public int CityId { get; set; } // Primary Key
        public string CityName { get; set; } = string.Empty; // Required


        public int ProvinceId { get; set; } // Foreign Key to Province
        public Province Province { get; set; } = null!; // Navigation Property
    
    
        public ICollection<Barangay> Barangays { get; set; } = null!; // Navigation Property
    }
}
    