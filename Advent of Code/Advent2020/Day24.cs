using System.Text.RegularExpressions;

namespace Advent_of_Code.Advent2020;

public partial class Day24(bool isPart1) : IAdventPuzzle
{
    public string Solve(InputHelper inputHelper)
    {
        var blackTiles = inputHelper.EachLine(line => HexMoves((0, 0), StepPattern.Matches(line).Select(x => x.Value)))
            .GroupBy(tile => tile)
            .Where(g => int.IsOddInteger(g.Count()))
            .Select(x => x.Key)
            .ToHashSet();

        for (var n = 0; !isPart1 && n < 100; n++)
            blackTiles = LivingArt(blackTiles);

        return blackTiles.Count.ToString();
    }

    private static readonly IEnumerable<string> neighbors = ["e", "w", "ne", "nw", "se", "sw"];
    private static IEnumerable<(int, int)> HexNeighbors((int, int) tile) => neighbors.Select(move => HexMoves(tile, move));
    private static (int, int) HexMoves((int x, int y) start, params IEnumerable<string> moves) =>
        moves.Aggregate(start, (tile, move) => move switch
        {
            "e" => (tile.x + 1, tile.y), "w" => (tile.x - 1, tile.y),
            "se" => (tile.x, tile.y + 1), "nw" => (tile.x, tile.y - 1),
            "sw" => (tile.x - 1, tile.y + 1), "ne" => (tile.x + 1, tile.y - 1),
            _ => tile
        });

    private static HashSet<(int, int)> LivingArt(HashSet<(int, int)> wasBlack) => wasBlack
        .SelectMany(HexNeighbors)
        .GroupBy(tile => tile)
        .Where(g => (g.Count(), wasBlack.Contains(g.Key)) is (2, _) or (1, true))
        .Select(g => g.Key)
        .ToHashSet();

    [GeneratedRegex(@"[ns]?[ew]")]
    private static partial Regex StepPattern { get; }
}