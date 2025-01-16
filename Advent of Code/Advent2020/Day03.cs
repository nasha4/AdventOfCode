namespace Advent_of_Code.Advent2020;

public class Day03(bool isPart1) : IAdventPuzzle
{
    private readonly IEnumerable<int[]> slopes = isPart1 ? [[3, 1]] : [[1, 1], [3, 1], [5, 1], [7, 1], [1, 2]];
    public string Solve(InputHelper inputHelper)
    {
        var trees = new Grid.Helper(inputHelper.EachLine());

        return slopes
            .Select(slope => Enumerable.Range(0, (trees.Max[0] + 1) / slope[1])
                .Count(n => trees[[slope[1] * n, slope[0] * n % (trees.Max[1] + 1)]] == '#'))
            .Aggregate((product, treeCount) => product * treeCount).ToString();
    }
}
