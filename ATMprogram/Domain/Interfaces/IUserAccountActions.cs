namespace ATMprogram.Domain.Interfaces
{
    public interface IUserAccountActions
    {
        void CheckBalance();
        void PlaceDeposit();
        void MakeWithDrawal();
    }
}