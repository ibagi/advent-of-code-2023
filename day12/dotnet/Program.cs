using System.Buffers;
using System.Collections.Immutable;
using System.Data;

var records = File.ReadLines("input.txt")
    .Select(ParseRecord)
    .ToArray();

Console.WriteLine(PartOne(records));
Console.WriteLine(PartTwo(records));

static long PartOne(IEnumerable<ConditionRecord> records)
{
    var cache = new SolutionCache();
    return records.Sum(x => GetSolutions(x.Springs,
        ImmutableStack.CreateRange(x.GroupConstraints.Reverse()), cache));
}

static long PartTwo(IEnumerable<ConditionRecord> records)
{
    var cache = new SolutionCache();
    var unfoldedRecords = records.Select(x => x.UnFold()).ToArray();

    return unfoldedRecords.Sum(x => GetSolutions(x.Springs,
        ImmutableStack.CreateRange(x.GroupConstraints.Reverse()), cache));
}

static long GetSolutions(string springs, ImmutableStack<int> groups, SolutionCache cache)
{
    var key = new CacheKey(springs, groups);
    if (!cache.ContainsKey(key))
    {
        cache[key] = ProcessNextChar(springs, groups, cache);
    }

    return cache[key];
}

static long ProcessNextChar(string springs, ImmutableStack<int> groups, SolutionCache cache) => springs.FirstOrDefault() switch
{
    '.' => GetSolutions(springs[1..], groups, cache),
    '#' => HandleHashChar(springs, groups, cache),
    '?' => 
        GetSolutions('.' + springs[1..], groups, cache) + 
        GetSolutions('#' + springs[1..], groups, cache),
    _ => groups.Any() ? 0 : 1
};

static long HandleHashChar(string springs, ImmutableStack<int> groups, SolutionCache cache)
{
    if (!groups.Any())
    {
        return 0;
    }

    var n = groups.Peek();
    groups = groups.Pop();

    var possibleGroupChars = springs.TakeWhile(s => s == '#' || s == '?').Count();

    if (possibleGroupChars < n)
    {
        return 0;
    }

    if (springs.Length == n)
    {
        return GetSolutions("", groups, cache);
    }

    if (springs[n] == '#')
    {
        return 0;
    }

    return GetSolutions(springs[(n + 1)..], groups, cache);
}

static ConditionRecord ParseRecord(string line)
{
    var parts = line.Split(' ', 2);
    var groupConstraints = parts[1].Split(',')
        .Select(int.Parse)
        .ToArray();

    return new ConditionRecord(parts[0], groupConstraints);
}

record CacheKey(string Springs, IEnumerable<int> Groups)
{
    public override string ToString() => 
        $"{Springs}:{string.Join(',', Groups)}";
}

class SolutionCache
{
    private Dictionary<string, long> _cache = new();

    public long this[CacheKey key]
    {
        get => _cache[key.ToString()];
        set => _cache[key.ToString()] = value;
    }

    public bool ContainsKey(CacheKey key) =>
        _cache.ContainsKey(key.ToString());
}

record ConditionRecord(string Springs, int[] GroupConstraints)
{
    public ConditionRecord UnFold()
    {
        var springs = string.Join('?', Enumerable.Range(0, 5)
            .Select(_ => Springs));

        var groupConstraints = Enumerable.Range(0, 5)
            .Select(_ => GroupConstraints)
            .Aggregate(Enumerable.Empty<int>(), (acc, constraints) => acc.Concat(constraints))
            .ToArray();

        return new ConditionRecord(springs, groupConstraints);
    }
}
