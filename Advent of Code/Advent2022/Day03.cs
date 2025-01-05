namespace Advent_of_Code.Advent2022;

public class Day03(bool isPart1) : IAdventPuzzle
{
    private static readonly Dictionary<char, int> priority = " abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ"
        .Select((c, i) => (c, i)).ToDictionary(x => x.c, x => x.i);

    public string Solve(InputHelper inputHelper) => isPart1
        ? inputHelper.EachLine(line => line.Take(line.Length / 2).Intersect(line.Skip(line.Length / 2)).Single())
            .Sum(x => priority[x]).ToString()
        : inputHelper.EachLine().Chunk(3)
            .Sum(l => priority[l[0].Intersect(l[1]).Intersect(l[2]).Single()]).ToString();
}
