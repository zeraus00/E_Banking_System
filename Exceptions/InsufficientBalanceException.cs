namespace Exceptions
{
    public class InsufficientBalanceException : Exception
    {
        public InsufficientBalanceException() : base("You do not have enough balance for this transaction!") { }
    }
}
