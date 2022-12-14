// https://adventofcode.com/2022/day/13
await SolvePart1Async();
await SolvePart2Async();

async Task SolvePart1Async()
{
    int sum = 0;
    int currentPairNumber = 0;

    using StreamReader file = File.OpenText("input.txt");
    while (!file.EndOfStream)
    {
        string l1 = (await file.ReadLineAsync())!;
        if (string.IsNullOrEmpty(l1))
            continue;
        IPacket p1 = ParseListPacket(l1);

        string l2 = (await file.ReadLineAsync())!;
        IPacket p2 = ParseListPacket(l2);

        currentPairNumber++;
        if (p1.CompareTo(p2) <= 0)
            sum += currentPairNumber;
    }

    Console.WriteLine($"Part 1 answer is: {sum}");
}

async Task SolvePart2Async()
{
    List<IPacket> packets = new();

    using StreamReader file = File.OpenText("input.txt");
    while (!file.EndOfStream)
    {
        string l = (await file.ReadLineAsync())!;
        if (string.IsNullOrEmpty(l))
            continue;
        packets.Add(ParseListPacket(l));
    }

    IPacket d1 = new ListPacket(new ListPacket(new NumberPacket(2)));
    packets.Add(d1);

    IPacket d2 = new ListPacket(new ListPacket(new NumberPacket(6)));
    packets.Add(d2);

    packets = packets.OrderBy(x => x).ToList();

    // Packet indexes are one-based
    int d1Idx = packets.IndexOf(d1) + 1;
    int d2Idx = packets.IndexOf(d2) + 1;
    int decoderKey = d1Idx * d2Idx;

    Console.WriteLine($"Part 2 answer is: {decoderKey}");
}

IPacket ParseListPacket(string s)
{
    ArgumentNullException.ThrowIfNull(s, nameof(s));
    if (string.IsNullOrEmpty(s) || s[0] != '[')
        throw new InvalidOperationException($"Invalid list packet: {s}");
    return ParseListPacketInternal(s.Select(c => c).ToList());

    IPacket ParseListPacketInternal(IList<char> chrs)
    {
        chrs.RemoveAt(0); // Remove '['
        List<IPacket> packets = new();
        string numberStr = string.Empty;

        while (chrs.Any())
        {
            if (chrs[0] == '[')
            {
                packets.Add(ParseListPacketInternal(chrs));
            }
            else if (chrs[0] == ']' || chrs[0] == ',')
            {
                if (numberStr.Length > 0)
                {
                    packets.Add(new NumberPacket(int.Parse(numberStr)));
                    numberStr = string.Empty;
                }

                bool atEndOfList = chrs[0] == ']';
                chrs.RemoveAt(0);

                if (atEndOfList)
                    return new ListPacket(packets);
            }
            else if (chrs[0] >= '0' && chrs[0] <= '9')
            {
                numberStr += chrs[0];
                chrs.RemoveAt(0);
            }
            else
            {
                throw new InvalidOperationException($"Unsupported char '{chrs[0]}'");
            }
        }

        throw new InvalidOperationException($"Invalid list packet: {s}");
    }
}

interface IPacket : IComparable<IPacket> { }

sealed class NumberPacket : IPacket
{
    public NumberPacket(int number)
    {
        Number = number;
    }

    public int Number { get; }
    public override string ToString() => Number.ToString();

    public int CompareTo(IPacket? other)
    {
        if (other == null)
            return 1;

        if (other is NumberPacket otherNumber)
            return Number.CompareTo(otherNumber.Number);

        if (other is ListPacket l)
            return new ListPacket(this).CompareTo(l);

        throw new NotSupportedException($"Packet type {other.GetType()} is not supported");
    }
}

sealed class ListPacket : IPacket
{
    public ListPacket(IReadOnlyList<IPacket> packets)
    {
        Packets = packets ?? throw new ArgumentNullException(nameof(packets));
    }

    public ListPacket(IPacket packets, params IPacket[] otherPackets)
    {
        ArgumentNullException.ThrowIfNull(packets, nameof(packets));
        Packets = otherPackets == null
            ? new[] { packets }
            : new[] { packets }.Concat(otherPackets).ToList();
    }

    public IReadOnlyList<IPacket> Packets { get; }

    public override string ToString() => $"[{string.Join(",", Packets)}]";

    public int CompareTo(IPacket? other)
    {
        if (other == null)
            return 1;

        if (other is ListPacket otherList)
        {
            for (int i = 0; i < Packets.Count && i < otherList.Packets.Count; i++)
            {
                int compareResult = Packets[i].CompareTo(otherList.Packets[i]);
                if (compareResult != 0)
                    return compareResult;
            }

            if (Packets.Count < otherList.Packets.Count)
                return -1;

            if (Packets.Count > otherList.Packets.Count)
                return 1;

            return 0;
        }

        if (other is NumberPacket n)
            return CompareTo(new ListPacket(n));

        throw new NotSupportedException($"Packet type {other.GetType()} is not supported");
    }
}