namespace Data.Models.Place
{
    // Barangays Table
    public class Barangay
    {
        public int BarangayId { get; set; }                             // Primary Key
        public string BarangayName { get; set; } = string.Empty;        // Required; Max Length: 50
        public int CityId { get; set; }                                 // Foreign Key to City


        public City City { get; set; } = null!;                         // Navigation Property
        public ICollection<Address> Addresses { get; set; } = null!;    // Navigation Property
    }
}
