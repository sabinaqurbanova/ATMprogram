using ATMprogram.Domain.Entities;
using ATMprogram.Domain.Interfaces;
using ATMprogram.UI;
using ATMprogram.Domain.Enums;
using System.Linq;
using ConsoleTables;

namespace ATMprogram
{
    public class ATMprogram : IUserLogin, IUserAccountActions, ITansaction
    {
        private List<UserAccount> UserAccountList;
        private UserAccount selectedAccount;
        private List<Transaction> _listOfTransactions;
        private const decimal minimumKeptAmount = 500;
        private readonly AppScreen screen;

        public ATMprogram()
        {
            screen = new AppScreen();
        }

        public void CheckUserCardNumAndPassword()
        {
            bool isCorrectLogin = false;
            while (isCorrectLogin == false)
            {
                UserAccount inputAccount = AppScreen.UserLoginForm();
                AppScreen.LoginProgress();
                foreach (UserAccount account in UserAccountList)
                {
                    selectedAccount = account;

                    if (inputAccount.CardNumber.Equals(selectedAccount.CardNumber))
                    {
                        selectedAccount.TotalLogin++;

                        if (inputAccount.CardPin.Equals(selectedAccount.CardPin))
                        {
                            selectedAccount = account;

                            if (selectedAccount.IsLocked || selectedAccount.TotalLogin > 3)
                            {
                                AppScreen.PrintLockScreen();
                            }
                            else
                            {
                                selectedAccount.TotalLogin = 0;
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
                selectedAccount.IsLocked = selectedAccount.TotalLogin == 3;
                if (selectedAccount.IsLocked)
                {
                    AppScreen.PrintLockScreen();
                }
            }
            Console.Clear();
        }


        public void InitializeData()
        {
            UserAccountList = new List<UserAccount>
            {
                new UserAccount{ID=1, FullName="Sabina Qurbanova", AccountNumber=123456, CardNumber=321321, CardPin=123123, AccountBalance=50000.00m, IsLocked=false},
                new UserAccount{ID=2, FullName="Qurban Qurbanov", AccountNumber=123457, CardNumber=456456, CardPin=654654, AccountBalance=40000.00m, IsLocked=false},
                new UserAccount{ID=3, FullName="Qalib Qurbanov", AccountNumber=123458, CardNumber=789789, CardPin=987987, AccountBalance=30000.00m, IsLocked=true}
            };
            _listOfTransactions = new List<Transaction>();
        }

        public void CheckBalance()
        {
            Utility.PrintMessage($"Your acoount balance is: {Utility.FormatAmount(selectedAccount.AccountBalance)}");
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

            //bind transaction details to transaction object
            InsertTransaction(selectedAccount.ID, TransactionType.Deposit, transaction_amt, "");

            //update card balance
            selectedAccount.AccountBalance += transaction_amt;

            //print success message
            Utility.PrintMessage($"Your deposit of {Utility.FormatAmount(transaction_amt)} was successful", true);
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

        public void MakeWithDrawal()
        {
            var transaction_amt = 0;
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

            if (transaction_amt > selectedAccount.AccountBalance)
            {
                Utility.PrintMessage($"Withdrawal failed. Your balance is too low to withdraw {Utility.FormatAmount(transaction_amt)}", false);
                return;
            }
            if ((selectedAccount.AccountBalance - transaction_amt) < minimumKeptAmount)
            {
                Utility.PrintMessage($"Withdrawal failed. Your account needs to have minimum {Utility.FormatAmount(minimumKeptAmount)}", false);
                return;
            }

            //Bind withdrawal details to transaction object
            InsertTransaction(selectedAccount.ID, TransactionType.Withdrawal, -transaction_amt, "");

            //Update account balance
            selectedAccount.AccountBalance -= transaction_amt;

            //Success message
            Utility.PrintMessage($"You have successfully withdrawn {Utility.FormatAmount(transaction_amt)}.", true);
        }

        public void Run()
        {
            AppScreen.Welcome();
            CheckUserCardNumAndPassword();
            AppScreen.WelcomeCustomer(selectedAccount.FullName);
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
                    var internalTransfer = screen.InternalTransferForm();
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

        public void InsertTransaction(long _UserBankAccountId, TransactionType _tranType, decimal _tranAmount, string _description)
        {
            //create a new transaction object
            var transaction = new Transaction()
            {
                TransactionID = Utility.GettransactionId(),
                UserBankAccountID = _UserBankAccountId,
                TransactionDate = DateTime.Now,
                TransactionType = _tranType,
                TransactionAmount = _tranAmount,
                Description = _description
            };
            //add transaction object to the list 
            _listOfTransactions.Add(transaction);
        }



        public void ViewTransaction()
        {
            var filteredTransactionList = _listOfTransactions.Where(t => t.UserBankAccountID == selectedAccount.ID).ToList();
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

        private void ProcessInternalTransfer(InternalTransfer internalTransfer)
        {
            if (internalTransfer.TransferAmount <= 0)
            {
                Utility.PrintMessage("Amount needs to be more than zero. Try again.", false);
                return;
            }
            //check sender's account balance
            if (internalTransfer.TransferAmount > selectedAccount.AccountBalance)
            {
                Utility.PrintMessage($"Transfer failed. You do not have enough balance to transfer {Utility.FormatAmount(internalTransfer.TransferAmount)}", false);
                return;
            }
            //check the minimum kept amount
            if ((selectedAccount.AccountBalance - internalTransfer.TransferAmount) < minimumKeptAmount)
            {
                Utility.PrintMessage($"Transfer failed. Your account needs to have minimum {Utility.FormatAmount(minimumKeptAmount)}", false);
                return;
            }
            //check receiver's account number is valid
            var selectedBankAccountReceiver = (from userAcc in UserAccountList
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

            //add transaction to transactions record - sender
            InsertTransaction(selectedAccount.ID, TransactionType.Transfer, internalTransfer.TransferAmount, $"Transfered to {selectedBankAccountReceiver.AccountNumber} ({selectedBankAccountReceiver.FullName})");

            //update sender's account balance 
            selectedAccount.AccountBalance -= internalTransfer.TransferAmount;

            //add transaction record - receiver
            InsertTransaction(selectedBankAccountReceiver.ID, TransactionType.Transfer, internalTransfer.TransferAmount, $"Transfered from {selectedAccount.AccountNumber}({selectedAccount.FullName})");

            //update reciever's account balance 
            selectedBankAccountReceiver.AccountBalance += internalTransfer.TransferAmount;

            //success message
            Utility.PrintMessage($"You have successfully transfered {Utility.FormatAmount(internalTransfer.TransferAmount)} to {internalTransfer.RecipientBankAccountName}", true);
        }

    }
}
