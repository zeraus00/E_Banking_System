namespace Data.Models.Place
{
    // Cities Table
    public class City
    {
        /*  Properties    */
        public int CityId { get; set; }                         // Primary Key
        public string CityCode { get; set; } = string.Empty;    // Required
        public string CityName { get; set; } = string.Empty;    // Required; Max Length: 50
        public int? ProvinceId { get; set; }                    // Foreign Key to Province


        /*  Navigation Properties    */
        public Province? Province { get; set; }
        public ICollection<Barangay> Barangays { get; set; } = new List<Barangay>();
        public ICollection<Address> Addresses { get; set; } = new List<Address>();
        public ICollection<BirthInfo> BirthInfos { get; set; } = new List<BirthInfo>();
    }
}
