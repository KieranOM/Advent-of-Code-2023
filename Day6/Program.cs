using Utils;

string[] lines = File.ReadAllLines("res/input.txt");

if (!TryParseRaces(lines, out var races))
    return;

ulong marginOfError = CalculateMarginOfError(races);
Console.WriteLine(marginOfError);

if (!TryParseLongRace(lines, out var race))
    return;

ulong numberOfWaysToWin = Race.CalculateNumberOfWaysToWin(race);
Console.WriteLine(numberOfWaysToWin);

return;

ulong CalculateMarginOfError(in IList<Race> races)
{
    ulong result = Race.CalculateNumberOfWaysToWin(races[0]);
    for (int i = 1; i < races.Count; ++i)
        result *= Race.CalculateNumberOfWaysToWin(races[i]);

    return result;
}

bool TryParseRaces(in string[] lines, out List<Race> races)
{
    if (lines is { Length: 2 }
        && TryParseNumbers(lines[0], out var times)
        && TryParseNumbers(lines[1], out var distances)
        && times.Count == distances.Count)
    {
        races = new List<Race>(times.Count);
        for (int i = 0; i < times.Count; ++i)
            races.Add(new Race(times[i], distances[i]));

        return true;
    }

    races = null;
    return false;
}

bool TryParseNumbers(in ReadOnlySpan<char> line, out List<uint> numbers)
{
    return UintUtils.TryParseContinuousUints(line.SliceFromIndexOf(':'), ' ', out numbers);
}

bool TryParseLongRace(in string[] lines, out Race race)
{
    if (lines is { Length: 2 }
        && TryParseLongNumber(lines[0], out ulong time)
        && TryParseLongNumber(lines[1], out ulong distance))
    {
        race = new Race(time, distance);
        return true;
    }

    race = default;
    return false;
}

bool TryParseLongNumber(in ReadOnlySpan<char> line, out ulong number)
{
    return UlongUtils.TryParseIgnoringWhiteSpace(line.SliceFromIndexOf(':'), out number);
}

internal readonly record struct Race(in ulong Time, in ulong Distance)
{
    public static ulong CalculateNumberOfWaysToWin(in Race race)
    {
        // C charge, T time, D distance
        // c(t-c) > d
        // ct - c^2 > d
        // -c^2 + tc - d > 0
        // Creates a downward opening parabola

        (double minima, double maxima) = MathUtils.QuadraticFormula(-1d, race.Time, 0d - race.Distance).MinMax();

        long first = (long)Math.Ceiling(minima),
            last = (long)Math.Floor(maxima);

        // Create limits such that the range calculation will give 0 for only negative intercepts
        first = Math.Max(0, first);
        last = Math.Max(-1, last);

        return (ulong)(last - first + 1);
    }
}