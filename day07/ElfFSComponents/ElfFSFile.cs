namespace ElfFSComponents;

public class ElfFSFile : IElfFSItem
{
    private readonly int _size;

    public ElfFSFile(ElfFSDirectory parentDirectory, string name, int size)
    {
        ArgumentNullException.ThrowIfNull(parentDirectory);

        if (!ElfFSEnv.IsValidName(name))
            throw new ArgumentException($"Invalid file name '{name}'.", nameof(name));

        if (size < 0)
            throw new ArgumentOutOfRangeException(nameof(size), "File size must be non-negative.");

        ParentDirectory = parentDirectory;
        Name = name;
        _size = size;
    }

    public ElfFSDirectory ParentDirectory { get; }
    public string Name { get; }

    public int CalculateSize() => _size;
    public override string ToString() => $"{ParentDirectory}{Name}";
}
