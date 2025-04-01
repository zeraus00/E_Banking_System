namespace E_Banking.Models.Place
{
    // Barangays Table
    public class Barangay
    {
        public int BarangayId { get; set; } // Primary Key
        public string BarangayName { get; set; } = string.Empty; // Required


        public int CityId { get; set; } // Foreign Key to City
        public City City { get; set; } = null!; // Navigation Property
    }
}
