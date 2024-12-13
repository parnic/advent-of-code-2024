using aoc2024.Util;

namespace aoc2024;

internal class Day13 : Day
{
    private record machine(ivec2 ButtonA, ivec2 ButtonB, ivec2 Prize);

    private readonly List<machine> machines = [];

    internal override void Parse()
    {
        var lines = Util.Parsing.ReadAllLines($"{GetDay()}").ToList();
        for (int i = 0; i < lines.Count; i += 4)
        {
            var btnA = lines[i + 0].Split('+');
            var btnAX = int.Parse(btnA[1].Split(',')[0]);
            var btnAY = int.Parse(btnA[2]);

            var btnB = lines[i + 1].Split('+');
            var btnBX = int.Parse(btnB[1].Split(',')[0]);
            var btnBY = int.Parse(btnB[2]);

            var prize = lines[i + 2].Split('=');
            var prizeX = int.Parse(prize[1].Split(',')[0]);
            var prizeY = int.Parse(prize[2]);

            machines.Add(new machine(new ivec2(btnAX, btnAY), new ivec2(btnBX, btnBY), new ivec2(prizeX, prizeY)));
        }
    }

    // "Cramer's Rule" implementation for solving a system of equations
    private static long SolveMachine(machine m, long offset)
    {
        var prize = new ivec2(m.Prize.x + offset, m.Prize.y + offset);

        var determinant = (m.ButtonA.x * m.ButtonB.y) - (m.ButtonA.y * m.ButtonB.x);
        var a = ((prize.x * m.ButtonB.y) - (prize.y * m.ButtonB.x)) / determinant;
        var b = ((m.ButtonA.x * prize.y) - (m.ButtonA.y * prize.x)) / determinant;

        if (new ivec2((m.ButtonA.x * a) + (m.ButtonB.x * b), (m.ButtonA.y * a) + (m.ButtonB.y * b)) == prize)
        {
            return a * 3 + b;
        }

        return 0;
    }

    internal override string Part1()
    {
        long totalTokens = machines.Sum(m => SolveMachine(m, 0));
        return $"Tokens necessary to win possible prizes: <+white>{totalTokens}";
    }

    internal override string Part2()
    {
        long totalTokens = machines.Sum(m => SolveMachine(m, 10000000000000));
        return $"Tokens necessary to win inflated prizes: <+white>{totalTokens}";
    }
}
