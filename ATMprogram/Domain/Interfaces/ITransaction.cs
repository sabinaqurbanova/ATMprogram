using ATMprogram.Domain.DTOs;

namespace ATMprogram.Domain.Interfaces
{
    public interface ITansaction
    {
        void InsertTransaction(TransactionDetailsDTO transactionDetails);
        void ViewTransaction();
    }
}