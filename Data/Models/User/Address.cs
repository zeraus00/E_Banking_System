using Data.Models.Place;

namespace Data.Models.User
{
    // Addresses Table
    public class Address
    {
        /*  Properties   */
        public int AddressId { get; set; }                  // Primary Key
        public string House { get; set; } = string.Empty;   // Max Length : 10
        public string Street { get; set; } = string.Empty;  // Max Length : 50
        public int BarangayId { get; set; }                 // Foreign Key to Barangay
        public int CityId { get; set; }                     // Foreign Key to City
        public int ProvinceId { get; set; }                 // Foreign Key to Province
        public int RegionId { get; set; }                   // Foreign Key to Region
        public int PostalCode { get; set; }

        /*  Navigation Properties   */
        public Barangay Barangay { get; set; } = null!;
        public City City { get; set; } = null!;
        public Province Province { get; set; } = null!;
        public Region Region { get; set; } = null!;
        public ICollection<UserInfo> UsersInfo { get; set; } = null!; 

    }
}
