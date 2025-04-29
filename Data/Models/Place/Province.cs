namespace Data.Models.Place
{
    // Provinces Table
    public class Province
    {
        public int ProvinceId { get; set; }                         // Primary Key
        public string ProvinceCode { get; set; } = string.Empty;      // Required
        public string ProvinceName { get; set; } = string.Empty;    // Required
        public int? RegionId { get; set; }                          // Foreign Key to Region


        /*  Navigation Properties   */
        public Region? Region { get; set; } 
        public ICollection<City> Cities { get; set; } = new List<City>(); 
        public ICollection<Address> Addresses { get; set; } = new List<Address>(); 
        public ICollection<BirthInfo> BirthsInfo { get; set; } = new List<BirthInfo>(); 
    }
}
