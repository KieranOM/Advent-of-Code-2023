using Utils;

var lines = File.ReadLines("res/input.txt");
uint scoreSum = 0;

var cards = new List<Card>();

foreach (string line in lines)
    if (Card.TryParse(line, out var card))
    {
        cards.Add(card);
        scoreSum += card.Score;
    }

Console.WriteLine(scoreSum);

uint[] cardInstances = ArrayUtils.CreateWithDefaultValue(cards.Count, 1u);
uint totalCardInstances = 0;

for (int i = 0; i < cardInstances.Length; ++i)
{
    var card = cards[i];
    var copies = GetWonCopies(card, cards);
    uint instances = cardInstances[card.Id - 1];

    foreach (var copy in copies)
        cardInstances[copy.Id - 1] += instances;

    totalCardInstances += cardInstances[i];
}

Console.WriteLine(totalCardInstances);
return;

List<Card> GetWonCopies(in Card card, in List<Card> cards)
{
    var wonCopies = new List<Card>();

    int startIndex = (int)card.Id;
    int endIndex = startIndex + card.Matches.Count;

    for (int i = startIndex; i < endIndex && i < cards.Count; ++i)
        wonCopies.Add(cards[i]);

    return wonCopies;
}

internal readonly record struct Card(in uint Id, in IReadOnlySet<uint> Matches, in uint Score)
{
    private static bool TryReadUints(in ReadOnlySpan<char> span, out HashSet<uint> uints)
    {
        return UintUtils.TryParseContinuousUints(span, ' ', out uints);
    }

    private static uint CalculateScore(in HashSet<uint> matches)
    {
        int numberOfMatches = matches.Count;

        if (numberOfMatches == 0)
            return 0;

        return 1u << (numberOfMatches - 1);
    }

    public static bool TryParse(in ReadOnlySpan<char> span, out Card card)
    {
        const string prefix = "Card";

        var prefixlessSpan = span.Slice(prefix.Length).Trim();

        if (!UintUtils.TryParseSearch(prefixlessSpan, out uint id))
        {
            card = default;
            return false;
        }

        var remainder = prefixlessSpan.SliceFromIndexOf(':').Trim();
        var ranges = remainder.Split('|');

        var winningSpan = remainder.Slice(ranges[0]);
        var playedSpan = remainder.Slice(ranges[1]);

        if (TryReadUints(winningSpan, out var winningSet) && TryReadUints(playedSpan, out var playedSet))
        {
            var matches = winningSet.Intersect(playedSet);
            uint score = CalculateScore(matches);
            card = new Card(id, matches, score);
            return true;
        }

        card = default;
        return false;
    }
}