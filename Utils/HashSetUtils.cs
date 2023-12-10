namespace Utils;

public static class HashSetUtils
{
    public static HashSet<T> Intersect<T>(this HashSet<T> left, HashSet<T> right)
    {
        var (smallest, biggest) = left.Count < right.Count
            ? (left, right)
            : (right, left);

        var intersect = new HashSet<T>(smallest);
        intersect.IntersectWith(biggest);

        return intersect;
    }
}