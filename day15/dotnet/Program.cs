Console.WriteLine(PartOne());
Console.WriteLine(PartTwo());

static int PartOne() => GetInitializationSequence()
    .Aggregate(0, (total, i) => total + Hashing.Hash(i));

static int PartTwo()
{
    var instructions = GetInitializationSequence()
        .Select(ParseInstruction)
        .ToArray();

    var boxes = Enumerable.Range(0, 256)
        .Select(i => new Box(i + 1))
        .ToArray();

    foreach (var instruction in instructions)
    {
        instruction.ApplyOn(boxes[instruction.TargetBox]);
    }

    return boxes.Aggregate(0, (total, b) => total + b.FocusPower());
}

static IEnumerable<string> GetInitializationSequence() => File.ReadAllText("input.txt")
    .Replace(Environment.NewLine, string.Empty)
    .Split(',');

static IInstruction ParseInstruction(string input)
{
    var operation = input.FirstOrDefault(c => c == '=' || c == '-');
    var parts = input.Split(operation);
    var label = parts[0];
    var targetBox = Hashing.Hash(label);

    if (operation == '=')
    {
        return new EqualsInstruction(label, targetBox, int.Parse(parts[1]));
    }
    else
    {
        return new MinusInstruction(label, targetBox);
    }
}

record EqualsInstruction(string Label, int TargetBox, int FocalLength) : IInstruction
{
    public void ApplyOn(Box box)
    {
        if (box.Map.ContainsKey(Label))
        {
            box.Map[Label] = FocalLength;
        }
        else
        {
            box.Map[Label] = FocalLength;
            box.Order[box.Order.IndexOf(string.Empty)] = Label;
        }
    }
}

record MinusInstruction(string Label, int TargetBox) : IInstruction
{
    public void ApplyOn(Box box)
    {
        int index;

        while ((index = box.Order.IndexOf(Label)) > -1)
        {
            for (var i = index; i < box.Order.Count - 1; i++)
            {
                box.Order[i] = box.Order[i + 1];
            }
        }

        box.Map.Remove(Label);
    }
}

class Box(int boxNumber)
{
    public Dictionary<string, int> Map { get; } = [];
    public List<string> Order { get; } = Enumerable.Range(0, 9).Select(_ => string.Empty).ToList();

    public int FocusPower()
    {
        var total = 0;

        for (var i = 0; i < Order.Count; i++)
        {
            if (Order[i] != "")
            {
                total += boxNumber * (i + 1) * Map[Order[i]];
            }
        }

        return total;
    }
}

interface IInstruction
{
    int TargetBox { get; }
    void ApplyOn(Box box);
}

static class Hashing
{
    public static int Hash(string input) =>
        input.Aggregate(0, (sum, c) => (sum + c) * 17 % 256);
}
