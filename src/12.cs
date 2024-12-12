using aoc2024.Util;

namespace aoc2024;

internal class Day12 : Day
{
    private char[][] grid = [];

    internal override void Parse()
    {
        var lines = Util.Parsing.ReadAllLines($"{GetDay()}").ToList();
        grid = new char[lines.Count][];
        for (int y = 0; y < lines.Count; y++)
        {
            var row = new char[lines[y].Length];
            for (int x = 0; x < lines[y].Length; x++)
            {
                row[x] = lines[y][x];
            }
            grid[y] = row;
        }
    }

    internal override string Part1()
    {
        HashSet<ivec2> used = [];
        HashSet<(char, HashSet<ivec2>)> plots = [];
        for (int y = 0; y < grid.Length; y++)
        {
            for (int x = 0; x < grid[y].Length; x++)
            {
                var v = new ivec2(x, y);
                if (used.Contains(v))
                {
                    continue;
                }

                var plot = (grid[y][x], new HashSet<ivec2>());
                plots.Add(plot);
                var q = new Queue<ivec2>();
                q.Enqueue(v);
                plot.Item2.Add(v);
                while (q.Count > 0)
                {
                    var check = q.Dequeue();
                    var neighbors = check.GetBoundedOrthogonalNeighbors(0, 0, grid[0].Length - 1, grid.Length - 1);
                    foreach (var n in neighbors)
                    {
                        if (grid[n.y][n.x] == grid[y][x] && !plot.Item2.Contains(n))
                        {
                            q.Enqueue(n);
                            plot.Item2.Add(n);
                        }
                    }
                }

                used.UnionWith(plot.Item2);
            }
        }

        long total = 0;
        foreach (var plot in plots)
        {
            var area = plot.Item2.Count;
            var perimeter = 0;
            foreach (var plotPt in plot.Item2)
            {
                perimeter += plotPt.GetOrthogonalNeighbors().Count(pt => !plot.Item2.Contains(pt));
            }

            var plotTotal = area * perimeter;
            total += plotTotal;
        }

        return $"Area * perimeter cost: <+white>{total}";
    }

    private bool IsSame(char c, ivec2 pt)
    {
        if (!pt.IsWithinRange(0, 0, grid[0].Length - 1, grid.Length - 1))
        {
            return false;
        }

        return grid[pt.y][pt.x] == c;
    }

    private int NumCorners(IEnumerable<ivec2> points)
    {
        int numCorners = 0;
        foreach (var pt in points)
        {
            var ch = grid[pt.y][pt.x];
            // convex
            {
                // top-left
                if (!IsSame(ch, pt - new ivec2(1, 0)) &&
                    !IsSame(ch, pt - new ivec2(0, 1)))
                {
                    numCorners++;
                }

                // top-right
                if (!IsSame(ch, pt + new ivec2(1, 0)) &&
                    !IsSame(ch, pt - new ivec2(0, 1)))
                {
                    numCorners++;
                }

                // bottom-left
                if (!IsSame(ch, pt - new ivec2(1, 0)) &&
                    !IsSame(ch, pt + new ivec2(0, 1)))
                {
                    numCorners++;
                }

                // bottom-right
                if (!IsSame(ch, pt + new ivec2(1, 0)) &&
                    !IsSame(ch, pt + new ivec2(0, 1)))
                {
                    numCorners++;
                }
            }
            // concave
            {
                // top-left
                if (IsSame(ch, pt - new ivec2(1, 0)) &&
                    IsSame(ch, pt - new ivec2(0, 1)) &&
                    !IsSame(ch, pt - ivec2.ONE))
                {
                    numCorners++;
                }
                // top-right
                if (IsSame(ch, pt + new ivec2(1, 0)) &&
                    IsSame(ch, pt - new ivec2(0, 1)) &&
                    !IsSame(ch, pt + new ivec2(1, -1)))
                {
                    numCorners++;
                }
                // bottom-left
                if (IsSame(ch, pt - new ivec2(1, 0)) &&
                    IsSame(ch, pt + new ivec2(0, 1)) &&
                    !IsSame(ch, pt + new ivec2(-1, 1)))
                {
                    numCorners++;
                }
                // bottom-right
                if (IsSame(ch, pt + new ivec2(1, 0)) &&
                    IsSame(ch, pt + new ivec2(0, 1)) &&
                    !IsSame(ch, pt + new ivec2(1, 1)))
                {
                    numCorners++;
                }
            }
        }

        return numCorners;
    }

    internal override string Part2()
    {
        HashSet<ivec2> used = [];
        List<(char, HashSet<ivec2>)> plots = [];
        for (int y = 0; y < grid.Length; y++)
        {
            for (int x = 0; x < grid[y].Length; x++)
            {
                var v = new ivec2(x, y);
                if (used.Contains(v))
                {
                    continue;
                }

                var plot = (grid[y][x], new HashSet<ivec2>());
                plots.Add(plot);
                var q = new Queue<ivec2>();
                q.Enqueue(v);
                plot.Item2.Add(v);
                while (q.Count > 0)
                {
                    var check = q.Dequeue();
                    var neighbors = check.GetBoundedOrthogonalNeighbors(0, 0, grid[0].Length - 1, grid.Length - 1);
                    foreach (var n in neighbors)
                    {
                        if (grid[n.y][n.x] != grid[y][x] || plot.Item2.Contains(n))
                        {
                            continue;
                        }

                        q.Enqueue(n);
                        plot.Item2.Add(n);
                    }
                }

                used.UnionWith(plot.Item2);
            }
        }

        long total = 0;
        foreach (var plot in plots)
        {
            var area = plot.Item2.Count;
            var numSides = NumCorners(plot.Item2);

            total += area * numSides;
        }

        return $"Area * sides cost: <+white>{total}";
    }
}
