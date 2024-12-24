namespace aoc2024;

internal class Day24 : Day
{
    class op(string gate1, string gate2, string outGate, string operation)
    {
        public string Gate1 = gate1;
        public string Gate2 = gate2;
        public string OutGate = outGate;
        public string Operation = operation;
    }

    private Dictionary<string, int> gates = [];
    private List<op> operations = [];

    internal override void Parse()
    {
        var lines = Util.Parsing.ReadAllLines($"{GetDay()}").ToList();
        var mode = 0;
        foreach (var line in lines)
        {
            if (line.Length == 0)
            {
                mode++;
                continue;
            }

            switch (mode)
            {
                case 0:
                {
                    var split = line.Split(": ", StringSplitOptions.RemoveEmptyEntries);
                    gates.Add(split[0], int.Parse(split[1]));
                    break;
                }

                case 1:
                {
                    var split = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    var o = new op(split[0], split[2], split[4], split[1]);
                    operations.Add(o);
                    break;
                }
            }
        }
    }

    private static long GetResultOf(char c, Dictionary<string, int> gs)
    {
        long result = 0;
        var ordered = gs.Where(g => g.Key.StartsWith(c)).OrderBy(g => g.Key).ToList();
        for (int i = 0; i < ordered.Count; i++)
        {
            result += (long)gs[$"{c}{i:00}"] << i;
        }

        return result;
    }

    internal override string Part1()
    {
        var gatesCopy = Run(gates, operations);
        long result = GetResultOf('z', gatesCopy);
        return $"Computed number: <+white>{result}";
    }

    private static Dictionary<string, int> Run(Dictionary<string, int> gs, List<op> ops)
    {
        var gatesCopy = gs.ToDictionary();
        var q = new Queue<op>();
        foreach (var inst in ops)
        {
            q.Enqueue(inst);
        }

        while (q.TryDequeue(out var inst))
        {
            if (!gatesCopy.ContainsKey(inst.Gate1) || !gatesCopy.ContainsKey(inst.Gate2))
            {
                q.Enqueue(inst);
                continue;
            }

            gatesCopy.TryAdd(inst.OutGate, 0);

            switch (inst.Operation)
            {
                case "AND":
                    gatesCopy[inst.OutGate] = gatesCopy[inst.Gate1] == 1 && gatesCopy[inst.Gate2] == 1 ? 1 : 0;
                    break;

                case "XOR":
                    gatesCopy[inst.OutGate] = gatesCopy[inst.Gate1] != gatesCopy[inst.Gate2] ? 1 : 0;
                    break;

                case "OR":
                    gatesCopy[inst.OutGate] = gatesCopy[inst.Gate1] == 0 && gatesCopy[inst.Gate2] == 0 ? 0 : 1;
                    break;
            }
        }

        return gatesCopy;
    }

    private static List<string> GetWrongBits(Dictionary<string, int> gs)
    {
        List<string> wrongBits = [];

        var x = GetResultOf('x', gs);
        var y = GetResultOf('y', gs);
        var actualVal = GetResultOf('z', gs);
        var desiredVal = x + y;
        // Logger.LogLine(Convert.ToString(actualVal, 2));
        // Logger.LogLine(Convert.ToString(desiredVal, 2));
        for (int i = 0; i < gs.Count(g => g.Key.StartsWith('z')); i++)
        {
            if ((desiredVal >> i) % 2 != (actualVal >> i) % 2)
            {
                wrongBits.Add($"z{i:00}");
            }
        }

        return wrongBits;
    }

    private static string Find(string gate1, string gate2, string operation, List<op> operations)
    {
        var test = operations.Where(o => o.Gate1 == gate1 && o.Gate2 == gate2 && o.Operation == operation)
            .Select(o => o.OutGate).FirstOrDefault("");
        if (!string.IsNullOrEmpty(test))
        {
            return test;
        }

        test = operations.Where(o => o.Gate1 == gate2 && o.Gate2 == gate1 && o.Operation == operation)
            .Select(o => o.OutGate).FirstOrDefault("");
        return test;
    }

    private static string FixupOutputs(List<op> operations)
    {
        var highestZ = int.Parse(operations.Select(o => o.OutGate).OrderDescending().First()[1..]);
        List<string> swapped = [];
        string lastXyAnd = "";

        for (int i = 0; i < highestZ; i++)
        {
            var n = $"{i:00}";
            string newXyXorOut = "";
            string xyOrOut = "";

            string xyXorOut = Find($"x{n}", $"y{n}", "XOR", operations);
            string xyAndOut = Find($"x{n}", $"y{n}", "AND", operations);

            if (!string.IsNullOrEmpty(lastXyAnd))
            {
                var newXyAndOut = Find(lastXyAnd, xyXorOut, "AND", operations);
                if (string.IsNullOrEmpty(newXyAndOut))
                {
                    (xyXorOut, xyAndOut) = (xyAndOut, xyXorOut);
                    swapped.AddRange([xyXorOut, xyAndOut]);
                    newXyAndOut = Find(lastXyAnd, xyXorOut, "AND", operations);
                }

                newXyXorOut = Find(lastXyAnd, xyXorOut, "XOR", operations);
                if (xyXorOut.StartsWith('z'))
                {
                    (xyXorOut, newXyXorOut) = (newXyXorOut, xyXorOut);
                    swapped.AddRange([xyXorOut, newXyXorOut]);
                }
                if (xyAndOut.StartsWith('z'))
                {
                    (xyAndOut, newXyXorOut) = (newXyXorOut, xyAndOut);
                    swapped.AddRange([xyAndOut, newXyXorOut]);
                }
                if (newXyAndOut.StartsWith('z'))
                {
                    (newXyAndOut, newXyXorOut) = (newXyXorOut, newXyAndOut);
                    swapped.AddRange([newXyAndOut, newXyXorOut]);
                }

                xyOrOut = Find(newXyAndOut, xyAndOut, "OR", operations);
            }

            if (xyOrOut.StartsWith('z') && xyOrOut != $"z{highestZ}")
            {
                (xyOrOut, newXyXorOut) = (newXyXorOut, xyOrOut);
                swapped.AddRange([xyOrOut, newXyXorOut]);
            }

            if (string.IsNullOrEmpty(lastXyAnd))
            {
                lastXyAnd = xyAndOut;
            }
            else
            {
                lastXyAnd = xyOrOut;
            }
        }

        swapped.Sort();
        return string.Join(',', swapped);
    }

    internal override string Part2()
    {
        return $"Incorrect outputs: <+white>{FixupOutputs(operations)}";
    }
}
