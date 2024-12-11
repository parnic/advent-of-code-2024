namespace aoc2024;

internal class Day11 : Day
{
    private readonly Dictionary<long, long> stones = [];

    internal override void Parse()
    {
        var lines = Util.Parsing.ReadAllText($"{GetDay()}");
        var split = lines.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        foreach (var s in split)
        {
            stones.Add(long.Parse(s), 1);
        }
    }

    private bool HasEvenNumDigits(long v)
    {
        return v switch
        {
            < 100 and >= 10 => true,
            < 10000 and >= 1000 => true,
            < 1000000 and >= 100000 => true,
            < 100000000 and >= 10000000 => true,
            < 10000000000 and >= 1000000000 => true,
            < 1000000000000 and >= 100000000000 => true,
            < 100000000000000 and >= 10000000000000 => true,
            < 10000000000000000 and >= 1000000000000000 => true,
            < 1000000000000000000 and >= 100000000000000000 => true,
            _ => false,
        };
    }

    private static void Add(Dictionary<long, long> d, long v, long times)
    {
        if (!d.TryAdd(v, times))
        {
            d[v] += times;
        }
    }

    private void Blink(Dictionary<long, long> modified, int times)
    {
        var cache = new Dictionary<long, (long, long)>();

        for (int i = 0; i < times; i++)
        {
            var old = modified.ToDictionary();
            foreach (var s in old)
            {
                if (s.Value == 0)
                {
                    continue;
                }

                if (s.Key == 0)
                {
                    modified[0] -= s.Value;
                    Add(modified, 1, s.Value);
                }
                else if (HasEvenNumDigits(s.Key))
                {
                    var (left, right) = split(s.Key);
                    modified[s.Key] -= s.Value;
                    Add(modified, left, s.Value);
                    Add(modified, right, s.Value);
                }
                else
                {
                    modified[s.Key] -= s.Value;
                    Add(modified, s.Key * 2024, s.Value);
                }
            }
        }

        return;

        (long, long) split(long v)
        {
            if (cache.TryGetValue(v, out var value))
            {
                return value;
            }

            var num = v.ToString();
            var left = num[..(num.Length / 2)];
            var right = num[(num.Length / 2)..];
            cache[v] = (long.Parse(left), long.Parse(right));
            return cache[v];
        }
    }

    internal override string Part1()
    {
        var modified = stones.ToDictionary();

        Blink(modified, 25);

        var total = modified.Sum(x => x.Value);
        return $"# stones after 25 blinks: <+white>{total}";
    }

    internal override string Part2()
    {
        var modified = stones.ToDictionary();

        Blink(modified, 75);

        var total = modified.Sum(x => x.Value);
        return $"# stones after 75 blinks: <+white>{total}";
    }
}
