using System.Text.RegularExpressions;

Console.WriteLine($"PartOne: {PartOne()}");
Console.WriteLine($"PartTwo: {PartTwo()}");

static int PartOne()
{
    var nodes = ReadNodes().ToList();
    var positionLookup = nodes.ToLookup(x => x.Position);

    return nodes
        .Where(x =>
            x.NodeType == NodeType.Number &&
            x.GetAdjacentPositions()
                .Any(pos => positionLookup[pos].Any(node => node.NodeType == NodeType.Symbol)))
        .Sum(x => x.ToNumber());
}

static int PartTwo()
{
    var nodes = ReadNodes().ToArray();

    var gearRations =
        from node in nodes
        where node.IsGear
        let positionsToCheck = node.GetAdjacentPositions().ToArray()
        let connections = nodes
            .Where(x => x.NodeType == NodeType.Number && positionsToCheck.Any(pos => x.IsAt(pos)))
            .ToArray()
        where connections.Length > 1
        let gearRatio = connections.Aggregate(1, (ratio, node) => ratio * node.ToNumber())
        select gearRatio;

    return gearRations.Sum();
}

static IEnumerable<Node> ReadNodes() => File.ReadAllLines("../input.txt").SelectMany((line, y) =>
{
    var numbers = Regex.Matches(line, @"\d{1,}")
       .Select(match =>
            new Node(new Position(match.Index, y), NodeType.Number, match.Length, match.Value));

    var symbols = Regex.Matches(line, @"[^\d^\.\n]")
       .Select(match =>
            new Node(new Position(match.Index, y), NodeType.Symbol, match.Length, match.Value));

    return numbers.Concat(symbols);
});

record Position(int X, int Y);

enum NodeType
{
    Symbol,
    Number
}

record Node(Position Position, NodeType NodeType, int Length, string Value)
{
    public bool IsGear =>
        NodeType == NodeType.Symbol && Value == "*";

    public IEnumerable<Position> GetAdjacentPositions()
    {
        var x = Position.X;
        var y = Position.Y;

        for (int i = -1; i <= Length; i++)
        {
            yield return new Position(x + i, y - 1);
            yield return new Position(x + i, y + 1);

            if (i < 0 || i == Length)
            {
                yield return new Position(x + i, y);
            }
        }
    }

    public bool IsAt(Position position) =>
        position.Y == Position.Y &&
        position.X >= Position.X &&
        position.X <= Position.X + Length - 1;

    public int ToNumber()
    {
        if (NodeType != NodeType.Number)
        {
            throw new InvalidOperationException();
        }

        return int.Parse(Value);
    }
}
