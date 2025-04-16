namespace Data.Repositories.User
{
    public class NameRepository : Repository
    {
        public NameRepository(EBankingContext context) : base(context) { }
    }
    
    /// <summary>
    /// Builder class for Name
    /// </summary>
    public class NameBuilder
    {
        private string _firstName = string.Empty;
        private string? _middleName;
        private string _lastName = string.Empty;
        private string? _suffix;

        public NameBuilder WithFirstName(string firstName)
        {
            _firstName = firstName.Trim();
            return this;
        }

        public NameBuilder WithMiddleName(string middleName)
        {
            _middleName = middleName.Trim();
            return this;
        }

        public NameBuilder WithLastName(string lastName)
        {
            _lastName = lastName.Trim();
            return this;
        }

        public NameBuilder WithSuffix(string suffix)
        {
            _suffix = suffix.Trim();
            return this;
        }

        /// <summary>
        /// Builds the Name object with the specified properties
        /// </summary>
        /// <returns></returns>
        public Name Build()
        {
            return new Name
            {
                FirstName = _firstName,
                MiddleName = _middleName,
                LastName = _lastName,
                Suffix = _suffix
            };
        }
    }
}
