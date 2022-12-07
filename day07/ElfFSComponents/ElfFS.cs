using System.Diagnostics;

namespace ElfFSComponents;

public class ElfFS
{
    public ElfFS(int totalSpace)
    {
        TotalSpace = totalSpace;
    }

    public int TotalSpace { get; }
    public int UsedSpace => Root.CalculateSize();
    public int FreeSpace => TotalSpace - UsedSpace;

    public ElfFSDirectory Root { get; } = new ElfFSDirectory();

    public ElfFSFile CreateFile(ElfFSDirectory currentDirectory, string path, int size)
    {
        ArgumentNullException.ThrowIfNull(currentDirectory);
        ArgumentNullException.ThrowIfNull(path);

        if (path.StartsWith(ElfFSEnv.DirectorySeparatorChar))
        {
            if (path.Length == 1)
                throw new InvalidOperationException("Cannot create root directory.");
            currentDirectory = Root;
            path = path.Substring(1);
        }

        string[] pathParts = path.Split(ElfFSEnv.DirectorySeparatorChar);
        foreach (string currentPathPart in pathParts.Take(pathParts.Length - 1))
        {
            IElfFSItem? item = currentDirectory.GetItem(currentPathPart);
            ElfFSDirectory? subDirectory = item as ElfFSDirectory;

            if (item != null && subDirectory == null)
                throw new InvalidOperationException($"Not a directory: {item}.");

            if (subDirectory == null)
                subDirectory = currentDirectory.CreateDirectory(currentPathPart);

            currentDirectory = subDirectory;
        }

        string fileName = pathParts.Last();
        if (!ElfFSEnv.IsValidName(fileName))
            throw new ArgumentException($"Invalid file name: {fileName}.", nameof(path));

        if (currentDirectory.GetItem(pathParts.Last()) != null)
            throw new InvalidOperationException($"There is already an item named '{fileName}' in the directory.");

        ElfFSFile file = currentDirectory.CreateFile(fileName, size);
        return file;
    }

    public ElfFSDirectory CreateDirectory(ElfFSDirectory currentDirectory, string path)
    {
        ArgumentNullException.ThrowIfNull(currentDirectory);
        ArgumentNullException.ThrowIfNull(path);

        if (path.StartsWith(ElfFSEnv.DirectorySeparatorChar))
        {
            if (path.Length == 1)
                throw new InvalidOperationException("Cannot create root directory.");
            currentDirectory = Root;
            path = path.Substring(1);
        }

        if (path.EndsWith(ElfFSEnv.DirectorySeparatorChar))
            path = path.Substring(0, path.Length - 1);

        string[] pathParts = path.Split(ElfFSEnv.DirectorySeparatorChar);
        foreach (string currentPathPart in pathParts.Take(pathParts.Length - 1))
        {
            IElfFSItem? item = currentDirectory.GetItem(currentPathPart);
            ElfFSDirectory? subDirectory = item as ElfFSDirectory;

            if (item != null && subDirectory == null)
                throw new InvalidOperationException($"Not a directory: {item}.");

            if (subDirectory == null)
                subDirectory = currentDirectory.CreateDirectory(currentPathPart);

            currentDirectory = subDirectory;
        }

        string directoryName = pathParts.Last();
        if (!ElfFSEnv.IsValidName(directoryName))
            throw new ArgumentException($"Invalid directory name: {directoryName}.", nameof(path));

        if (currentDirectory.GetItem(pathParts.Last()) != null)
            throw new InvalidOperationException($"There is already an item named '{directoryName}' in the directory.");

        currentDirectory = currentDirectory.CreateDirectory(pathParts.Last());
        return currentDirectory;
    }

    public ElfFSDirectory GetDirectory(ElfFSDirectory currentDirectory, string path)
    {
        ArgumentNullException.ThrowIfNull(currentDirectory);
        ArgumentNullException.ThrowIfNull(path);

        if (path.StartsWith(ElfFSEnv.DirectorySeparatorChar))
        {
            if (path.Length == 1)
                return Root;
            currentDirectory = Root;
            path = path.Substring(1);
        }

        if (path.EndsWith(ElfFSEnv.DirectorySeparatorChar))
            path += '.';

        string[] pathParts = path.Split(ElfFSEnv.DirectorySeparatorChar);
        foreach (string currentPathPart in pathParts)
        {
            ElfFSDirectory? subDirectory = currentDirectory.GetItem(currentPathPart) as ElfFSDirectory;
            if (subDirectory == null)
                throw new InvalidOperationException($"Directory not found: '{path}'.");
            currentDirectory = subDirectory;
        }

        return currentDirectory;
    }

    public bool DirectoryExists(ElfFSDirectory currentDirectory, string path)
    {
        ArgumentNullException.ThrowIfNull(currentDirectory);
        ArgumentNullException.ThrowIfNull(path);

        if (path.StartsWith(ElfFSEnv.DirectorySeparatorChar))
        {
            if (path.Length == 1)
                return true;
            path = path.Substring(1);
        }

        if (path.EndsWith(ElfFSEnv.DirectorySeparatorChar))
            path += '.';

        string[] pathParts = path.Split(ElfFSEnv.DirectorySeparatorChar);
        foreach (string currentPathPart in pathParts.Take(pathParts.Length - 1))
        {
            ElfFSDirectory? subDirectory = currentDirectory.GetItem(currentPathPart) as ElfFSDirectory;
            if (subDirectory == null)
                return false;
            currentDirectory = subDirectory;
        }

        return currentDirectory.GetItem(pathParts.Last()) as ElfFSDirectory != null;
    }

    public bool FileExists(ElfFSDirectory currentDirectory, string path)
    {
        ArgumentNullException.ThrowIfNull(currentDirectory);
        ArgumentNullException.ThrowIfNull(path);

        if (path.StartsWith(ElfFSEnv.DirectorySeparatorChar))
        {
            if (path.Length == 1)
                return false;
            currentDirectory = Root;
            path = path.Substring(1);
        }

        string[] pathParts = path.Split(ElfFSEnv.DirectorySeparatorChar);
        foreach (string currentPathPart in pathParts.Take(pathParts.Length - 1))
        {
            ElfFSDirectory? subDirectory = currentDirectory.GetItem(currentPathPart) as ElfFSDirectory;
            if (subDirectory == null)
                return false;
            currentDirectory = subDirectory;
        }

        return currentDirectory.GetItem(pathParts.Last()) as ElfFSFile != null;
    }
}