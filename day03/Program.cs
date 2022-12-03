// https://adventofcode.com/2022/day/3
using System.Diagnostics;
using System.Text;

await SolvePart1Async();
await SolvePart2Async();

async Task SolvePart1Async()
{
    char[] _priorities = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

    int sumPriorities = 0;
    using StreamReader file = File.OpenText("input.txt");
    while (!file.EndOfStream)
    {
        string? line = await file.ReadLineAsync();
        Debug.Assert(line != null);

        string[] parts = new[] {
            line.Substring(0, line.Length /2),
            line.Substring(line.Length /2)
        };

        char commonItem = parts[0].Intersect(parts[1]).Single();
        int priority = Array.IndexOf(_priorities, commonItem) + 1;
        sumPriorities += priority;
    }

    Console.WriteLine($"Part 1 answer is: {sumPriorities}");
}

async Task SolvePart2Async()
{
    char[] _priorities = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

    int sumPriorities = 0;
    using StreamReader file = File.OpenText("input.txt");
    while (!file.EndOfStream)
    {
        string? line1 = await file.ReadLineAsync();
        string? line2 = await file.ReadLineAsync();
        string? line3 = await file.ReadLineAsync();

        Debug.Assert(line1 != null);
        Debug.Assert(line2 != null);
        Debug.Assert(line3 != null);

        char commonItem = line1.Intersect(line2).Intersect(line3).Single();
        int priority = Array.IndexOf(_priorities, commonItem) + 1;
        sumPriorities += priority;
    }

    Console.WriteLine($"Part 1 answer is: {sumPriorities}");
}
