namespace ATMprogram.Domain.Enums
{
    public enum AppMenu : byte
    {
        None = 0,
        CheckBalance,
        PlaceDeposit,
        MakeWithDrawal,
        InternalTransfer,
        ViewTransaction,
        Logout
    }
}