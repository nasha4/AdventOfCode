namespace Advent_of_Code.Advent2025;

public class Day07(bool isPart1) : IAdventPuzzle
{
    public string Solve(InputHelper inputHelper)
    {
        var grid = new Grid.Helper(inputHelper);
        var beams = new List<Dictionary<int, long>>() { new(){ [grid['S'].Single()[1]] = 1 } };
        var splits = 0;
        foreach (var y in grid.Ranges[0].Skip(1))
        {
            beams.Add([]);
            foreach (var (x, beamsIn) in beams[y - 1])
            {
                switch (grid[[y, x]])
                {
                    case '.': beams[y][x] = beams[y].GetValueOrDefault(x, 0) + beamsIn; break;
                    case '^':
                        beams[y][x - 1] = beams[y].GetValueOrDefault(x - 1, 0) + beamsIn;
                        beams[y][x + 1] = beams[y].GetValueOrDefault(x + 1, 0) + beamsIn;
                        splits++;
                        break;
                }
            }
        }
        return isPart1 ? splits.ToString() : beams[^1].Values.Sum().ToString();
    }
}