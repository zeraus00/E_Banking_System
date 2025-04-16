namespace Exceptions
{
    public class FieldMissingException : Exception
    {
        public FieldMissingException() : base("This field is required.") { }

        public FieldMissingException(string fieldName) : base($"{fieldName} is required.") { }
    }
}