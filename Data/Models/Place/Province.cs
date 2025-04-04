namespace Data.Models.Place
{
    // Provinces Table
    public class Province
    {
        public int ProvinceId { get; set; }                         // Primary Key
        public string ProvinceName { get; set; } = string.Empty;    // Required
        public int RegionId { get; set; }                           // Foreign Key to Region


        /*  Navigation Properties   */
        public Region Region { get; set; } = null!; 
        public ICollection<City> Cities { get; set; } = null!; 
        public ICollection<Address> Addresses { get; set; } = null!; 
        public ICollection<BirthInfo> BirthsInfo { get; set; } = null!; 
    }
}
