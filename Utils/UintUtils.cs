namespace Utils;

public static class UintUtils
{
    public static bool TryParseSearch(in ReadOnlySpan<char> span, out uint value)
    {
        return TryParseSearch(span, 0, out value);
    }
    
    public static bool TryParseSearch(in ReadOnlySpan<char> span, in int index, out uint value)
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

        var numericSpan = span.Slice(index, length);
        return uint.TryParse(numericSpan, out value);
    }
}