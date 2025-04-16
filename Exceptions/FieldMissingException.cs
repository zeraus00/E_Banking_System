namespace Exceptions
{
    public class FieldMissingException : Exception
    {
        public FieldMissingException() : base("This field is required.") { }

        public FieldMissingException(string fieldName, bool CustomFieldName) : base($"{fieldName} is required.") { }

        public static FieldMissingException firstName() => new FieldMissingException("First Name", true);

        public static FieldMissingException middleName() => new FieldMissingException("Middle Name", true);

        public static FieldMissingException lastName() => new FieldMissingException("Last Name", true);

        public static FieldMissingException Age() => new FieldMissingException("Age", true);

        public static FieldMissingException birthDate() => new FieldMissingException("Birth Date", true);

        public static FieldMissingException birthPlace() => new FieldMissingException("Birth Place", true);

        public static FieldMissingException Sex() => new FieldMissingException("Sex", true);

        public static FieldMissingException civilStatus() => new FieldMissingException("Civil Status", true);

        public static FieldMissingException Religion() => new FieldMissingException("Religion", true);

        public static FieldMissingException Region() => new FieldMissingException("Region", true);
        public static FieldMissingException Province() => new FieldMissingException("Province", true);
        public static FieldMissingException City() => new FieldMissingException("City", true);
        public static FieldMissingException Barangay() => new FieldMissingException("Barangay", true);
        public static FieldMissingException PostalCode() => new FieldMissingException("Postal Code", true);
        public static FieldMissingException streetNo() => new FieldMissingException("Street No.", true);
        public static FieldMissingException houseNo() => new FieldMissingException("House No.", true);
        public static FieldMissingException Email() => new FieldMissingException("Email", true);
        public static FieldMissingException contactNo() => new FieldMissingException("Contact No.", true);
        public static FieldMissingException fatherFirstName() => new FieldMissingException("Father's First Name", true);
        public static FieldMissingException fatherLastName() => new FieldMissingException("Father's Last Name", true);
        public static FieldMissingException motherFirstName() => new FieldMissingException("Mother's First Name", true);
        public static FieldMissingException motherLastName() => new FieldMissingException("Mother's Last Name", true);
        public static FieldMissingException validId() => new FieldMissingException("Government Valid ID", true);
        public static FieldMissingException Picture1x1() => new FieldMissingException("Picture 1x1", true);
    }
}
