using aoc2024.Util;

namespace aoc2024;

internal class Day08 : Day
{
    private readonly Dictionary<char, List<ivec2>> antennas = [];
    private ivec2 dimensions = ivec2.ZERO;

    internal override void Parse()
    {
        var lines = Util.Parsing.ReadAllLines($"{GetDay()}").ToList();
        dimensions = new ivec2(lines[0].Length, lines.Count);
        for (int i = 0; i < lines.Count; i++)
        {
            var line = lines[i];
            for (int j = 0; j < line.Length; j++)
            {
                if (line[j] == '.' || line[j] == '#')
                {
                    continue;
                }

                var loc = new ivec2(j, i);
                if (!antennas.TryAdd(line[j], [loc]))
                {
                    antennas[line[j]].Add(loc);
                }
            }
        }
    }

    internal override string Part1()
    {
        HashSet<ivec2> antinodes = [];
        foreach (var (_, locs) in antennas)
        {
            foreach (var loc in locs)
            {
                var others = locs.Except([loc]);
                foreach (var other in others)
                {
                    var between = loc - other;
                    var a1 = loc + between;
                    var a2 = other - between;
                    if (a1.IsWithinRange(0, 0, dimensions.x - 1, dimensions.y - 1))
                    {
                        antinodes.Add(loc + between);
                    }

                    if (a2.IsWithinRange(0, 0, dimensions.x - 1, dimensions.y - 1))
                    {
                        antinodes.Add(other - between);
                    }
                }
            }
        }

        return $"# antinodes: <+white>{antinodes.Count}";
    }

    internal override string Part2()
    {
        HashSet<ivec2> antinodes = [];
        foreach (var (_, locs) in antennas)
        {
            antinodes.UnionWith(locs);
            foreach (var loc in locs)
            {
                var others = locs.Except([loc]);
                foreach (var other in others)
                {
                    var between = loc - other;
                    var a1 = loc + between;
                    var a2 = other - between;
                    while (a1.IsWithinRange(0, 0, dimensions.x - 1, dimensions.y - 1))
                    {
                        antinodes.Add(a1);
                        a1 += between;
                    }

                    while (a2.IsWithinRange(0, 0, dimensions.x - 1, dimensions.y - 1))
                    {
                        antinodes.Add(a2);
                        a2 -= between;
                    }
                }
            }
        }

        return $"# antinodes including harmonics: <+white>{antinodes.Count}";
    }
}
