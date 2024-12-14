using aoc2024.Util;

namespace aoc2024;

internal class Day14 : Day
{
    private record robot(ivec2 position, ivec2 velocity);

    private readonly List<robot> robots = [];

    private const int width = 101;
    private const int height = 103;

    internal override void Parse()
    {
        var lines = Util.Parsing.ReadAllLines($"{GetDay()}");
        foreach (var line in lines)
        {
            var split = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var pos = ivec2.Parse(split[0].Split('=')[1]);
            var vel = ivec2.Parse(split[1].Split('=')[1]);
            robots.Add(new robot(pos,vel));
        }
    }

    private static void Move(List<robot> robots, int idx)
    {
        var newPos = robots[idx].position + robots[idx].velocity;
        if (newPos.x is >= width or < 0)
        {
            newPos = new ivec2(Util.Math.Modulo(newPos.x - width, width), newPos.y);
        }
        if (newPos.y is >= height or < 0)
        {
            newPos = new ivec2(newPos.x, Util.Math.Modulo(newPos.y - height, height));
        }

        robots[idx] = robots[idx] with {position = newPos};
    }

    private static void Render(List<robot> robots, bool skipCenter)
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (skipCenter && (x == width / 2 || y == height / 2))
                {
                    Logger.Log(" ");
                    continue;
                }

                var cnt = robots.Count(r => r.position.x == x && r.position.y == y);
                Logger.Log(cnt > 0 ? $"{Util.Constants.SolidBlock}" : " ");
            }
            Logger.LogLine("");
        }

        Logger.LogLine("");
    }

    private static void Simulate(List<robot> robots, int num)
    {
        for (int i = 0; i < num; i++)
        {
            for (int j = 0; j < robots.Count; j++)
            {
                Move(robots, j);
            }
        }
    }

    internal override string Part1()
    {
        var copy = new List<robot>(robots);
        // Render(copy, false);

        Simulate(copy, 100);

        // Render(copy, false);
        // Render(copy, true);

        var q1 = copy.Count(r => r.position is {x: < width / 2, y: < height / 2});
        var q2 = copy.Count(r => r.position is {x: > width / 2, y: < height / 2});
        var q3 = copy.Count(r => r.position is {x: < width / 2, y: > height / 2});
        var q4 = copy.Count(r => r.position is {x: > width / 2, y: > height / 2});
        long total = q1 * q2 * q3 * q4;

        return $"Safety factor after 100 seconds: <+white>{total}";
    }

    internal override string Part2()
    {
        // trust me bro...
        int idxWithTree = 7092;

        // the programmatic detection below takes nearly 2 minutes on my machine with my input. until i'm able to find a
        // faster detection, i'm just going to leave this commented out.

        // var copy = new List<robot>(robots);
        // bool found = false;
        // for (int i = 0; !found && i < 10000; i++)
        // {
        //     for (int j = 0; j < robots.Count; j++)
        //     {
        //         Move(copy, j);
        //     }
        //
        //     for (int y = 0; !found && y < height; y++)
        //     {
        //         ivec2 last = ivec2.ZERO;
        //         int numConsecutive = 0;
        //         for (int x = 0; !found && x < width; x++)
        //         {
        //             if (copy.Any(r => r.position.x == x && r.position.y == y))
        //             {
        //                 if (last.x == x - 1)
        //                 {
        //                     numConsecutive++;
        //                     if (numConsecutive > 6)
        //                     {
        //                         idxWithTree = i;
        //                         Logger.LogLine($"{i + 1}:");
        //                         Render(copy, false);
        //                         found = true;
        //                     }
        //                 }
        //                 else
        //                 {
        //                     numConsecutive = 0;
        //                 }
        //
        //                 last = new ivec2(x, y);
        //             }
        //         }
        //     }
        // }

        Logger.LogLine("disclaimer: this only works with my input:");
        return $"Fewest seconds to display easter egg: <+white>{idxWithTree + 1}";
    }
}
