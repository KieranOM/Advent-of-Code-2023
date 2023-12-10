using System.Runtime.InteropServices;

namespace Utils;

public static class ReadOnlySpanUtils
{
    public static IList<Range> Split<T>(in this ReadOnlySpan<T> toSplit, in T separator)
        where T : unmanaged, IEquatable<T>?
    {
        var localSeparator = separator;
        var separatorSpan = MemoryMarshal.CreateReadOnlySpan(ref localSeparator, 1);
        return Split(toSplit, separatorSpan);
    }

    public static IList<Range> Split<T>(in this ReadOnlySpan<T> toSplit, in ReadOnlySpan<T> separator)
        where T : IEquatable<T>?
    {
        var ranges = new List<Range>();

        int currentSplitStartIndex = 0;

        for (int i = 0; i < toSplit.Length; ++i)
        {
            if (!toSplit.IndexStartsWith(i, separator))
                continue;

            var splitRange = RangeUtils.FromIndices(currentSplitStartIndex, i);
            ranges.Add(splitRange);

            currentSplitStartIndex = i + separator.Length;
        }

        var finalSplitRange = RangeUtils.FromIndices(currentSplitStartIndex, toSplit.Length);
        ranges.Add(finalSplitRange);

        return ranges;
    }

    public static ReadOnlySpan<T> Slice<T>(in this ReadOnlySpan<T> span, in Range range)
    {
        int startIndex = range.Start.Value;
        int length = range.End.Value - startIndex;
        return span.Slice(startIndex, length);
    }

    public static bool IndexStartsWith<T>(in this ReadOnlySpan<T> span, in int index, in ReadOnlySpan<T> value)
        where T : IEquatable<T>?
    {
        return span.Slice(index).StartsWith(value);
    }

    public static ReadOnlySpan<char> SliceFromIndexOf(in this ReadOnlySpan<char> span, in char value)
    {
        return span.Slice(span.IndexOf(value) + 1);
    }
}