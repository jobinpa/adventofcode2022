// https://adventofcode.com/2022/day/1
await SolvePart1Async();
await SolvePart2Async();

async Task SolvePart1Async()
{
    int calorieCounter = 0;
    int maxCalories = 0;

    using StreamReader file = File.OpenText("input.txt");
    while (!file.EndOfStream)
    {
        string? line = await file.ReadLineAsync();
        if (file.EndOfStream || string.IsNullOrWhiteSpace(line))
        {
            if (calorieCounter > maxCalories)
                maxCalories = calorieCounter;
            calorieCounter = 0;
        }
        else
        {
            calorieCounter += int.Parse(line);
        }
    }

    Console.WriteLine($"Part 1 answer is: {maxCalories}");
}

async Task SolvePart2Async()
{
    int calorieCounter = 0;
    int[] topMaxCalories = new int[] { 0, 0, 0 };

    using StreamReader file = File.OpenText("input.txt");
    while (!file.EndOfStream)
    {
        string? line = await file.ReadLineAsync();
        if (file.EndOfStream || string.IsNullOrWhiteSpace(line))
        {
            for (int i = 0; i < topMaxCalories.Length; i++)
            {
                if (calorieCounter > topMaxCalories[i])
                {
                    topMaxCalories[i] = calorieCounter;
                    Array.Sort(topMaxCalories);
                    break;
                }
            }

            calorieCounter = 0;
        }
        else
        {
            calorieCounter += int.Parse(line);
        }
    }

    Console.WriteLine($"Part 2 answer is: {topMaxCalories.Sum()}");
}