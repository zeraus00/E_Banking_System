namespace Data.Models.Place
{
    // Cities Table
    public class City
    {
        /*  Properties    */
        public int CityId { get; set; }                         // Primary Key
        public string CityName { get; set; } = string.Empty;    // Required; Max Length: 50
        public int ProvinceId { get; set; }                     // Foreign Key to Province


        /*  Navigation Properties    */
        public Province Province { get; set; } = null!;
        public ICollection<Barangay> Barangays { get; set; } = null!;
        public ICollection<Address> Addresses { get; set; } = null!;
        public ICollection<BirthInfo> BirthInfos { get; set; } = null!;
    }
}
