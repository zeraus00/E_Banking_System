namespace Data.Models.Finance
{
    public class LoanType
    {
        /*  Properties   */
        public int LoanTypeId { get; set; }                         //  Primary key
        public string LoanTypeName { get; set; } = string.Empty;    //  Required; Max Length: 20


        /*  Navigation Properties   */
        public ICollection<LoanTransaction> LoanTransactions { get; set; } = null!;
    }
}
