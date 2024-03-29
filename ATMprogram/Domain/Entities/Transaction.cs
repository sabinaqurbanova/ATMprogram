﻿using ATMprogram.Domain.Enums;

namespace ATMprogram.Domain.Entities
{
    public class Transaction
    {
        public long TransactionID { get; set; }
        public long UserBankAccountID { get; set; }
        public DateTime TransactionDate { get; set; }
        public TransactionType TransactionType { get; set; }
        public string Description { get; set; }
        public Decimal TransactionAmount { get; set; }
    }
}