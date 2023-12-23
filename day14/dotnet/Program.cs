var input = File.ReadLines("input.txt").ToArray();
var bounds = new Bounds(input.Length, input[0].Length);

Console.WriteLine(PartOne(GetNodeMap(input), bounds));
Console.WriteLine(PartTwo(GetNodeMap(input), bounds));

static Dictionary<(int X, int Y), Node> GetNodeMap(string[] rawInput)
{
    var nodes =
     from y in Enumerable.Range(0, rawInput.Length)
     from x in Enumerable.Range(0, rawInput[0].Length)
     let node = ParseNode(x, y, rawInput[y])
     where node.NodeType != NodeType.None
     select node;

    return nodes.ToDictionary(n => (n.X, n.Y));
}

static int PartOne(Dictionary<(int X, int Y), Node> nodeMap, Bounds bounds)
{
    Tilt(nodeMap, TiltDirection.Up, bounds);
    return nodeMap.Values.Sum(n => CalculateLoad(n, bounds));
}

static int PartTwo(Dictionary<(int X, int Y), Node> nodeMap, Bounds bounds)
{
    TiltALot(nodeMap, bounds);
    return nodeMap.Values.Sum(n => CalculateLoad(n, bounds));
}

static void TiltALot(Dictionary<(int X, int Y), Node> nodeMap, Bounds bounds)
{
    var direction = TiltDirection.Up;
    var states = new List<MapState> { new(nodeMap) };
    var step = 0;
    var cycleStart = 0;

    TiltDirection nextDirection() => direction == TiltDirection.Right
        ? TiltDirection.Up
        : direction + 1;

    while (true)
    {
        Tilt(nodeMap, direction, bounds);
        step++;

        var currentState = new MapState(nodeMap);
        var prevState = states.FirstOrDefault(s => s.IsTheSame(currentState));

        if (prevState != null)
        {
            cycleStart = states.IndexOf(prevState);
            break;
        }

        states.Add(currentState);
        direction = nextDirection();
    }

    var stepsLeft = (4 * Math.Pow(10, 9) - cycleStart) % (step - cycleStart);

    for (var i = 0; i < stepsLeft; i++)
    {
        direction = nextDirection();
        Tilt(nodeMap, direction, bounds);
    }
}

static void Tilt(Dictionary<(int X, int Y), Node> nodeMap, TiltDirection direction, Bounds bounds)
{
    foreach (var node in GetNodeOrder(nodeMap.Values, direction))
    {
        if (node.NodeType != NodeType.Ellipse)
        {
            continue;
        }

        var updated = (node.X, node.Y);

        foreach (var pos in GetTiltRange(node, direction, bounds))
        {
            if (nodeMap.ContainsKey(pos))
            {
                break;
            }

            updated = pos;
        }

        nodeMap.Remove((node.X, node.Y));
        nodeMap.Add(updated, node with { X = updated.X, Y = updated.Y });
    }
}

static int CalculateLoad(Node node, Bounds bounds)
{
    if (node.NodeType != NodeType.Ellipse)
    {
        return 0;
    }

    return bounds.Y - node.Y;
}

static Node ParseNode(int x, int y, string line)
{
    var nodeType = line[x] switch
    {
        'O' => NodeType.Ellipse,
        '#' => NodeType.Cube,
        _ => NodeType.None
    };

    return new Node(Guid.NewGuid(), x, y, nodeType);
}

static IEnumerable<Node> GetNodeOrder(IEnumerable<Node> nodes, TiltDirection direction) => direction switch
{
    TiltDirection.Up =>
        nodes.OrderBy(x => x.Y),
    TiltDirection.Right =>
        nodes.OrderByDescending(x => x.X),
    TiltDirection.Down =>
        nodes.OrderByDescending(x => x.Y),
    TiltDirection.Left =>
        nodes.OrderBy(x => x.X),
    _ => throw new Exception()
};

static IEnumerable<(int X, int Y)> GetTiltRange(Node n, TiltDirection direction, Bounds bounds) => direction switch
{
    TiltDirection.Up =>
        Enumerable.Range(0, n.Y).Reverse().Select(y => (n.X, y)),
    TiltDirection.Right =>
        Enumerable.Range(0, bounds.X).Where(x => x > n.X).Select(x => (x, n.Y)),
    TiltDirection.Down =>
        Enumerable.Range(0, bounds.Y).Where(y => y > n.Y).Select(y => (n.X, y)),
    TiltDirection.Left =>
        Enumerable.Range(0, n.X).Reverse().Select(x => (x, n.Y)),
    _ => throw new Exception()
};

enum NodeType
{
    None,
    Cube,
    Ellipse
}

enum TiltDirection
{
    Up = 1,
    Left,
    Down,
    Right
}

class MapState
{
    private readonly HashSet<(int X, int Y)> _positions = new();

    public MapState(Dictionary<(int X, int Y), Node> nodeMap)
    {
        foreach (var entry in nodeMap)
        {
            if (entry.Value.NodeType == NodeType.Ellipse)
            {
                _positions.Add(entry.Key);
            }
        }
    }

    public bool IsTheSame(MapState state)
    {
        return _positions.SetEquals(state._positions);
    }
}

record Bounds(int Y, int X);
record Node(Guid NodeId, int X, int Y, NodeType NodeType);
