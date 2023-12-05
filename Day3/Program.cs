using Utils;

string[] lines = File.ReadAllLines("res/input.txt");
ParseSymbolsAndNumbers(lines, out var numbers, out var symbols);

var partNumbers = FindPartNumbers(numbers, symbols);

uint partNumberSum = 0;
foreach (var partNumber in partNumbers)
    partNumberSum += partNumber.Value;

Console.WriteLine(partNumberSum);

var gears = FindGears(numbers, symbols);

uint gearRatioSum = 0;
foreach (var gear in gears)
    gearRatioSum += gear.Ratio;

Console.WriteLine(gearRatioSum);

return;

void ParseSymbolsAndNumbers(in string[] lines, out IList<Number> numbers, out IList<Symbol> symbols)
{
    numbers = new List<Number>();
    symbols = new List<Symbol>();

    int height = lines.Length;
    int width = lines[0].Length;

    for (int y = 0; y < height; ++y)
    {
        var line = lines[y].AsSpan();
        for (int x = 0; x < width; ++x)
            if (Number.TryParse(line, x, y, out var number))
            {
                numbers.Add(number);
                x = number.End.X;
            }
            else if (Symbol.TryParse(line, x, y, out var symbol))
            {
                symbols.Add(symbol);
            }
    }
}

bool IsNumberAdjacentToAnySymbol(in Number number, in IList<Symbol> symbols)
{
    foreach (var symbol in symbols)
        if (IsNumberAdjacentToSymbol(number, symbol))
            return true;

    return false;
}

bool IsNumberAdjacentToSymbol(in Number number, in Symbol symbol)
{
    const int adjacencyThreshold = 1;

    int y = number.Start.Y;

    int minX = number.Start.X - adjacencyThreshold,
        maxX = number.End.X + adjacencyThreshold,
        minY = y - adjacencyThreshold,
        maxY = y + adjacencyThreshold;

    int symbolX = symbol.X, symbolY = symbol.Y;

    return symbolX >= minX && symbolX <= maxX && symbolY >= minY && symbolY <= maxY;
}

IList<Number> FindPartNumbers(in IList<Number> numbers, in IList<Symbol> symbols)
{
    var partNumbers = new List<Number>(numbers.Count);
    foreach (var number in numbers)
        if (IsNumberAdjacentToAnySymbol(number, symbols))
            partNumbers.Add(number);
    return partNumbers;
}

bool TryGetTwoAdjacentNumbers(in Symbol symbol, in IList<Number> numbers, out Number left, out Number right)
{
    const int adjacencyTarget = 2;

    int count = 0;
    Span<Number> adjacentNumbers = stackalloc Number[2];

    foreach (var number in numbers)
    {
        if (!IsNumberAdjacentToSymbol(number, symbol))
            continue;

        if (count == adjacencyTarget)
            break;

        adjacentNumbers[count++] = number;
    }

    left = adjacentNumbers[0];
    right = adjacentNumbers[1];
    return count == adjacencyTarget;
}

IList<Gear> FindGears(in IList<Number> numbers, in IList<Symbol> symbols)
{
    var gears = new List<Gear>(symbols.Count);
    foreach (var symbol in symbols)
    {
        if (!symbol.HasGearValue)
            continue;

        if (TryGetTwoAdjacentNumbers(symbol, numbers, out var left, out var right))
            gears.Add(new Gear(symbol, left, right));
    }

    return gears;
}

internal readonly record struct Number(in uint Value, in Vector2I Start, in Vector2I End)
{
    public static bool IsValidChar(in char character)
    {
        return char.IsDigit(character);
    }

    public static bool TryParse(in ReadOnlySpan<char> line, in int x, in int y, out Number number)
    {
        if (UintUtils.TryParseSearch(line, x, out uint value, out var span))
        {
            var start = new Vector2I(x, y);
            var end = new Vector2I(x + span.Length - 1, y);

            number = new Number(value, start, end);
            return true;
        }

        number = default;
        return false;
    }
}

internal readonly record struct Symbol(in char Value, in Vector2I Position)
{
    private const char GearValue = '*';

    public int X => Position.X;
    public int Y => Position.Y;
    public bool HasGearValue => Value == GearValue;

    public static bool IsValidChar(in char character)
    {
        const char period = '.';
        return character != period && !Number.IsValidChar(character);
    }

    public static bool TryParse(in ReadOnlySpan<char> line, in int x, in int y, out Symbol symbol)
    {
        char character = line[x];
        if (IsValidChar(character))
        {
            symbol = new Symbol(character, new Vector2I(x, y));
            return true;
        }

        symbol = default;
        return false;
    }
}

internal readonly record struct Gear(in Symbol symbol, in Number left, in Number right)
{
    public uint Ratio => left.Value * right.Value;
}