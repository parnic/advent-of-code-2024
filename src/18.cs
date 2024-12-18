using aoc2024.Util;

namespace aoc2024;

internal class Day18 : Day
{
    private const int spaceSize = 71;

    private List<ivec2> startPoints = [];
    private readonly ivec2 endPos = new(spaceSize - 1, spaceSize - 1);

    internal override void Parse()
    {
        startPoints = Util.Parsing.ReadAllLines($"{GetDay()}").Select(ivec2.Parse).ToList();
    }

    private int GetStepsToExit(bool[][] grid)
    {
        var q = new Queue<(ivec2 pos, int dist)>();
        q.Enqueue((ivec2.ZERO, 0));
        Dictionary<ivec2, int> visited = [];
        while (q.TryDequeue(out var pt))
        {
            if (visited.TryGetValue(pt.pos, out var dist) && dist <= pt.dist)
            {
                continue;
            }
            visited[pt.pos] = pt.dist;

            foreach (var point in pt.pos.GetBoundedOrthogonalNeighbors(0, 0, spaceSize - 1, spaceSize - 1))
            {
                if (grid[point.y][point.x])
                {
                    continue;
                }

                if (!visited.TryGetValue(point, out int dist2) || dist2 > pt.dist + 1)
                {
                    q.Enqueue((point, pt.dist + 1));
                }
            }
        }

        if (!visited.TryGetValue(endPos, out int steps))
        {
            return -1;
        }
        return steps;
    }

    internal override string Part1()
    {
        bool[][] grid = new bool[spaceSize][];
        for (int i = 0; i < grid.Length; i++)
        {
            grid[i] = new bool[spaceSize];
        }

        const int numToRead = 1024;
        for (int i = 0; i < numToRead; i++)
        {
            grid[startPoints[i].y][startPoints[i].x] = true;
        }

        var shortestDist = GetStepsToExit(grid);

        return $"Minimum steps to exit: <+white>{shortestDist}";
    }

    internal override string Part2()
    {
        var endPos = new ivec2(spaceSize - 1, spaceSize - 1);

        bool[][] grid = new bool[spaceSize][];
        for (int i = 0; i < grid.Length; i++)
        {
            grid[i] = new bool[spaceSize];
        }

        const int numToRead = 1024;
        int blockerIdx = -1;
        for (int i = 0; i < numToRead; i++)
        {
            grid[startPoints[i].y][startPoints[i].x] = true;
        }
        for (int i = numToRead; i < startPoints.Count; i++)
        {
            grid[startPoints[i].y][startPoints[i].x] = true;
            var steps = GetStepsToExit(grid);
            if (steps == -1)
            {
                blockerIdx = i;
                break;
            }
        }

        return $"Coordinates of exit-blocking corruption: <+white>{startPoints[blockerIdx]}";
    }
}
