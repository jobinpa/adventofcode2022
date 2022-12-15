// https://adventofcode.com/2022/day/14
await SolvePart1Async();
await SolvePart2Async();

async Task SolvePart1Async()
{
    IDictionary<Point, CellType> cells = await LoadInputAsync();
    Point source = new(500, 0);
    cells.Add(source, CellType.Source);
    int maxX = cells.Keys.Max(p => p.X);
    int maxY = cells.Keys.Max(p => p.Y);

    while (true)
    {
        Point sandBlock = source;
        while (IsWithinBoudaries(sandBlock))
        {
            if (CanFallStraightDown(sandBlock))
                sandBlock = MoveStraightDown(sandBlock);
            else if (CanFallDownLeft(sandBlock))
                sandBlock = MoveDownLeft(sandBlock);
            else if (CanFallDownRight(sandBlock))
                sandBlock = MoveDownRight(sandBlock);
            else
                break;
        }

        if (!IsWithinBoudaries(sandBlock))
            break;
        if (IsCellEmpty(sandBlock))
            cells.Add(sandBlock, CellType.Sand);
    }

    int sandBlockCount = cells.Values.Count(v => v == CellType.Sand);
    Console.WriteLine($"Part 1 answer is: {sandBlockCount}");

    Point MoveStraightDown(Point p) => p with { Y = p.Y + 1 };
    Point MoveDownLeft(Point p) => p with { X = p.X - 1, Y = p.Y + 1 };
    Point MoveDownRight(Point p) => p with { X = p.X + 1, Y = p.Y + 1 };
    bool CanFallStraightDown(Point p) => IsCellEmpty(p with { Y = p.Y + 1 });
    bool CanFallDownLeft(Point p) => IsCellEmpty(p with { X = p.X - 1, Y = p.Y + 1 });
    bool CanFallDownRight(Point p) => IsCellEmpty(p with { X = p.X + 1, Y = p.Y + 1 });
    bool IsWithinBoudaries(Point p) => p.X >= 0 && p.X <= maxX && p.Y >= 0 && p.Y <= maxY;
    bool IsCellEmpty(Point p) => !IsWithinBoudaries(p) || !cells.ContainsKey(p);
}

async Task SolvePart2Async()
{
    IDictionary<Point, CellType> cells = await LoadInputAsync();
    Point source = new(500, 0);
    cells.Add(source, CellType.Source);
    int floor = cells.Keys.Max(p => p.Y) + 2;

    while (true)
    {
        Point sandBlock = source;
        while (IsWithinBoudaries(sandBlock))
        {
            if (CanFallStraightDown(sandBlock))
                sandBlock = MoveStraightDown(sandBlock);
            else if (CanFallDownLeft(sandBlock))
                sandBlock = MoveDownLeft(sandBlock);
            else if (CanFallDownRight(sandBlock))
                sandBlock = MoveDownRight(sandBlock);
            else
                break;
        }

        if (sandBlock == source)
        {
            cells[sandBlock] = CellType.Sand;
            break;
        }

        if (IsCellEmpty(sandBlock))
            cells.Add(sandBlock, CellType.Sand);
    }

    int sandBlockCount = cells.Values.Count(v => v == CellType.Sand);
    Console.WriteLine($"Part 2 answer is: {sandBlockCount}");

    Point MoveStraightDown(Point p) => p with { Y = p.Y + 1 };
    Point MoveDownLeft(Point p) => p with { X = p.X - 1, Y = p.Y + 1 };
    Point MoveDownRight(Point p) => p with { X = p.X + 1, Y = p.Y + 1 };
    bool CanFallStraightDown(Point p) => IsCellEmpty(p with { Y = p.Y + 1 });
    bool CanFallDownLeft(Point p) => IsCellEmpty(p with { X = p.X - 1, Y = p.Y + 1 });
    bool CanFallDownRight(Point p) => IsCellEmpty(p with { X = p.X + 1, Y = p.Y + 1 });
    bool IsWithinBoudaries(Point p) => p.Y < floor;
    bool IsCellEmpty(Point p) => IsWithinBoudaries(p) && !cells.ContainsKey(p);
}

async Task<IDictionary<Point, CellType>> LoadInputAsync()
{
    Dictionary<Point, CellType> rocks = new();

    using StreamReader file = File.OpenText("input.txt");
    while (!file.EndOfStream)
    {
        string? line = (await file.ReadLineAsync())!;
        string[] segments = line.Split("->", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        for (int i = 0; i < segments.Length; i++)
        {
            string[] fromXY = segments[i].Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            string[] toXY = i + 1 < segments.Length
                ? segments[i + 1].Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                : fromXY;

            int fromX = int.Parse(fromXY[0]);
            int fromY = int.Parse(fromXY[1]);
            int toX = int.Parse(toXY[0]);
            int toY = int.Parse(toXY[1]);

            int minX = Math.Min(fromX, toX);
            int maxX = Math.Max(fromX, toX);
            int minY = Math.Min(fromY, toY);
            int maxY = Math.Max(fromY, toY);

            for (int x = minX; x <= maxX; x++)
                for (int y = minY; y <= maxY; y++)
                    rocks.TryAdd(new Point(x, y), CellType.Rock);
        }
    }

    return rocks;
}

enum CellType { Rock, Sand, Source }
record Point(int X, int Y);