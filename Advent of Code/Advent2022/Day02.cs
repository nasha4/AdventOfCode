namespace Advent_of_Code.Advent2022;

public class Day02(bool isPart1) : IAdventPuzzle
{
    public string Solve(InputHelper inputHelper) =>
        inputHelper.EachLine(line => (isPart1, line) switch {
            (true, "A X") => 3 + 1, (true, "B X") => 0 + 1, (true, "C X") => 6 + 1,
            (true, "A Y") => 6 + 2, (true, "B Y") => 3 + 2, (true, "C Y") => 0 + 2,
            (true, "A Z") => 0 + 3, (true, "B Z") => 6 + 3, (true, "C Z") => 3 + 3,
            (false, "A X") => 0 + 3, (false, "B X") => 0 + 1, (false, "C X") => 0 + 2,
            (false, "A Y") => 3 + 1, (false, "B Y") => 3 + 2, (false, "C Y") => 3 + 3,
            (false, "A Z") => 6 + 2, (false, "B Z") => 6 + 3, (false, "C Z") => 6 + 1,
            _ => -99999
        }).Sum().ToString();
}
