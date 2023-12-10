namespace Utils;

public static class ArrayUtils
{
    public static T[] CreateWithDefaultValue<T>(in int size, in T defaultValue)
    {
        var array = new T[size];
        for (int i = 0; i < size; ++i)
            array[i] = defaultValue;
        return array;
    }
}