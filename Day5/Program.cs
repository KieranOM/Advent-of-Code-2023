using Utils;

string[] lines = File.ReadAllLines("res/input.txt");

var seeds = new List<uint>();

List<Lookup> lookupChain = new List<Lookup>(7)
{
    new("seed-to-soil"),
    new("soil-to-fertilizer"),
    new("fertilizer-to-water"),
    new("water-to-light"),
    new("light-to-temperature"),
    new("temperature-to-humidity"),
    new("humidity-to-location")
};

UintUtils.TryParseContinuousUints(lines[0].AsSpan().SliceFromIndexOf(':'), ' ', seeds);

ParseLookups(lines, lookupChain);

ulong lowestLocation = ulong.MaxValue;
foreach (var seed in seeds)
{
    lowestLocation = Math.Min(EvaluateLookupChain(seed, lookupChain), lowestLocation);
}

Console.WriteLine(lowestLocation);

lowestLocation = ulong.MaxValue;

var tasks = new List<Task<ulong>>();

for (int i = 0; i < seeds.Count - 1; i += 2)
{
    uint start = seeds[i],
        length = seeds[i + 1];

    tasks.Add(Task.Run(CalculateLowestLocationForLocalRange));
    continue;

    ulong CalculateLowestLocationForLocalRange()
    {
        ulong rangeLowestLocation = ulong.MaxValue;
        for (uint j = 0; j < length; ++j)
        {
            rangeLowestLocation = Math.Min(EvaluateLookupChain(start + j, lookupChain), rangeLowestLocation);
        }

        return rangeLowestLocation;
    }
}

await Task.WhenAll(tasks);

foreach (var task in tasks)
{
    lowestLocation = Math.Min(task.Result, lowestLocation);
}

Console.WriteLine(lowestLocation);

return;

ulong EvaluateLookupChain(in ulong value, in List<Lookup> lookups)
{
    ulong search = value;
    ulong result = 0;

    foreach (var lookup in lookups)
    {
        result = lookup.Get(search);
        search = result;
    }

    return result;
}

void ParseLookups(string[] lines, List<Lookup> lookups)
{
    const int mapsLinesStart = 2;
    Lookup parsing = null;

    for (int i = mapsLinesStart; i < lines.Length; ++i)
    {
        ReadOnlySpan<char> line = lines[i];
        if (line.IsWhiteSpace())
        {
            parsing = null;
            continue;
        }

        char firstChar = line[0];
        if (char.IsDigit(firstChar))
        {
            ParseRangeMapping(line, out uint destinationStart, out uint sourceStart, out uint length);
            parsing.AddMapping(destinationStart, sourceStart, length);
        }
        else
        {
            parsing = FindLookupFromLine(line, lookups);
        }
    }
}

Lookup FindLookupFromLine(in ReadOnlySpan<char> line, IEnumerable<Lookup> lookups)
{
    foreach (var lookup in lookups)
    {
        if (!line.StartsWith(lookup.Name))
            continue;

        return lookup;
    }

    return null;
}

void ParseRangeMapping(in ReadOnlySpan<char> line, out uint destinationStart, out uint sourceStart, out uint length)
{
    var mappings = new List<uint>(3);
    UintUtils.TryParseContinuousUints(line, ' ', mappings);

    (destinationStart, sourceStart, length) = (mappings[0], mappings[1], mappings[2]);
}

internal class RangeMap
{
    private readonly ulong _sourceStart, _sourceEnd, _destinationStart;

    public RangeMap(in ulong destinationStart, in ulong sourceStart, in ulong length)
    {
        _destinationStart = destinationStart;
        _sourceStart = sourceStart;
        _sourceEnd = sourceStart + length;
    }

    public bool TryGetValue(in ulong sourceValue, out ulong destinationValue)
    {
        if (IsInRange(sourceValue))
        {
            destinationValue = sourceValue - _sourceStart + _destinationStart;
            return true;
        }

        destinationValue = default;
        return false;
    }

    private bool IsInRange(in ulong sourceValue)
    {
        return sourceValue >= _sourceStart && sourceValue <= _sourceEnd;
    }
}

internal class Lookup
{
    private readonly List<RangeMap> _mappings = new();
    public string Name { get; private set; }

    public Lookup(string name)
    {
        Name = name;
    }

    public void AddMapping(in uint destinationStart, in uint sourceStart, in uint length)
    {
        var mapping = new RangeMap(destinationStart, sourceStart, length);
        _mappings.Add(mapping);
    }

    public ulong Get(in ulong source)
    {
        foreach (var mapping in _mappings)
        {
            if (mapping.TryGetValue(source, out ulong destination))
            {
                return destination;
            }
        }

        return source;
    }
}