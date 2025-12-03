namespace Advent_of_Code.Advent2025;

public class Day03(bool isPart1) : IAdventPuzzle
{
    public string Solve(InputHelper inputHelper) => inputHelper.EachLine(line => line).Sum(bank => MaxJoltage(bank, isPart1 ? 2 : 12)).ToString();

    private static long MaxJoltage(string bank, int size) => Enumerable.Range(0, size).Reverse().Aggregate((bank, sum: 0L), NextBest).sum;

    private static (string bank, long sum) NextBest((string bank, long sum) state, int i)
    {
        var bestIndex = state.bank[..^i].Select((x, i) => (x, i)).MaxBy(x => x.x).i + 1;
        return (state.bank[bestIndex..], state.sum * 10 + state.bank[bestIndex - 1] - '0');
    }
}