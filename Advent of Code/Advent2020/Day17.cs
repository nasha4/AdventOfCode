namespace Advent_of_Code.Advent2020;

public class Day17(bool isPart1) : IAdventPuzzle
{
    public string Solve(InputHelper inputHelper)
    {
        var lines = inputHelper.EachLine();
        var grid = new Grid.Helper(isPart1 ? [lines] : [new[] { lines }.AsEnumerable()]);
        for (var n = 0; n < 6; n++)
        {
            grid.Add([.. grid.Min.Select(x => x - 1)], '.');
            grid.Add([.. grid.Max.Select(x => x + 1)], '.');
            var neighbors = new Grid.Helper<bool>(grid['#'].SelectMany(x => grid.Orthodiagonal(x)).ToHashSet(grid));
            grid = new Grid.Helper(neighbors[true].ToDictionary(x => x, x => Cycle(grid, x)));
        }
        return grid['#'].Count.ToString();
    }
    private static char Cycle(Grid.Helper grid, int[] p) =>
        (grid[p], grid.Orthodiagonal(p).Count(x => grid[x] == '#')) switch { ('#', 2) or (_, 3) => '#', _ => '.' };
}