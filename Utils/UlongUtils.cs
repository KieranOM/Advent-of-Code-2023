namespace Utils;

public static class UlongUtils
{
    public static bool TryParseIgnoringWhiteSpace(in ReadOnlySpan<char> span, out ulong value)
    {
        const char zero = '0';

        var trimmed = span.Trim();

        ulong result = 0;
        bool foundFirstDigit = false;

        for (int i = 0; i < trimmed.Length; ++i)
        {
            char character = trimmed[i];
            if (char.IsWhiteSpace(character))
                continue;

            if (!char.IsDigit(character))
            {
                value = result;
                return foundFirstDigit;
            }

            ulong number = (ulong)(character - zero);

            if (!foundFirstDigit && number == 0)
            {
                continue;
            }

            result = 10 * result + number;
            foundFirstDigit = true;
        }

        value = result;
        return foundFirstDigit;
    }
}