namespace Advent_of_Code.Advent2022;

public class Day01(bool isPart1) : IAdventPuzzle
{
    public string Solve(InputHelper inputHelper)
    {
        var elves = inputHelper.EachSection(section => section.Sum(Convert.ToInt64));

        return isPart1 ? elves.Max().ToString() : elves.Order().TakeLast(3).Sum().ToString();
    }
}
