Console.WriteLine(PartOne());
Console.WriteLine(PartTwo());

static int PartOne()
{
    var hands = File.ReadLines("../input.txt")
        .Select(x => x.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        .Select(x => new Hand(x[0], int.Parse(x[1]), h => h.Rank))
        .ToArray();

    return hands.OrderBy(x => x).Select((hand, idx) => hand.Bid * (idx + 1)).Aggregate((a, b) => a + b);
}

static int PartTwo()
{
    var hands = File.ReadLines("../input.txt")
        .Select(x => x.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        .Select(x => new Hand(x[0].Replace('J', '*'), int.Parse(x[1]), h => GetHandRankWithJokerRule(h)))
        .ToArray();

    return hands.OrderBy(x => x).Select((hand, idx) => hand.Bid * (idx + 1)).Aggregate((a, b) => a + b);
}

static Rank GetHandRankWithJokerRule(Hand hand)
{
    var jokers = hand.Cards.Select((c, i) => (c, i))
        .Where(x => x.c == '*')
        .Select(x => x.i)
        .ToArray();

    if (jokers.Length >= 4)
    {
        return Rank.FiveOKind;
    }

    if (jokers.Length == 0)
    {
        return hand.Rank;
    }

    var replacements = new[] { '2', '3', '4', '5', '6', '7', '8', '9', 'T', 'J', 'Q', 'K', 'A' };

    var possibilities = CartesianProduct(jokers.Select(_ => replacements))
        .Select(x => x
            .Select((c, i) => (c, i))
            .ToDictionary(x => jokers[x.i], x => x.c));

    return possibilities
       .Select(x => hand with
       {
           Cards = new string(hand.Cards.Select((c, i) => x.ContainsKey(i) ? x[i] : c).ToArray())
       })
       .Max(x => x.Rank);
}

static IEnumerable<IEnumerable<T>> CartesianProduct<T>(IEnumerable<IEnumerable<T>> sets)
{
    var temp = new List<List<T>> { new() };

    for (var i = 0; i < sets.Count(); i++)
    {
        var newTemp = new List<List<T>>();

        foreach (var item in temp)
        {
            foreach (var element in sets.ElementAt(i))
            {
                var tempCopy = new List<T>(item) { element };
                newTemp.Add(tempCopy);
            }
        }

        temp = newTemp;
    }

    foreach (var item in temp)
    {
        yield return item;
    }
}

enum Rank
{
    HighCard = 1,
    Pair,
    TwoPair,
    ThreeOKind,
    FullHouse,
    FourOKind,
    FiveOKind
}

record Hand(string Cards, int Bid, Func<Hand, Rank> RankCalculator) : IComparable<Hand>
{
    private static readonly List<char> CardRanks = new() { '*', '2', '3', '4', '5', '6', '7', '8', '9', 'T', 'J', 'Q', 'K', 'A' };

    public Rank Rank
    {
        get
        {
            var nums = Cards.GroupBy(x => x).Select(x => x.Count()).OrderByDescending(x => x).ToArray();
            return nums switch
            {
            [5] => Rank.FiveOKind,
            [4, ..] => Rank.FourOKind,
            [3, 2, ..] => Rank.FullHouse,
            [3, ..] => Rank.ThreeOKind,
            [2, 2, ..] => Rank.TwoPair,
            [2, ..] => Rank.Pair,
                _ => Rank.HighCard
            };
        }
    }

    public int CompareTo(Hand? other)
    {
        var rankA = RankCalculator(this);
        var rankB = RankCalculator(other!);

        if (rankA > rankB)
        {
            return 1;
        }

        if (rankB > rankA)
        {
            return -1;
        }

        foreach (var (c1, c2) in Cards.Zip(other!.Cards, (c1, c2) => (c1, c2)))
        {
            if (CardRanks.IndexOf(c1) > CardRanks.IndexOf(c2))
            {
                return 1;
            }

            if (CardRanks.IndexOf(c2) > CardRanks.IndexOf(c1))
            {
                return -1;
            }
        }

        return 0;
    }
}