namespace Advent_of_Code.Advent2024;

public class Day20(InputHelper inputHelper) : IAdventPuzzle
{
    public long Solve(bool isPart1)
    {
        var racetrack = new Cartesian<int>.GridHelper(inputHelper.EachLine(x => x));
        var (start, goal) = (racetrack['S'].Single(), racetrack['E'].Single());

        var search = new PriorityQueue<int[], int>([(start, 0)]);
        var reachable = new Dictionary<int[], int>(racetrack);
        while (search.TryDequeue(out var position, out var steps))
        {
            if (racetrack['#'].Contains(position)) continue;
            if (!reachable.TryAdd(position, steps)) continue;
            if (position.SequenceEqual(goal)) break;
            search.EnqueueRange(racetrack.Orthogonal(position).Select(p => (p, steps + 1)));
        }

        var bestPath = new Dictionary<int[], int>(racetrack);
        var backtrack = new Queue<int[]>([goal]);
        while (backtrack.TryDequeue(out var position))
        {
            bestPath[position] = reachable[position];
            if (!position.SequenceEqual(start))
                backtrack.Enqueue(racetrack.Orthogonal(position).Single(p => reachable.TryGetValue(p, out var reach) && reach < reachable[position]));
        }

        if (isPart1)
        {
            var shortcuts = bestPath.SelectMany(a => racetrack.Orthogonal(a.Key, 2).Where(b => bestPath.TryGetValue(b, out var shortcut) && shortcut < a.Value - 2),
                (p, q) => p.Value - bestPath[q] - 2);
            return shortcuts.Count(x => x >= 100);
        }
        return bestPath.SelectMany(a => bestPath.Where(b => TaxiCab(a.Key, b.Key) <= 20 && b.Value <= a.Value - TaxiCab(a.Key, b.Key) - 100)).Count();
    }

    private static int TaxiCab(int[] a, int[] b) => Math.Abs(a[0] - b[0]) + Math.Abs(a[1] - b[1]);
}
