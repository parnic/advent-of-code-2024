namespace aoc2024;

internal class Day22 : Day
{
    private List<long> secretNumbers = [];
    internal override void Parse()
    {
        secretNumbers = [..Util.Parsing.ReadAllLinesAsInts($"{GetDay()}")];
    }

    private long Mix(long num, long secret)
    {
        var result = num ^ secret;
        return result;
    }

    private long Prune(long num)
    {
        var result = aoc2024.Util.Math.Modulo(num, 16777216);
        return result;
    }

    private long GenerateNext(long initial)
    {
        var step1 = initial * 64;
        var secret = Mix(step1, initial);
        secret = Prune(secret);

        var step2 = secret / 32;
        secret = Mix(step2, secret);
        secret = Prune(secret);

        var step3 = secret * 2048;
        secret = Mix(step3, secret);
        secret = Prune(secret);

        return secret;
    }

    internal override string Part1()
    {
        long total = 0;
        foreach (var num in secretNumbers)
        {
            var next = num;
            for (int i = 0; i < 2000; i++)
            {
                next = GenerateNext(next);
            }

            total += next;
        }

        return $"Sum of 2000th numbers: <+white>{total}";
    }

    internal override string Part2()
    {
        var prices = new List<List<long>>(secretNumbers.Count);
        var changes = new List<List<long>>(secretNumbers.Count);
        for (int idx = 0; idx < secretNumbers.Count; idx++)
        {
            prices.Add(new List<long>(2001));
            changes.Add(new List<long>(2000));

            var num = secretNumbers[idx];
            var next = num;
            var lastPrice = next % 10;
            prices[idx].Add(lastPrice);
            for (int i = 0; i < 2000; i++)
            {
                next = GenerateNext(next);
                var price = next % 10;
                var change = price - lastPrice;

                prices[idx].Add(price);
                changes[idx].Add(change);

                lastPrice = price;
            }
        }

        Dictionary<int, long> tuplePrices = [];

        for (int i = 0; i < secretNumbers.Count; i++)
        {
            var seen = new HashSet<int>(2000);
            for (int j = 0; j < 2000 - 4; j++)
            {
                // 5 bits per number gives us 0-31 to work with. adding 10 ensures negatives don't cause problems with shifting.
                var encoded = (int)((10 + changes[i][j]) |
                                    (10 + changes[i][j + 1]) << 5|
                                    (10 + changes[i][j + 2]) << 10 |
                                    (10 + changes[i][j + 3]) << 15);
                var price = prices[i][j + 4];
                if (!seen.Add(encoded))
                {
                    continue;
                }

                if (!tuplePrices.TryAdd(encoded, price))
                {
                    tuplePrices[encoded] += price;
                }
            }
        }

        var highestPrice = tuplePrices.OrderByDescending(t => t.Value).First().Value;
        return $"Most bananas I can get: <+white>{highestPrice}";
    }
}
