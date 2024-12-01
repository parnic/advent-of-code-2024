namespace aoc2024.Util;

internal static class DotEnv
{
    internal static string GetDotEnvContents(string fromPath = "")
    {
        if (string.IsNullOrEmpty(fromPath))
        {
            fromPath = Directory.GetCurrentDirectory();
        }

        try
        {
            var dir = new DirectoryInfo(fromPath);
            while (dir != null)
            {
                var dotEnv = Path.Combine(dir.FullName, ".env");
                if (File.Exists(dotEnv))
                {
                    return dotEnv;
                }

                dir = dir.Parent;
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Exception searching for .env path from {fromPath}: {ex}");
        }

        return "";
    }

    internal static bool SetEnvironment(string fromPath = "")
    {
        var dotEnv = GetDotEnvContents(fromPath);
        if (string.IsNullOrEmpty(dotEnv))
        {
            return false;
        }

        var lines = File.ReadAllLines(dotEnv);
        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i].Trim();

            if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#'))
            {
                continue;
            }

            var parts = line.Split('=');
            if (parts.Length != 2)
            {
                Logger.LogErrorLine($"DotEnv file {dotEnv} line {i + 1} does not match expected `key=value` format. Line: {line}");
                continue;
            }

            System.Diagnostics.Debug.WriteLine($"Setting environment variable `{parts[0]}` = `{parts[1]}`");
            Environment.SetEnvironmentVariable(parts[0], parts[1]);
        }

        return true;
    }
}
