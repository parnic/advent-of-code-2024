using aoc2024.Util;

namespace aoc2024;

internal class Day10 : Day
{
    int[][] grid = new int[10][];
    internal override void Parse()
    {
        var lines = Util.Parsing.ReadAllLines($"{GetDay()}").ToList();
        grid = new int[lines.Count][];
        for (int y = 0; y < lines.Count; y++)
        {
            var row = new int[lines[y].Length];
            for (int x = 0; x < row.Length; x++)
            {
                try
                {
                    row[x] = int.Parse(lines[y][x].ToString());
                }
                catch
                {
                    // support example inputs with '.' as part of the path
                    row[x] = -1;
                }
            }

            grid[y] = row;
        }
    }

    internal override string Part1()
    {
        Queue<(ivec2 pos, ivec2 start)> paths = [];
        Dictionary<ivec2, HashSet<ivec2>> trails = [];
        for (int y = 0; y < grid.Length; y++)
        {
            for (int x = 0; x < grid[y].Length; x++)
            {
                if (grid[y][x] == 0)
                {
                    paths.Enqueue((new ivec2(x, y), new ivec2(x, y)));
                }
            }
        }

        while (paths.TryDequeue(out var segment))
        {
            foreach (var pt in segment.pos.GetBoundedOrthogonalNeighbors(0, 0, grid[0].Length - 1, grid.Length - 1))
            {
                if (grid[pt.y][pt.x] != grid[segment.pos.y][segment.pos.x] + 1)
                {
                    continue;
                }

                if (grid[pt.y][pt.x] == 9)
                {
                    if (!trails.TryAdd(segment.start, [pt]))
                    {
                        trails[segment.start].Add(pt);
                    }

                    continue;
                }

                paths.Enqueue((pt, segment.start));
            }
        }

        return $"Trailhead scores sum: <+white>{trails.Sum(t => t.Value.Count)}";
    }

    internal override string Part2()
    {
        Queue<(ivec2 pos, ivec2 start)> paths = [];
        Dictionary<ivec2, Dictionary<ivec2, int>> trails = [];
        for (int y = 0; y < grid.Length; y++)
        {
            for (int x = 0; x < grid[y].Length; x++)
            {
                if (grid[y][x] == 0)
                {
                    paths.Enqueue((new ivec2(x, y), new ivec2(x, y)));
                }
            }
        }

        while (paths.TryDequeue(out var segment))
        {
            foreach (var pt in segment.pos.GetBoundedOrthogonalNeighbors(0, 0, grid[0].Length - 1, grid.Length - 1))
            {
                if (grid[pt.y][pt.x] != grid[segment.pos.y][segment.pos.x] + 1)
                {
                    continue;
                }

                if (grid[pt.y][pt.x] == 9)
                {
                    if (!trails.TryGetValue(segment.start, out var value))
                    {
                        value = [];
                        trails.Add(segment.start, value);
                    }

                    if (!value.TryAdd(pt, 1))
                    {
                        value[pt]++;
                    }

                    continue;
                }

                paths.Enqueue((pt, segment.start));
            }
        }

        return $"Trailhead ratings sum: <+white>{trails.Sum(t => t.Value.Sum(e => e.Value))}";
    }
}
