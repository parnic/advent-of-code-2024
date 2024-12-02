namespace aoc2024;

internal class Day02 : Day
{
    private List<List<int>> nums = [];
    internal override void Parse()
    {
        var lines = Util.Parsing.ReadAllLines($"{GetDay()}");
        foreach (var line in lines)
        {
            var split = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            List<int> list = [];
            split.ForEach(n => list.Add(int.Parse(n)));
            nums.Add(list);
        }
    }

    internal override string Part1()
    {
        int numSafe = 0;
        foreach (var list in nums)
        {
            bool isValid = true;
            bool dir = list[1] > list[0];
            for (int i = 1; i < list.Count; i++)
            {
                var diff = list[i] - list[i - 1];
                if (Math.Abs(diff) > 3 || dir != list[i] > list[i - 1] || list[i] == list[i - 1])
                {
                    isValid = false;
                    break;
                }
            }

            if (isValid)
            {
                numSafe++;
            }
        }

        return $"Safe reports: <+white>{numSafe}";
    }

    internal override string Part2()
    {
        bool IsValidFunc(List<int> list)
        {
            bool dir = list[1] > list[0];
            for (int i = 1; i < list.Count; i++)
            {
                var diff = list[i] - list[i - 1];
                if (Math.Abs(diff) > 3 || dir != list[i] > list[i - 1] || list[i] == list[i - 1])
                {
                    return false;
                }
            }

            return true;
        }

        int numSafe = 0;
        foreach (var list in nums)
        {
            bool isValid = IsValidFunc(list);
            if (!isValid)
            {
                // super crappy way to do this, but the input set is small enough for it to work.
                for (int i = 0; i < list.Count; i++)
                {
                    var list2 = new List<int>(list);
                    list2.RemoveAt(i);
                    if (IsValidFunc(list2))
                    {
                        isValid = true;
                        break;
                    }
                }
            }

            if (isValid)
            {
                numSafe++;
            }
        }

        return $"Problem-dampener safe reports: <+white>{numSafe}";
    }
}
