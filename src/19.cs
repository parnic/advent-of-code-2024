namespace aoc2024;

internal class Day19 : Day
{
    private readonly List<string> towels = [];
    private readonly List<string> desiredPatterns = [];

    internal override void Parse()
    {
        var lines = Util.Parsing.ReadAllLines($"{GetDay()}").ToList();
        towels.AddRange(lines[0].Split(", ", StringSplitOptions.RemoveEmptyEntries));
        for (int i = 2; i < lines.Count; i++)
        {
            desiredPatterns.Add(lines[i]);
        }
    }

    private readonly HashSet<string> knownGood = [];
    private bool IsPossible(ReadOnlySpan<char> desired)
    {
        if (knownGood.Contains(desired.ToString()))
        {
            return true;
        }

        foreach (var towel in towels)
        {
            if (!desired.StartsWith(towel))
            {
                continue;
            }

            if (desired.Length == towel.Length || IsPossible(desired[towel.Length..]))
            {
                knownGood.Add(towel);
                return true;
            }
        }

        return false;
    }

    private readonly Dictionary<string, long> knownGoodCount = [];
    private long NumPossible(ReadOnlySpan<char> desired)
    {
        var desiredStr = desired.ToString();
        if (knownGoodCount.TryGetValue(desiredStr, out var count))
        {
            return count;
        }

        foreach (var towel in towels)
        {
            if (!desired.StartsWith(towel))
            {
                continue;
            }

            var numPossible = desired.Length != towel.Length ? NumPossible(desired[towel.Length..]) : 1;
            if (numPossible == 0)
            {
                continue;
            }

            if (!knownGoodCount.TryAdd(desiredStr, numPossible))
            {
                knownGoodCount[desiredStr] += numPossible;
            }
        }

        return knownGoodCount.GetValueOrDefault(desiredStr, 0);
    }

    internal override string Part1()
    {
        int numPossible = 0;
        foreach (var desired in desiredPatterns)
        {
            if (IsPossible(desired))
            {
                numPossible++;
            }
        }

        return $"Possible designs: <+white>{numPossible}";
    }

    internal override string Part2()
    {
        long total = 0;
        foreach (var desired in desiredPatterns)
        {
            var combinations = NumPossible(desired);
            if (combinations > 0)
            {
                total += combinations;
            }
        }

        return $"Ways to make each design: <+white>{total}";
    }
}
