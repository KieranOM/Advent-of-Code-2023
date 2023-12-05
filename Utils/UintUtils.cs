namespace Utils;

public static class UintUtils
{
    public static bool TryParseSearch(in ReadOnlySpan<char> span, out uint value)
    {
        return TryParseSearch(span, 0, out value);
    }
    
    public static bool TryParseSearch(in ReadOnlySpan<char> span, in int index, out uint value)
    {
        return TryParseSearch(span, index, out value, out _);
    }

    public static bool TryParseSearch(in ReadOnlySpan<char> span, in int index, out uint value,
        out ReadOnlySpan<char> parsedSpan)
    {
        int length = 0;

        for (int i = index; i < span.Length; ++i)
        {
            char character = span[i];
            if (char.IsDigit(character))
                ++length;
            else
                break;
        }

        parsedSpan = span.Slice(index, length);
        return uint.TryParse(parsedSpan, out value);
    }
}