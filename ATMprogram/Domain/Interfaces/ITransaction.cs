using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ATMprogram.Domain.Enums;

namespace ATMprogram.Domain.Interfaces
{
    public interface ITansaction
    {
        void InsertTransaction(long _UserBankAccountId, TransactionType _tranType, decimal _tranAmount, string _description);
        void ViewTransaction();
    }
}
