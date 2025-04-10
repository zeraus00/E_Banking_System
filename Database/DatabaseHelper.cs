namespace Database
{
    public class DatabaseHelper
    {
        public static void CreateDatabase(DbContext context)
        {
            context.Database.EnsureCreated();
        }
    }
}
