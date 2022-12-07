namespace ElfFSComponents;

public class ElfFSDirectory : IElfFSItem
{
    private readonly List<IElfFSItem> _items;

    public ElfFSDirectory()
    {
        _items = new List<IElfFSItem>();
        Items = _items.AsReadOnly();
        ParentDirectory = null;
        Name = string.Empty;
    }

    public ElfFSDirectory(ElfFSDirectory parentDirectory, string name)
    {
        ArgumentNullException.ThrowIfNull(parentDirectory);

        if (!ElfFSEnv.IsValidName(name))
            throw new ArgumentException($"Invalid directory name '{name}'.", nameof(name));

        _items = new List<IElfFSItem>();
        Items = _items.AsReadOnly();
        ParentDirectory = parentDirectory;
        Name = name;
    }

    public IReadOnlyCollection<IElfFSItem> Items { get; }
    public ElfFSDirectory? ParentDirectory { get; }
    public string Name { get; }

    public IElfFSItem? GetItem(string name)
    {
        switch (name)
        {
            case ".":
                return this;
            case "..":
                return ParentDirectory ?? this;
            default:
                return _items.FirstOrDefault(item => item.Name == name);
        }

    }

    public ElfFSFile CreateFile(string name, int size)
    {
        if (GetItem(name) != null)
            throw new ArgumentException($"Item with name '{name}' already exists under directory '{this}'.");
        ElfFSFile file = new(this, name, size);
        _items.Add(file);
        return file;
    }

    public ElfFSDirectory CreateDirectory(string name)
    {
        if (GetItem(name) != null)
            throw new ArgumentException($"Item with name '{name}' already exists under directory '{this}'.");
        ElfFSDirectory directory = new(this, name);
        _items.Add(directory);
        return directory;
    }

    public int CalculateSize() => _items.Sum(item => item.CalculateSize());

    public override string ToString() => $"{ParentDirectory}{Name}{ElfFSEnv.DirectorySeparatorChar}";
}
