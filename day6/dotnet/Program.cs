using System.Text.RegularExpressions;

Console.WriteLine(PartOne());
Console.WriteLine(PartTwo());

static int PartOne()
{
    var input = File.ReadLines("../input.txt")
        .Select(x => Regex.Matches(x, @"\d+").Select(m => long.Parse(m.Value)).ToArray())
        .ToArray();

    return input[0].Zip(input[1], BruteForceSolution).Aggregate((res, x) => res * x);
}

static long PartTwo()
{
    var input = File.ReadLines("../input.txt")
        .Select(x => long.Parse(Regex.Replace(x.Split(':')[1], @"\s", string.Empty)))
        .ToArray();

    return BruteForceSolution(input[0], input[1]);
}

// TODO: Use the quadratic equation formula
static int BruteForceSolution(long time, long distance)
{
    var result = 0;

    for (var i = 1; i < time; i++)
    {
        var speed = i;
        var remainingTime = time - i;
        var calculatedDistance = remainingTime * speed;

        if (calculatedDistance > distance)
        {
            result++;
        }
    }

    return result;
}
