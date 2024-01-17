using ATMprogram.Domain.Enums;

namespace ATMprogram.Domain.DTOs
{
    public class TransactionDetailsDTO
    {
        private TransactionDetailsDTO() { }

        public long UserBankAccountId { get; set; }
        public TransactionType Type { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }

        public static TransactionDetailsDTO CreateObject(long userBankAccountId, TransactionType type, decimal amount, string description)
        {
            return new TransactionDetailsDTO
            {
                UserBankAccountId = userBankAccountId,
                Type = type,
                Amount = amount,
                Description = description
            };
        }
    }
}