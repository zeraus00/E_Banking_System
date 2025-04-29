namespace Data.Models.Place
{
    // Barangays Table
    public class Barangay
    {
        /*  Table Properties    */
        public int BarangayId { get; set; }                                           // Primary Key
        public string BarangayCode { get; set; } = string.Empty;                      // Required;
        public string BarangayName { get; set; } = string.Empty;                      // Required; Max Length: 50
        public int? CityId { get; set; }                                              // Foreign Key to City


        /*  Navigation Properties   */
        public City? City { get; set; }                                      // Navigation Property
        public ICollection<Address> Addresses { get; set; } = new List<Address>();    // Navigation Property
    }
}
