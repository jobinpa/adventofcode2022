// https://adventofcode.com/2022/day/2
using System.Diagnostics;

await SolvePart1Async();
await SolvePart2Async();

async Task SolvePart1Async()
{
    int playerScore = 0;

    using StreamReader file = File.OpenText("input.txt");
    while (!file.EndOfStream)
    {
        string? line = await file.ReadLineAsync();
        Debug.Assert(line != null);
        string[] parts = line.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        HandShape elfHand = parts[0].ToUpperInvariant() switch
        {
            "A" => new Rock(),
            "B" => new Paper(),
            "C" => new Scissors(),
            _ => throw new NotImplementedException()
        };

        HandShape playerHand = parts[1].ToUpperInvariant() switch
        {
            "X" => new Rock(),
            "Y" => new Paper(),
            "Z" => new Scissors(),
            _ => throw new NotImplementedException()
        };

        RoundOutcome outcome = playerHand.Play(elfHand);
        playerScore += playerHand.GetScore() + outcome.GetScore();
    }

    Console.WriteLine($"Part 1 answer is: {playerScore}");
}

async Task SolvePart2Async()
{
    int playerScore = 0;

    using StreamReader file = File.OpenText("input.txt");
    while (!file.EndOfStream)
    {
        string? line = await file.ReadLineAsync();
        Debug.Assert(line != null);
        string[] parts = line.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        HandShape elfHand = parts[0].ToUpperInvariant() switch
        {
            "A" => new Rock(),
            "B" => new Paper(),
            "C" => new Scissors(),
            _ => throw new NotImplementedException()
        };

        RoundOutcome outcome = parts[1].ToUpperInvariant() switch
        {
            "X" => RoundOutcome.Lost,
            "Y" => RoundOutcome.Draw,
            "Z" => RoundOutcome.Won,
            _ => throw new NotImplementedException()
        };

        HandShape playerHand = elfHand.GetOtherHand(outcome.Invert());
        playerScore += playerHand.GetScore() + outcome.GetScore();
    }

    Console.WriteLine($"Part 2 answer is: {playerScore}");
}
