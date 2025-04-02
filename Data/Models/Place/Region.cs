namespace E_BankingSystem.Data.Models.Place
{
    // Regions Table
    public class Region
    {
        public int RegionId { get; set; }   // Primary Key
        public string RegionName { get; set; } = string.Empty;  // Required


        public ICollection<Province> Provinces { get; set; } = null!; // Navigation Property
    }
}
