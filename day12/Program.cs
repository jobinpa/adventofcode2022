// https://adventofcode.com/2022/day/12
await SolvePart1Async();
await SolvePart2Async();

async Task SolvePart1Async()
{
    (int[][] elevations, Point start, Point end) = await LoadInputAsync();

    Func<Point, bool> isEnd = p => p == end;

    Func<Point, Point, bool> canMoveTo = (from, to) =>
        IsValidPoint(to, elevations) &&
        elevations[to.Y][to.X] <= elevations[from.Y][from.X] + 1;

    int cost = CalculateShortestPathCost(start, isEnd, canMoveTo);
    Console.WriteLine($"Part 1 answer is: {cost}.");
}

async Task SolvePart2Async()
{
    (int[][] elevations, Point _, Point end) = await LoadInputAsync();

    Func<Point, bool> isEnd = p => elevations[p.Y][p.X] == 0;

    Func<Point, Point, bool> canMoveTo = (from, to) =>
        IsValidPoint(to, elevations) &&
        elevations[to.Y][to.X] + 1 >= elevations[from.Y][from.X];

    int cost = CalculateShortestPathCost(end, isEnd, canMoveTo);
    Console.WriteLine($"Part 2 answer is: {cost}.");
}

async Task<Input> LoadInputAsync()
{
    IDictionary<char, int> charToElevationMap = "abcdefghijklmnopqrstuvwxyz"
        .Select((x, i) => new { x, i })
        .ToDictionary(x => x.x, x => x.i);
    charToElevationMap['S'] = charToElevationMap['a'];
    charToElevationMap['E'] = charToElevationMap['z'];

    List<int[]> elevations = new();
    Point? start = null;
    Point? end = null;

    using StreamReader file = File.OpenText("input.txt");
    while (!file.EndOfStream)
    {
        string line = (await file.ReadLineAsync())!;
        elevations.Add(new int[line.Length]);
        int y = elevations.Count - 1;

        for (int x = 0; x < line.Length; x++)
        {
            char c = line[x];
            elevations[y][x] = charToElevationMap[c];

            if (c == 'E')
                end = end == null
                    ? new Point(x, y)
                    : throw new InvalidOperationException("More than one end point found.");

            if (c == 'S')
                start = start == null
                    ? new Point(x, y)
                    : throw new InvalidOperationException("More than one start point found.");
        }
    }

    return new Input(elevations.ToArray(), start!, end!);
}

bool IsValidPoint(Point point, int[][] arr) =>
        point.X >= 0 &&
        point.Y >= 0 &&
        point.X < (arr.Length == 0 ? 0 : arr[0].Length) &&
        point.Y < arr.Length;

int CalculateShortestPathCost(
    Point start,
    Func<Point, bool> isEnd,
    Func<Point, Point, bool> isMoveAllowed)
{
    if (isEnd(start))
        return 0;

    HashSet<Point> visited = new();
    List<Path> pathQueue = new() { new Path(start, 0) };

    while (pathQueue.Any())
    {
        Path currentPath = pathQueue[0];
        pathQueue.RemoveAt(0);

        if (visited.Contains(currentPath.Tail))
            continue;
        visited.Add(currentPath.Tail);

        Path[] pathsToNeighbors = currentPath.Tail.GetNeighbors()
            .Where(p => isMoveAllowed(currentPath.Tail, p))
            .Select(p => new Path(p, currentPath.Cost + 1))
            .ToArray();

        if (pathsToNeighbors.Any(x => isEnd(x.Tail)))
            return currentPath.Cost + 1;

        pathQueue = pathQueue.Concat(pathsToNeighbors).OrderBy(x => x.Cost).ToList();
    }

    throw new InvalidOperationException($"Unable to find path to target.");
}

record Point(int X, int Y);
record Input(int[][] Elevations, Point Start, Point End);
record Path(Point Tail, int Cost);

static class PointExtensions
{
    public static ICollection<Point> GetNeighbors(this Point point) => new List<Point>
        {
            new Point(point.X -1, point.Y),
            new Point(point.X + 1, point.Y),
            new Point(point.X, point.Y - 1),
            new Point(point.X, point.Y + 1)
        };
}