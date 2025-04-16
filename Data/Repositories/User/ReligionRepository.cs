namespace Data.Repositories.User
{
    public class ReligionRepository : Repository
    {
        public ReligionRepository(EBankingContext context) : base(context) { }
    }

    /// <summary>
    /// Builder class for Religion
    /// </summary>
    public class ReligionBuilder
    {
        private string _religionName = string.Empty;

        public ReligionBuilder WithReligionName(string religionName)
        {
            _religionName = religionName.Trim();
            return this;
        }

        /// <summary>
        /// Builds the Religion object with the specified properties.
        /// </summary>
        /// <returns></returns>
        public Religion Build()
        {
            return new Religion
            {
                ReligionName = _religionName
            };
        }
    }
}
