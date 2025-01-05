namespace Advent_of_Code.Advent2022;

public class Day12(bool isPart1) : IAdventPuzzle
{
    public string Solve(InputHelper inputHelper)
    {
        var grid = new GridHelper(inputHelper.EachLine());
        var startFrom = grid[isPart1 ? 'S' : 'a'];
        return $"{FindPath(startFrom, grid).Count() - 1}";
    }

    private static int Height(char c) => c switch { 'E' => 'z', 'S' => 'a', _ => c };

    private static IEnumerable<int[]> FindPath(IEnumerable<int[]> startingSquares, GridHelper grid)
    {
        var toSearch = new Queue<int[]>(startingSquares);
        var visitedFrom = startingSquares.ToDictionary(s => s, s => null as int[], grid);
        while (toSearch.TryDequeue(out var square))
        {
            foreach (var neighbor in grid.Orthogonal(square).Where(s => Height(grid[s]) <= Height(grid[square]) + 1 && visitedFrom.TryAdd(s, square)))
            {
                if (grid[neighbor] == 'E')
                {
                    for (var s = neighbor; s is not null; s = visitedFrom[s])
                        yield return s;
                    yield break;
                }
                toSearch.Enqueue(neighbor);
            }
        }
    }
}
