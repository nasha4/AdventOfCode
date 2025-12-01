namespace Advent_of_Code.Advent2025;

public class Day01(bool isPart1) : IAdventPuzzle
{
    public string Solve(InputHelper inputHelper) =>
        inputHelper.EachLine(line => int.Parse(line[1..]) * (line[0] == 'L' ? -1 : 1))
            .Aggregate((dial: 50, zeroes: 0),
                (state, turn) => (state.dial + turn, state.zeroes +
                    (isPart1 && (state.dial + turn) % 100 == 0 ? 1 : 0) +
                    (!isPart1 ? Math.Abs(Clicks(state.dial, turn) - Clicks(state.dial + turn, turn)) : 0)))
        .zeroes.ToString();

    private static int Clicks(int dial, int turn) => dial / 100 - (dial % 100 != 0 && dial < 0 ? 1 : 0) - (dial % 100 == 0 && turn < 0 ? 1 : 0);
}