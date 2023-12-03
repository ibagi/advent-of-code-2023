using System.IO;
using System.Text.RegularExpressions;

Console.WriteLine($"PartOne: {PartOne()}");
Console.WriteLine($"PartTwo: {PartTwo()}");

static int PartOne() => File.ReadLines("../input.txt")
    .Select(Game.Parse)
    .Where(x => x.IsValid(new CubeSet(12, 13, 14)))
    .Sum(x => x.ID);

static int PartTwo() => File.ReadLines("../input.txt")
    .Select(x => Game.Parse(x).MinimalSet().Pow())
    .Sum();

record Game(int ID, CubeSet[] CubeSets)
{
    public static Game Parse(string input)
    {
        var data = input.Split(':', 2);
        return new Game(ParseId(data[0]), CubeSet.Parse(data[1]).ToArray());
    }

    private static int ParseId(string header) =>
        int.Parse(header.Split(' ', 2)[1]);

    public bool IsValid(CubeSet configuration) => CubeSets.All(x =>
        x.Red <= configuration.Red &&
        x.Green <= configuration.Green &&
        x.Blue <= configuration.Blue);

    public CubeSet MinimalSet() => new CubeSet(
        CubeSets.Max(x => x.Red),
        CubeSets.Max(x => x.Green),
        CubeSets.Max(x => x.Blue));
}

record CubeSet(int Red, int Green, int Blue)
{
    public static IEnumerable<CubeSet> Parse(string input) =>
        input.Split(';').Select(ParseCubeSet);

    private static CubeSet ParseCubeSet(string input)
    {
        int.TryParse(Regex.Match(input, @"(\d{1,}) red").Groups[1].Value, out var red);
        int.TryParse(Regex.Match(input, @"(\d{1,}) green").Groups[1].Value, out var green);
        int.TryParse(Regex.Match(input, @"(\d{1,}) blue").Groups[1].Value, out var blue);
        return new CubeSet(red, green, blue);
    }

    public int Pow() => Red * Green * Blue;
}