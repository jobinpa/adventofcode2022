namespace ElfFSComponents;

public static class ElfFSEnv
{
    public const char DirectorySeparatorChar = '/';
    public const string CurrentDirectory = ".";
    public const string PreviousDirectory = "..";

    public static bool IsValidName(string name)
    {
        if (string.IsNullOrEmpty(name))
            return false;
        if (name.Contains(DirectorySeparatorChar))
            return false;
        if (name == CurrentDirectory || name == PreviousDirectory)
            return false;
        return true;
    }
}