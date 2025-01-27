namespace Advent_of_Code.Advent2022;

public class Day15(bool isPart1) : IAdventPuzzle
{
    private readonly record struct Interval(int Min, int Max)
    {
        public bool Contains(int val) => val >= Min && val <= Max;
        public int Size => Max - Min + 1;
        public bool Abuts(Interval other) => Max + 1 == other.Min || other.Max + 1 == Min;
        public bool Overlaps(Interval other) =>
            Contains(other.Min) || Contains(other.Max) || other.Contains(Min);

        public static IEnumerable<Interval> MergeInto(Interval merger, IEnumerable<Interval> others)
        {
            var (min, max) = (merger.Min, merger.Max);
            var groups = others.ToLookup(o => merger.Overlaps(o) || merger.Abuts(o));
            foreach (var o in groups[true])
            {
                min = int.Min(min, o.Min);
                max = int.Max(max, o.Max);
            }
            return groups[false].Append(new(min, max));
        }
    }

    public string Solve(InputHelper inputHelper)
    {
        var intervals = Enumerable.Empty<Interval>();
        var sensors = new List<(int x, int y, int r)>();
        var beacons = new HashSet<(int x, int y)>();
        var part1Row = 2_000_000;
        foreach (var line in inputHelper.EachLine(line => line.Split('=', ',', ':').Chunk(2).Select(p => int.Parse(p[1])).ToArray()))
        {
            var (sensor, beacon) = ((x: line[0], y: line[1]), (x: line[2], y: line[3]));
            var radius = Math.Abs(sensor.x - beacon.x) + Math.Abs(sensor.y - beacon.y);
            sensors.Add((sensor.x, sensor.y, radius));
            beacons.Add(beacon);

            var halfIntervalWidth = radius - Math.Abs(sensor.y - part1Row);
            if (halfIntervalWidth >= 0)
            {
                var interval = new Interval(sensor.x - halfIntervalWidth, sensor.x + halfIntervalWidth);
                intervals = Interval.MergeInto(interval, intervals);
            }
        }
        if (isPart1)
            return $"{intervals.Sum(i => i.Size) - beacons.Count(b => b.y == part1Row)}";

        var interestingYValues = sensors
            .SelectMany(_ => sensors, (a, b) => (a, b))
            .Where(p => p.a != p.b)
            .Aggregate(Enumerable.Empty<int>(), (acc, sensor) => acc.Union(
                // Solve for all intersections of the extended border lines of the sensor pair.
                // Assume the missing beacon must be on a row with one of these intersections.
                //   2*y = A.y + B.y ± A.r ± B.r ± (A.x-B.x)   <== 8 solutions for y
                // only 2 or 0 solutions are actual intersections
                new[] { -1, 1 }.SelectMany(_ => new[] { -1, 1 }.SelectMany(_ => new[] { -1, 1 }, (b, a) => (b, a)), (c, t) => (t.a, t.b, c))
                    .Select(f => (f.a * sensor.a.r + f.b * sensor.b.r + f.c * sensor.a.x - f.c * sensor.b.x + sensor.a.y + sensor.b.y) / 2)
                    .Where(y => y >= 0 && y <= 4_000_000
                        && y >= sensor.a.y - sensor.a.r && y <= sensor.a.y + sensor.a.r
                        && y >= sensor.b.y - sensor.b.r && y <= sensor.b.y + sensor.b.r)));

        // Instead of returning early (smart, faster), I want to search the whole puzzle
        // space in order to get a better sense of my optimizations. That is, make sure we
        // really are faster, and don't just happen to get lucky and hit the right row first.
        var solutions = new List<(int x, int y)>();
        foreach (var row in interestingYValues)
        {
            intervals = [ // our missing beacon might be on the edge! it's not though.
                new(-1, -1),
                new(4_000_001, 4_000_001)
            ];
            foreach (var (sensorX, sensorY, radius) in sensors)
            {
                var halfIntervalWidth = radius - Math.Abs(sensorY - row);
                if (halfIntervalWidth >= 0)
                {
                    var interval = new Interval(sensorX - halfIntervalWidth, sensorX + halfIntervalWidth);
                    intervals = Interval.MergeInto(interval, intervals);
                }
            }
            if (intervals.Count() > 1)
            {   // Most rows will be completely covered, i.e. have one big interval.
                // If we find a row that has two intervals, there must be space in between
                // them where a beacon can hide. The puzzle assures us there is only one
                // such space, so we have found our beacon.
                int x = intervals.Min(x => x.Max) + 1;
                solutions.Add((x, row));
                // we could just return early here if we weren't benchmarking optimizations
            }
        }
        var solution = solutions.Single();
        return $"{4_000_000L * solution.x + solution.y}";
    }
}