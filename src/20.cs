using aoc2024.Util;

namespace aoc2024;

internal class Day20 : Day
{
    private ivec2 startPos = ivec2.ZERO;
    private ivec2 endPos = ivec2.ZERO;
    private bool[,] grid = new bool[1,1];
    internal override void Parse()
    {
        var lines = Util.Parsing.ReadAllLines($"{GetDay()}").ToList();
        grid = new bool[lines.Count, lines[0].Length];
        for (var y = 0; y < lines.Count; y++)
        {
            for (var x = 0; x < lines[y].Length; x++)
            {
                if (lines[y][x] == '#')
                {
                    grid[y, x] = true;
                }
                else if (lines[y][x] == 'S')
                {
                    startPos = new ivec2(x, y);
                }
                else if (lines[y][x] == 'E')
                {
                    endPos = new ivec2(x, y);
                }
            }
        }
    }

    internal override string Part1()
    {
        Dictionary<ivec2, int> distances = [];
        distances[endPos] = 0;
        Queue<ivec2> q = [];
        q.Enqueue(endPos);
        while (q.TryDequeue(out var pos))
        {
            foreach (var neighbor in pos.GetBoundedOrthogonalNeighbors(0, 0, grid.GetLength(0) - 1,
                         grid.GetLength(1) - 1))
            {
                if (!grid[neighbor.y, neighbor.x] && (!distances.TryGetValue(neighbor, out var distance) || distance > distances[pos] + 1))
                {
                    if (neighbor != startPos)
                    {
                        q.Enqueue(neighbor);
                    }

                    distances.Add(neighbor, distances[pos] + 1);
                }
            }
        }

        var shortcuts = new Dictionary<int, int>();
        foreach (var src in distances)
        {
            foreach (var target in distances)
            {
                if (src.Key.ManhattanDistanceTo(target.Key) != 2)
                {
                    continue;
                }

                var distSaved = src.Value - target.Value - 2;
                if (distSaved <= 0)
                {
                    continue;
                }
                if (!shortcuts.TryAdd(distSaved, 1))
                {
                    shortcuts[distSaved]++;
                }
            }
        }

        long numSavingAtLeast100 = shortcuts.Where(s => s.Key >= 100).Sum(s => s.Value);
        return $"2-cell skips saving 100+ picoseconds: <+white>{numSavingAtLeast100}";
    }

    internal override string Part2()
    {
        Dictionary<ivec2, int> distances = [];
        distances[endPos] = 0;
        Queue<ivec2> q = [];
        q.Enqueue(endPos);
        while (q.TryDequeue(out var pos))
        {
            foreach (var neighbor in pos.GetBoundedOrthogonalNeighbors(0, 0, grid.GetLength(0) - 1,
                         grid.GetLength(1) - 1))
            {
                if (!grid[neighbor.y, neighbor.x] && (!distances.TryGetValue(neighbor, out var distance) || distance > distances[pos] + 1))
                {
                    if (neighbor != startPos)
                    {
                        q.Enqueue(neighbor);
                    }

                    distances.Add(neighbor, distances[pos] + 1);
                }
            }
        }

        var shortcuts = new Dictionary<long, long>();
        foreach (var src in distances)
        {
            foreach (var target in distances)
            {
                var distTo = src.Key.ManhattanDistanceTo(target.Key);
                if (distTo is 0 or > 20)
                {
                    continue;
                }

                var distSaved = src.Value - target.Value - distTo;
                if (distSaved <= 0)
                {
                    continue;
                }

                if (!shortcuts.TryAdd(distSaved, 1))
                {
                    shortcuts[distSaved]++;
                }
            }
        }

        long numSavingAtLeast100 = shortcuts.Where(s => s.Key >= 100).Sum(s => s.Value);
        return $"0-20-cell skips saving 100+ picoseconds: <+white>{numSavingAtLeast100}";
    }
}
