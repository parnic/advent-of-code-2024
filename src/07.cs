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

    private static bool CanSolveAddMult((long result, List<long> inputs) eq)
    {
        if (eq.inputs.Count > 2)
        {
            var added = eq.inputs[0] + eq.inputs[1];
            var multd = eq.inputs[0] * eq.inputs[1];
            return CanSolveAddMult(eq with { inputs = [added, .. eq.inputs.Skip(2)] }) || CanSolveAddMult(eq with { inputs = [multd, .. eq.inputs.Skip(2)] });
        }

        return eq.inputs[0] + eq.inputs[1] == eq.result || eq.inputs[0] * eq.inputs[1] == eq.result;
    }

    internal override string Part1()
    {
        long total = eqs.Where(CanSolveAddMult).Sum(eq => eq.result);
        return $"Add | Mult calibration result: <+white>{total}";
    }

    private static bool CanSolveAddMultConcat((long result, List<long> inputs) eq)
    {
        if (eq.inputs.Count > 2)
        {
            var added = eq.inputs[0] + eq.inputs[1];
            var multd = eq.inputs[0] * eq.inputs[1];
            var cated = long.Parse($"{eq.inputs[0]}{eq.inputs[1]}");
            return CanSolveAddMultConcat(eq with { inputs = [added, .. eq.inputs.Skip(2)] })
                   || CanSolveAddMultConcat(eq with { inputs = [multd, .. eq.inputs.Skip(2)] })
                   || CanSolveAddMultConcat(eq with { inputs = [cated, .. eq.inputs.Skip(2)] });
        }

        return eq.inputs[0] + eq.inputs[1] == eq.result || eq.inputs[0] * eq.inputs[1] == eq.result || long.Parse($"{eq.inputs[0]}{eq.inputs[1]}") == eq.result;
    }

    internal override string Part2()
    {
        long total = eqs.Where(CanSolveAddMultConcat).Sum(eq => eq.result);
        return $"Add | Mult | Concat calibration result: <+white>{total}";
    }
}
