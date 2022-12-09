// https://adventofcode.com/2022/day/9
await SolvePart1Async();
await SolvePart2Async();

async Task SolvePart1Async()
{
    Rope rope = new Rope(new[] { new Point(0, 0), new Point(0, 0) });

    using StreamReader file = File.OpenText("input.txt");
    while (!file.EndOfStream)
    {
        string line = (await file.ReadLineAsync())!;
        Motion motion = ParseMotion(line);

        for (int d = 1; d <= motion.Distance; d++)
            rope.Move(motion.Direction);
    }

    Console.WriteLine($"Part 1 answer is: {rope.Trail.Distinct().Count()}");
}

async Task SolvePart2Async()
{
    Rope rope = new Rope(new[] {
        new Point(0, 0), new Point(0, 0), new Point(0, 0), new Point(0, 0),
        new Point(0, 0), new Point(0, 0), new Point(0, 0), new Point(0, 0),
        new Point(0, 0), new Point(0, 0)});

    using StreamReader file = File.OpenText("input.txt");
    while (!file.EndOfStream)
    {
        string line = (await file.ReadLineAsync())!;
        Motion motion = ParseMotion(line);

        for (int d = 1; d <= motion.Distance; d++)
            rope.Move(motion.Direction);
    }

    Console.WriteLine($"Part 2 answer is: {rope.Trail.Distinct().Count()}");
}

Motion ParseMotion(string s)
{
    string[] parts = s.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    if (parts.Length != 2)
        throw new InvalidOperationException($"Invalid motion: {s}");

    switch (parts[0].ToUpperInvariant())
    {
        case "U": return new Motion(Direction.Up, int.Parse(parts[1]));
        case "R": return new Motion(Direction.Right, int.Parse(parts[1]));
        case "D": return new Motion(Direction.Down, int.Parse(parts[1]));
        case "L": return new Motion(Direction.Left, int.Parse(parts[1]));
        default: throw new InvalidOperationException($"Invalid motion: {s}");
    }
}

class Rope
{
    private readonly List<Point> _knots;
    private List<Point> _trail;

    public Rope(IList<Point> knots)
    {
        if (knots.Count < 2)
            throw new ArgumentException("The rope must have at least two knots.", nameof(knots));
        _knots = new List<Point>(knots);
        _trail = new List<Point> { knots.Last() };
    }

    public IReadOnlyList<Point> Knots => _knots;
    public IReadOnlyList<Point> Trail => _trail;

    public void Move(Direction direction)
    {
        Point head = _knots[0];
        switch (direction)
        {
            case Direction.Up: _knots[0] = new Point(head.X, head.Y + 1); break;
            case Direction.Right: _knots[0] = new Point(head.X + 1, head.Y); break;
            case Direction.Down: _knots[0] = new Point(head.X, head.Y - 1); break;
            case Direction.Left: _knots[0] = new Point(head.X - 1, head.Y); break;
            default: throw new InvalidOperationException($"Invalid direction: {direction}");
        }

        for (int i = 1; i < _knots.Count; i++)
        {
            Point prevKnot = _knots[i - 1];
            Point knot = _knots[i];

            if (Math.Abs(prevKnot.X - knot.X) > 1 || Math.Abs(prevKnot.Y - knot.Y) > 1)
            {
                int vx = knot.X == prevKnot.X
                    ? 0
                    : prevKnot.X > knot.X
                        ? 1    // Moving right
                        : -1;  // Moving left

                int vy = knot.Y == prevKnot.Y
                    ? 0
                    : prevKnot.Y > knot.Y
                        ? 1    // Moving up
                        : -1;  // Moving down

                _knots[i] = new Point(knot.X + vx, knot.Y + vy);

                if (i == _knots.Count - 1)
                    _trail.Add(_knots[i]);
            }
        }
    }
}

enum Direction
{
    Up,
    Right,
    Down,
    Left
}

record Point(int X, int Y);
record Motion(Direction Direction, int Distance);
