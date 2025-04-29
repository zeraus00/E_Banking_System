namespace Data.Models.Place
{
    // Regions Table
    public class Region
    {   
        public int RegionId { get; set; }                       // Primary Key
        public string RegionCode { get; set; } = string.Empty;  // Required
        public string RegionName { get; set; } = string.Empty;  // Required


        /*  Navigation Properties   */
        public ICollection<Province> Provinces { get; set; } = new List<Province>(); 
        public ICollection<Address> Addresses { get; set; } = new List<Address>(); 
        public ICollection<BirthInfo> BirthsInfo { get; set; } = new List<BirthInfo>();
    }
}
