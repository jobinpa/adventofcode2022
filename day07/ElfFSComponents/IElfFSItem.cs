namespace ElfFSComponents;

public interface IElfFSItem
{
    public ElfFSDirectory? ParentDirectory { get; }
    public string Name { get; }
    public int CalculateSize();
}
