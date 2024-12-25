namespace Advent_of_Code.Advent2024;

public class Day11(InputHelper inputHelper) : IAdventPuzzle
{
    public long Solve(bool isPart1) =>
        Enumerable.Range(1, isPart1 ? 25 : 75).Aggregate(
                inputHelper.EachLine(line => line.Split(' ').Select(long.Parse)).Single()
                    .GroupBy(x => x)
                    .ToDictionary(g => g.Key, g => g.LongCount()),
                (stones, _) => stones.SelectMany(stone => Blink(stone.Key), (stone, n) => (n, count: stone.Value))
                    .GroupBy(t => t.n, t => t.count)
                    .ToDictionary(g => g.Key, g => g.Sum()))
            .Values.Sum();

    private static IEnumerable<long> Blink(long n) => n switch {
        0 => [1],
        _ when n.ToString().Length % 2 == 0 =>
            [int.Parse(n.ToString()[..(n.ToString().Length / 2)]),
                 int.Parse(n.ToString()[(n.ToString().Length / 2)..])],
        _ => [n * 2024]
    };
}
