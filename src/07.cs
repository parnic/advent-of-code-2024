using aoc2024.Util;

namespace aoc2024;

internal class Day07 : Day
{
    private readonly List<(long result, List<long> inputs)> eqs = [];
    internal override void Parse()
    {
        var lines = Util.Parsing.ReadAllLines($"{GetDay()}");
        foreach (var line in lines)
        {
            var split = line.Split(':');
            var nums = split[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToList();
            eqs.Add((long.Parse(split[0]), nums));
        }
    }

    private static bool CanSolveAddMult(long target, long current, List<long> inputs, int idx)
    {
        if (idx == inputs.Count)
        {
            return current == target;
        }

        var next = inputs[idx];
        var added = current + next;
        var multd = current * next;
        return CanSolveAddMult(target, added, inputs, idx + 1) ||
               CanSolveAddMult(target, multd, inputs, idx + 1);
    }

    internal override string Part1()
    {
        long total = eqs.Where(eq => CanSolveAddMult(eq.result, eq.inputs[0], eq.inputs, 1)).Sum(eq => eq.result);
        return $"Add | Mult calibration result: <+white>{total}";
    }

    private static bool CanSolveAddMultConcat(long target, long current, List<long> inputs, int idx)
    {
        if (idx == inputs.Count)
        {
            return current == target;
        }

        var next = inputs[idx];
        var added = current + next;
        var multd = current * next;
        var cated = NumConcat.Longs(current, next);
        return CanSolveAddMultConcat(target, added, inputs, idx + 1) ||
               CanSolveAddMultConcat(target, multd, inputs, idx + 1) ||
               CanSolveAddMultConcat(target, cated, inputs, idx + 1);
    }

    internal override string Part2()
    {
        long total = eqs.Where(eq => CanSolveAddMultConcat(eq.result, eq.inputs[0], eq.inputs, 1)).Sum(eq => eq.result);
        return $"Add | Mult | Concat calibration result: <+white>{total}";
    }
}
