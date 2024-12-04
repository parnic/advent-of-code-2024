using aoc2024.Util;

namespace aoc2024;

internal class Day04 : Day
{
    private char[][] puzzle = [];
    internal override void Parse()
    {
        var lines = Util.Parsing.ReadAllLines($"{GetDay()}").ToList();
        int numRows = lines.Count;
        puzzle = new char[numRows][];
        for (int i = 0; i < numRows; i++)
        {
            var row = lines[i].ToCharArray();
            puzzle[i] = row;
        }
    }

    private static bool IsValid(char[][] puzzle, long row, long col)
    {
        return row >= 0 && col >= 0 && row < puzzle.Length && col < puzzle[row].Length;
    }

    private static int HasXmas(char[][] puzzle, long row, long col)
    {
        int numHas = 0;
        const string lookingFor = "XMAS";
        var dirs = ivec2.EIGHTWAY;
        foreach (var dir in dirs)
        {
            int onChar = 1;
            var nextPos = new ivec2(row, col) + dir;
            while (IsValid(puzzle, nextPos.x, nextPos.y) && puzzle[nextPos.x][nextPos.y] == lookingFor[onChar])
            {
                onChar++;
                nextPos += dir;

                if (onChar == lookingFor.Length)
                {
                    numHas++;
                    break;
                }
            }
        }

        return numHas;
    }

    internal override string Part1()
    {
        int numFound = 0;
        for (int row = 0; row < puzzle.Length; row++)
        {
            for (int col = 0; col < puzzle[row].Length; col++)
            {
                if (puzzle[row][col] == 'X')
                {
                    numFound += HasXmas(puzzle, row, col);
                }
            }
        }

        return $"Num XMAS: <+white>{numFound}";
    }

    private static bool HasMasInX(char[][] puzzle, long row, long col)
    {
        const int maxDist = 2;
        if (!IsValid(puzzle, row + maxDist, col) || !IsValid(puzzle, row, col + maxDist) || !IsValid(puzzle, row + maxDist, col + maxDist))
        {
            // not enough spots to search
            return false;
        }

        List<ivec2> ms = [];
        for (int y = 0; y <= maxDist; y++)
        {
            if (y == 1)
            {
                continue;
            }

            for (int x = 0; x <= maxDist; x++)
            {
                if (x == 1)
                {
                    continue;
                }

                if (puzzle[row + y][col + x] == 'M')
                {
                    ms.Add(new ivec2(col + x, row + y));
                }
            }
        }

        if (ms.Count != 2)
        {
            return false;
        }

        foreach (var m in ms)
        {
            var requiredDir = ivec2.ZERO;
            if (m.y == row && m.x == col)
            {
                requiredDir = ivec2.DOWN + ivec2.RIGHT;
            }
            else if (m.y == row && m.x == col + maxDist)
            {
                requiredDir = ivec2.DOWN + ivec2.LEFT;
            }
            else if (m.y == row + maxDist && m.x == col)
            {
                requiredDir = ivec2.UP + ivec2.RIGHT;
            }
            else if (m.y == row + maxDist && m.x == col + maxDist)
            {
                requiredDir = ivec2.UP + ivec2.LEFT;
            }

            var next1 = m + requiredDir;
            var next2 = m + requiredDir + requiredDir;
            if (puzzle[next1.y][next1.x] != 'A' || puzzle[next2.y][next2.x] != 'S')
            {
                return false;
            }
        }

        return true;
    }

    internal override string Part2()
    {
        int numFound = 0;
        for (int row = 0; row < puzzle.Length; row++)
        {
            for (int col = 0; col < puzzle[row].Length; col++)
            {
                if (HasMasInX(puzzle, row, col))
                {
                    numFound++;
                }
            }
        }

        return $"Num X-MAS: <+white>{numFound}";
    }
}
