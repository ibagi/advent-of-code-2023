Console.WriteLine(PartOne());
Console.WriteLine(PartTwo());

static long PartOne()
{
    var input = File.ReadAllLines("../input.txt").ToArray();
    var seeds = ParseSeeds(input).ToArray();
    var mapping = ParseMaps(input);

    return seeds.Min(x => mapping.Aggregate(x, (res, map) => map.Process(res)));
}

static long PartTwo()
{
    var input = File.ReadAllLines("../input.txt").ToArray();
    var seeds = ParseSeeds(input);
    var mapping = ParseMaps(input);

    var initialRanges = new List<Range>();

    for (var i = 0; i < seeds.Length; i += 2)
    {
        initialRanges.Add(new Range(seeds[i], seeds[i] + seeds[i + 1]));
    }

    var ranges = initialRanges.ToList();

    foreach (var map in mapping)
    {
        ranges = map.Process(ranges).ToList();
    }

    return ranges.Min(x => x.Start);
}

static List<Map> ParseMaps(IEnumerable<string> input) => new List<Map>
{
    ParseMap(input, "seed-to-soil"),
    ParseMap(input, "soil-to-fertilizer"),
    ParseMap(input, "fertilizer-to-water"),
    ParseMap(input, "water-to-light"),
    ParseMap(input, "light-to-temperature"),
    ParseMap(input, "temperature-to-humidity"),
    ParseMap(input, "humidity-to-location")
};

static long[] ParseSeeds(string[] lines) =>
    ParseNumbers(lines[0].Split(':')[1]).ToArray();

static Map ParseMap(IEnumerable<string> lines, string mapPrefix) => new Map(lines
    .SkipWhile(x => !x.StartsWith(mapPrefix))
    .Skip(1)
    .TakeWhile(x => x.Length > 0)
    .Select(x => ParseNumbers(x).ToArray())
    .Select(x => new Link(x[1], x[0], x[2]))
    .ToList());

static IEnumerable<long> ParseNumbers(string input) => input
    .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
    .Select(long.Parse);


record Map(List<Link> Links)
{
    public long Process(long input)
    {
        var link = Links.FirstOrDefault(x => x.IsLinked(input));
        return link switch
        {
            null => input,
            _ => link.Destination + (input - link.Source)
        };
    }

    public IEnumerable<Range> Process(List<Range> input) => Links
        .Aggregate(input, (ranges, x) => 
            ranges.SelectMany(range => range.MapThrough(x.SourceRange, x.DestinationRange)).ToList());
}

record Link(long Source, long Destination, long Length)
{
    public Range SourceRange => new Range(Source, Source + Length);
    public Range DestinationRange => new Range(Destination, Destination + Length);

    public bool IsLinked(long value)
    {
        return Source <= value && value < (Source + Length);
    }
}


record Range(long Start, long End)
{
    public IEnumerable<Range> MapThrough(Range range, Range destination)
    {
        var points = new[] { Start, End, range.Start, range.End }
            .Distinct()
            .Where(x => x >= Start && x <= End)
            .OrderBy(x => x)
            .ToArray();

        for (var i = 0; i < points.Length - 1; i++)
        {
            var subRange = new Range(points[i], points[i + 1] - 1);

            if (IsIn(subRange) && range.IsIn(subRange))
            {
                var offset = subRange.Start - range.Start;

                yield return new Range(
                    destination.Start + offset,
                    destination.Start + offset + (subRange.End - subRange.Start)
                );
            }
            else
            {
                yield return subRange;
            }
        }
    }

    public bool IsIn(Range range) =>
        Start <= range.Start && range.End <= End;
}
