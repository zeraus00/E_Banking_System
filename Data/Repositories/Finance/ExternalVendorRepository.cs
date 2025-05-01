namespace Data.Repositories.Finance
{
    public class ExternalVendorRepository : Repository
    {
        public ExternalVendorRepository(EBankingContext context) : base(context) { }

        public async Task GetExternalVendorById(int externalVendorId) => await GetById<ExternalVendor>(externalVendorId);
    }
}
