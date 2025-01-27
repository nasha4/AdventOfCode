namespace Advent_of_Code.Advent2020;

public class Day13(bool isPart1) : IAdventPuzzle
{
    public string Solve(InputHelper inputHelper)
    {
        var startTime = inputHelper.EachLine(int.Parse).First();
        var busTimes = inputHelper.EachLine(line => line.Split(',')).Single()
            .Select((x, i) => (x, i))
            .Where(x => x is not ("x", _))
            .ToDictionary(x => int.Parse(x.x), x => - x.i);

        if (isPart1)
        {
            var bestBus = busTimes.Keys.MinBy(x => x - startTime % x);
            return $"{bestBus * (bestBus - startTime % bestBus)}";
        }
        var product = busTimes.Keys.Aggregate(1L, (a, b) => a * b);
        return $"{(CRT(busTimes) % product + product) % product}";
    }

    private static long CRT(IReadOnlyDictionary<int, int> remainders) =>
        remainders.Sum(kvp => kvp.Value * MMI(kvp.Key, remainders.Keys.Except([kvp.Key]).Aggregate(1L, (a, b) => a * b)));

    private static long MMI(long n, long product)
    {
        var (x, y, x0, x1, y0, y1) = (n, product, 1L, 0L, 0L, 1L);
        while (y > 0)
        {
            (var q, x, y) = (x / y, y, x % y);
            (x0, x1) = (x1, x0 - q * x1);
            (y0, y1) = (y1, y0 - q * y1);
        }
        return y0 * product;
    }
}