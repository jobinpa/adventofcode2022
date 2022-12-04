// https://adventofcode.com/2022/day/4
using System.Diagnostics;

await SolvePart1Async();
await SolvePart2Async();

async Task SolvePart1Async()
{
    int fullOverlapCount = 0;
    using StreamReader file = File.OpenText("input.txt");
    while (!file.EndOfStream)
    {
        string? line = await file.ReadLineAsync();
        Debug.Assert(line != null);
        string[] parts = line.Split(",", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        int[] range1 = ParseRange(parts[0]);
        int[] range2 = ParseRange(parts[1]);

        if (range1.Intersect(range2).SequenceEqual(range1) ||
            range2.Intersect(range1).SequenceEqual(range2))
        {
            fullOverlapCount++;
        }
    }

    Console.WriteLine($"Part 1 answer is: {fullOverlapCount}.");
}

async Task SolvePart2Async()
{
    int overlapCount = 0;
    using StreamReader file = File.OpenText("input.txt");
    while (!file.EndOfStream)
    {
        string? line = await file.ReadLineAsync();
        Debug.Assert(line != null);
        string[] parts = line.Split(",", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        int[] range1 = ParseRange(parts[0]);
        int[] range2 = ParseRange(parts[1]);

        if (range1.Intersect(range2).Any())
            overlapCount++;
    }

    Console.WriteLine($"Part 2 answer is: {overlapCount}.");
}

static int[] ParseRange(string s)
{
    int[] rangeBoundaries = s
        .Split("-", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
        .Select(int.Parse)
        .OrderBy(x => x)
        .ToArray();
    int[] range = Enumerable
        .Range(rangeBoundaries[0], rangeBoundaries[1] - rangeBoundaries[0] + 1)
        .ToArray();
    return range;
}
