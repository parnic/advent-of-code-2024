namespace aoc2024;

internal class Day01 : Day
{
    private readonly List<long> list1 = [];
    private readonly List<long> list2 = [];
    internal override void Parse()
    {
        var lines = Util.Parsing.ReadAllLines($"{GetDay()}");
        foreach (var line in lines)
        {
            var vals = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            list1.Add(long.Parse(vals[0]));
            list2.Add(long.Parse(vals[1]));
        }
    }

    internal override string Part1()
    {
        list1.Sort();
        list2.Sort();
        long totalDist = list1.Select((num1, idx) => Math.Abs(num1 - list2[idx])).Sum();
        return $"Total distance between lists: <+white>{totalDist}";
    }

    internal override string Part2()
    {
        long score = list1.Aggregate(0L, (accum, num1) => accum + num1 * list2.Count(num2 => num2 == num1));
        return $"Lists similarity score: <+white>{score}";
    }
}
