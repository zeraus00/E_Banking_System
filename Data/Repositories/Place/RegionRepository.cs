namespace Data.Repositories.Place
{
    /// <summary>
    /// CRUD operations handler for Regions table.
    /// Methods for adding, updating, deleting and retrieving data from the database
    /// 
    /// NOTE: There is no persistent saving here. You MUST call
    /// DbContext.SaveChanges or DbContext.SaveChangesAsync externally.
    /// </summary>
    /// <param name="_context"></param>
    public class RegionRepository : Repository
    {
        public RegionRepository(EBankingContext context) : base(context) { }
    }
    
    /// <summary>
    /// Builder class for Region
    /// </summary>
    public class RegionBuilder
    {
        private string _regionName = string.Empty;
        
        public RegionBuilder WithRegionName (string regionName)
        {
            _regionName = regionName.Trim();
            return this;
        }

        /// <summary>
        /// Builds the Region object with the specified properties
        /// </summary>
        /// <returns></returns>
        public Region Build()
        {
            return new Region
            {
                RegionName = _regionName
            };
        }
    }
}
