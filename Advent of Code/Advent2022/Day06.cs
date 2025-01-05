namespace Advent_of_Code.Advent2022;

public class Day06(bool isPart1) : IAdventPuzzle
{
    public string Solve(InputHelper inputHelper)
    {
        var dataStream = inputHelper.EachLine().Single();
        var markerLength = isPart1 ? 4 : 14;
        for (var startIndex = 0; startIndex < dataStream.Length; startIndex++)
        {
            if (dataStream.Skip(startIndex).Take(markerLength).Distinct().Count() == markerLength)
                return $"{startIndex + markerLength}";
        }
        return "none found";
    }
}
