// https://adventofcode.com/2022/day/11
using System.Text.RegularExpressions;

await SolvePart1Async();
await SolvePart2Async();

async Task SolvePart1Async()
{
    IList<Monkey> monkeys = await LoadInitialStateAsync();

    for (int x = 0; x < 20; x++)
        foreach (Monkey monkey in monkeys)
            monkey.ThrowItems(monkeys, withRelief: true);

    Monkey[] mostActiveMonkeys = monkeys
        .OrderByDescending(x => x.InspectedItemCount)
        .Take(2)
        .ToArray();
    int product = checked(mostActiveMonkeys[0].InspectedItemCount * mostActiveMonkeys[1].InspectedItemCount);

    Console.WriteLine($"Part 1 answer is: {product}");
}

async Task SolvePart2Async()
{
    IList<Monkey> monkeys = await LoadInitialStateAsync();

    for (int x = 0; x < 10_000; x++)
        foreach (Monkey monkey in monkeys)
            monkey.ThrowItems(monkeys, withRelief: false);

    Monkey[] mostActiveMonkeys = monkeys
        .OrderByDescending(x => x.InspectedItemCount)
        .Take(2)
        .ToArray();
    long product = checked((long)mostActiveMonkeys[0].InspectedItemCount * (long)mostActiveMonkeys[1].InspectedItemCount);

    Console.WriteLine($"Part 2 answer is: {product}");
}

async Task<IList<Monkey>> LoadInitialStateAsync()
{
    Regex Line1Regex = new Regex(@"\AMonkey (?<number>\d+):$", RegexOptions.Compiled);
    Regex Line2Regex = new Regex(@"\A[ ]*Starting items: (?<items>(?:\d+)(, \d+)*)$", RegexOptions.Compiled);
    Regex Line3Regex = new Regex(@"\A[ ]*Operation: new = (?<leftOperand>[A-z0-9]+) (?<operator>[-+\/*]) (?<rightOperand>[A-z0-9]+)$", RegexOptions.Compiled);
    Regex Line4Regex = new Regex(@"\A[ ]*Test: divisible by (?<divisor>\d+)$", RegexOptions.Compiled);
    Regex Line5Regex = new Regex(@"\A[ ]*If true: throw to monkey (?<target>\d+)$", RegexOptions.Compiled);
    Regex Line6Regex = new Regex(@"\A[ ]*If false: throw to monkey (?<target>\d+)$", RegexOptions.Compiled);

    List<Monkey> monkeys = new();
    using StreamReader file = File.OpenText("input.txt");
    while (!file.EndOfStream)
    {
        string line = (await file.ReadLineAsync())!;

        // New monkey marker
        Match line1Match = Line1Regex.Match(line);
        if (!line1Match.Success)
            continue;

        // Initial items
        line = (await file.ReadLineAsync())!;
        Match line2Match = Line2Regex.Match(line);
        if (!line2Match.Success)
            throw new InvalidOperationException($"Invalid line: {line}");
        List<long> items = line2Match.Groups["items"].Value
            .Split(",", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Select(long.Parse)
            .ToList();

        // Operation
        line = (await file.ReadLineAsync())!;
        Match line3Match = Line3Regex.Match(line);
        if (!line3Match.Success)
            throw new InvalidOperationException($"Invalid line: {line}");

        int? leftOperand = int.TryParse(line3Match.Groups["leftOperand"].Value, out int leftOperandParsed)
            ? leftOperandParsed
            : null;

        string @operator = line3Match.Groups["operator"].Value;

        int? rightOperand = int.TryParse(line3Match.Groups["rightOperand"].Value, out int rightOperandParsed)
            ? rightOperandParsed
            : null;

        // Divisor
        line = (await file.ReadLineAsync())!;
        Match line4Match = Line4Regex.Match(line);
        if (!line4Match.Success)
            throw new InvalidOperationException($"Invalid line: {line}");
        int divisor = int.Parse(line4Match.Groups["divisor"].Value);

        // Target when true
        line = (await file.ReadLineAsync())!;
        Match line5Match = Line5Regex.Match(line);
        if (!line5Match.Success)
            throw new InvalidOperationException($"Invalid line: {line}");
        int monkeyWhenTrue = int.Parse(line5Match.Groups["target"].Value);

        // Target when false
        line = (await file.ReadLineAsync())!;
        Match line6Match = Line6Regex.Match(line);
        if (!line6Match.Success)
            throw new InvalidOperationException($"Invalid line: {line}");
        int monkeyWhenFalse = int.Parse(line6Match.Groups["target"].Value);

        monkeys.Add(new Monkey(
            items,
            leftOperand,
            @operator,
            rightOperand,
            divisor,
            monkeyWhenTrue,
            monkeyWhenFalse));
    }

    return monkeys;
}

enum Operator
{
    Addition,
    Subtraction,
    Multiplication,
    Division
}

class Monkey
{
    private readonly List<long> _items = new();

    public Monkey(
         IList<long> items,
         int? leftOperand,
         string @operator,
         int? rightOperand,
         int divisor,
         int monkeyWhenTrue,
         int monkeyWhenFalse)
    {
        _items.AddRange(items);
        this.LeftOperand = leftOperand;

        this.Operator = @operator switch
        {
            "+" => Operator.Addition,
            "-" => Operator.Subtraction,
            "*" => Operator.Multiplication,
            "/" => Operator.Division,
            _ => throw new InvalidOperationException($"Invalid operator: {@operator}")
        };

        this.RightOperand = rightOperand;
        this.Divisor = divisor;
        this.MonkeyWhenTestTrue = monkeyWhenTrue;
        this.MontkeyWhenTestFalse = monkeyWhenFalse;
    }

    public IReadOnlyList<long> Items => _items;
    public int? LeftOperand { get; }
    public Operator Operator { get; }
    public int? RightOperand { get; }
    public int Divisor { get; }
    public int MonkeyWhenTestTrue { get; }
    public int MontkeyWhenTestFalse { get; }
    public int InspectedItemCount { get; private set; }

    public void ThrowItems(IList<Monkey> allMonkeys, bool withRelief)
    {
        // Calculate the product of all monkey's divisors. 
        // As the decision of which monkey to throw the item to is based on the 
        // remainder of the item's worry level divided by the monkey's divisor, 
        // the product of all divisors will get us a common value that can be
        // divided by any divisor without a reminder. That means the modulo 
        // operation of any divisor will always return a value lower than this
        // common product. We can use this fact to scale down the worry levels.
        int productAllDivisors = allMonkeys
            .Select(x => x.Divisor)
            .Aggregate((a, x) => a * x);

        while (_items.Any())
        {
            InspectedItemCount++;

            long worryLevel = _items[0];
            _items.RemoveAt(0);

            long leftOperand = LeftOperand ?? worryLevel;
            long rightOperand = RightOperand ?? worryLevel;

            worryLevel = Operator switch
            {
                Operator.Addition => checked(leftOperand + rightOperand),
                Operator.Subtraction => checked(leftOperand - rightOperand),
                Operator.Multiplication => checked(leftOperand * rightOperand),
                Operator.Division => checked(leftOperand / rightOperand),
                _ => throw new InvalidOperationException($"Invalid operator: {Operator}")
            };

            if (withRelief)
                worryLevel = checked(worryLevel / 3);

            // Scale down worry level
            worryLevel = checked(worryLevel % productAllDivisors);

            int monkeyIdx = checked(worryLevel % Divisor) == 0
                ? MonkeyWhenTestTrue
                : MontkeyWhenTestFalse;

            allMonkeys[monkeyIdx]._items.Add(worryLevel);
        }
    }
}