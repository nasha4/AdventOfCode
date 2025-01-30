namespace Advent_of_Code.Advent2020;

public class Day20(bool isPart1) : IAdventPuzzle
{
    private readonly record struct Tile(int Id, bool Flipped, string EdgeN, string EdgeE, string EdgeS, string EdgeW, int? Rotation)
    {
        public Tile(int Id, bool Flipped, string[] Edges) : this(Id, Flipped, Edges[0], Edges[1], Edges[2], Edges[3], null) { }
        public readonly string[] Edges => [EdgeN, EdgeE, EdgeS, EdgeW];
        public readonly int Corner(Dictionary<string, Tile[]> edges) => Edges.Select(x => edges[x].Length).ToArray() switch { [1, 2, 2, 1] => 0, [2, 2, 1, 1] => 1, [2, 1, 1, 2] => 2, [1, 1, 2, 2] => 3, _ => -1 };
        public Tile Rotate(int quarterTurns) => this with
        {
            EdgeN = Edges[(4 - quarterTurns) % 4],
            EdgeE = Edges[(5 - quarterTurns) % 4],
            EdgeS = Edges[(6 - quarterTurns) % 4],
            EdgeW = Edges[(7 - quarterTurns) % 4],
            Rotation = (Rotation.GetValueOrDefault() + quarterTurns) % 4
        };
    }

    public string Solve(InputHelper inputHelper)
    {
        var grids = inputHelper.EachSection(section => section).ToDictionary(s => int.Parse(s.First()[5..9]), s => new Grid.Helper(s.Skip(1)));

        var tiles = grids.SelectMany(_ => new[] { true, false }, (grid, flip) => new Tile(grid.Key, flip, ReadEdges(grid.Value)[flip ? 0..4 : 4..8]))
            .ToDictionary(t => t, t => t.Edges);
        var edges = tiles.SelectMany(t => t.Value.Zip(Enumerable.Range(0, 4)), (t, edge) => (tile: t.Key, edge))
            .GroupBy(p => p.edge.First, p => p.tile.Rotate(4 - p.edge.Second))
            .ToDictionary(g => new string(g.Key.Reverse().ToArray()), g => g.ToArray());

        var corners = tiles.Where(t => t.Value.Count(edge => edges[edge].Length == 1) == 2).Select(p => p.Key);
        if (isPart1) return corners.Where(c => !c.Flipped).Aggregate(1L, (prod, c) => prod * c.Id).ToString();

        // generate all 4x2 orientations of the puzzle.  Each corner in the top left, flipped or unflipped.
        // This way, we only have to look for one orientation of sea monster.  Half of one, six dozen of the other.
        var jigsaws = corners.Select(c => new List<List<Tile>>() { new() { c.Rotate(c.Corner(edges)) } }).ToArray();

        foreach (var jigsaw in jigsaws)
        {
            while (TryConnect(2, jigsaw[^1][0], edges, out var connectSouth))
            {
                if (jigsaw[^1].Count > 1) jigsaw.Add([connectSouth]);

                while (TryConnect(1, jigsaw[^1][^1], edges, out var connectEast))
                {
                    jigsaw[^1].Add(connectEast);
                }
            }
        }

        var fullGrids = jigsaws.Select(jigsaw => new Grid.Helper(Stitch(jigsaw, grids))).ToArray();
        var seaMonster = new Grid.Helper([
            "                  # ",
            "#    ##    ##    ###",
            " #  #  #  #  #  #   "]);
        var monsterSegments = 0;
        foreach (var fullGrid in fullGrids)
        {
            for (var y = 0; y <= fullGrid.Max[0] - seaMonster.Max[0]; y++)
            {
                for (var x = 0; x <= fullGrid.Max[1] - seaMonster.Max[1]; x++)
                {
                    // Assume sea monsters don't overlap one another
                    monsterSegments += seaMonster['#'].All(p => fullGrid[[p[0] + y, p[1] + x]] == '#') ? seaMonster['#'].Count : 0;
                }
            }
        }

        return $"{fullGrids[0]['#'].Count - monsterSegments}";
    }

    private static IEnumerable<string> Stitch(List<List<Tile>> jigsaw, Dictionary<int, Grid.Helper> grids)
    {
        foreach (var row in jigsaw.AsEnumerable().Reverse())
        {
            foreach (var y in grids[row[0].Id].Ranges[0].Skip(1).SkipLast(1))
            {
                var sb = new System.Text.StringBuilder();

                foreach (var piece in row)
                {
                    var grid = grids[piece.Id];
                    foreach (var x in grid.Ranges[1].Skip(1).SkipLast(1))
                    {
                        sb.Append(grids[piece.Id][(piece.Flipped, piece.Rotation) switch
                        {
                            (false, 2) => [y, x],
                            (true, 2)  => [y, grid.Max[1] - x],
                            (false, 0) => [grid.Max[0] - y, grid.Max[1] - x],
                            (true, 0)  => [grid.Max[0] - y, x],
                            (false, 1) => [grid.Max[1] - x, y],
                            (true, 1)  => [grid.Max[1] - x, grid.Max[0] - y],
                            (false, 3) => [x, grid.Max[0] - y],
                            (true, 3)  => [x, y],
                            _ => throw new NotImplementedException("unknown orientation")
                        }]);
                    }
                }
                yield return sb.ToString();
            }
        }
    }

    private static bool TryConnect(int dir, Tile tile, Dictionary<string, Tile[]> edges, out Tile connector)
    {
        var edge = tile.Edges[dir];
        connector = edges[edge].SingleOrDefault(x => x.Id != tile.Id).Rotate((dir + 2) % 4);
        return connector.Id > 0;
    }

    private static string[] ReadEdges(Grid.Helper grid) => [
        new string(grid.Ranges[1].Aggregate(Enumerable.Empty<char>(), (s, x) => s.Append(grid[[grid.Min[0], x]])).ToArray()),
        new string(grid.Ranges[0].Aggregate(Enumerable.Empty<char>(), (s, y) => s.Append(grid[[y, grid.Max[1]]])).ToArray()),
        new string(grid.Ranges[1].Reverse().Aggregate(Enumerable.Empty<char>(), (s, x) => s.Append(grid[[grid.Max[0], x]])).ToArray()),
        new string(grid.Ranges[0].Reverse().Aggregate(Enumerable.Empty<char>(), (s, y) => s.Append(grid[[y, grid.Min[1]]])).ToArray()),
        new string(grid.Ranges[1].Reverse().Aggregate(Enumerable.Empty<char>(), (s, x) => s.Append(grid[[grid.Min[0], x]])).ToArray()),
        new string(grid.Ranges[0].Aggregate(Enumerable.Empty<char>(), (s, y) => s.Append(grid[[y, grid.Min[1]]])).ToArray()),
        new string(grid.Ranges[1].Aggregate(Enumerable.Empty<char>(), (s, x) => s.Append(grid[[grid.Max[0], x]])).ToArray()),
        new string(grid.Ranges[0].Reverse().Aggregate(Enumerable.Empty<char>(), (s, y) => s.Append(grid[[y, grid.Max[1]]])).ToArray()),
    ];
}