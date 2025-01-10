namespace Advent_of_Code.Advent2020;

public class Day11(bool isPart1) : IAdventPuzzle
{
    public string Solve(InputHelper inputHelper)
    {
        var seats = new GridHelper(inputHelper.EachLine());

        var adj = seats['L'].ToDictionary(p => p, p => isPart1 ? seats.Orthodiagonal(p) : SeenFrom(p, seats), seats);

        var stable = seats['#'];
        for (var (prev, next) = (stable, seats['L']);
            !prev.SetEquals(next);
            (prev, next) = (next, Arrive(next, seats, adj, isPart1 ? 4 : 5)))
            stable = next;

        return stable.Count.ToString();
    }

    private static HashSet<int[]> Arrive(IReadOnlySet<int[]> occupied, GridHelper seats, Dictionary<int[], IEnumerable<int[]>> adj, int neighborThreshold) =>
        adj.Select(kvp => (kvp.Key, neighbors: kvp.Value.Count(occupied.Contains)))
            .Where(p => p.neighbors == 0 || occupied.Contains(p.Key) && p.neighbors < neighborThreshold)
            .Select(p => p.Key).ToHashSet(seats);

    private static IEnumerable<int[]> SeenFrom(int[] p, GridHelper seats) =>
        seats.Space.OrthodiagUnit.Select(o => {
            var q = p;
            for (q = Cartesian<int>.VectorSum(q, o); seats[q] == '.'; q = Cartesian<int>.VectorSum(q, o)) ;
            return q;
        }).Where(x => seats[x] == 'L');
}
