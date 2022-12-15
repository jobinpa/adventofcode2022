// https://adventofcode.com/2022/day/15
using System.Text.RegularExpressions;

await SolvePart1Async();
await SolvePart2Async();

async Task SolvePart1Async()
{
    const int TARGET_Y = 2000000;

    ICollection<Sensor> sensors = await LoadInputAsync();

    int numberOfOccupiedCells =
        sensors
            .Select(x => x.NearestBeacon)
            .Where(x => x.Y == TARGET_Y)
            .Union(
                sensors
                    .Select(x => x.Location)
                    .Where(x => x.Y == TARGET_Y)
            ).Count();

    List<ValueRange> sensorXCoverages = sensors
        .Select(p => GetXSensorCoverageAtY(p, TARGET_Y))
        .Where(s => s != null)
        .OrderBy(s => s!.MinValue)
        .ToList()!;

    int numberOfPositionsWithoutBeacon = 0;
    int? highestPositionCoveredSoFar = null;
    foreach (ValueRange range in sensorXCoverages)
    {
        int startAt = highestPositionCoveredSoFar.HasValue
            ? Math.Max(range.MinValue, highestPositionCoveredSoFar.Value + 1)
            : range.MinValue;
        int endAt = range.MaxValue;
        if (endAt <= highestPositionCoveredSoFar)
            continue;
        numberOfPositionsWithoutBeacon += endAt - startAt + 1;
        highestPositionCoveredSoFar = endAt;
    }

    Console.WriteLine($"Part 1 answer is: {numberOfPositionsWithoutBeacon - numberOfOccupiedCells}");
}

async Task SolvePart2Async()
{
    Point boundingBoxUpperLeftCorner = new Point(0, 0);
    Point boudingBoxLowerRightCorner = new Point(4000000, 4000000);
    ICollection<Sensor> sensors = await LoadInputAsync();

    Point? beacon = null;
    for (int y = boundingBoxUpperLeftCorner.Y; y <= boudingBoxLowerRightCorner.Y; y++)
    {
        List<ValueRange> sensorXCoverages = sensors
            .Select(p => GetXSensorCoverageAtY(p, y))
            .Where(s => s != null)
            .OrderBy(s => s!.MinValue)
            .ToList()!;

        int x = 0;
        foreach (ValueRange range in sensorXCoverages)
        {
            // Sensor coverage starts after current x position. Current position
            // is not covered by any sensor and is our beacon.
            if (range.MinValue > x)
            {
                beacon = new Point(x, y);
                break;
            }

            // Move current X position after current sensor coverage.
            if (x <= range.MaxValue)
                x = range.MaxValue + 1;
        }

        // We have reach the end of the row area covered by sensors. If x is
        // still inside the bouding box at this stage, and if the beacon hasn't
        // been found yet, then the current x position is our beacon.
        if (beacon != null && x <= boudingBoxLowerRightCorner.X)
            beacon = new Point(x, y);

        if (beacon != null)
            break;
    }

    if (beacon == null)
        throw new InvalidOperationException("No beacon found");

    long frecuency = beacon.X * 4000000L + beacon.Y;
    Console.WriteLine($"Part 2 answer is: {frecuency}");
}

async Task<ICollection<Sensor>> LoadInputAsync()
{
    Regex regex = new Regex(
        @"\ASensor at x=(?<sx>-?\d+), y=(?<sy>-?\d+): closest beacon is at x=(?<bx>-?\d+), y=(?<by>-?\d+)\z",
        RegexOptions.Compiled);
    List<Sensor> sensorBeaconPairs = new();

    using StreamReader file = File.OpenText("input.txt");
    while (!file.EndOfStream)
    {
        string? line = (await file.ReadLineAsync())!;

        Match match = regex.Match(line);
        if (!match.Success)
            throw new InvalidOperationException($"Invalid input line: {line}");

        Point sensor = new Point(int.Parse(match.Groups["sx"].Value), int.Parse(match.Groups["sy"].Value));
        Point beacon = new Point(int.Parse(match.Groups["bx"].Value), int.Parse(match.Groups["by"].Value));
        sensorBeaconPairs.Add(new Sensor(sensor, beacon, CalculateManhattanDistance(sensor, beacon)));
    }

    return sensorBeaconPairs;
}

int CalculateManhattanDistance(Point a, Point b) => Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);

ValueRange? GetXSensorCoverageAtY(Sensor sensor, int y)
{
    int yDistance = Math.Abs(y - sensor.Location.Y);
    if (yDistance > sensor.Distance)
        return null;
    int residualDistance = sensor.Distance - yDistance;
    return new ValueRange(sensor.Location.X - residualDistance, sensor.Location.X + residualDistance);
}

record Point(int X, int Y);
record Sensor(Point Location, Point NearestBeacon, int Distance);
record ValueRange(int MinValue, int MaxValue);
