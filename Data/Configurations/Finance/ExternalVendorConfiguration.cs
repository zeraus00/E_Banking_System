namespace Data.Configurations.Finance
{
    public class ExternalVendorConfiguration : IEntityTypeConfiguration<ExternalVendor>
    {
        public void Configure(EntityTypeBuilder<ExternalVendor> ExternalVendors)
        {
            ExternalVendors.ToTable("ExternalVendors", "FinanceSchema");

            /*  Configure Table Properties  */
            ExternalVendors
                .HasKey(ev => ev.VendorId);

            ExternalVendors
                .Property(ev => ev.VendorId)
                .ValueGeneratedOnAdd();

            ExternalVendors
                .Property(ev => ev.VendorName)
                .HasMaxLength(30)
                .IsRequired();

            /*
             *  Relationships
             *  Transactions (one to many)
             */
            ExternalVendors
                .HasData(
                    new ExternalVendor { VendorId = 1, VendorName = "GCash" },
                    new ExternalVendor { VendorId = 2, VendorName = "Paymaya" },
                    new ExternalVendor { VendorId = 3, VendorName = "GoTyme" }
                );


        }
    }
}
