namespace Advent_of_Code.Advent2024;

public class Day15(bool isPart1) : IAdventPuzzle
{
    public string Solve(InputHelper inputHelper)
    {
        var grid = inputHelper.EachLineInSection(line => line)
            .SelectMany((line, y) => line.Select((c, x) => (c, x, y)))
            .Where(t => "#O@".Contains(t.c))
            .ToDictionary(t => (x: t.x * 2, t.y), t => t.c);
        var directions = string.Join(string.Empty, inputHelper.EachLineInSection(line => line));

        var robot = grid.Single(kvp => kvp.Value == '@').Key;

        foreach (var half in grid.Where(kvp => "#O".Contains(kvp.Value)).ToList())
            grid[Move('+')(half.Key)] = half.Value == '#' ? '#' : ']';

        foreach (var move in directions.Select(dir => Move(dir, isPart1)))
        {
            List<HashSet<(int x, int y)>> stack = [[robot]];
            while (stack[^1].Count > 0 && !stack[^1].Any(xy => grid.GetValueOrDefault(xy, '.') == '#'))
            {
                var push = stack[^1].Select(move).Where(grid.ContainsKey).ToHashSet();
                if (move((0, 0)).x == 0 && !isPart1)
                {
                    push.UnionWith(push.Where(xy => grid[xy] == 'O').Select(Move('+')).ToHashSet());
                    push.UnionWith(push.Where(xy => grid[xy] == ']').Select(Move('-')).ToHashSet());
                }
                stack.Add(push);
            }

            if (stack[^1].Count == 0)
            {
                robot = move(robot);
                foreach (var box in stack.AsEnumerable().Reverse().SelectMany(x => x))
                {
                    grid[move(box)] = grid[box];
                    grid.Remove(box);
                }
            }
        }

        return grid.Where(kvp => kvp.Value == 'O')
            .Sum(kvp => kvp.Key.x / (isPart1 ? 2 : 1) + kvp.Key.y * 100).ToString();
    }

    private static Func<(int, int), (int x, int y)> Move(char dir, bool isPart1 = false) => t => TuplesAdd(t, dir switch
    {
        '^' => (0, -1),
        '+' => (1, 0),
        '>' => (isPart1 ? 2 : 1, 0),
        'v' => (0, 1),
        '-' => (-1, 0),
        '<' => (isPart1 ? -2 : -1, 0),
        _ => throw new ArgumentException($"unknown dir {dir}")
    });

    private static (int x, int y) TuplesAdd(params (int x, int y)[] tuples) => (tuples.Sum(x => x.x), tuples.Sum(y => y.y));
}