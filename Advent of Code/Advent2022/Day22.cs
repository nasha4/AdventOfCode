using System.Text.RegularExpressions;

namespace Advent_of_Code.Advent2022;

public partial class Day22(bool isPart1) : IAdventPuzzle
{
    private const int FaceSize = 50;
    private Grid.Helper grid = new();
    private static readonly (int minY, int maxY)[] edgeX = [(2, 4), (0, 3), (0, 1)];
    private static readonly (int minX, int maxX)[] edgeY = [(1, 3), (1, 2), (0, 2), (0, 1)];
    private static readonly Dictionary<(int face, int facing), (int face, int facing, string type)> cube = new()
    {
        [(1, 3)] = (9, 0, "xy"), [(9, 2)] = (1, 1, "xy"),
        [(2, 3)] = (9, 3, "xx"), [(9, 1)] = (2, 1, "xx"),
        [(1, 2)] = (6, 0, "-x"), [(6, 2)] = (1, 0, "-x"),
        [(2, 0)] = (7, 2, "-x"), [(7, 0)] = (2, 2, "-x"),
        [(2, 1)] = (4, 2, "xy"), [(4, 0)] = (2, 3, "xy"),
        [(4, 2)] = (6, 1, "xy"), [(6, 3)] = (4, 0, "xy"),
        [(7, 1)] = (9, 2, "xy"), [(9, 0)] = (7, 3, "xy"),
    };

    public string Solve(InputHelper inputHelper)
    {
        grid = new Grid.Helper(inputHelper.EachLineInSection(x => x.PadRight(FaceSize * 3)));

        var location = new[] { 0, edgeY[0].minX * FaceSize, 0 }; // y, x, facing

        foreach (var (go, turn) in inputHelper.EachMatchGroup(StepPattern, matches => (int.Parse(matches[1]), matches[2])))
        {
            for (var i = 0; i < go; i++)
                location = isPart1 ? TryStep(location) : TryCubeStep(location);
            location[2] = (location[2] + turn switch { "L" => 3, "R" => 1, _ => 0 }) % 4;
        }
        return $"{(location[0] + 1) * 1000 + (location[1] + 1) * 4 + location[2]}";
    }

    private int[] TryStep(int[] loc)
    {
        int[] newLoc = loc[2] switch
        {
            0 => [loc[0], loc[1] + 1, loc[2]],
            2 => [loc[0], loc[1] - 1, loc[2]],
            1 => [loc[0] + 1, loc[1], loc[2]],
            _ => [loc[0] - 1, loc[1], loc[2]]
        };
        if (grid[newLoc[0..2]] is ' ' or '\0')
            newLoc = loc[2] switch
            {
                0 => [loc[0], edgeY[loc[0] / FaceSize].minX * FaceSize, loc[2]],
                2 => [loc[0], edgeY[loc[0] / FaceSize].maxX * FaceSize - 1, loc[2]],
                1 => [edgeX[loc[1] / FaceSize].minY * FaceSize, loc[1], loc[2]],
                _ => [edgeX[loc[1] / FaceSize].maxY * FaceSize - 1, loc[1], loc[2]]
            };
        return grid[newLoc[0..2]] == '#' ? loc : newLoc;
    }

    private int[] TryCubeStep(int[] loc)
    {
        int[] newLoc = loc[2] switch
        {
            0 => [loc[0], loc[1] + 1, loc[2]],
            2 => [loc[0], loc[1] - 1, loc[2]],
            1 => [loc[0] + 1, loc[1], loc[2]],
            _ => [loc[0] - 1, loc[1], loc[2]]
        };
        if (grid[newLoc[0..2]] is ' ' or '\0')
        {
            var (face, newFacing, offsetType) = cube[(loc[1] / FaceSize + loc[0] / FaceSize * 3, loc[2])];
            int[] offsets = offsetType switch
            {
                "xx" => [loc[0] % FaceSize, loc[1] % FaceSize],
                "xy" => [loc[1] % FaceSize, loc[0] % FaceSize],
                _ => [FaceSize - 1 - (loc[0] % FaceSize), FaceSize - 1 - (loc[1] % FaceSize)]
            };
            newLoc = newFacing switch
            {
                0 => [face / 3 * FaceSize + offsets[0], face % 3 * FaceSize, newFacing],
                2 => [face / 3 * FaceSize + offsets[0], face % 3 * FaceSize + FaceSize - 1, newFacing],
                1 => [face / 3 * FaceSize, face % 3 * FaceSize + offsets[1], newFacing],
                _ => [face / 3 * FaceSize + FaceSize - 1, face % 3 * FaceSize + offsets[1], newFacing]
            };
        }
        return grid[newLoc[0..2]] == '#' ? loc : newLoc;
    }

    [GeneratedRegex(@"(\d+)([LR]|$)")]
    private static partial Regex StepPattern { get; }
}