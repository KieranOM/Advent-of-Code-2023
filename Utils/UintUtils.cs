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

    public static bool TryParseContinuousUints<T>(in ReadOnlySpan<char> span, in char separator, out T collection,
        in int startIndex = 0)
        where T : ICollection<uint>, new()
    {
        collection = new T();
        return TryParseContinuousUints(span, separator, collection, startIndex);
    }

    public static bool TryParseContinuousUints(in ReadOnlySpan<char> span, in char separator,
        in ICollection<uint> collection, in int startIndex = 0)
    {
        int i = startIndex;
        while (i < span.Length)
        {
            if (span[i] == separator)
            {
                ++i;
                continue;
            }

            if (!TryParseSearch(span, i, out uint value, out var parsed))
                return false;

            collection.Add(value);
            i += parsed.Length;
        }

        return true;
    }
}