using System;

class LargeNumberFormat
{
    public static string FormatTwoDecimal(long number)
    {
        return Format(number, "{0:0.00}");
    }

    public static string FormatNoDecimal(long number)
    {
        return Format(number, "{0:N0}");
    }

    public static string Format(long number, string format)
    {
        if (number < 1000) return number.ToString();
        if (number < 1_000_000) return String.Format(format, number / 1000f) + "K";
        if (number < 1_000_000_000) return String.Format(format, number / 1_000_000f) + "M";
        if (number < 1_000_000_000_000) return String.Format(format, number / 1_000_000_000f) + "B";
        return String.Format(format, number / 1_000_000_000_000f) + "T";
    }
}