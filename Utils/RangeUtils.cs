namespace Utils;

public static class RangeUtils
{
    public static Range FromIndices(in int startIndex, in int endIndex)
    {
        return new Range(
            Index.FromStart(startIndex),
            Index.FromEnd(endIndex)
        );
    }
}