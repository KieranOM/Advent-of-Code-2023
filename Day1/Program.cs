string[] numberWords = { "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };

var lines = File.ReadLines("res/input.txt");
int sum = 0;

foreach (string line in lines)
    if (TryParseNumber(line, out int number))
        sum += number;

Console.WriteLine(sum);
return;

bool TryParseNumber(in string line, out int number)
{
    if (TryParseDigits(line, out var digits))
    {
        int first = digits[0];
        int last = digits[^1];

        number = 10 * first + last;
        return true;
    }

    number = default;
    return false;
}

bool TryParseDigits(in string line, out IList<int> digits)
{
    digits = new List<int>(8);
    for (int i = 0; i < line.Length; ++i)
        if (TryParseDigit(line, i, out int digit))
            digits.Add(digit);

    return digits.Count > 0;
}

bool TryParseDigit(in string line, in int index, out int digit)
{
    return TryParseDigitAsCharacter(line[index], out digit)
           || TryParseDigitAsWord(line, index, out digit);
}

bool TryParseDigitAsCharacter(in char character, out int digit)
{
    const char zero = '0';
    if (char.IsDigit(character))
    {
        digit = character - zero;
        return true;
    }

    digit = default;
    return false;
}

bool TryParseDigitAsWord(in string line, in int index, out int digit)
{
    var remainder = line.AsSpan(index);
    for (int i = 0; i < numberWords.Length; ++i)
    {
        string numberWord = numberWords[i];

        if (!remainder.StartsWith(numberWord))
            continue;

        digit = i + 1;
        return true;
    }

    digit = default;
    return false;
}