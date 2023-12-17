var input = File.ReadLines("input.txt").ToArray();

Console.WriteLine(PartOne(input));
Console.WriteLine(PartTwo(input));

static int PartOne(string[] map)
{
    var loop = FindLoop(map);
    return (int)Math.Ceiling(loop.Count() / 2d);
}

static int PartTwo(string[] map)
{
    var loop = FindLoop(map).ToArray();
    var polygon = GetLoopPolygon(loop).ToArray();
    var loopPoints = new HashSet<Coordinates>(loop);

    var area = 0;
    var minX = loop.Min(_ => _.X);
    var maxX = loop.Max(_ => _.X);

    for (var y = loop.Min(_ => _.Y); y <= loop.Max(_ => _.Y); y++)
    {
        for (var x = minX; x < maxX; x++)
        {
            var point = new Coordinates(x, y);
            if (!loopPoints.Contains(point) && IsInPolygon(point, polygon))
            {
                area++;
            }
        }
    }

    return area;
}


static bool IsInPolygon(Coordinates point, Coordinates[] polygon)
{
    var hits = 0;
    var x = point.X;
    var y = point.Y;

    for (int i = 0; i < polygon.Length - 1; i++)
    {
        var x1 = polygon[i].X;
        var x2 = polygon[i + 1].X;
        var y1 = polygon[i].Y;
        var y2 = polygon[i + 1].Y;

        if (y < y1 != y < y2 && x < (x2 - x1) * (y - y1) / (y2 - y1) + x1)
        {
            hits++;
        }
    }

    return hits % 2 != 0;
}

static IEnumerable<Coordinates> GetLoopPolygon(Coordinates[] loop)
{
    var direction = loop[1].Direction(loop[0]);
    var start = 0;

    for (int i = 0; i < loop.Length; i++)
    {
        if (loop[i + 1].Direction(loop[i]) != direction)
        {
            start = i;
            break;
        }
    }

    for (int i = start; i < loop.Length; i++)
    {
        var d = loop[i].Direction(loop[i - 1]);

        if (d != direction)
        {
            yield return loop[i - 1];
        }

        direction = d;
    }
}

static IEnumerable<Coordinates> FindLoop(string[] map)
{
    var startingNode = FindStart(map);
    var queue = new Queue<Pipe>();
    var seen = new HashSet<Pipe>();

    var startingPipe = Neighbors(startingNode)
        .Select(x => PipeAt(map, x))
        .Where(x => x.IsConnectedWith(startingNode))
        .FirstOrDefault();

    queue.Enqueue(startingPipe!);
    var points = new List<Coordinates>();

    while (queue.Any())
    {
        var pipe = queue.Dequeue();
        seen.Add(pipe);

        var a = PipeAt(map, pipe.A);
        var b = PipeAt(map, pipe.B);

        if (!seen.Contains(a) && a != Pipe.None)
        {
            yield return pipe.A;
            queue.Enqueue(a);
        }

        if (!seen.Contains(b) && b != Pipe.None)
        {
            yield return pipe.B;
            queue.Enqueue(b);
        }
    }

    yield return startingNode;
}

static Coordinates FindStart(string[] map)
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

static IEnumerable<Coordinates> Neighbors(Coordinates pos)
{
    yield return pos with { X = pos.X, Y = pos.Y - 1 };
    yield return pos with { X = pos.X, Y = pos.Y + 1 };
    yield return pos with { X = pos.X + 1, Y = pos.Y };
    yield return pos with { X = pos.X - 1, Y = pos.Y };
}

static Pipe PipeAt(string[] map, Coordinates pos)
{
    if (pos.Y < 0 || pos.Y >= map.Length)
    {
        return Pipe.None;
    }

    if (pos.X < 0 || pos.X >= map[0].Length)
    {
        return Pipe.None;
    }

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

record Coordinates(int X, int Y)
{
    public Coordinates Direction(Coordinates point) =>
        new(X - point.X, Y - point.Y);
}

record Pipe(Coordinates A, Coordinates B)
{
    public static Pipe None =>
        new(new Coordinates(-1, -1), new Coordinates(-1, -1));

    public bool IsConnectedWith(Coordinates pos) => A == pos || B == pos;
}
