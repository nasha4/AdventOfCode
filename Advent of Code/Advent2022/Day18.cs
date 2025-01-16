namespace Advent_of_Code.Advent2022;

public class Day18(bool isPart1) : IAdventPuzzle
{
    public string Solve(InputHelper inputHelper)
    {
        var cubes = new Grid.Helper(inputHelper.EachLine(line => line.Split(',').Select(int.Parse).ToArray()).ToDictionary(p => p, _ => '#'));
        cubes.Add([.. cubes.Min.Select(x => x - 1)], 'x');
        cubes.Add([.. cubes.Max.Select(x => x + 1)], 'x');

        if (isPart1)
            return cubes['#'].SelectMany(p => cubes.Orthogonal(p),
                (_, neighbor) => cubes[neighbor] == '#' ? 0 : 1)
                .Sum().ToString();

        FloodFill(cubes, cubes.Min, 'x');

        return cubes['#'].SelectMany(p => cubes.Orthogonal(p),
            (_, neighbor) => cubes[neighbor] == 'x' ? 1 : 0)
            .Sum().ToString();
    }
    private static void FloodFill(Grid.Helper cubes, int[] start, char fill)
    {
        var toFlood = new Queue<int[]>([start]);
        cubes.Add(start, fill);
        while (toFlood.TryDequeue(out var cube))
        {
            foreach (var neighbor in cubes.Orthogonal(cube).Where(n => cubes[n] == '\0'))
            {
                toFlood.Enqueue(neighbor);
                cubes.Add(neighbor, fill);
            }
        }
    }
}
