using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ATMprogram.UI;

namespace ATMprogram.App
{
    public class Entry
    {
        static void Main(string[] args)
        {
            ATMprogram atmApp = new ATMprogram();
            atmApp.InitializeData();
            atmApp.Run();
            AppScreen.DisplayAppMenu();
        }
    }
}
