namespace Data.Models.Finance
{
    public class ExternalVendor
    {
        /*  Table Properties    */
        public int VendorId { get; set; }                           //  Primary Key
        public string VendorName { get; set; } = string.Empty;      //  Required

        /*  Navigation Properties   */
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
