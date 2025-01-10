using System.Data;

namespace Advent_of_Code.Advent2020;

public class Day05(bool isPart1) : IAdventPuzzle
{
    public string Solve(InputHelper inputHelper)
    {
        var seatCodes = inputHelper.EachLine(line => line.Aggregate((row: 0, col: 0), (acc, term) => term switch
        {
            'B' => (acc.row * 2 + 1, acc.col),
            'F' => (acc.row * 2, acc.col),
            'R' => (acc.row, acc.col * 2 + 1),
            'L' => (acc.row, acc.col * 2),
            _ => acc
        })).Select(seat => seat.row * 8 + seat.col).Order().ToList();

        return isPart1
            ? seatCodes[^1].ToString()
            : seatCodes.Zip(seatCodes.Skip(1), (a, b) => (n: a + 1, d: b - a)).Single(x => x.d > 1).n.ToString();
    }
}
