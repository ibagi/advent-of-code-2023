using System.Text;

var fileInput = File.ReadLines("input.txt").ToArray();

Console.WriteLine(PartOne(fileInput));
Console.WriteLine(PartTwo(fileInput));

int PartOne(string[] input)
{
    var map = Expand(input).ToArray();
    var num = 0;

    var nodes =
        from y in Enumerable.Range(0, map.Length)
        from x in Enumerable.Range(0, map[0].Length)
        where map[y][x] == '#'
        select new Node(num++, x, y);

    var nodeList = nodes.ToList();

    var pairs =
        from first in nodeList
        from second in nodeList
        where first != second
        select new Pair(first, second);

    return pairs.DistinctBy(x => x.Key).Aggregate(0, (sum, pair) => sum + pair.Distance);
}

int PartTwo(string[] input)
{
    return 0;
}

IEnumerable<string> Expand(string[] input)
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

    var lines = input.ToList();

    for (var i = 0; i < rows.Length; i++)
    {
        lines.Insert(rows[i] + i, new string('.', input[0].Length));
    }

    foreach (var line in lines)
    {
        var sb = new StringBuilder(line);

        for (var i = 0; i < columns.Length; i++)
        {
            sb.Insert(columns[i] + i, ".");
        }

        yield return sb.ToString();
    }
}


record PairKey(int MinNum, int MaxNum);

record Pair(Node First, Node Second)
{
    public PairKey Key =>
        new(Math.Min(First.Num, Second.Num), Math.Max(First.Num, Second.Num));

    public int Distance =>
        First.Distance(Second);
}

record Node(int Num, int X, int Y)
{
    public int Distance(Node other) =>
        Math.Abs(X - other.X) + Math.Abs(Y - other.Y);
}
