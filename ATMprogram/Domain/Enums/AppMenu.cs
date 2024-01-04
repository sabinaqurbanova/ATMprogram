using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATMprogram.Domain.Enums
{
    public enum AppMenu : byte
    {
        CheckBalance=1,
        PlaceDeposit,
        MakeWithDrawal,
        InternalTransfer,
        ViewTransaction,
        Logout
    }
   
}
