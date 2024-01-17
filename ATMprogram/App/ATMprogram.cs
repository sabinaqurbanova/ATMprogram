using ATMprogram.Domain.Interfaces;
using ATMprogram.Domain.Extensions;
using ATMprogram.Domain.Entities;
using ATMprogram.Domain.Enums;
using ATMprogram.Domain.DTOs;
using ConsoleTables;
using ATMprogram.UI;

namespace ATMprogram
{
    public class ATMprogram : IUserLogin, IUserAccountActions, ITansaction
    {
        private List<UserAccount> _userAccountList;
        private UserAccount _selectedAccount;
        private List<Transaction> _listOfTransactions;
        private const decimal _minimumKeptAmount = 500;
        private readonly AppScreen _screen;

        public ATMprogram()
        {
            this._screen = new AppScreen();
        }

        #region ATMprogram
        public void InitializeData()
        {
            _userAccountList = new List<UserAccount>
            {
                new UserAccount{ID=1, FullName="Sabina Qurbanova", AccountNumber=123456, CardNumber=321321, CardPin=123123, AccountBalance=50000.00m, IsLocked=false},
                new UserAccount{ID=2, FullName="Qurban Qurbanov", AccountNumber=123457, CardNumber=456456, CardPin=654654, AccountBalance=40000.00m, IsLocked=false},
                new UserAccount{ID=3, FullName="Qalib Qurbanov", AccountNumber=123458, CardNumber=789789, CardPin=987987, AccountBalance=30000.00m, IsLocked=true}
            };
            _listOfTransactions = new List<Transaction>();
        }

        private void ProcessInternalTransfer(InternalTransfer internalTransfer)
        {
            if (internalTransfer.TransferAmount <= 0)
            {
                Utility.PrintMessage("Amount needs to be more than zero. Try again.", false);
                return;
            }
            //check sender's account balance
            if (internalTransfer.TransferAmount > _selectedAccount.AccountBalance)
            {
                Utility.PrintMessage($"Transfer failed. You do not have enough balance to transfer {Utility.FormatAmount(internalTransfer.TransferAmount)}", false);
                return;
            }
            //check the minimum kept amount
            if ((_selectedAccount.AccountBalance - internalTransfer.TransferAmount) < _minimumKeptAmount)
            {
                Utility.PrintMessage($"Transfer failed. Your account needs to have minimum {Utility.FormatAmount(_minimumKeptAmount)}", false);
                return;
            }
            //check receiver's account number is valid
            var selectedBankAccountReceiver = (from userAcc in _userAccountList
                                               where userAcc.AccountNumber == internalTransfer.RecipientBankAccountNumber
                                               select userAcc).FirstOrDefault();

            if (selectedBankAccountReceiver == null)
            {
                Utility.PrintMessage("Transfer failed. Receiver bank account number is invalid.", false);
                return;
            }
            //check receiver's name
            if (selectedBankAccountReceiver.FullName != internalTransfer.RecipientBankAccountName)
            {
                Utility.PrintMessage("Transfer failed. Recipient's bank account name does not match.", false);
                return;
            }

            TransactionDetailsDTO transferedTo = TransactionDetailsDTO.CreateObject(_selectedAccount.ID, TransactionType.Transfer, internalTransfer.TransferAmount, $"Transfered to {selectedBankAccountReceiver.AccountNumber} ({selectedBankAccountReceiver.FullName})");
            {
                //add transaction to transactions record - sender
                InsertTransaction(transferedTo);
            }

            //update sender's account balance 
            _selectedAccount.AccountBalance -= internalTransfer.TransferAmount;

            TransactionDetailsDTO transferedFrom = TransactionDetailsDTO.CreateObject(selectedBankAccountReceiver.ID, TransactionType.Transfer, internalTransfer.TransferAmount, $"Transfered from {_selectedAccount.AccountNumber}({_selectedAccount.FullName})");
            {
                //add transaction record - receiver
                InsertTransaction(transferedFrom);
            }

            //update reciever's account balance 
            selectedBankAccountReceiver.AccountBalance += internalTransfer.TransferAmount;

            //success message
            Utility.PrintMessage($"You have successfully transfered {Utility.FormatAmount(internalTransfer.TransferAmount)} to {internalTransfer.RecipientBankAccountName}", true);
        }

        private bool PreviewBankNotesCount(int amount)
        {
            int thousandNotesCount = amount / 1000;
            int fiveHundredCount = (amount % 1000) / 500;

            Console.WriteLine($"\nSummary");
            Console.WriteLine("------");
            Console.WriteLine($"{AppScreen.cur}1000 X {thousandNotesCount} = {1000 * thousandNotesCount}");
            Console.WriteLine($"{AppScreen.cur}500 X {fiveHundredCount} = {500 * fiveHundredCount}");
            Console.WriteLine($"Total amount:{Utility.FormatAmount(amount)}\n");

            int opt = Validator.Convert<int>("1 to confirm");
            return opt.Equals(1);

        }

        public void Run()
        {
            AppScreen.Welcome();
            CheckUserCardNumAndPassword();
            AppScreen.WelcomeCustomer(_selectedAccount.FullName);
            while (true)
            {
                AppScreen.DisplayAppMenu();
                ProcessMenuOption();
            }
        }

        internal void ProcessMenuOption()
        {
            switch (Validator.Convert<int>("an option:"))
            {
                case (int)AppMenu.CheckBalance:
                    CheckBalance();
                    break;
                case (int)AppMenu.PlaceDeposit:
                    PlaceDeposit();
                    break;
                case (int)AppMenu.MakeWithDrawal:
                    MakeWithDrawal();
                    break;
                case (int)AppMenu.InternalTransfer:
                    var internalTransfer = _screen.InternalTransferForm();
                    ProcessInternalTransfer(internalTransfer);
                    break;
                case (int)AppMenu.ViewTransaction:
                    ViewTransaction();
                    break;
                case (int)AppMenu.Logout:
                    AppScreen.LogOutProgress();
                    Utility.PrintMessage("You have successfully logged out. Collect your ATM card.");
                    Run();
                    break;
                default:
                    Utility.PrintMessage("Invalid option.", false);
                    break;
            }
        }
        #endregion ATMprogram

        #region IUserAccountActions
        public void CheckBalance()
        {
            Utility.PrintMessage($"Your acoount balance is: {Utility.FormatAmount(_selectedAccount.AccountBalance)}");
        }

        public void PlaceDeposit()
        {
            Console.WriteLine("\nOnly multiples of 500 and 1000 AZN allowed.\n");
            var transaction_amt = Validator.Convert<int>($"amount {AppScreen.cur}");

            //simulate counting
            Console.WriteLine("\nChecking and Counting bank notes.");
            Utility.PrintDotAnimation();
            Console.WriteLine("");

            //some guard clause 
            if (transaction_amt <= 0)
            {
                Utility.PrintMessage("Amount needs to be greater than zero. Try again.", false);
                return;
            }
            if (transaction_amt % 500 != 0)
            {
                Utility.PrintMessage($"Enter deposit amount in multiples of 500 or 1000. Try again.", false);
                return;
            }
            if (PreviewBankNotesCount(transaction_amt) == false)
            {
                Utility.PrintMessage($"You have cancelled your action.", false);
                return;
            }

            TransactionDetailsDTO transactionDetails = TransactionDetailsDTO.CreateObject(_selectedAccount.ID, TransactionType.Deposit, transaction_amt, "");
            {
                //bind transaction details to transaction object
                InsertTransaction(transactionDetails);
            }

            //update card balance
            _selectedAccount.AccountBalance += transaction_amt;

            //print success message
            Utility.PrintMessage($"Your deposit of {Utility.FormatAmount(transaction_amt)} was successful", true);
        }

        public void MakeWithDrawal()
        {
            decimal transaction_amt = 0m;
            int selectedAmount = AppScreen.SelectAmount();
            if (selectedAmount == -1)
            {
                MakeWithDrawal();
                return;
            }
            else if (selectedAmount != 0)
            {
                transaction_amt = selectedAmount;
            }
            else
            {
                transaction_amt = Validator.Convert<int>($"amount {AppScreen.cur}");
            }

            //input validation
            if (transaction_amt <= 0)
            {
                Utility.PrintMessage("Amounts needs to be greater than zero. Try again.");
                return;
            }
            if (transaction_amt % 500 != 0)
            {
                Utility.PrintMessage("You can only withdraw amount in multiples of 500 or 1000 AZN .Try again.", false);
                return;
            }

            //bussiness logic validations

            if (transaction_amt > _selectedAccount.AccountBalance)
            {
                Utility.PrintMessage($"Withdrawal failed. Your balance is too low to withdraw {Utility.FormatAmount(transaction_amt)}", false);
                return;
            }
            if ((_selectedAccount.AccountBalance - transaction_amt) < _minimumKeptAmount)
            {
                Utility.PrintMessage($"Withdrawal failed. Your account needs to have minimum {Utility.FormatAmount(_minimumKeptAmount)}", false);
                return;
            }

            TransactionDetailsDTO transactionDetails = TransactionDetailsDTO.CreateObject(_selectedAccount.ID, TransactionType.Withdrawal, transaction_amt.ReverseSign(), ""); // amount ,menfidir
            {
                //Bind withdrawal details to transaction object
                InsertTransaction(transactionDetails);
            }

            //Update account balance
            _selectedAccount.AccountBalance -= transaction_amt;

            //Success message
            Utility.PrintMessage($"You have successfully withdrawn {Utility.FormatAmount(transaction_amt)}.", true);
        }
        #endregion IUserAccountActions

        #region ITransaction
        public void InsertTransaction(TransactionDetailsDTO transactionDetails)
        {
            //create a new transaction object
            var transaction = new Transaction()
            {
                TransactionID = Utility.GettransactionId(),
                UserBankAccountID = transactionDetails.UserBankAccountId,
                TransactionDate = DateTime.Now,
                TransactionType = transactionDetails.Type,
                TransactionAmount = transactionDetails.Amount,
                Description = transactionDetails.Description
            };
            //add transaction object to the list 
            _listOfTransactions.Add(transaction);
        }

        public void ViewTransaction()
        {
            var filteredTransactionList = _listOfTransactions.Where(t => t.UserBankAccountID == _selectedAccount.ID).ToList();
            //check if there's transaction
            if (filteredTransactionList.Count <= 0)
            {
                Utility.PrintMessage("You have no transaction yet.", true);
            }
            else
            {
                var table = new ConsoleTable("ID", "Tranzaction Date", "Type", "Descriptions", $"Amount {AppScreen.cur}");
                foreach (var tran in filteredTransactionList)
                {
                    table.AddRow(tran.TransactionID, tran.TransactionDate, tran.TransactionType, tran.Description, tran.TransactionAmount);
                }
                table.Options.EnableCount = false;
                table.Write();
                Utility.PrintMessage($"You have {filteredTransactionList.Count} transaction(s)", true);
            }
        }
        # endregion ITransaction

        #region IUserLogin
        public void CheckUserCardNumAndPassword()
        {
            bool isCorrectLogin = false;
            while (isCorrectLogin == false)
            {
                UserAccount inputAccount = AppScreen.UserLoginForm();
                AppScreen.LoginProgress();
                foreach (UserAccount account in _userAccountList)
                {
                    _selectedAccount = account;

                    if (inputAccount.CardNumber.Equals(_selectedAccount.CardNumber))
                    {
                        _selectedAccount.TotalLogin++;

                        if (inputAccount.CardPin.Equals(_selectedAccount.CardPin))
                        {
                            _selectedAccount = account;

                            if (_selectedAccount.IsLocked || _selectedAccount.TotalLogin > 3)
                            {
                                AppScreen.PrintLockScreen();
                            }
                            else
                            {
                                _selectedAccount.TotalLogin = 0;
                                isCorrectLogin = true;
                                break;
                            }
                        }
                    }
                }
            }
            if (isCorrectLogin == false)
            {
                Utility.PrintMessage("/nInvalid card number or PIN.", false);
                _selectedAccount.IsLocked = _selectedAccount.TotalLogin == 3;
                if (_selectedAccount.IsLocked)
                {
                    AppScreen.PrintLockScreen();
                }
            }
            Console.Clear();
        }
        #endregion IUserLogin
    }
}