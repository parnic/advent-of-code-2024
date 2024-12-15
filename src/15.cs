using aoc2024.Util;

namespace aoc2024;

internal class Day15 : Day
{
    private readonly List<ivec2> dirs = [];
    private ivec2 startPos = ivec2.ZERO;

    private readonly List<bool[]> grid = [];
    private readonly HashSet<ivec2> boxes = [];

    internal override void Parse()
    {
        var lines = Util.Parsing.ReadAllLines($"{GetDay()}").ToList();
        int mode = 0;
        var width = lines[0].Length;
        for (int i = 0; i < lines.Count; i++)
        {
            if (lines[i].Length == 0)
            {
                mode++;
                continue;
            }

            switch (mode)
            {
                case 0:
                    var row = new bool[width];
                    for (int j = 0; j < width; j++)
                    {
                        if (lines[i][j] == '#')
                        {
                            row[j] = false;
                        }
                        else
                        {
                            row[j] = true;

                            if (lines[i][j] == 'O')
                            {
                                boxes.Add(new ivec2(j, i));
                            }
                            else if (lines[i][j] == '@')
                            {
                                startPos = new ivec2(j, i);
                            }
                        }
                    }

                    grid.Add(row);
                    break;

                case 1:
                    foreach (var ch in lines[i])
                    {
                        dirs.Add(ivec2.DirFromChar(ch));
                    }
                    break;
            }
        }
    }

    private ivec2 AttemptMove(ivec2 pos, ivec2 dir, List<ivec2> boxLocs)
    {
        var newPos = pos + dir;
        if (!grid[(int)newPos.y][newPos.x])
        {
            return pos;
        }

        var boxTest = newPos;
        var idx = boxLocs.IndexOf(boxTest);
        // first test whether this is possible or not
        while (idx >= 0)
        {
            boxTest += dir;
            if (!grid[(int) boxTest.y][(int) boxTest.x])
            {
                return pos;
            }

            idx = boxLocs.IndexOf(boxTest);
        }

        boxTest = newPos;
        idx = boxLocs.IndexOf(boxTest);
        while (idx >= 0)
        {
            boxLocs.RemoveAt(idx);
            boxTest += dir;
            idx = boxLocs.IndexOf(boxTest);
            boxLocs.Add(boxTest);
        }

        return newPos;
    }

    private void Render(ivec2 robotPos, List<ivec2> boxLocs)
    {
        for (int y = 0; y < grid.Count; y++)
        {
            for (int x = 0; x < grid[y].Length; x++)
            {
                if (!grid[y][x])
                {
                    Logger.Log("#");
                }
                else
                {
                    var pos = new ivec2(x, y);
                    if (pos == robotPos)
                    {
                        Logger.Log("@");
                    }
                    else if (boxLocs.Contains(pos))
                    {
                        Logger.Log($"{Util.Constants.SolidSquare}");
                    }
                    else
                    {
                        Logger.Log(" ");
                    }
                }
            }

            Logger.LogLine("");
        }

        Logger.LogLine("");
    }

    private static void Render(List<bool[]> grid, ivec2 robotPos, List<(ivec2, ivec2)> boxLocs)
    {
        for (int y = 0; y < grid.Count; y++)
        {
            for (int x = 0; x < grid[y].Length; x++)
            {
                if (!grid[y][x])
                {
                    Logger.Log("#");
                }
                else
                {
                    var pos = new ivec2(x, y);
                    if (pos == robotPos)
                    {
                        Logger.Log("@");
                    }
                    else if (boxLocs.Any(b => b.Item1 == pos))
                    {
                        Logger.Log("[");
                    }
                    else if (boxLocs.Any(b => b.Item2 == pos))
                    {
                        Logger.Log("]");
                    }
                    else
                    {
                        Logger.Log(" ");
                    }
                }
            }

            Logger.LogLine("");
        }

        Logger.LogLine("");
    }

    internal override string Part1()
    {
        var currPos = startPos;
        var boxLocs = new List<ivec2>(boxes);
        foreach (var dir in dirs)
        {
            currPos = AttemptMove(currPos, dir, boxLocs);
            // Render(currPos, boxLocs);
        }
        // Render(currPos, boxLocs);

        long sum = boxLocs.Sum(b => (100 * b.y) + b.x);

        return $"GPS coordinate sum: <+white>{sum}";
    }

    private static List<((ivec2, ivec2) pre, (ivec2, ivec2) post)> GetBoxesPushed(List<bool[]> grid, IEnumerable<ivec2> fromPos, ivec2 dir, List<(ivec2 l, ivec2 r)> boxLocs, out bool success)
    {
        success = true;

        var nextPos = fromPos.Select(p => p + dir).ToList();
        var boxes = boxLocs.FindAll(b => nextPos.Contains(b.l) || nextPos.Contains(b.r));
        var nextBoxLocs = boxes.Select(b => ((b.l, b.r), (b.l + dir, b.r + dir))).ToList();
        if (nextBoxLocs.Any(b =>
                !grid[(int) b.Item2.Item1.y][b.Item2.Item1.x] ||
                !grid[(int) b.Item2.Item2.y][b.Item2.Item2.x]))
        {
            success = false;
        }

        return nextBoxLocs;
    }

    private static ivec2 AttemptMove(List<bool[]> grid, ivec2 pos, ivec2 dir, ref List<(ivec2, ivec2)> boxLocs)
    {
        var newPos = pos + dir;
        if (!grid[(int)newPos.y][newPos.x])
        {
            return pos;
        }

        var possibleBoxLocs = new List<(ivec2, ivec2)>(boxLocs);
        var boxUpdates = GetBoxesPushed(grid, [pos], dir, possibleBoxLocs, out var success);
        while (success && boxUpdates.Count > 0)
        {
            var newBoxLocs = boxUpdates.Select(u => u.post).ToList();

            boxUpdates.ForEach(u => possibleBoxLocs.Remove(u.pre));
            var newBoxPos = boxUpdates.Aggregate(new List<ivec2>(), (list, next) =>
            {
                list.Add(next.pre.Item1);
                list.Add(next.pre.Item2);
                return list;
            });
            boxUpdates = GetBoxesPushed(grid, newBoxPos, dir, possibleBoxLocs, out success);

            possibleBoxLocs.AddRange(newBoxLocs);
        }

        if (!success)
        {
            return pos;
        }

        boxLocs = possibleBoxLocs;
        return newPos;
    }

    internal override string Part2()
    {
        var doubledStart = new ivec2(startPos.x * 2, startPos.y);
        List<bool[]> doubledGrid = [];
        foreach (var row in grid)
        {
            var doubledRow = new bool[row.Length * 2];
            for (int j = 0; j < row.Length; j++)
            {
                doubledRow[j * 2] = row[j];
                doubledRow[(j * 2) + 1] = row[j];
            }

            doubledGrid.Add(doubledRow);
        }

        List<(ivec2, ivec2)> doubledBoxes = [];
        doubledBoxes.AddRange(boxes.Select(box => (box * new ivec2(2, 1), box * new ivec2(2, 1) + ivec2.RIGHT)));

        var currPos = doubledStart;
        var boxLocs = new List<(ivec2, ivec2)>(doubledBoxes);
        foreach (var dir in dirs)
        {
            currPos = AttemptMove(doubledGrid, currPos, dir, ref boxLocs);
            // Render(doubledGrid, currPos, boxLocs);
        }
        // Render(doubledGrid, currPos, boxLocs);

        long sum = boxLocs.Sum(b => (100 * b.Item1.y) + b.Item1.x);

        return $"Doubled GPS coordinate sum: <+white>{sum}";
    }
}
