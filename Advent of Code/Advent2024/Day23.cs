namespace Advent_of_Code.Advent2024;

public class Day23(bool isPart1) : IAdventPuzzle
{
    private sealed class CliqueEqual : EqualityComparer<string[]>
    {
        public override bool Equals(string[]? x, string[]? y) =>
            (x, y) switch
            {
                (null, null) => true,
                (_, null) => false,
                (null, _) => false,
                (_, _) => x.SequenceEqual(y) // assume cliques are always ordered
            };
        public override int GetHashCode([System.Diagnostics.CodeAnalysis.DisallowNull] string[] obj) =>
            (obj as System.Collections.IStructuralEquatable).GetHashCode(EqualityComparer<string>.Default);
    }

    private Dictionary<string, HashSet<string>> adj = [];
    private readonly CliqueEqual comparer = new();
    public string Solve(InputHelper inputHelper)
    {
        var pairs = inputHelper.EachLine(line => line.Split('-'))
            .SelectMany(pair => new[] { (a: pair[0], b: pair[1]), (a: pair[1], b: pair[0]) });
        adj = pairs.GroupBy(p => p.a, p => p.b).ToDictionary(g => g.Key, g => g.ToHashSet());
        var cliques = adj.Keys.SelectMany(k => FindCliques(k, 3)).ToHashSet(comparer);

        if (isPart1)
            return cliques.Count(clique => clique.Any(s => s.StartsWith('t'))).ToString();

        while (cliques.Count > 1)
            cliques = cliques.SelectMany(GrowClique).ToHashSet(comparer);

        return string.Join(',', cliques.Single());
    }

    private HashSet<string[]> FindCliques(string seed, int size) =>
        (size switch {
            1 => [[seed]],
            _ => FindCliques(seed, size - 1).SelectMany(GrowClique)
        }).ToHashSet(comparer);

    private IEnumerable<string[]> GrowClique(string[] clique) =>
        clique.Select(node => adj[node].AsEnumerable())
            .Aggregate((common, neighbors) => common.Intersect(neighbors))
            .Select(n => clique.Append(n).Order().ToArray());
}