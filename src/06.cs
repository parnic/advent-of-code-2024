using aoc2024.Util;

namespace aoc2024;

internal class Day06 : Day
{
    private bool[][] grid = [];
    private ivec2 startPos;
    private ivec2 startDir;
    internal override void Parse()
    {
        var lines = Util.Parsing.ReadAllLines($"{GetDay()}").ToList();
        grid = new bool[lines.Count][];
        for (int i = 0; i < lines.Count; i++)
        {
            var line = lines[i];
            var row = new bool[line.Length];
            for (int j = 0; j < line.Length; j++)
            {
                if (line[j] == '#')
                {
                    row[j] = true;
                }
                else
                {
                    row[j] = false;
                    if (line[j] != '.')
                    {
                        startPos = new ivec2(j, i);
                        startDir = ivec2.DirFromChar(line[j]);
                    }
                }
            }

            grid[i] = row;
        }
    }

    private (bool, HashSet<ivec2>) HasLoop()
    {
        var currPos = startPos;
        var currDir = startDir;
        var loopSet = new HashSet<(ivec2, ivec2)>(10000);
        var visited = new HashSet<ivec2>(10000);
        while (true)
        {
            visited.Add(currPos);
            if (!loopSet.Add((currPos, currDir)))
            {
                return (true, visited);
            }

            var nextPos = currPos + currDir;
            if (!nextPos.IsWithinRange(0, 0, grid[0].Length - 1, grid.Length - 1))
            {
                return (false, visited);
            }

            if (grid[nextPos.y][nextPos.x])
            {
                currDir = currDir.GetRotatedRight();
            }
            else
            {
                currPos += currDir;
            }
        }
    }

    internal override string Part1()
    {
        var (_, visited) = HasLoop();
        return $"Locations visited: <+white>{visited.Count}";
    }

    internal override string Part2()
    {
        var (_, allVisited) = HasLoop();

        int numBlockedLoops = 0;
        foreach (var pos in allVisited)
        {
            var (x, y) = (pos.x, pos.y);
            if (startPos == pos)
            {
                continue;
            }

            grid[y][x] = true;

            var (hasLoop, _) = HasLoop();
            if (hasLoop)
            {
                numBlockedLoops++;
            }

            grid[y][x] = false;
        }

        return $"# obstructions that cause a loop: <+white>{numBlockedLoops}";
    }
}
