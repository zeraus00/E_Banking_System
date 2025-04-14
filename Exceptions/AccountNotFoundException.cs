namespace Exceptions
{
    public class AccountNotFoundException : Exception
    {
        public AccountNotFoundException(int accountId) : base($"Account with id {accountId} does not exist.") { }

        public AccountNotFoundException(string accountNumber) : base($"Account with number {accountNumber} does not exist.") { }
    }
}
