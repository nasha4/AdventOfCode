namespace Advent_of_Code.Advent2025;

public class Day04(bool isPart1) : IAdventPuzzle
{
    public string Solve(InputHelper inputHelper)
    {
        var grid = new Grid.Helper(inputHelper);
        var paper = grid['@'].ToHashSet(grid);
        var removable = paper.Where(roll => grid.Orthodiagonal(roll).Count(paper.Contains) < 4);
        if (isPart1) return removable.Count().ToString();

        while (removable.Any())
        {
            paper.ExceptWith(removable);
            removable = paper.Where(roll => grid.Orthodiagonal(roll).Count(paper.Contains) < 4);
        }
        return (grid['@'].Count - paper.Count).ToString();
    }
}