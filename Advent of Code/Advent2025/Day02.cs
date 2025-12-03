namespace Advent_of_Code.Advent2025;

public class Day02(bool isPart1) : IAdventPuzzle
{
    private static readonly int[] Ten = [1, 10, 100, 1000, 10000, 100000, 1000000, 10000000, 100000000, 1000000000];
    public string Solve(InputHelper inputHelper) =>
        inputHelper.EachLine(line => line.Split(',').Select(r => r.Split('-'))).Single().Sum(range =>
        {
            var (l0, l1) = (range[0].Length, range[1].Length);
            var (n0, n1) = (long.Parse(range[0]), long.Parse(range[1]));
            var invalid = new HashSet<long>();
            foreach (var reps in isPart1 ? [2] : Enumerable.Range(2, l1 - 1))
            {
                if (l0 % reps == 0)
                    invalid.UnionWith(Repeats(n0 / Ten[l0 - l0 / reps], n1 / Ten[l0 - l0 / reps], reps));
                else if (l1 % reps == 0)
                    invalid.UnionWith(Repeats(Ten[l1 / reps - 1], n1 / Ten[l1 - l1 / reps], reps));
            }
            return invalid.Where(x => x >= n0 && x <= n1).Sum();
        }).ToString();

    private static IEnumerable<long> Repeats(long start, long end, int reps)
    {
        var ten = Ten.First(x => x > start);
        var factor = Enumerable.Repeat(ten, reps).Aggregate(0, (prod, term) => prod * term + 1);
        for (var n = start; n <= end && n < ten; n++) yield return n * factor;
    }
}