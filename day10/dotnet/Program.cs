var input = File.ReadLines("input.txt").ToArray();

Console.WriteLine(PartOne(input));

static Position FindStart(string[] map)
{
    for (var y = 0; y < map.Length; y++)
    {
        for (var x = 0; x < map[y].Length; x++)
        {
            if (map[y][x] == 'S')
            {
                return new(x, y);
            }
        }
    }

    throw new Exception("Invalid map!");
}

static int PartOne(string[] map)
{
    var startingNode = FindStart(map);
    var queue = new Queue<Pipe>();
    var seen = new HashSet<Pipe>();
    var steps = 1;

    var startingPipe = Neighbors(startingNode)
        .Select(x => PipeAt(map, x))
        .Where(x => x.IsConnectedWith(startingNode))
        .FirstOrDefault();

    queue.Enqueue(startingPipe!);

    while (queue.Any())
    {
        var pipe = queue.Dequeue();
        seen.Add(pipe);

        var a = PipeAt(map, pipe.A);
        var b = PipeAt(map, pipe.B);

        if (!seen.Contains(a) && a != Pipe.None)
        {
            queue.Enqueue(a);
        }

        if (!seen.Contains(b) && b != Pipe.None)
        {
            queue.Enqueue(b);
        }

        steps++;
    }

    return steps / 2;
}

static IEnumerable<Position> Neighbors(Position pos)
{
    yield return pos with { X = pos.X, Y = pos.Y - 1 };
    yield return pos with { X = pos.X, Y = pos.Y + 1 };
    yield return pos with { X = pos.X + 1, Y = pos.Y };
    yield return pos with { X = pos.X - 1, Y = pos.Y };
}

static Pipe PipeAt(string[] map, Position pos)
{
    var value = map[pos.Y][pos.X];
    return value switch
    {
        '|' => new Pipe(pos with { Y = pos.Y - 1 }, pos with { Y = pos.Y + 1 }),
        '-' => new Pipe(pos with { X = pos.X - 1 }, pos with { X = pos.X + 1 }),
        'L' => new Pipe(pos with { Y = pos.Y - 1 }, pos with { X = pos.X + 1 }),
        'J' => new Pipe(pos with { Y = pos.Y - 1 }, pos with { X = pos.X - 1 }),
        '7' => new Pipe(pos with { Y = pos.Y + 1 }, pos with { X = pos.X - 1 }),
        'F' => new Pipe(pos with { Y = pos.Y + 1 }, pos with { X = pos.X + 1 }),
        _ => Pipe.None
    };
}

record Position(int X, int Y);
record Pipe(Position A, Position B)
{
    public static Pipe None =>
        new(new Position(-1, -1), new Position(-1, -1));

    public bool IsConnectedWith(Position pos) => A == pos || B == pos;
}
