using ATMprogram.UI;

namespace ATMprogram.App
{
    public class Entry
    {
        static void Main()
        {
            ATMprogram atmApp = new ATMprogram();
            atmApp.InitializeData();
            atmApp.Run();

            AppScreen.DisplayAppMenu();
        }
    }
}