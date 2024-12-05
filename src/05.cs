namespace aoc2024;

internal class Day05 : Day
{
    private static readonly List<(int, int)> orderingRules = [];
    private static readonly List<List<int>> updates = [];
    internal override void Parse()
    {
        var lines = Util.Parsing.ReadAllLines($"{GetDay()}");
        int mode = 0;
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                mode++;
                continue;
            }

            if (mode == 0)
            {
                var split = line.Split('|');
                orderingRules.Add((int.Parse(split[0]), int.Parse(split[1])));
            }
            else if (mode == 1)
            {
                var split = line.Split(',');
                List<int> update = [];
                split.ForEach(n => update.Add(int.Parse(n)));
                updates.Add(update);
            }
        }
    }

    private static bool IsOrdered(List<int> update)
    {
        for (int i = 0; i < update.Count; i++)
        {
            var numsBefore = orderingRules.Where(r => r.Item2 == update[i]).Select(r => r.Item1).ToList();
            var numsAfter = orderingRules.Where(r => r.Item1 == update[i]).Select(r => r.Item2).ToList();
            if (update.Take(i).Any(a => numsAfter.Contains(a)))
            {
                return false;
            }

            if (update.Skip(i + 1).Any(a => numsBefore.Contains(a)))
            {
                return false;
            }
        }

        return true;
    }

    internal override string Part1()
    {
        long total = updates.Where(IsOrdered).Sum(update => update[update.Count / 2]);
        return $"Ordered update middle number sum: <+white>{total}";
    }

    private static List<int> FixOrder(List<int> update)
    {
        var fix = update.ToList();
        fix.Sort((a, b) =>
        {
            var numsBefore = orderingRules.Where(r => r.Item2 == b).Select(r => r.Item1).ToList();
            var numsAfter = orderingRules.Where(r => r.Item1 == b).Select(r => r.Item2).ToList();
            if (numsBefore.Contains(a))
            {
                return -1;
            }

            if (numsAfter.Contains(a))
            {
                return 1;
            }

            return 0;
        });

        return fix;
    }

    internal override string Part2()
    {
        long total = updates.Where(update => !IsOrdered(update)).Select(FixOrder).Sum(update => update[update.Count / 2]);
        return $"Fixed unordered middle number sum: <+white>{total}";
    }
}
