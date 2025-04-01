namespace E_Banking.Models.Place
{
    // Provinces Table
    public class Province
    {
        public int ProvinceId { get; set; } // Primary Key
        public string ProvinceName { get; set; } = string.Empty;    // Required


        public int RegionId { get; set; }   // Foreign Key to Region
        public Region Region { get; set; } = null!; // Navigation Property
    }
}
