namespace ATMprogram.Domain.Enums
{
    public enum TransactionType : byte
    {
        None = 0,
        Deposit,
        Withdrawal,
        Transfer
    }
}