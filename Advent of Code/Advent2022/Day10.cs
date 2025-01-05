using System.Text;

namespace Advent_of_Code.Advent2022;

public class Day10(bool isPart1) : IAdventPuzzle
{
    private const int spriteWidth = 3, lineWidth = 40;

    public string Solve(InputHelper inputHelper)
    {
        var xRegister = 1;
        var interestingCycles = new[] { 20, 60, 100, 140, 180, 220 };
        var xHistory = new List<int>() { xRegister }; // cycles start at 1 not 0, so add a dummy value here
        foreach (var parts in inputHelper.EachLine(line => line.Split(' ')))
        {
            xHistory.Add(xRegister); // one cycle
            if (parts[0] == "addx")
            {
                xHistory.Add(xRegister); // two cycles
                xRegister += int.Parse(parts[1]); // addx completes after the second cycle
            }
        }

        return isPart1
            ? interestingCycles.Sum(cycle => cycle * xHistory[cycle]).ToString()
            : string.Join(Environment.NewLine, xHistory.Select(SpriteCheck).Skip(1).Chunk(lineWidth).Select(x => new string(x)));
    }

    private static char SpriteCheck(int x, int col) => col % lineWidth >= x && col % lineWidth < x + spriteWidth ? '#' : '.';
}
