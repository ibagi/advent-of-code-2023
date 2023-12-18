var fileInput = File.ReadLines("input.txt").ToArray();

Console.WriteLine(PartOne(fileInput));
Console.WriteLine(PartTwo(fileInput));

long PartOne(string[] input)
{
    return MinimalDistanceSum(input, 1);
}

long PartTwo(string[] input)
{
    return MinimalDistanceSum(input, 1_000_000);
}

long MinimalDistanceSum(string[] input, int expansionRate)
{
    var num = 0;
    var map = input.ToArray();
    var (rows, columns) = ExpansionIndices(map);

    var expRate = expansionRate > 1
        ? (expansionRate - 1)
        : expansionRate;

    var nodes =
        from y in Enumerable.Range(0, map.Length)
        from x in Enumerable.Range(0, map[0].Length)
        where map[y][x] == '#'
        let expansionY = rows.Count(r => r < y) * expRate
        let expansionX = columns.Count(c => c < x) * expRate
        select new Node(num++, x + expansionX, y + expansionY);

    var nodeList = nodes.ToList();

    var pairs =
        from first in nodeList
        from second in nodeList
        where first != second
        select new Pair(first, second);

    return pairs.DistinctBy(x => x.Key).Aggregate(0L, (sum, pair) => sum + pair.Distance);
}

(int[] Rows, int[] Columns) ExpansionIndices(string[] input)
{
    var points =
        from y in Enumerable.Range(0, input.Length)
        from x in Enumerable.Range(0, input[0].Length)
        select (x, y);

    var rows = points.GroupBy(_ => _.y)
        .Where(g => g.All(_ => input[g.Key][_.x] == '.'))
        .Select(g => g.Key)
        .ToArray();

    var columns = points.GroupBy(_ => _.x)
        .Where(g => g.All(_ => input[_.y][g.Key] == '.'))
        .Select(g => g.Key)
        .ToArray();

    return (rows, columns);
}

record PairKey(int MinNum, int MaxNum);

record Pair(Node First, Node Second)
{
    public PairKey Key =>
        new(Math.Min(First.Num, Second.Num), Math.Max(First.Num, Second.Num));

    public long Distance =>
        First.Distance(Second);
}

record Node(int Num, long X, long Y)
{
    public long Distance(Node other) =>
        Math.Abs(X - other.X) + Math.Abs(Y - other.Y);
}
