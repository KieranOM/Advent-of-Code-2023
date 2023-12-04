using Utils;

var lines = File.ReadLines("res/input.txt");

var maxCubes = new Set(12, 13, 14);
uint idSumWithinMaxCubes = 0, minimumSetPowerSum = 0;

foreach (string line in lines)
{
    if (TryParseGame(line, out var game))
    {
        if (Game.IsWithinMaxSet(game, maxCubes))
            idSumWithinMaxCubes += game.Id;

        var minimumSet = Game.CalculateMinimumSet(game);
        minimumSetPowerSum += minimumSet.Power;
    }
}

Console.WriteLine(idSumWithinMaxCubes);
Console.WriteLine(minimumSetPowerSum);

return;

bool TryParseGame(in ReadOnlySpan<char> line, out Game game)
{
    const char separator = ':';

    if (TryParseGameId(line, out uint id))
    {
        int index = line.IndexOf(separator);
        if (index != -1)
        {
            var setsSpan = line[(index + 1)..];

            if (TryParseSets(setsSpan, out var sets))
            {
                game = new Game(id, sets);
                return true;
            }
        }
    }

    game = default;
    return false;
}

bool TryParseGameId(in ReadOnlySpan<char> line, out uint id)
{
    const string prefix = "Game ";

    int start = prefix.Length;
    return UintUtils.TryParseSearch(line, start, out id);
}

bool TryParseSets(in ReadOnlySpan<char> setsSpan, out List<Set> sets)
{
    var toParse = setsSpan.Trim();

    sets = new List<Set>();

    var setRanges = toParse.Split(';');
    foreach (var range in setRanges)
    {
        var setSpan = toParse.Slice(range).Trim();
        if (TryParseSet(setSpan, out var set))
            sets.Add(set);
        else
            return false;
    }

    return true;
}

bool TryParseSet(in ReadOnlySpan<char> setSpan, out Set set)
{
    uint red = 0, green = 0, blue = 0;

    var cubeRanges = setSpan.Split(',');
    foreach (var range in cubeRanges)
    {
        var cubeSpan = setSpan.Slice(range).Trim();

        if (TryParseCubeAndIncrementTotal(cubeSpan, "red", ref red)
            || TryParseCubeAndIncrementTotal(cubeSpan, "green", ref green)
            || TryParseCubeAndIncrementTotal(cubeSpan, "blue", ref blue))
            continue;

        set = default;
        return false;
    }

    set = new Set(red, green, blue);
    return true;
}

bool TryParseCubeAndIncrementTotal(in ReadOnlySpan<char> cubeSpan, in ReadOnlySpan<char> colour, ref uint total)
{
    if (!cubeSpan.EndsWith(colour) || !UintUtils.TryParseSearch(cubeSpan, out uint quantity))
        return false;

    total += quantity;
    return true;
}

internal readonly struct Game
{
    public readonly uint Id;
    private readonly List<Set> _sets;

    public Game(in uint id, in List<Set> sets)
    {
        Id = id;
        _sets = sets;
    }

    public static bool IsWithinMaxSet(in Game game, in Set max)
    {
        foreach (var set in game._sets)
            if (!Set.LessThanOrEqualTo(set, max))
                return false;

        return true;
    }

    public static Set CalculateMinimumSet(in Game game)
    {
        var minimumSet = Set.Zero;
        foreach (var set in game._sets)
            minimumSet = Set.MaxComponents(minimumSet, set);

        return minimumSet;
    }
}

internal readonly struct Set
{
    public static readonly Set Zero = new(0, 0, 0);

    private readonly uint _red, _green, _blue;

    public uint Power => _red * _green * _blue;

    public Set(uint red, uint green, uint blue)
    {
        _red = red;
        _green = green;
        _blue = blue;
    }

    public static bool LessThanOrEqualTo(in Set left, in Set right)
    {
        return left._red <= right._red
               && left._green <= right._green
               && left._blue <= right._blue;
    }

    public static Set MaxComponents(in Set left, in Set right)
    {
        return new Set(
            Math.Max(left._red, right._red),
            Math.Max(left._green, right._green),
            Math.Max(left._blue, right._blue)
        );
    }
}