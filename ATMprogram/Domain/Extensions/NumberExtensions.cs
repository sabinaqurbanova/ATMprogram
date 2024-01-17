namespace ATMprogram.Domain.Extensions
{
    public static class NumberExtensions
    {
        public static decimal ReverseSign(this decimal value)
        {
            if (value >= 0)
                return Math.Abs(value);
            else
                return -value;
        }
    }
}