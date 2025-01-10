namespace Advent_of_Code.Advent2020;

public class Day17(bool isPart1) : IAdventPuzzle
{
    public string Solve(InputHelper inputHelper)
    {
        var lines = inputHelper.EachLine();
        var grid = isPart1 ? new GridHelper([lines]) : new GridHelper([new[] { lines }.AsEnumerable()]);

        return "";
    }
}
