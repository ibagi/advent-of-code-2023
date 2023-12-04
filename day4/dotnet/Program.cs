using System.Text.RegularExpressions;

Console.WriteLine($"PartOne: {PartOne()}");
Console.WriteLine($"PartTwo: {PartTwo()}");

static double PartOne() => File.ReadAllLines("../input.txt")
    .Select(ScratchCard.Parse)
    .Sum(x => x.Score());

static int PartTwo()
{
    var scratchCards = File.ReadAllLines("../input.txt")
        .Select(ScratchCard.Parse)
        .ToDictionary(x => x.Idx);

    return scratchCards.Values.Sum(x => x.GetWinningCopiesCount(scratchCards));
}

record ScratchCard(int Idx, string[] WinningNumbers, string[] MyNumbers)
{
    private const string NumberPattern = @"\d+";
    private static Dictionary<int, int> _partTwoCache = new();

    public static ScratchCard Parse(string input)
    {
        var data = input.Split(':');
        var idx = int.Parse(Regex.Match(data[0], NumberPattern).Value);
        var numbers = data[1].Split('|');

        return new ScratchCard(idx,
            Regex.Matches(numbers[0], NumberPattern).Select(x => x.Value).ToArray(),
            Regex.Matches(numbers[1], NumberPattern).Select(x => x.Value).ToArray());
    }

    private Lazy<int> _matches =>
        new(() => WinningNumbers.Intersect(MyNumbers).Count());

    public int Matches => _matches.Value;

    public double Score() => Matches switch
    {
        0 => 0,
        1 => 1,
        _ => Math.Pow(2, Matches - 1)
    };

    public int GetWinningCopiesCount(Dictionary<int, ScratchCard> cards)
    {
        if (_partTwoCache.ContainsKey(Idx))
        {
            return _partTwoCache[Idx];
        }

        var result = 1;

        for (var i = 1; i < Matches + 1; i++)
        {
            if (cards.ContainsKey(Idx + i))
            {
                result += cards[Idx + i].GetWinningCopiesCount(cards);
            }
        }

        _partTwoCache[Idx] = result;
        return result;
    }
}
