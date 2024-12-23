namespace aoc2024;

internal class Day23 : Day
{
    private readonly Dictionary<string, HashSet<string>> connections = [];

    internal override void Parse()
    {
        var lines = Util.Parsing.ReadAllLines($"{GetDay()}");
        var pairs = lines.Select(line => line.Split('-'));
        foreach (var pair in pairs)
        {
            if (!connections.TryAdd(pair.First(), [pair.Last()]))
            {
                connections[pair.First()].Add(pair.Last());
            }

            if (!connections.TryAdd(pair.Last(), [pair.First()]))
            {
                connections[pair.Last()].Add(pair.First());
            }
        }
    }

    internal override string Part1()
    {
        HashSet<string> seen = [];
        var numGroups = 0;

        foreach (var d1 in connections)
        {
            foreach (var d2Name in d1.Value)
            {
                foreach (var d3Name in connections[d2Name])
                {
                    if (!connections[d3Name].Contains(d1.Key) ||
                        (d1.Key[0] != 't' && d2Name[0] != 't' && d3Name[0] != 't'))
                    {
                        continue;
                    }

                    List<string> set = [d1.Key, d2Name, d3Name];
                    set.Sort();
                    var setStr = string.Join(',', set);
                    if (!seen.Add(setStr))
                    {
                        continue;
                    }

                    numGroups++;
                }
            }
        }

        return $"# cliques containing a computer starting with 't': <+white>{numGroups}";
    }

    // Bron–Kerbosch algorithm, see https://en.wikipedia.org/wiki/Bron%E2%80%93Kerbosch_algorithm
    private void BronKerbosch(HashSet<string> r, HashSet<string> p, HashSet<string> x, List<HashSet<string>> cliques)
    {
        if (p.Count == 0 && x.Count == 0)
        {
            cliques.Add(r);
            return;
        }

        foreach (var v in p.Except(connections[FindNonNeighbor(p, x)]))
        {
            BronKerbosch([..r, v], [..p.Intersect(connections[v])], [..x.Intersect(connections[v])], cliques);

            p.Remove(v);
            x.Add(v);
        }
    }

    private string FindNonNeighbor(HashSet<string> p, HashSet<string> x)
        => p.Union(x).OrderByDescending(v => connections[v].Count).First();

    private List<HashSet<string>> GetAllCliques()
    {
        var cliques = new List<HashSet<string>>();
        BronKerbosch([], [..connections.Keys], [], cliques);

        return cliques;
    }

    internal override string Part2()
    {
        var cliques = GetAllCliques();
        var password = string.Join(",", cliques.MaxBy(c => c.Count)!.Order());
        return $"LAN party password: <+white>{password}";
    }
}
