// https://adventofcode.com/2022/day/10
await SolvePart1Async();
await SolvePart2Async();

async Task SolvePart1Async()
{
    int cycle = 0;
    int regx = 1;
    int sum = 0;

    using StreamReader file = File.OpenText("input.txt");
    while (!file.EndOfStream)
    {
        string line = (await file.ReadLineAsync())!;
        Instruction instruction = ParseInstruction(line);

        for (int x = 1; x <= instruction.CpuCycles; x++)
        {
            cycle++;

            // Cycle 20, the each 40th cycle after that.
            if (cycle == 20 || (cycle - 20) % 40 == 0)
                sum += cycle * regx;
        }

        // The instruction is processed AT THE END of the CPU cycle,
        // not DURING the CPU cycle.
        if (instruction.Type == InstructionType.ADDX)
            regx += instruction.Value;
    }

    Console.WriteLine($"Part 1 answer is: {sum}");
}

async Task SolvePart2Async()
{
    int cycle = 0;
    int regx = 1;

    using StreamReader file = File.OpenText("input.txt");
    while (!file.EndOfStream)
    {
        string line = (await file.ReadLineAsync())!;
        Instruction instruction = ParseInstruction(line);

        for (int x = 1; x <= instruction.CpuCycles; x++)
        {
            cycle++;

            // 40 pixels per line, cycle starts at 1, pixel index is zero-based,
            // so we need to subtract 1 from the cycle number before passing it
            // to the modulo operator.
            int crtColumnIdx = ((cycle - 1) % 40);

            // Column 0 means we are statring a new line.
            if (crtColumnIdx == 0)
                Console.WriteLine();

            // Sprite is 3 pixel wide, regx is the center of the sprite,
            // so sprite's left edge is regx - 1, sprite's right edge is regx + 1.
            bool isSpriteVisible = 
                crtColumnIdx >= regx - 1 && 
                crtColumnIdx <= regx + 1;
            Console.Write(isSpriteVisible ? '#' : '.');
        }

        // The instruction is processed AT THE END of the CPU cycle,
        // not DURING the CPU cycle.
        if (instruction.Type == InstructionType.ADDX)
            regx += instruction.Value;
    }
}

Instruction ParseInstruction(string s)
{
    string[] parts = s.Split(' ', 2, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

    switch (parts[0].ToUpperInvariant())
    {
        case "ADDX":
            return new Instruction(InstructionType.ADDX, int.Parse(parts[1]), 2);
        case "NOOP":
            return new Instruction(InstructionType.NOOP, 0, 1);
        default:
            throw new InvalidOperationException($"Unknown instruction: {s}");
    }
}

enum InstructionType
{
    NOOP,
    ADDX
}

record Instruction(InstructionType Type, int Value, int CpuCycles);