namespace Data.Constants
{
    public class ExternalVendors
    {
        public const string GCASH = "GCash";
        public const string PAYMAYA = "PayMaya";
        public const string GOTYME = "GoTyme";

        public static List<string> AS_STRING_LIST { get; } = new()
        {
            GCASH,
            PAYMAYA,
            GOTYME
        };
    }
}
