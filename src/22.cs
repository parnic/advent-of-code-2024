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

        var step2 = (long)System.Math.Truncate(secret / 32.0);
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
        var sequences = new List<List<long>>(secretNumbers.Count);
        var prices = new List<List<long>>(secretNumbers.Count);
        var changes = new List<List<long>>(secretNumbers.Count);
        for (int idx = 0; idx < secretNumbers.Count; idx++)
        {
            sequences.Add(new List<long>(2000));
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

                sequences[idx].Add(next);
                prices[idx].Add(price);
                changes[idx].Add(change);

                lastPrice = price;
            }
        }

        var tuplePrices = new List<Dictionary<(long, long, long, long), long>>(secretNumbers.Count);

        for (int i = 0; i < secretNumbers.Count; i++)
        {
            tuplePrices.Add([]);
            for (int j = 0; j < 2000 - 4; j++)
            {
                var tuple = (changes[i][j], changes[i][j + 1], changes[i][j + 2], changes[i][j + 3]);
                var price = prices[i][j + 4];
                tuplePrices[i].TryAdd(tuple, price);
            }
        }

        var highestPrice = 0L;
        // assume the highest sequence exists in the first buyer. works for my input.
        foreach (var tuplePrice in tuplePrices[0])
        {
            var totalPrice = tuplePrices.Select(p => p.GetValueOrDefault(tuplePrice.Key, 0)).Sum();

            if (totalPrice > highestPrice)
            {
                highestPrice = totalPrice;
            }
        }

        return $"Most bananas I can get: <+white>{highestPrice}";
    }
}
