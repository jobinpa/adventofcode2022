// https://adventofcode.com/2022/day/6
await SolvePart1Async();
await SolvePart2Async();

async Task SolvePart1Async()
{
    const int MARKER_LENGTH = 4;
    int charCounter = 0;
    List<char> seq = new();
    char[] fileBuffer = new char[1];

    using StreamReader file = File.OpenText("input.txt");
    while (!file.EndOfStream)
    {
        charCounter++;
        await file.ReadAsync(fileBuffer, 0, 1);
        seq.Add(fileBuffer[0]);

        if (seq.Count > MARKER_LENGTH)
            seq.RemoveAt(0);

        if (seq.Count == MARKER_LENGTH && seq.Distinct().Count() == MARKER_LENGTH)
            break;
    }

    Console.WriteLine($"Part 1 answer is: {charCounter}.");
}

async Task SolvePart2Async()
{
    const int MARKER_LENGTH = 14;
    int charCounter = 0;
    List<char> seq = new();
    char[] fileBuffer = new char[1];

    using StreamReader file = File.OpenText("input.txt");
    while (!file.EndOfStream)
    {
        charCounter++;
        await file.ReadAsync(fileBuffer, 0, 1);
        seq.Add(fileBuffer[0]);

        if (seq.Count > MARKER_LENGTH)
            seq.RemoveAt(0);

        if (seq.Count == MARKER_LENGTH && seq.Distinct().Count() == MARKER_LENGTH)
            break;
    }

    Console.WriteLine($"Part 2 answer is: {charCounter}.");
}
