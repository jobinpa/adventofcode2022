// https://adventofcode.com/2022/day/5
using System.Diagnostics;
using System.Text.RegularExpressions;

Regex _regexInstruction = new(@"\Amove (?<quantity>\d+) from (?<start>\d+) to (?<end>\d+)\z", RegexOptions.Compiled);
const char NOITEM = ' ';
char[] _validItemCodes = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
char[] _validStackNumbers = "123456789".ToCharArray();

await SolvePart1Async();
await SolvePart2Async();

async Task SolvePart1Async()
{
    using StreamReader file = File.OpenText("input.txt");
    IList<IList<char>> stacks = await LoadStacks(file);

    while (!file.EndOfStream)
    {
        string? line = await file.ReadLineAsync();
        Debug.Assert(line != null);

        Instruction m = ParseInstruction(line);
        stacks[m.To] = stacks[m.From].Take(m.Quantity).Reverse().Concat(stacks[m.To]).ToList();
        stacks[m.From] = stacks[m.From].Skip(m.Quantity).ToList();
    }

    string result = string.Empty;
    foreach (IList<char> s in stacks)
        result += s.Any() ? s.First() : NOITEM;

    Console.WriteLine($"Part 1 answer is: {result}.");
}

async Task SolvePart2Async()
{
    using StreamReader file = File.OpenText("input.txt");
    IList<IList<char>> stacks = await LoadStacks(file);

    while (!file.EndOfStream)
    {
        string? line = await file.ReadLineAsync();
        Debug.Assert(line != null);

        Instruction m = ParseInstruction(line);
        stacks[m.To] = stacks[m.From].Take(m.Quantity).Concat(stacks[m.To]).ToList();
        stacks[m.From] = stacks[m.From].Skip(m.Quantity).ToList();
    }

    string result = string.Empty;
    foreach (IList<char> s in stacks)
        result += s.Any() ? s.First() : NOITEM;

    Console.WriteLine($"Part 2 answer is: {result}.");
}

async Task<IList<IList<char>>> LoadStacks(StreamReader file)
{
    List<IList<char>> stacks = new();

    while (!file.EndOfStream)
    {
        string? line = await file.ReadLineAsync();
        Debug.Assert(line != null);

        if (string.IsNullOrWhiteSpace(line))
            break;

        for (int i = 1; i < line.Length; i += 4)
        {
            char currentItem = line[i];

            bool isValidLine =
                currentItem == NOITEM ||
                _validItemCodes.Contains(currentItem) ||
                _validStackNumbers.Contains(currentItem);

            if (!isValidLine)
                throw new InvalidOperationException($"Invalid stack line: {line}");

            int stackIndex = i / 4;
            if (stacks.Count <= stackIndex)
                stacks.Add(new List<char>());

            if (char.IsLetter(line[i]))
                stacks[stackIndex].Add(currentItem);
        }
    }

    return stacks;
}

Instruction ParseInstruction(string s)
{
    Match match = _regexInstruction.Match(s);

    if (!match.Success)
        throw new InvalidOperationException($"Invalid instruction: {s}");

    int quantity = int.Parse(match.Groups["quantity"].Value);
    int from = int.Parse(match.Groups["start"].Value) - 1;
    int to = int.Parse(match.Groups["end"].Value) - 1;

    return new Instruction(quantity, from, to);
}

record Instruction(int Quantity, int From, int To);