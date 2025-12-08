namespace Advent_of_Code.Advent2025;

public class Day08(bool isPart1) : IAdventPuzzle
{
    public string Solve(InputHelper inputHelper)
    {
        var boxes = inputHelper.EachLine(line => new JunctionBox([.. line.Split(',').Select(long.Parse)])).ToList();
        var pairs = new PriorityQueue<(JunctionBox a, JunctionBox b), long>(
            Enumerable.Range(0, boxes.Count).SelectMany(i => boxes[(i + 1)..], (i, b) => (a: boxes[i], b)).Select(pair => (pair, pair.a.Distance(pair.b))));

        var circuits = boxes.ToDictionary(box => box, box => new HashSet<JunctionBox>([box]));
        var connections = boxes.Count == 20 ? 10 : 1000; // detect sample input and only do ten iterations
        for (var n = 0; (!isPart1 || n < connections) && pairs.TryDequeue(out var pair, out _); n++)
        {
            circuits[pair.a].UnionWith(circuits[pair.b]);
            if (circuits[pair.a].Count == boxes.Count) // Part 2
                return (pair.a.X * pair.b.X).ToString();
            foreach (var box in circuits[pair.b]) circuits[box] = circuits[pair.a];
        }
        return circuits.Values.OrderByDescending(c => c.Count).Distinct().Take(3).Aggregate(1, (acc, term) => acc * term.Count).ToString();
    }

    private readonly record struct JunctionBox(long X, long Y, long Z)
    {
        public JunctionBox(long[] xyz) : this(xyz[0], xyz[1], xyz[2]) { }
        public long Distance(JunctionBox b) => (X - b.X) * (X - b.X) + (Y - b.Y) * (Y - b.Y) + (Z - b.Z) * (Z - b.Z);
    }
}