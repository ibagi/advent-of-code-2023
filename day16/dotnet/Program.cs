using System.Collections.Concurrent;

var rawInput = File.ReadLines("input.txt").ToArray();
var bounds = new Bounds(rawInput[0].Length, rawInput.Length);

var nodes =
    (from y in Enumerable.Range(0, rawInput.Length)
     from x in Enumerable.Range(0, rawInput[0].Length)
     let value = rawInput[y][x]
     where value != '.'
     let position = new Position(x, y)
     select (position, nodeType: GetNodeType(value))).ToDictionary(x => x.position, x => x.nodeType);

Console.WriteLine(PartOne(nodes, bounds));
Console.WriteLine(PartTwo(nodes, bounds));

static int PartOne(Dictionary<Position, NodeType> nodes, Bounds bounds)
{
    var start = new Cursor(new Position(0, 0), Direction.Right);
    return Traverse(start, nodes, bounds);
}

static int PartTwo(Dictionary<Position, NodeType> nodes, Bounds bounds)
{
    var energizedCells = new ConcurrentBag<int>();

    Parallel.For(0, bounds.Y, y =>
    {
        energizedCells.Add(Traverse(new(new(0, y), Direction.Right), nodes, bounds));
        energizedCells.Add(Traverse(new(new(bounds.X - 1, y), Direction.Left), nodes, bounds));
    });

    Parallel.For(0, bounds.X, x =>
    {
        energizedCells.Add(Traverse(new(new(x, 0), Direction.Down), nodes, bounds));
        energizedCells.Add(Traverse(new(new(x, bounds.Y - 1), Direction.Up), nodes, bounds));
    });

    return energizedCells.Max();
}

static int Traverse(Cursor start, Dictionary<Position, NodeType> nodes, Bounds bounds)
{
    var visited = new HashSet<Cursor>();
    var queue = new Queue<Cursor>();

    queue.Enqueue(start);

    while (queue.Any())
    {
        var state = queue.Dequeue();
        var pos = state.Position;

        if (!pos.IsIn(bounds) || visited.Contains(state))
        {
            continue;
        }

        visited.Add(state);

        if (!nodes.ContainsKey(state.Position))
        {
            var nextPostion = state.Direction switch
            {
                Direction.Up => pos with { Y = pos.Y - 1 },
                Direction.Right => pos with { X = pos.X + 1 },
                Direction.Down => pos with { Y = pos.Y + 1 },
                Direction.Left => pos with { X = pos.X - 1 }
            };

            var nextState = new Cursor(nextPostion, state.Direction);
            queue.Enqueue(nextState);

            continue;
        }

        var newStates = (nodes[pos], state.Direction) switch
        {
            (NodeType.SplitterH, Direction.Up) =>
                [new(pos with { X = pos.X - 1 }, Direction.Left), new(pos with { X = pos.X + 1 }, Direction.Right)],
            (NodeType.SplitterH, Direction.Right) =>
                 [new(pos with { X = pos.X + 1 }, state.Direction)],
            (NodeType.SplitterH, Direction.Down) =>
                [new(pos with { X = pos.X - 1 }, Direction.Left), new(pos with { X = pos.X + 1 }, Direction.Right)],
            (NodeType.SplitterH, Direction.Left) =>
                [new(pos with { X = pos.X - 1 }, state.Direction)],

            (NodeType.SplitterV, Direction.Up) =>
                [new(pos with { Y = pos.Y - 1 }, state.Direction)],
            (NodeType.SplitterV, Direction.Right) =>
                [new(pos with { Y = pos.Y - 1 }, Direction.Up), new(pos with { Y = pos.Y + 1 }, Direction.Down)],
            (NodeType.SplitterV, Direction.Down) =>
                [new(pos with { Y = pos.Y + 1 }, state.Direction)],
            (NodeType.SplitterV, Direction.Left) =>
                [new(pos with { Y = pos.Y - 1 }, Direction.Up), new(pos with { Y = pos.Y + 1 }, Direction.Down)],

            (NodeType.Mirror, Direction.Up) =>
                [new(pos with { X = pos.X + 1 }, Direction.Right)],
            (NodeType.Mirror, Direction.Right) =>
                [new(pos with { Y = pos.Y - 1 }, Direction.Up)],
            (NodeType.Mirror, Direction.Down) =>
                [new(pos with { X = pos.X - 1 }, Direction.Left)],
            (NodeType.Mirror, Direction.Left) =>
                [new(pos with { Y = pos.Y + 1 }, Direction.Down)],

            (NodeType.MirrorBack, Direction.Up) =>
                [new(pos with { X = pos.X - 1 }, Direction.Left)],
            (NodeType.MirrorBack, Direction.Right) =>
                [new(pos with { Y = pos.Y + 1 }, Direction.Down)],
            (NodeType.MirrorBack, Direction.Down) =>
                [new(pos with { X = pos.X + 1 }, Direction.Right)],
            (NodeType.MirrorBack, Direction.Left) =>
                new Cursor[] { new(pos with { Y = pos.Y - 1 }, Direction.Up) }
        };

        foreach (var s in newStates)
        {
            queue.Enqueue(s);
        }
    }

    return visited.DistinctBy(x => x.Position).Count();
}

static NodeType GetNodeType(char value) => value switch
{
    '/' => NodeType.Mirror,
    '\\' => NodeType.MirrorBack,
    '|' => NodeType.SplitterV,
    '-' => NodeType.SplitterH,
    _ => throw new Exception("Invalid char!")
};

enum NodeType
{
    SplitterV,
    SplitterH,
    Mirror,
    MirrorBack,
}

enum Direction
{
    Up,
    Right,
    Down,
    Left
}

record Cursor(Position Position, Direction Direction);

record Position(int X, int Y)
{
    public bool IsIn(Bounds bounds) =>
        X >= 0 && X < bounds.X && Y >= 0 && Y < bounds.Y;
}

record Bounds(int X, int Y);
