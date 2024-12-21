using aoc2024.Util;

namespace aoc2024;

internal class Day21 : Day
{
    private List<string> codes = [];

    private static readonly Dictionary<char, ivec2> numericButtonLayout =
        new()
        {
            {'7', new ivec2(0, 0)},
            {'8', new ivec2(1, 0)},
            {'9', new ivec2(2, 0)},
            {'4', new ivec2(0, 1)},
            {'5', new ivec2(1, 1)},
            {'6', new ivec2(2, 1)},
            {'1', new ivec2(0, 2)},
            {'2', new ivec2(1, 2)},
            {'3', new ivec2(2, 2)},
            {'0', new ivec2(1, 3)},
            {'A', new ivec2(2, 3)},
        };

    private static readonly Dictionary<ivec2, char> reverseNumericButtonLayout =
        new()
        {
            {new ivec2(0, 0), '7'},
            {new ivec2(1, 0), '8'},
            {new ivec2(2, 0), '9'},
            {new ivec2(0, 1), '4'},
            {new ivec2(1, 1), '5'},
            {new ivec2(2, 1), '6'},
            {new ivec2(0, 2), '1'},
            {new ivec2(1, 2), '2'},
            {new ivec2(2, 2), '3'},
            {new ivec2(1, 3), '0'},
            {new ivec2(2, 3), 'A'},
        };

    private static readonly Dictionary<char, ivec2> directionalKeypadLayout =
        new()
        {
            {'^', new ivec2(1, 0)},
            {'A', new ivec2(2, 0)},
            {'<', new ivec2(0, 1)},
            {'v', new ivec2(1, 1)},
            {'>', new ivec2(2, 1)},
        };

    private static readonly Dictionary<ivec2, char> reverseDirectionalKeypadLayout =
        new()
        {
            {new ivec2(1, 0), '^'},
            {new ivec2(2, 0), 'A'},
            {new ivec2(0, 1), '<'},
            {new ivec2(1, 1), 'v'},
            {new ivec2(2, 1), '>'},
        };

    private static readonly Dictionary<(char from, char to), List<string>> numericKeypadMoves = GetMoves(numericButtonLayout);
    private static readonly Dictionary<(char from, char to), List<string>> directionalKeypadMoves = GetMoves(directionalKeypadLayout);
    private static readonly Dictionary<(char from, char to), List<string>> allMoves = numericKeypadMoves.Where(k => !directionalKeypadMoves.ContainsKey(k.Key)).Union(directionalKeypadMoves).ToDictionary();

    internal override void Parse()
    {
        codes = [..Util.Parsing.ReadAllLines($"{GetDay()}")];
    }

    private static ivec2? GetPosFromKey(char key, Dictionary<char, ivec2> layout)
    {
        if (!layout.TryGetValue(key, out var pos))
        {
            return null;
        }

        return pos;
    }

    private static char? GetKeyFromPos(ivec2 pos, Dictionary<char, ivec2> layout)
    {
        var dict = reverseDirectionalKeypadLayout;
        if (layout.Count == numericButtonLayout.Count)
        {
            dict = reverseNumericButtonLayout;
        }

        if (!dict.TryGetValue(pos, out var key))
        {
            return null;
        }

        return key;
    }

    private static Dictionary<(char from, char to), List<string>> GetMoves(Dictionary<char, ivec2> keypad)
    {
        var keys = string.Concat(keypad.Select(k => k.Key));

        Dictionary<(char from, char to), List<string>> keypadMoves = [];
        for (int fromIdx = 0; fromIdx < keys.Length; fromIdx++)
        {
            char from = keys[fromIdx];
            var keyPos = GetPosFromKey(from, keypad);

            for (int toIdx = fromIdx; toIdx < keys.Length; toIdx++)
            {
                char to = keys[toIdx];

                var pq = new PriorityQueue<ivec2, int>();
                pq.Enqueue(keyPos!.Value, 0);

                Dictionary<ivec2, (int cost, HashSet<string> options)> state = [];
                state.Add(keyPos.Value, (0, [""]));

                while (pq.TryDequeue(out var pos, out var _))
                {
                    var (cost, options) = state[pos];
                    if (GetKeyFromPos(pos, keypad) == to)
                    {
                        keypadMoves.Add((from, to), [.. options]);
                        if (from != to)
                        {
                            HashSet<string> reverseOptions = [];
                            foreach (string s1 in options)
                            {
                                string rev = s1.Reverse()
                                    .Aggregate("", (current, c) => current + c switch
                                    {
                                        '>' => '<',
                                        '<' => ">",
                                        '^' => 'v',
                                        'v' => '^',
                                        _ => c,
                                    });

                                reverseOptions.Add(rev);
                            }

                            keypadMoves.Add((to, from), [.. reverseOptions]);
                        }
                        break;
                    }

                    foreach (var neighbor in pos.GetBoundedOrthogonalNeighbors(0, 0, keypad.Max(k => k.Value.x), keypad.Max(k => k.Value.y)))
                    {
                        if (GetKeyFromPos(pos, keypad) == null)
                        {
                            continue;
                        }

                        var newCost = cost + 1;
                        var hasSeen = state.TryGetValue(neighbor, out var thisState);
                        if (!hasSeen)
                        {
                            thisState = (newCost, []);
                            state[neighbor] = thisState;
                        }

                        if (newCost != thisState.cost)
                        {
                            continue;
                        }

                        foreach (string s in options)
                        {
                            thisState.options.Add(s + ivec2.Sign(neighbor - pos).CharFromDir());
                        }

                        if (!hasSeen)
                        {
                            pq.Enqueue(neighbor, newCost);
                        }
                    }
                }
            }
        }

        return keypadMoves;
    }

    private static List<string> GetSequenceList(string code, Dictionary<(char from, char to), List<string>> keypadMoves)
    {
        List<string> sequence = [""];
        char prevKey = 'A';

        foreach (var key in code)
        {
            var newSeq = new List<string>();
            var moves = keypadMoves[(prevKey, key)];

            foreach (var prevStrokes in sequence)
            {
                foreach (var nextStroke in moves)
                {
                    newSeq.Add(prevStrokes + nextStroke + 'A');
                }
            }

            prevKey = key;
            sequence = newSeq;
        }

        return sequence;
    }

    private static long ShortestMovesTo(string current, int level, int stopLevel, Dictionary<(string key, int level), long> cache)
    {
        if (cache.TryGetValue((current, level), out var cost))
        {
            return cost;
        }

        if (level == stopLevel)
        {
            var result = GetSequenceList(current, allMoves).Select(a => a.Length).Min();
            cache.Add((current, level), result);
            return result;
        }

        var firstMoveIdx = current.IndexOf('A');
        var firstMove = current[..(firstMoveIdx + 1)];
        var remainingMoves = current[(firstMoveIdx + 1)..];

        long shortest = long.MaxValue;
        var sequences = GetSequenceList(firstMove, allMoves);
        foreach (var seq in sequences)
        {
            long count = ShortestMovesTo(seq, level + 1, stopLevel, cache);
            if (shortest > count)
            {
                shortest = count;
            }
        }

        if (remainingMoves.Length > 0)
        {
            shortest += ShortestMovesTo(remainingMoves, level, stopLevel, cache);
        }

        cache.Add((current, level), shortest);
        return shortest;
    }

    internal override string Part1()
    {
        var cache = new Dictionary<(string key, int level), long>();
        long total = 0;
        foreach (var code in codes)
        {
            var num = int.Parse(code[..3]);
            var moveLen = ShortestMovesTo(code, 0, 2, cache);
            total += moveLen * num;
        }

        return $"3-level code complexity sum: <+white>{total}";
    }

    internal override string Part2()
    {
        var cache = new Dictionary<(string key, int level), long>();
        long total = 0;
        foreach (var code in codes)
        {
            var num = int.Parse(code[..3]);
            var moveLen = ShortestMovesTo(code, 0, 25, cache);
            total += moveLen * num;
        }

        return $"26-level code complexity sum: <+white>{total}";
    }
}