using aoc2024.Util;

namespace aoc2024;

internal class Day16 : Day
{
    private ivec2 startPos = ivec2.ZERO;
    private ivec2 endPos = ivec2.ZERO;
    private bool[][] grid = [];

    internal override void Parse()
    {
        var lines = Util.Parsing.ReadAllLines($"{GetDay()}").ToList();
        grid = new bool[lines.Count][];
        for (int i = 0; i < lines.Count; i++)
        {
            var row = new bool[lines[i].Length];
            for (int j = 0; j < lines[i].Length; j++)
            {
                row[j] = lines[i][j] != '#';
                if (lines[i][j] == 'S')
                {
                    startPos = new ivec2(j, i);
                }
                else if (lines[i][j] == 'E')
                {
                    endPos = new ivec2(j, i);
                }
            }

            grid[i] = row;
        }
    }

    internal override string Part1()
    {
        Dictionary<(ivec2 facing, ivec2 pos), int> visited = [];
        var currDir = ivec2.RIGHT;
        var q = new Queue<(int steps, int turns, ivec2 facing, ivec2 pos)>();
        q.Enqueue((1, 0, currDir, startPos + currDir));
        q.Enqueue((0, 1, currDir.GetRotatedLeft(), startPos));
        q.Enqueue((0, 1, currDir.GetRotatedRight(), startPos));
        while (q.TryDequeue(out var step))
        {
            var thisStepCost = step.turns * 1000 + step.steps;
            var tuple = (step.facing, step.pos);
            if (visited.TryGetValue(tuple, out var turns) && turns <= thisStepCost)
            {
                continue;
            }

            visited[tuple] = thisStepCost;

            if (step.pos == endPos)
            {
                continue;
            }

            if (grid[step.pos.y][step.pos.x])
            {
                q.Enqueue((step.steps + 1, step.turns, step.facing, step.pos + step.facing));
            }

            q.Enqueue((step.steps, step.turns + 1, step.facing.GetRotatedLeft(), step.pos));
            q.Enqueue((step.steps, step.turns + 1, step.facing.GetRotatedRight(), step.pos));
        }

        var endCosts = visited.Where(v => v.Key.pos == endPos).ToList();
        var cheapest = endCosts.Min(c => c.Value);

        return $"Lowest score: <+white>{cheapest}";
    }

    internal override string Part2()
    {
        Dictionary<(ivec2 facing, ivec2 pos), int> visited = [];
        Dictionary<int, List<List<ivec2>>> winners = [];
        var currDir = ivec2.RIGHT;
        var q = new Queue<(int steps, int turns, ivec2 facing, ivec2 pos, List<ivec2> history)>();
        q.Enqueue((1, 0, currDir, startPos + currDir, [startPos]));
        q.Enqueue((0, 1, currDir.GetRotatedLeft(), startPos, [startPos]));
        q.Enqueue((0, 1, currDir.GetRotatedRight(), startPos, [startPos]));
        while (q.TryDequeue(out var step))
        {
            var thisStepScore = step.turns * 1000 + step.steps;
            var tuple = (step.facing, step.pos);
            if (visited.TryGetValue(tuple, out var turns) && turns < thisStepScore)
            {
                continue;
            }

            visited[tuple] = thisStepScore;

            if (step.pos == endPos)
            {
                if (winners.TryGetValue(thisStepScore, out var history))
                {
                    history.Add(step.history);
                }
                else
                {
                    winners[thisStepScore] = [step.history];
                }
                continue;
            }

            if (grid[step.pos.y][step.pos.x])
            {
                List<ivec2> newHistory = [..step.history, step.pos];
                q.Enqueue((step.steps + 1, step.turns, step.facing, step.pos + step.facing, newHistory));
            }

            q.Enqueue((step.steps, step.turns + 1, step.facing.GetRotatedLeft(), step.pos, step.history));
            q.Enqueue((step.steps, step.turns + 1, step.facing.GetRotatedRight(), step.pos, step.history));
        }

        var endCosts = visited.Where(v => v.Key.pos == endPos).ToList();
        var cheapest = endCosts.Min(c => c.Value);
        var cheapestWinners = winners[cheapest];
        HashSet<ivec2> winnersVisited = [];
        foreach (var set in cheapestWinners)
        {
            foreach (var loc in set)
            {
                winnersVisited.Add(loc);
            }
        }

        return $"# tiles part of a best path: <+white>{winnersVisited.Count + 1}";
    }
}
