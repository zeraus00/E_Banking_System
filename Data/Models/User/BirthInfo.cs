using Data.Models.Place;

namespace Data.Models.User
{
    // BirthsInfo Table
    public class BirthInfo
    {
        /*  Properties    */
        public int BirthInfoId { get; set; }    // Primary Key
        public DateTime BirthDate { get; set; } // Required
        public int CityId { get; set; }         // Foreign Key to City
        public int ProvinceId { get; set; }     // Foreign Key to Province
        public int RegionId { get; set; }       // Foreign Key to Region


        /*  Navigation Properties   */
        public City City { get; set; } = null!;
        public Province Province { get; set; } = null!;
        public Region Region { get; set; } = null!;
        public ICollection<UserInfo> UsersInfo { get; set; } = null!; 
    }
}
