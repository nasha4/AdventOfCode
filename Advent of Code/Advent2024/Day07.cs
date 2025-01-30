namespace Advent_of_Code.Advent2024;

public class Day07(bool isPart1) : IAdventPuzzle
{
    public string Solve(InputHelper inputHelper) =>
        inputHelper.EachLine(line => line.Split([':', ' '], StringSplitOptions.RemoveEmptyEntries).Select(long.Parse))
            .Where(longs => longs.Skip(2).Reverse()
                .Aggregate(longs.Take(1),
                    (potentials, term) => potentials.SelectMany(result => new long?[] {
                        result > term ? result - term : null, // Un-add
                        result % term == 0 ? result / term : null, // Un-multiply
                        !isPart1 && result > term && result.ToString().EndsWith(term.ToString())
                            ? long.Parse(result.ToString()[..^term.ToString().Length]) : null // Un-concatenate
                    }.OfType<long>()).Distinct())
                .Contains(longs.Skip(1).First()))
            .Sum(x => x.First()).ToString();
}