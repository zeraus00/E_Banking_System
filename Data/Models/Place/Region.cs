namespace Data.Models.Place
{
    // Regions Table
    public class Region
    {
        public int RegionId { get; set; }                       // Primary Key
        public string RegionName { get; set; } = string.Empty;  // Required


        /*  Navigation Properties   */
        public ICollection<Province> Provinces { get; set; } = null!; 
        public ICollection<Address> Addresses { get; set; } = null!; 
        public ICollection<BirthInfo> BirthsInfo { get; set; } = null!;
    }
}
