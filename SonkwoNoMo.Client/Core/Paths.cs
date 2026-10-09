namespace SonkwoNoMo.Client.Core;

internal static class Paths
{
    public static string BasePath => GetBasePath();

    private static string GetBasePath()
    {
#if DEBUG
        return "";
#endif

        var currentDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());

        while (currentDirectory != null)
        {
            if (currentDirectory.Name.Equals("Engine", StringComparison.OrdinalIgnoreCase) ||
                currentDirectory.Name.Equals("GameProject", StringComparison.OrdinalIgnoreCase))
            {
                return currentDirectory.Parent?.FullName ?? "";
            }

            currentDirectory = currentDirectory.Parent;
        }

        return "";
    }
}