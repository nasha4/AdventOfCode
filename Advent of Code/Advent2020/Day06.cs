namespace Advent_of_Code.Advent2020;

public class Day06(bool isPart1) : IAdventPuzzle
{
    public string Solve(InputHelper inputHelper) =>
        inputHelper.EachSection(lines => lines.Select(x => x.AsEnumerable()))
            .Sum(lines => lines.Aggregate((acc, term) => isPart1 ? acc.Union(term) : acc.Intersect(term)).Count()).ToString();
}
