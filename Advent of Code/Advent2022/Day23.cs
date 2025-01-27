namespace Advent_of_Code.Advent2022;

public class Day23(bool isPart1) : IAdventPuzzle
{
    public string Solve(InputHelper inputHelper)
    {
        var grid = new Grid.Helper(inputHelper.EachLine()); // grid.Add() is slow as hell, really not what Grid.Helper was designed for
        var elves = grid['#'].ToHashSet(grid);

        for (var round = 1; round <= 10 || !isPart1; round++)
        {
            var consider = elves
                .Where(elf => Neighbors(elf).Any(elves.Contains)) // get elves with neighbor elves
                .GroupBy(elf => Consider(elves, elf, round), grid) // get each elf's proposed move (if any)
                .Where(group => group.Count() == 1) // get those moves only proposed by one elf
                .Select(group => (to: group.Key, from: group.Single())) // pair each such move with its elf
                .Where(pair => !grid.Equals(pair.to, pair.from)) // filter out moves to the same spot
                .ToList();

            elves.ExceptWith(consider.Select(pair => pair.from));
            elves.UnionWith(consider.Select(pair => pair.to));

            if (consider.Count == 0 && !isPart1)
                return round.ToString();
        }
        var (minX, minY, maxX, maxY) = elves.Aggregate((minX: int.MaxValue, minY: int.MaxValue, maxX: int.MinValue, maxY: int.MinValue),
            (m, elf) => (int.Min(m.minX, elf[1]), int.Min(m.minY, elf[0]), int.Max(m.maxX, elf[1]), int.Max(m.maxY, elf[0])));
        return $"{(maxX - minX + 1) * (maxY - minY + 1) - elves.Count}";
    }

    private static int[][] Neighbors(int[] elf) => [
        [elf[0]-1, elf[1]-1], [elf[0]-1, elf[1]], [elf[0]-1, elf[1]+1],
        [elf[0],   elf[1]-1],                     [elf[0],   elf[1]+1],
        [elf[0]+1, elf[1]-1], [elf[0]+1, elf[1]], [elf[0]+1, elf[1]+1]];

    private static readonly IEnumerable<int> Indices = [3, 0, 1, 2, 3, 0, 1];
    private static int[] Consider(HashSet<int[]> elves, int[] elf, int step) => Indices
        .Skip(step % 4).Take(4).Select(dir => Look(elves, elf, dir)).OfType<int[]>().FirstOrDefault() ?? elf;

    private static int[]? Look(HashSet<int[]> elves, int[] elf, int direction) => direction switch
    {
        0 => Neighbors(elf).Any(n => n[0] < elf[0] && elves.Contains(n)) ? null : [elf[0] - 1, elf[1]],
        1 => Neighbors(elf).Any(n => n[0] > elf[0] && elves.Contains(n)) ? null : [elf[0] + 1, elf[1]],
        2 => Neighbors(elf).Any(n => n[1] < elf[1] && elves.Contains(n)) ? null : [elf[0], elf[1] - 1],
        3 => Neighbors(elf).Any(n => n[1] > elf[1] && elves.Contains(n)) ? null : [elf[0], elf[1] + 1],
        _ => null
    };
}