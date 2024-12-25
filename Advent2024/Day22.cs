namespace Advent_of_Code.Advent2024;

public class Day22(InputHelper inputHelper) : IAdventPuzzle
{
    public long Solve(bool isPart1)
    {
        if (isPart1)
            return inputHelper.EachLine(long.Parse)
                .Select(secret => Enumerable.Range(0, 2000).Aggregate(secret, Evolve)).Sum();

        var priceBySequence = new List<Dictionary<(int, int, int, int), int>>();
        foreach (var seed in inputHelper.EachLine(long.Parse))
        {
            var sequence = Sequence(seed).Take(2000).Select(x => (int)(x % 10)).ToArray();
            var changes = sequence.Zip(sequence.Skip(1), (a, b) => (change: b - a, price: b)).ToArray();
            var last4ChangesAndPrice = new List<(int a, int b, int c, int d, int price)>();
            for (var i = 0; i < changes.Length - 3; i++)
            {
                var last4 = changes[i..(i + 4)];
                last4ChangesAndPrice.Add((last4[0].change, last4[1].change, last4[2].change, last4[3].change, last4[3].price));
            }
            priceBySequence.Add(last4ChangesAndPrice.GroupBy(x => (x.a, x.b, x.c, x.d), x => x.price).ToDictionary(g => g.Key, g => g.First()));
        }
        var best = 0;
        for (var a = -9; a <= 0; a++)
            for (var b = 0; b < 10; b++)
                for (var c = -9; c <= 0; c++)
                    for (var d = 0; d < 10; d++)
                    {
                        var price = priceBySequence.Sum(pbs => pbs.GetValueOrDefault((a, b, c, d), 0));
                        best = price > best ? price : best;
                    }

        return best;
    }

    private static IEnumerable<long> Sequence(long n)
    {
        while (true)
        {
            yield return n;
            n = Evolve(n, 0);
        }
    }

    private static long Evolve(long n, int _)
    {
        n = MixPrune(n * 64, n);
        n = MixPrune(n / 32, n);
        n = MixPrune(n * 2048, n);
        return n;
    }

    private static long MixPrune(long v, long n) => (v ^ n) % 16777216;
}
