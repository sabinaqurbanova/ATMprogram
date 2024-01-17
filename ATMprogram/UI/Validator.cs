using System.ComponentModel;

namespace ATMprogram.UI
{
    public static class Validator
    {
        /// <summary>
        /// Tip cevrilmesi reallawdirir.
        /// </summary>
        /// <typeparam name="T">Hansi tipe cevrilsin?</typeparam>
        /// <param name="prompt">Cevrilecek data.</param>
        /// <returns>Geriye cevrilmiw datani dondurur.</returns>
        public static T Convert<T>(string prompt)
        {
            bool isConvertationValid = false; // Konvertasiya emeliyyati hazirda(while-ib hazirki dovrunde) validdirmi, yeni ugurla yekunlawibmi?
            string userInput;

            while (!isConvertationValid)
            {
                userInput = Utility.GetUserInput(prompt);

                try
                {
                    // T ucun tip cevirici elde edirik:
                    TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));

                    // 
                    if (converter != null)
                    {
                        return (T)converter.ConvertFromString(userInput);
                    }
                    else
                    {
                        return default;
                    }
                }
                catch (Exception error)
                {
                    Utility.PrintMessage($"Invalid input. Error message: \"{error.Message}\". Try again.", false);
                }
            }

            return default;
        }
    }
}