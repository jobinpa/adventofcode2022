// https://adventofcode.com/2022/day/16
using System.Text.RegularExpressions;

await SolvePart1Async();
await SolvePart2Async();

async Task SolvePart1Async()
{
    IDictionary<string, Room> rooms =
        await LoadInputAsync();

    Room[] roomsWithWorkingValve = rooms.Values
        .Where(x => x.ValveFlowRate > 0)
        .ToArray();

    IDictionary<Room, Path[]> bestPaths =
        CalculateShortestPathBetweenAllRooms(rooms);

    var i = CalculateBestItinerary(rooms["AA"], bestPaths, 30);
    Console.WriteLine($"Part 1 answer is: {i.Released}");
}

async Task SolvePart2Async()
{
    IDictionary<string, Room> rooms =
        await LoadInputAsync();

    Room[] roomsWithWorkingValve = rooms.Values
        .Where(x => x.ValveFlowRate > 0)
        .ToArray();

    IDictionary<Room, Path[]> bestPaths =
        CalculateShortestPathBetweenAllRooms(rooms);

    // Calculates our best itinerary.
    var i1 = CalculateBestItinerary(rooms["AA"], bestPaths, 26);

    // Calculates the elephant's best itinerary. We are interested only in
    // rooms we haven't visited yet.
    var i2 = CalculateBestItinerary(rooms["AA"], bestPaths, 26, i1.Visited);

    Console.WriteLine($"Part 2 answer is: {i1.Released + i2.Released}");
}

async Task<IDictionary<string, Room>> LoadInputAsync()
{
    Regex regex = new(
        @"\AValve (?<id>[A-Z]{2}) has flow rate=(?<flowRate>\d+); tunnel[s]? lead[s]? to valve[s]? (?<leadsTo>[A-Z]{2}(, [A-Z]{2})*)\z",
        RegexOptions.Compiled);

    List<Room> rooms = new();

    using StreamReader file = File.OpenText("input.txt");
    while (!file.EndOfStream)
    {
        string line = (await file.ReadLineAsync())!;
        Match match = regex.Match(line);
        if (!match.Success)
            throw new InvalidOperationException($"Invalid line: {line}");
        string id = match.Groups["id"].Value;
        int flowRate = int.Parse(match.Groups["flowRate"].Value);
        string[] leadsTo = match.Groups["leadsTo"].Value.Split(
            ',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        rooms.Add(new Room(id, flowRate, leadsTo));
    }

    return rooms.ToDictionary(x => x.Id, x => x);
}

Path CalculateShortestPath(
    Room start,
    Room end,
    IDictionary<string, Room> rooms)
{
    if (start == end)
        return new Path(new[] { start }, 0);

    HashSet<Room> visited = new();
    PriorityQueue<Path, int> pathQueue = new();
    pathQueue.Enqueue(new Path(new[] { start }, 0), 0);

    while (pathQueue.Count > 0)
    {
        Path currentPath = pathQueue.Dequeue();
        Room tail = currentPath.To();

        if (visited.Contains(tail))
            continue;
        visited.Add(tail);

        Path[] pathsToNeighbors = tail.LeadsTo
            .Select(x => rooms[x])
            .Select(x => new Path(currentPath.Rooms.Append(x).ToArray(), currentPath.Cost + 1))
            .ToArray();

        Path? pathToEnd = pathsToNeighbors.FirstOrDefault(x => x.Rooms.Last() == end);
        if (pathToEnd != null)
            return pathToEnd;

        pathQueue.EnqueueRange(pathsToNeighbors.Select(x => (x, x.Cost)));
    }

    throw new InvalidOperationException($"Unable to find a path from {start.Id} to {end.Id}");
}

IDictionary<Room, Path[]> CalculateShortestPathBetweenAllRooms(
    IDictionary<string, Room> rooms)
{
    Room start = rooms["AA"];

    Room[] roomsWithWorkingValve = rooms.Values
        .Where(x => x.ValveFlowRate > 0)
        .ToArray();

    Dictionary<(Room FirstRoom, Room SecondRoom), Path> shortestPaths = new();
    foreach (Room firstRoom in new[] { start }.Concat(roomsWithWorkingValve))
    {
        foreach (Room secondRoom in roomsWithWorkingValve)
        {
            if (firstRoom == secondRoom)
                continue;

            (Room FirstRoom, Room SecondRoom) roomPair = new(firstRoom, secondRoom);
            if (shortestPaths.ContainsKey(roomPair))
                continue;

            Path shortestPath = CalculateShortestPath(firstRoom, secondRoom, rooms);

            // Add both directions
            shortestPaths[roomPair] = shortestPath;
            shortestPaths[new(secondRoom, firstRoom)] = new Path(shortestPath.Rooms.Reverse().ToArray(), shortestPath.Cost);
        }
    }

    return shortestPaths.Values
        .GroupBy(p => p.From())
        .ToDictionary(
            g => g.Key,
            g => g.ToArray());
}

Itinerary CalculateBestItinerary(
    Room startRoom,
    IDictionary<Room, Path[]> shortestPaths,
     int time,
     IReadOnlyList<Room>? alreadyOpened = null)
{
    alreadyOpened ??= Array.Empty<Room>();
    Itinerary bestItinerary = MoveToNextRoom(startRoom, new Itinerary(new[] { startRoom }, time, 0));
    return bestItinerary;

    Itinerary MoveToNextRoom(Room room, Itinerary itinerary)
    {
        Itinerary bestItinerary = itinerary;

        foreach (Path path in shortestPaths[room]
            .Where(x => (x.Cost + 1) < itinerary.RemainingTime)
            .Where(x => !alreadyOpened.Contains(x.To()))
            .Where(p => !itinerary.Visited.Contains(p.To())))
        {
            int deplacementCost = path.Cost + 1;
            int released2 = 
                itinerary.Visited.Sum(x => (x.ValveFlowRate) * deplacementCost) + 
                itinerary.Released;
            int remainingTime2 = itinerary.RemainingTime - deplacementCost;
            Itinerary itinerary2 = new Itinerary(
                itinerary.Visited.Append(path.To()).ToList(),
                remainingTime2,
                released2);
            itinerary2 = MoveToNextRoom(path.To(), itinerary2);

            if (bestItinerary == null || itinerary2.Released > bestItinerary.Released)
                bestItinerary = itinerary2;
        }

        // At this point either all rooms have been visited or there is no more time left.
        // If there is still some time left, we need to update the amount of released pressure.
        int finalReleased = 
            (bestItinerary.Visited.Sum(x => x.ValveFlowRate) * bestItinerary.RemainingTime) + 
            bestItinerary.Released;
        return new Itinerary(bestItinerary.Visited, 0, finalReleased);
    }
}

record Itinerary(IReadOnlyList<Room> Visited, int RemainingTime, int Released);
record Room(string Id, int ValveFlowRate, IReadOnlyList<string> LeadsTo);
record Path(IReadOnlyList<Room> Rooms, int Cost);

static class RoomExtensions
{
    public static Room From(this Path path) => path.Rooms.First();
    public static Room To(this Path path) => path.Rooms.Last();
}