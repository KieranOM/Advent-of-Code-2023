using System.Numerics;

namespace Utils;

public static class MathUtils
{
    public static (T min, T max) MinMax<T>(in T left, in T right) where T : IComparisonOperators<T, T, bool>
    {
        return left < right
            ? (left, right)
            : (right, left);
    }

    public static (T min, T max) MinMax<T>(in this (T left, T right) values) where T : IComparisonOperators<T, T, bool>
    {
        return MinMax(values.left, values.right);
    }

    public static (double plus, double minus) QuadraticFormula(in double a, in double b, in double c)
    {
        // x = (-b ± sqrt(b^2 - 4ac)) / 2a
        double minusB = -b;
        double sqrt = Math.Sqrt(b * b - 4 * a * c);
        double overDenominator = 1 / (2 * a);

        double plus = (minusB + sqrt) * overDenominator;
        double minus = (minusB - sqrt) * overDenominator;

        return (plus, minus);
    }
}