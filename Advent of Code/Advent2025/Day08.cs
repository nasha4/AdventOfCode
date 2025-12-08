namespace Advent_of_Code.Advent2025;

public class Day08(bool isPart1) : IAdventPuzzle
{
    public string Solve(InputHelper inputHelper)
    {
        var boxes = inputHelper.EachLine(line => JunctionBox.Create([.. line.Split(',').Select(long.Parse)])).ToList();
        var pairs = new PriorityQueue<(JunctionBox a, JunctionBox b), long>(
            boxes.SelectMany(box0 => boxes.Where(box1 => box0.CompareTo(box1) < 0), (a, b) => (a, b)).Select(pair => (pair, pair.a - pair.b)));

        var circuits = boxes.ToDictionary(box => box, box => new HashSet<JunctionBox>() { box });
        var connections = boxes.Count == 20 ? 10 : 1000; // hack to detect sample input
        for (var n = 0; (!isPart1 || n < connections) && pairs.TryDequeue(out var pair, out var distance); n++)
        {
            circuits[pair.a].UnionWith(circuits[pair.b]);
            foreach (var box in circuits[pair.b]) circuits[box] = circuits[pair.a];

            if (circuits[pair.a].Count == boxes.Count) // Part 2
                return (pair.a.X * pair.b.X).ToString();
        }
        return circuits.Values.Distinct().OrderByDescending(c => c.Count).Select(c => c.Count).Take(3).Aggregate((a, b) => a * b).ToString();
    }
    private readonly record struct JunctionBox(long X, long Y, long Z) : IComparable<JunctionBox>
    {
        public static JunctionBox Create(long[] xyz) => new(xyz[0], xyz[1], xyz[2]);
        public int CompareTo(JunctionBox other) => (X - other.X, Y - other.Y, Z - other.Z) switch
        {
            ( < 0, _, _) or (0, < 0, _) or (0, 0, < 0) => -1,
            ( > 0, _, _) or (0, > 0, _) or (0, 0, > 0) => 1,
            (0, 0, 0) => 0
        };
        public static long operator -(JunctionBox a, JunctionBox b) => (a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y) + (a.Z - b.Z) * (a.Z - b.Z);
    }
}