using System.Text.RegularExpressions;
using ElfFSComponents;

const int FS_TOTAL_SPACE = 70_000_000;
Regex commandRegex = new Regex(@"\A\$[ ]*(?<cmd>cd|ls)[ ]*(?:[ ](?<param>[A-z0-9\.\/]+))?\z");
Regex directoryEntryRegex = new Regex(@"\A(?<size>[0-9]+|dir) (?<name>.+)\z");

// https://adventofcode.com/2022/day/7
await SolvePart1Async();
await SolvePart2Async();

async Task SolvePart1Async()
{
    ElfFS fs = await LoadFileSystemAsync();
    ICollection<ElfFSDirectory> directories = GetDirectoriesRecursive(fs.Root);
    int sumOfDirectorySizes = directories
        .Select(d => d.CalculateSize())
        .Where(s => s <= 100_000)
        .Sum();
    Console.WriteLine($"Part 1 answer is: {sumOfDirectorySizes}.");
}

async Task SolvePart2Async()
{
    const int TARGET_FREE_SPACE = 30_000_000;
    ElfFS fs = await LoadFileSystemAsync();
    int spaceToReclaim = TARGET_FREE_SPACE - fs.FreeSpace;
    ICollection<ElfFSDirectory> directories = GetDirectoriesRecursive(fs.Root);
    int minDirectorySize = directories
        .Select(d => d.CalculateSize())
        .Where(s => s >= spaceToReclaim)
        .Min();
    Console.WriteLine($"Part 2 answer is: {minDirectorySize}.");
}

async Task<ElfFS> LoadFileSystemAsync()
{
    ElfFS fs = new(FS_TOTAL_SPACE);
    ElfFSDirectory currentDirectory = fs.Root;

    bool isListing = false;
    using StreamReader file = File.OpenText("input.txt");
    while (!file.EndOfStream)
    {
        string line = (await file.ReadLineAsync())!;

        if (TryParseAsCommand(line, out Command? cmd))
        {
            switch (cmd!.CommandType)
            {
                case CommandType.ChangeDirectory:
                    isListing = false;
                    if (!fs.DirectoryExists(currentDirectory, cmd.Parameter!))
                        fs.CreateDirectory(currentDirectory, cmd.Parameter!);
                    currentDirectory = fs.GetDirectory(currentDirectory, cmd.Parameter!);
                    break;
                case CommandType.ListDirectory:
                    isListing = true;
                    break;
            }

            continue;
        }

        if (!isListing)
            throw new InvalidOperationException($"Invalid command line: {line}.");

        if (!TryParseDirectoryEntry(line, out DirectoryEntry? item))
            throw new InvalidOperationException($"Invalid directory entry line: {line}.");

        if (item!.IsDirectory && !fs.DirectoryExists(currentDirectory, item.Name))
        {
            fs.CreateDirectory(currentDirectory, item.Name);
        }
        else if (!fs.FileExists(currentDirectory, item.Name))
        {
            fs.CreateFile(currentDirectory, item.Name, item.Size);
        }
    }

    return fs;
}

ICollection<ElfFSDirectory> GetDirectoriesRecursive(ElfFSDirectory d)
{
    List<ElfFSDirectory> result = new();
    result.Add(d);

    foreach (ElfFSDirectory directory in d.Items.Where(i => i is ElfFSDirectory).Cast<ElfFSDirectory>())
        result.AddRange(GetDirectoriesRecursive(directory));

    return result;
}

bool TryParseAsCommand(string line, out Command? parsed)
{
    Match match = commandRegex.Match(line);
    if (!match.Success)
    {
        parsed = null;
        return false;
    }

    string command = match.Groups["cmd"].Value;
    string param = match.Groups["param"].Value;

    if (command == "cd")
    {
        parsed = new Command(CommandType.ChangeDirectory, param);
        return true;
    }

    if (command == "ls")
    {
        parsed = new Command(CommandType.ListDirectory, null);
        return true;
    }

    parsed = null;
    return false;
}

bool TryParseDirectoryEntry(string line, out DirectoryEntry? parsed)
{
    Match match = directoryEntryRegex.Match(line);
    if (!match.Success)
    {
        parsed = null;
        return false;
    }

    string size = match.Groups["size"].Value;
    string name = match.Groups["name"].Value;

    if (size == "dir")
    {
        parsed = new DirectoryEntry(name, 0, true);
        return true;
    }

    parsed = new DirectoryEntry(name, int.Parse(size), false);
    return true;
}

record Command(CommandType CommandType, string? Parameter);
record DirectoryEntry(string Name, int Size, bool IsDirectory);

enum CommandType
{
    ChangeDirectory,
    ListDirectory
}