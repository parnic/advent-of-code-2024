namespace aoc2024;

internal class Day25 : Day
{
    private List<List<int>> locks = [];
    private List<List<int>> keys = [];

    internal override void Parse()
    {
        var lines = Util.Parsing.ReadAllLines($"{GetDay()}").ToList();
        var type = -1;
        var grid = new bool[5, 5];
        int count = 0;
        bool checking = false;
        foreach (var line in lines)
        {
            if (line.Length == 0)
            {
                type = -1;
                count = 0;
                continue;
            }

            if (type == -1)
            {
                type = line == "#####" ? 0 : 1;
            }

            if (line is "#####" or "....." && (!checking || count == 5))
            {
                checking = true;

                if (count == 5)
                {
                    List<int> arrangement = [];
                    for (int x = 0; x < grid.GetLength(1); x++)
                    {
                        int height = 0;
                        for (int y = 0; y < grid.GetLength(0); y++)
                        {
                            if (grid[x, y])
                            {
                                height++;
                            }
                        }

                        arrangement.Add(height);
                    }

                    if (type == 0)
                    {
                        locks.Add(arrangement);
                    }
                    else
                    {
                        keys.Add(arrangement);
                    }

                    checking = false;
                }
                continue;
            }

            for (int i = 0; i < line.Length; i++)
            {
                grid[i, count] = line[i] == '#';
            }

            count++;
        }
    }

    internal override string Part1()
    {
        long matches = 0;
        foreach (var key in keys)
        {
            foreach (var lck in locks)
            {
                bool match = true;
                for (int i = 0; i < 5; i++)
                {
                    if (key[i] + lck[i] > 5)
                    {
                        match = false;
                        break;
                    }
                }

                if (match)
                {
                    matches++;
                }
            }
        }

        return $"Lock/key fits: <+white>{matches}";
    }

    internal override string Part2()
    {
        return "<red>M<green>e<red>r<green>r<red>y <green>C<red>h<green>r<red>i<green>s<red>t<green>m<red>a<green>s<red>!";
    }
}
