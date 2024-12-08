using aoc2024;

using aoc2024.Timer t = new("Full program");
var types = System.Reflection.Assembly
             .GetExecutingAssembly()
             .GetTypes()
             .Where(ty => ty.IsSubclassOf(typeof(Day)) && !ty.IsAbstract && ty.Name != "DayTemplate")
             .OrderBy(ty => ty.Name).ToList();

bool runAll = false;
bool? runPart1 = null;
bool? runPart2 = null;
List<string> desiredDays = new();
foreach (var arg in args)
{
    if (arg.Equals("-part1", StringComparison.CurrentCultureIgnoreCase))
    {
        runPart1 = true;
    }
    else if (arg.Equals("-part2", StringComparison.CurrentCultureIgnoreCase))
    {
        runPart2 = true;
    }
    else if (arg.Equals("all", StringComparison.CurrentCultureIgnoreCase))
    {
        runAll = true;
    }
    else
    {
        desiredDays.Add(arg);
    }
}

if (runPart1 != null || runPart2 != null)
{
    runPart1 ??= false;
    runPart2 ??= false;
}

if (runAll)
{
    desiredDays.Clear();
    foreach (var type in types)
    {
        desiredDays.Add(type.Name[^2..]);
    }
}

if (desiredDays.Count == 0)
{
    desiredDays.Add("");
}

var getDayNumFromArg = (string arg) =>
{
    if (string.IsNullOrEmpty(arg))
    {
        arg = types.Last().ToString()[^2..];
    }

    return int.Parse(arg);
};

var getDayInstanceFromArg = (string arg) =>
{
    var num = getDayNumFromArg(arg);
    var typ = types.FirstOrDefault(x => x.Name == $"Day{num.ToString().PadLeft(2, '0')}");
    if (typ == null)
    {
        return null;
    }

    var day = (Day?) Activator.CreateInstance(typ);
    return day;
};

aoc2024.Util.DotEnv.SetEnvironment();
var sessionVal = Environment.GetEnvironmentVariable("AOC_SESSION");
if (!string.IsNullOrEmpty(sessionVal))
{
    var year = Environment.GetEnvironmentVariable("AOC_YEAR");
    if (string.IsNullOrEmpty(year))
    {
        // was going to use the current year, but this solution is specifically designed for a certain year, so using that makes more sense.
        // don't forget your find/replace when copying this code for a new aoc year!
        //year = DateTime.Now.Year.ToString();
        year = "2024";
        System.Diagnostics.Debug.WriteLine($"No AOC_YEAR env var defined or set in .env file, so assuming year {year}");
    }

    foreach (var day in desiredDays)
    {
        var dayNum = getDayNumFromArg(day);
        if (!File.Exists(Path.Combine("inputs", $"{dayNum.ToString().PadLeft(2, '0')}.txt")))
        {
            Logger.Log($"Downloading input for day {dayNum}...");
            try
            {
                await aoc2024.Util.RetrieveInput.Get(sessionVal, year, dayNum);
                Logger.LogLine("<green>done!<r>");
            }
            catch (Exception ex)
            {
                Logger.LogLine($"<red>failed!<r> {ex}");
            }

            Logger.LogLine("");
        }
    }
}
else
{
    System.Diagnostics.Debug.WriteLine("No AOC_SESSION env var defined or set in .env file, so automatic input downloading not available.");
}

foreach (var desiredDay in desiredDays)
{
    using Day? day = getDayInstanceFromArg(desiredDay);
    if (day == null)
    {
        Logger.LogLine($"Unknown day <cyan>{desiredDay}<r>");
    }

    day?.Go(runPart1 ?? true, runPart2 ?? true);
}
