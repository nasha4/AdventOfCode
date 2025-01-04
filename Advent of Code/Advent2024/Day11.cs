namespace Advent_of_Code.Advent2024;

public class Day11(bool isPart1) : IAdventPuzzle
{
    public string Solve(InputHelper inputHelper) =>
        Enumerable.Range(1, isPart1 ? 25 : 75).Aggregate(
                inputHelper.EachLine(line => line.Split(' ').Select(long.Parse)).Single()
                    .GroupBy(x => x)
                    .ToDictionary(g => g.Key, g => g.LongCount()),
                (stones, _) => stones.SelectMany(stone => Blink(stone.Key), (stone, n) => (n, count: stone.Value))
                    .GroupBy(t => t.n, t => t.count)
                    .ToDictionary(g => g.Key, g => g.Sum()))
            .Values.Sum().ToString();

    private static IEnumerable<long> Blink(long n) => n switch {
        0 => [1],
        _ when n.ToString().Length % 2 == 0 =>
            [int.Parse(n.ToString()[..(n.ToString().Length / 2)]),
                 int.Parse(n.ToString()[(n.ToString().Length / 2)..])],
        _ => [n * 2024]
    };
}
