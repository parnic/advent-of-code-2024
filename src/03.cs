using System.Text.RegularExpressions;

namespace aoc2024;

internal class Day03 : Day
{
    private string instructions = string.Empty;
    internal override void Parse()
    {
        // instructions = "xmul(2,4)%&mul[3,7]!@^do_not_mul(5,5)+mul(32,64]then(mul(11,8)mul(8,5))";
        // instructions = "xmul(2,4)&mul[3,7]!^don't()_mul(5,5)+mul(32,64](mul(11,8)undo()?mul(8,5))";
        instructions = Util.Parsing.ReadAllText(GetDay());
    }

    internal override string Part1()
    {
        List<(long, long)> mults = [];
        var regex = new Regex(@"mul\((\d+),(\d+)\)", RegexOptions.Compiled);
        foreach (var match in regex.Matches(instructions))
        {
            var num1 = long.Parse(((Match)match).Groups[1].Value);
            var num2 = long.Parse(((Match)match).Groups[2].Value);
            mults.Add((num1, num2));
        }

        var total = mults.Sum(m => m.Item1 * m.Item2);

        return $"Instruction result: <+white>{total}";
    }

    internal override string Part2()
    {
        bool enabled = true;
        List<(long, long)> mults = [];
        var regex = new Regex(@"do\(\)|don't\(\)|mul\((\d+),(\d+)\)", RegexOptions.Compiled);
        foreach (var match in regex.Matches(instructions))
        {
            if (((Match) match).Groups[0].Value == "do()")
            {
                enabled = true;
            }
            else if (((Match) match).Groups[0].Value == "don't()")
            {
                enabled = false;
            }
            else if (enabled)
            {
                var num1 = long.Parse(((Match) match).Groups[1].Value);
                var num2 = long.Parse(((Match) match).Groups[2].Value);
                mults.Add((num1, num2));
            }
        }

        var total = mults.Sum(m => m.Item1 * m.Item2);

        return $"Enabled instruction result: <+white>{total}";
    }
}
