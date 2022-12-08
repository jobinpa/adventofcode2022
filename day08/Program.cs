// https://adventofcode.com/2022/day/8
await SolvePart1Async();
await SolvePart2Async();

async Task SolvePart1Async()
{
    byte[][] data = await LoadFileAsync();
    bool[][] visibility = InitializeVisibilityArray(data);
    ApplyVisibilityHorizontally(data, visibility);
    ApplyVisibilityVertically(data, visibility);
    int visibleCount = visibility.SelectMany(r => r).Count(v => v);
    Console.WriteLine($"Part 1 answer is: {visibleCount}.");
}

async Task SolvePart2Async()
{
    byte[][] data = await LoadFileAsync();
    int[][] scenicScores = InitializeScenicScoreArray(data);

    int rowCount = data[0].Length;
    int columnCount = rowCount == 0 ? 0 : data[0].Length;

    // Ignore trees that are located on an edge of the forest.
    // These trees have at least one of their side with a scenic score of 0,
    // which would make the total scenic score 0.
    for (int r = 1; r < rowCount - 1; r++)
    {
        for (int c = 1; c < columnCount - 1; c++)
        {
            int scoreUp = CalculateScenicScoreUp(r, c, data);
            int scoreLeft = CalculateScenicScoreLeft(r, c, data);
            int scoreRight = CalculateScenicScoreRight(r, c, data);
            int scoreDown = CalculateScenicScoreDown(r, c, data);
            scenicScores[r][c] = scoreUp * scoreLeft * scoreRight * scoreDown;
        }
    }

    int maxScenicScore = scenicScores.SelectMany(r => r).Max();
    Console.WriteLine($"Part 2 answer is: {maxScenicScore}.");
}

async Task<byte[][]> LoadFileAsync()
{
    List<IList<byte>> data = new();

    using StreamReader file = File.OpenText("input.txt");
    while (!file.EndOfStream)
    {
        string line = (await file.ReadLineAsync())!;
        data.Add(line.Select(c => byte.Parse(c.ToString())).ToList());
    }

    return data.Select(r => r.ToArray()).ToArray();
}

bool[][] InitializeVisibilityArray(byte[][] data)
{
    int rowCount = data[0].Length;
    int columnCount = rowCount == 0 ? 0 : data[0].Length;

    bool[][] visibility = new bool[rowCount][];
    for (int i = 0; i < rowCount; i++)
        visibility[i] = new bool[columnCount];

    return visibility;
}

void ApplyVisibilityHorizontally(byte[][] data, bool[][] visibility)
{
    int rowCount = data[0].Length;
    int columnCount = rowCount == 0 ? 0 : data[0].Length;

    for (int r = 0; r < rowCount; r++)
    {
        int highestValue = -1;
        for (int c = 0; c < columnCount; c++)
        {
            if (data[r][c] > highestValue)
            {
                visibility[r][c] = true;
                highestValue = data[r][c];
            }
        }

        highestValue = -1;
        for (int c = columnCount - 1; c >= 0; c--)
        {
            if (data[r][c] > highestValue)
            {
                visibility[r][c] = true;
                highestValue = data[r][c];
            }
        }
    }
}

void ApplyVisibilityVertically(byte[][] data, bool[][] visibility)
{
    int rowCount = data[0].Length;
    int columnCount = rowCount == 0 ? 0 : data[0].Length;

    for (int c = 0; c < columnCount; c++)
    {
        int highestValue = -1;
        for (int r = 0; r < rowCount; r++)
        {
            if (data[r][c] > highestValue)
            {
                visibility[r][c] = true;
                highestValue = data[r][c];
            }
        }

        highestValue = -1;
        for (int r = rowCount - 1; r >= 0; r--)
        {
            if (data[r][c] > highestValue)
            {
                visibility[r][c] = true;
                highestValue = data[r][c];
            }
        }
    }
}

int[][] InitializeScenicScoreArray(byte[][] data)
{
    int rowCount = data[0].Length;
    int columnCount = rowCount == 0 ? 0 : data[0].Length;

    int[][] scenicScore = new int[rowCount][];
    for (int i = 0; i < rowCount; i++)
        scenicScore[i] = new int[rowCount];

    return scenicScore;
}

int CalculateScenicScoreLeft(int row, int column, byte[][] data)
{
    int rowCount = data[0].Length;
    int columnCount = rowCount == 0 ? 0 : data[0].Length;

    int leftScore = 0;
    for (int c = column - 1; c >= 0; c--)
    {
        leftScore++;
        if (data[row][c] >= data[row][column])
            break;
    }

    return leftScore;
}

int CalculateScenicScoreRight(int row, int column, byte[][] data)
{
    int rowCount = data[0].Length;
    int columnCount = rowCount == 0 ? 0 : data[0].Length;

    int rightScore = 0;
    for (int c = column + 1; c < columnCount; c++)
    {
        rightScore++;
        if (data[row][c] >= data[row][column])
            break;
    }

    return rightScore;
}

int CalculateScenicScoreUp(int row, int column, byte[][] data)
{
    int rowCount = data[0].Length;
    int columnCount = rowCount == 0 ? 0 : data[0].Length;

    int upScore = 0;
    for (int r = row - 1; r >= 0; r--)
    {
        upScore++;
        if (data[r][column] >= data[row][column])
            break;
    }

    return upScore;
}

int CalculateScenicScoreDown(int row, int column, byte[][] data)
{
    int rowCount = data[0].Length;
    int columnCount = rowCount == 0 ? 0 : data[0].Length;

    int downScore = 0;
    for (int r = row + 1; r < rowCount; r++)
    {
        downScore++;
        if (data[r][column] >= data[row][column])
            break;
    }

    return downScore;
}