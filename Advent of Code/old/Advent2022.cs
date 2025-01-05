using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace Advent_of_Code.Advent2022;

public static class Advent2022
{
    public static int Day1(int part)
    {
        var elves = new List<int>();
        for (string? line = string.Empty; line is not null; line = Console.ReadLine())
        {
            if (string.IsNullOrWhiteSpace(line))
                elves.Add(0);
            else
                elves[^1] += int.Parse(line);
        }

        return part switch
        {
            1 => elves.Max(),
            2 => elves.Order().TakeLast(3).Sum(),
            _ => -1,
        };
    }
    public static int Day2(int part)
    {
        var score = 0;
        for (var line = Console.ReadLine(); line is not null; line = Console.ReadLine())
        {
            score += (part, line) switch
            {
                (1, "A X") => 3 + 1,
                (1, "B X") => 0 + 1,
                (1, "C X") => 6 + 1,
                (1, "A Y") => 6 + 2,
                (1, "B Y") => 3 + 2,
                (1, "C Y") => 0 + 2,
                (1, "A Z") => 0 + 3,
                (1, "B Z") => 6 + 3,
                (1, "C Z") => 3 + 3,
                (2, "A X") => 0 + 3,
                (2, "B X") => 0 + 1,
                (2, "C X") => 0 + 2,
                (2, "A Y") => 3 + 1,
                (2, "B Y") => 3 + 2,
                (2, "C Y") => 3 + 3,
                (2, "A Z") => 6 + 2,
                (2, "B Z") => 6 + 3,
                (2, "C Z") => 6 + 1,
                _ => -99999
            };
        }
        return score;
    }
    public static int Day3(int part)
    {
        var score = 0;
        var priority = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ"
            .Select((c, i) => new { c, i }).ToDictionary(x => x.c, x => x.i + 1);
        for (var line = Console.ReadLine(); line is not null; line = Console.ReadLine())
        {
            if (part == 1)
            {
                var firstHalf = line.Take(line.Length / 2);
                var lastHalf = line.Skip(line.Length / 2);
                var inBoth = firstHalf.Intersect(lastHalf).SingleOrDefault();
                score += priority[inBoth];
            }
            else if (part == 2)
            {
                var (line2, line3) = (Console.ReadLine(), Console.ReadLine());
                var inAll3 = line.Intersect(line2).Intersect(line3).SingleOrDefault();
                score += priority[inAll3];
            }
        }
        return score;
    }
    public static int Day4(int part)
    {
        var count = 0;
        var rx = new Regex(@"(\d+)-(\d+),(\d+)-(\d+)");
        for (var line = Console.ReadLine(); line is not null; line = Console.ReadLine())
        {
            var match = rx.Match(line);
            var (first1, last1) = (int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value));
            var (first2, last2) = (int.Parse(match.Groups[3].Value), int.Parse(match.Groups[4].Value));
            var elf1 = Enumerable.Range(first1, last1 - first1 + 1);
            var elf2 = Enumerable.Range(first2, last2 - first2 + 1);
            var both = elf1.Intersect(elf2);
            if (part == 1)
            {
                if (both.Count() == elf1.Count() || both.Count() == elf2.Count())
                    count++;
            }
            else if (part == 2)
            {
                if (both.Any())
                    count++;
            }
        }
        return count;
    }
    public static string Day5(int part)
    {
        var line = Console.ReadLine();
        var stacks = new List<char>[(line.Length + 1) / 4]
            .Select(x => new List<char>()).ToArray();
        while (!string.IsNullOrWhiteSpace(line))
        { // get the initial crate positions
            foreach (Match match in Regex.Matches(line, @"[A-Z]"))
                stacks[(match.Index - 1) / 4].Insert(0, match.Value[0]);
            line = Console.ReadLine();
        }

        var instruction = new Regex(@"move (\d+) from (\d+) to (\d+)");
        for (line = Console.ReadLine(); line is not null; line = Console.ReadLine())
        {
            var match = instruction.Match(line);
            var nMoves = int.Parse(match.Groups[1].Value);
            var from = int.Parse(match.Groups[2].Value) - 1;
            var to = int.Parse(match.Groups[3].Value) - 1;

            if (part == 1)
                stacks[to].AddRange(stacks[from].TakeLast(nMoves).Reverse());
            else if (part == 2)
                stacks[to].AddRange(stacks[from].TakeLast(nMoves));
            stacks[from].RemoveRange(stacks[from].Count - nMoves, nMoves);
        }
        return new string(stacks.Select(x => x.Last()).ToArray());
    }
    public static int Day6(int part)
    {
        var datastream = Console.ReadLine() ?? string.Empty;
        var markerLength = part == 1 ? 4 : 14;
        for (var startIndex = 0; startIndex < datastream.Length; startIndex++)
        {
            var candidate = datastream.Skip(startIndex).Take(markerLength);
            if (candidate.Distinct().Count() == markerLength)
                return startIndex + markerLength;
        }
        return -1;
    }
    internal class Dir
    {
        public string Name { get; }
        public Dir? Parent { get; }
        public Dictionary<string, Dir> Children { get; } = new();
        public Dictionary<string, long> Files { get; } = new();
        public Dir(string name, Dir? parent) => (Name, Parent) = (name, parent);
        public void Add(string dirName) => Children.Add(dirName, new Dir(dirName, this));
        public void Add(string filename, long size) => Files.Add(filename, size);
        public long Size => Files.Sum(kv => kv.Value) + Children.Sum(kv => kv.Value.Size);
        public IEnumerable<Dir> Descendants => Children.Values.Concat(Children.Values.SelectMany(dir => dir.Descendants));
    }
    public static long Day7(int part)
    {
        _ = Console.ReadLine(); // always "$ cd /"
        var root = new Dir("/", null);
        var cwd = root;
        for (var line = Console.ReadLine(); line is not null; line = Console.ReadLine())
        {
            cwd ??= root; // in case we tried to .. above root
            var lineParts = line.Split(' ');
            if (line == "$ ls")
                continue; // ignore, assume any noncommand is a new entry of cwd
            else if (line == "$ cd ..")
                cwd = cwd.Parent;
            else if (lineParts[0] == "$" && lineParts[1] == "cd")
                cwd = cwd.Children[lineParts[2]];
            else if (lineParts[0] == "dir")  // dir dirName
                cwd.Add(lineParts[1]);
            else // filesize filename
                cwd.Add(lineParts[1], long.Parse(lineParts[0]));
        }
        /* input processing bugs that I later noticed:
         * we don't handle multiple `ls` of the same dir.  fortunately no such inputs are used.
         * we don't handle `cd`ing to a dir that we haven't already seen in an `ls`.  no such inputs.
         * we assume `cd /` doesn't occur except on line 0.  would be trivial to implement. */
        if (part == 1)
            return root.Descendants.Select(dir => dir.Size).Where(size => size <= 100000).Sum();

        var availableSpace = 70000000 - root.Size;
        return root.Descendants.Select(dir => dir.Size).Where(size => availableSpace + size >= 30000000).Min();
    }
    public static int Day8(int part)
    {
        static void CheckVisibility(int[,] trees, HashSet<(int, int)> isVisible, char orientation)
        { // some over-cleverness to avoid listing out the same nested loops 4 times
            var aRange = Enumerable.Range(0, trees.GetLength(0));
            var bRange = Enumerable.Range(0, trees.GetLength(1));
            if ("EW".Contains(orientation)) // vertical, so swap x and y (swap them back again later)
                (aRange, bRange) = (bRange, aRange);
            if ("ES".Contains(orientation)) // negative, so reverse the inner loop
                bRange = bRange.Reverse();

            foreach (var a in aRange)
            {
                var maxHeight = -1;
                foreach (var b in bRange)
                {
                    var (x, y) = "EW".Contains(orientation) ? (a, b) : (b, a);
                    if (trees[x, y] > maxHeight)
                    {
                        maxHeight = trees[x, y];
                        isVisible.Add((x, y));
                    }
                }
            }
        }

        static int ScenicScore(int[,] trees, int x, int y)
        {
            var (xMax, yMax) = (trees.GetLength(0) - 1, trees.GetLength(1) - 1);
            int n, s, e, w; // look in each direction until we find a value >= our start tree (or find the edge)
            for (e = x + 1; e < xMax && trees[e, y] < trees[x, y]; e++) ;
            for (w = x - 1; w > 0 && trees[w, y] < trees[x, y]; w--) ;
            for (s = y + 1; s < yMax && trees[x, s] < trees[x, y]; s++) ;
            for (n = y - 1; n > 0 && trees[x, n] < trees[x, y]; n--) ;
            return (y - n) * (y - s) * (x - e) * (x - w);
        }

        var lines = new List<string>();
        for (var line = Console.ReadLine(); line is not null; line = Console.ReadLine())
            lines.Add(line);

        int xSize = lines[0].Length, ySize = lines.Count; // I think it's always square but let's support rectangles
        var trees = new int[xSize, ySize];
        var y = 0;
        foreach (var line in lines)
        {
            for (var x = 0; x < xSize; x++)
                trees[x, y] = line[x] - '0';
            y++;
        }

        if (part == 1)
        {
            var isVisible = new HashSet<(int, int)>();
            foreach (var orientation in "NSEW")
                CheckVisibility(trees, isVisible, orientation);

            return isVisible.Count;
        }

        var mostScenic = 0;
        for (y = 1; y < ySize - 1; y++) // edges will always score 0
        {
            for (var x = 1; x < xSize - 1; x++)
            {
                var score = ScenicScore(trees, x, y);
                if (score > mostScenic)
                    mostScenic = score;
            }
        }
        return mostScenic;
    }
    internal class Knot
    {
        private int _X = 0, _Y = 0;
        public (int x, int y) Position => (_X, _Y);
        public static Knot operator +(Knot a, (int dx, int dy) b)
        {
            a._X += b.dx;
            a._Y += b.dy;
            return a;
        }
        public static (int dx, int dy) operator -(Knot a, Knot b) => (a._X - b._X, a._Y - b._Y);
        public static (int dx, int dy) OneStepToward(int dx, int dy) => (Math.Sign(dx), Math.Sign(dy));
    }
    public static int Day9(int part)
    {
        var ropeSize = part == 1 ? 2 : 10;
        var rope = new Knot[ropeSize].Select(_ => new Knot()).ToArray();
        var seenTail = new HashSet<(int, int)>() { (0, 0) };
        for (var line = Console.ReadLine(); line is not null; line = Console.ReadLine())
        {
            var lineParts = line.Split(' ');
            var moveCount = int.Parse(lineParts[1]);
            var direction = lineParts[0] switch
            {
                "D" => (0, -1),
                "U" => (0, 1),
                "L" => (-1, 0),
                "R" => (1, 0),
                _ => (0, 0)
            };
            for (var move = 0; move < moveCount; move++)
            {
                rope[0] += direction;
                for (var i = 1; i < ropeSize; i++)
                {
                    var (dx, dy) = rope[i - 1] - rope[i];
                    if (Math.Abs(dx) < 2 && Math.Abs(dy) < 2)
                        break; // rope is settled, we can skip the rest of the knots
                    rope[i] += Knot.OneStepToward(dx, dy);
                }
                seenTail.Add(rope.Last().Position);
            }
        }
        return seenTail.Count;
    }
    public static int Day10(int part)
    {
        var xRegister = 1;
        var interestingCycles = new List<int>() { 20, 60, 100, 140, 180, 220 };
        var xHistory = new List<int>() { xRegister }; // cycles start at 1 not 0, so add a dummy value here
        for (var line = Console.ReadLine(); line is not null; line = Console.ReadLine())
        {
            var lineParts = line.Split(' ');
            xHistory.Add(xRegister); // one cycle
            if (lineParts[0] == "addx")
            {
                xHistory.Add(xRegister); // two cycles
                xRegister += int.Parse(lineParts[1]); // addx completes after the second cycle
            }
        }

        if (part == 1)
            return interestingCycles.Select(cycle => cycle * xHistory[cycle]).Sum();

        const int spriteWidth = 3, lineWidth = 40;
        static bool SpriteCheck(int x, int col) => col >= x && col < x + spriteWidth;
        var pixels = xHistory.Select((x, i) => SpriteCheck(x, i % lineWidth) ? '#' : ' ');
        foreach (var row in pixels.Skip(1).Chunk(lineWidth)) // skip over that dummy value
            Console.WriteLine(row);
        return 0;
    }
    internal class Monkey
    {
        public Queue<long> Items { get; set; } = new();
        public Func<long, long> Operation { get; set; } = x => x;
        public int DivisibilityTest { get; set; } = 1;
        public int ThrowToWhenTrue { get; set; } = 0;
        public int ThrowToWhenFalse { get; set; } = 0;
        public int Throw(long x) => x % DivisibilityTest == 0 ? ThrowToWhenTrue : ThrowToWhenFalse;
        public void Catch(long item) => Items.Enqueue(item);
        public long Inspected { get; private set; } = 0;
        public long Inspect()
        {
            Inspected++;
            return WorryReducer(Operation(Items.Dequeue()));
        }
        public static Func<long, long> WorryReducer { get; set; } = x => x / 3;
    }
    public static long Day11(int part)
    {
        var monkeys = new List<Monkey>();
        var opPattern = new Regex(@"new = old (.) (\d+|old)");
        for (var line = Console.ReadLine(); line is not null; line = Console.ReadLine())
        {
            if (line.StartsWith("Monkey"))
                monkeys.Add(new Monkey());
            else if (line.StartsWith("  Starting items:"))
                monkeys.Last().Items = new(line.Split(": ")[1].Split(", ").Select(long.Parse));
            else if (line.StartsWith("  Operation:"))
            {
                var groups = opPattern.Match(line).Groups;
                if (groups[1].Value == "*" && groups[2].Value == "old")
                    monkeys.Last().Operation = x => x * x;
                else if (int.TryParse(groups[2].Value, out var n))
                    monkeys.Last().Operation = groups[1].Value == "*" ? (x => x * n) : (x => x + n);
            }
            else if (line.StartsWith("  Test:"))
                monkeys.Last().DivisibilityTest = int.Parse(line.Split("by ")[1]);
            else if (line.StartsWith("    If true:"))
                monkeys.Last().ThrowToWhenTrue = int.Parse(line.Split("monkey ")[1]);
            else if (line.StartsWith("    If false:"))
                monkeys.Last().ThrowToWhenFalse = int.Parse(line.Split("monkey ")[1]);
        }
        var testProduct = monkeys.Aggregate(1, (product, next) => product * next.DivisibilityTest);
        if (part == 2)
            Monkey.WorryReducer = x => x % testProduct; // The secret!!!
        for (int round = 0; round < (part == 1 ? 20 : 10000); round++)
        {
            foreach (var monkey in monkeys)
            {
                while (monkey.Items.Any())
                {
                    var item = monkey.Inspect();
                    monkeys[monkey.Throw(item)].Catch(item);
                }
            }
        }
        var monkeyBusiness = monkeys.Select(m => m.Inspected).Order().TakeLast(2);
        return monkeyBusiness.First() * monkeyBusiness.Last();
    }
    internal class HeightSquare
    {
        public HeightSquare(int x, int y, char height)
        {
            (X, Y, Height) = (x, y, height);
            if (height == 'S') (Start, Height) = (this, 'a');
            if (height == 'E') (Goal, Height) = (this, 'z');
        }
        private readonly int X, Y;
        public char Height { get; }
        public IEnumerable<HeightSquare> Neighbors(HeightSquare[,] grid)
        {
            var neighbors = new List<(int x, int y)>() {
                (X - 1, Y), (X + 1, Y), (X, Y - 1), (X, Y + 1) };
            return neighbors
                .Where(p => p.x >= 0 && p.x < grid.GetLength(0))
                .Where(p => p.y >= 0 && p.y < grid.GetLength(1))
                .Select(p => grid[p.x, p.y])
                .Where(s => s.Height - Height <= 1);
        }
        public static HeightSquare? GetStart => Start;
        private static HeightSquare? Goal;
        private static HeightSquare? Start;
        public static IEnumerable<HeightSquare>? FindPath(
            IEnumerable<HeightSquare> startingSquares,
            HeightSquare[,] grid)
        {
            var toSearch = new Queue<HeightSquare>(startingSquares);
            var visitedFrom = startingSquares.ToDictionary(s => s, s => null as HeightSquare);
            while (toSearch.Any())
            {
                var square = toSearch.Dequeue();
                foreach (var neighbor in square.Neighbors(grid).Where(s => !visitedFrom.ContainsKey(s)))
                {
                    visitedFrom[neighbor] = square;
                    toSearch.Enqueue(neighbor);
                    if (neighbor == Goal)
                    {
                        var path = new List<HeightSquare>();
                        for (var s = neighbor; s is not null; s = visitedFrom[s])
                            path.Add(s);
                        return path;
                    }
                }
            }
            return null!; // no such path!
        }
    }
    public static int Day12(int part)
    {
        var lines = new List<string>();
        for (var line = Console.ReadLine(); line is not null; line = Console.ReadLine())
            lines.Add(line);
        var (xSize, ySize) = (lines[0].Length, lines.Count);

        var grid = new HeightSquare[xSize, ySize];
        for (var y = 0; y < ySize; y++)
            for (var x = 0; x < xSize; x++)
                grid[x, y] = new(x, y, lines[y][x]);

        var startFrom = part == 1
            ? new List<HeightSquare>() { HeightSquare.GetStart! }
            : grid.Cast<HeightSquare>().Where(s => s.Height == 'a');
        var path = HeightSquare.FindPath(startFrom, grid);
        return path!.Count() - 1; // path is null if there is no solution, but let's assume there is
    }
    internal class Packet : IComparable<Packet>
    {
        private readonly int AsInt;
        private readonly List<Packet> AsList;
        private readonly bool IsInt;
        public override string ToString()
        {
            if (IsInt) return AsInt.ToString();
            return $"[{string.Join(',', AsList.Select(x => x.ToString()))}]";
        }
        public int CompareTo(Packet? right)
        {
            if (right == null) return 1;
            if (IsInt && right.IsInt)
                return AsInt - right.AsInt;
            var (lList, rList) = (AsList, right.AsList);
            for (int i = 0; ; i++)
            {
                if (i >= lList.Count && i >= rList.Count) return 0;
                else if (i >= lList.Count) return -1;
                else if (i >= rList.Count) return 1;
                var cmp = lList[i].CompareTo(rList[i]);
                if (cmp != 0) return cmp;
            }
        }
        public Packet(string text)
        {
            ArgumentException.ThrowIfNullOrEmpty(text);
            if (text[0] == '[')
            {
                var list = new List<Packet>();
                var braceCount = 1;
                var prevComma = 1;
                for (int i = 1; i < text.Length; i++)
                {
                    if ((text[i] == ',' || text[i] == ']') && braceCount == 1)
                    {
                        var item = text[prevComma..i];
                        if (!string.IsNullOrEmpty(item))
                            list.Add(new Packet(item));
                        prevComma = i + 1;
                    }
                    if (text[i] == '[') braceCount++;
                    if (text[i] == ']') braceCount--;
                }
                (AsInt, IsInt, AsList) = (0, false, list);
            }
            else
            {
                var n = int.Parse(text);
                (AsInt, IsInt) = (n, true);
                // To avoid repeatedly creating and discarding temp singleton Lists
                // during comparisons, we store each int node with a List of itself.
                AsList = [this];
            }
        }
    }
    public static int Day13(int part)
    {
        var packets = new List<Packet>();
        for (var line = Console.ReadLine(); line is not null; line = Console.ReadLine())
            if (!string.IsNullOrEmpty(line))
                packets.Add(new(line));

        if (part == 1)
            return packets.Chunk(2)
                .Select((pair, i) => new { cmp = pair[0].CompareTo(pair[1]), i })
                .Where(x => x.cmp <= 0)
                .Sum(x => x.i + 1); // 1-based index

        var divider2 = new Packet("[[2]]");
        var divider6 = new Packet("[[6]]");
        packets.Add(divider2);
        packets.Add(divider6);
        packets.Sort(); // we implemented IComparable in part 1!
        return (packets.IndexOf(divider2) + 1) * (packets.IndexOf(divider6) + 1);
    }
    public static int Day14(int part)
    {
        var cave = new HashSet<(int x, int y)>();
        for (var line = Console.ReadLine(); line is not null; line = Console.ReadLine())
        { // just rip out every int from each line of input and pair them together as x,y points
            var points = Regex.Matches(line, @"\d+").Select(x => int.Parse(x.Value)).Chunk(2).ToList();
            for (var i = 1; i < points.Count; i++)
            { // treat each pair of points as corners of a rectangle. one dimension will always just be 1
                var (a, b) = (points[i - 1], points[i]);
                if (a[0] > b[0] || a[1] > b[1])
                    (a, b) = (b, a); // in order to always count up
                for (var x = a[0]; x <= b[0]; x++)
                    for (var y = a[1]; y <= b[1]; y++)
                        cave.Add((x, y));
            }
        }
        var caveDepth = cave.Max(p => p.y);

        for (var sandCount = 0; ; sandCount++)
        {
            var (sandX, sandY) = (500, 0);
            while (sandY < caveDepth || part == 2)
            {
                if (part == 2 && sandY > caveDepth)
                {
                    cave.Add((sandX, sandY));
                    break;
                }
                else if (!cave.Contains((sandX, sandY + 1)))
                    (sandX, sandY) = (sandX + 0, sandY + 1);
                else if (!cave.Contains((sandX - 1, sandY + 1)))
                    (sandX, sandY) = (sandX - 1, sandY + 1);
                else if (!cave.Contains((sandX + 1, sandY + 1)))
                    (sandX, sandY) = (sandX + 1, sandY + 1);
                else
                {
                    cave.Add((sandX, sandY));
                    break;
                }
            }
            if (sandY >= caveDepth && part == 1)
                return sandCount; // don't count the last sand, it fell into the abyss
            if (sandY == 0)
                return sandCount + 1; // do count the last sand, it settled at the source
        }
    }
    internal class Interval
    {
        public int Min { get; private set; }
        public int Max { get; private set; }
        public Interval(int min, int max) => (Min, Max) = min < max ? (min, max) : (max, min);
        public bool Contains(int val) => val >= Min && val <= Max;
        public int Size => Max - Min + 1;
        public bool Abuts(Interval other) => Max + 1 == other.Min || other.Max + 1 == Min;
        public bool Overlaps(Interval other) =>
            Contains(other.Min) || Contains(other.Max) || other.Contains(Min);
        public IEnumerable<Interval> MergeInto(IEnumerable<Interval> others)
        {
            var groups = others.ToLookup(o => Overlaps(o) || Abuts(o));
            foreach (var o in groups[true])
            {
                Min = Min < o.Min ? Min : o.Min;
                Max = Max > o.Max ? Max : o.Max;
            }
            return groups[false].Append(this);
        }
    }
    public static long Day15(int part)
    {
        var test = false;
        var intervals = Enumerable.Empty<Interval>();
        var sensors = new List<(int x, int y, int r)>();
        var beacons = new HashSet<(int x, int y)>();
        var part1Row = test ? 10 : 2_000_000;
        for (var line = Console.ReadLine(); line is not null; line = Console.ReadLine())
        {
            var points = Regex.Matches(line, @"-?\d+").Select(x => int.Parse(x.Value)).ToList();
            var (sensor, beacon) = ((x: points[0], y: points[1]), (x: points[2], y: points[3]));
            var radius = Math.Abs(sensor.x - beacon.x) + Math.Abs(sensor.y - beacon.y);
            sensors.Add((sensor.x, sensor.y, radius));
            beacons.Add(beacon);

            var halfIntervalWidth = radius - Math.Abs(sensor.y - part1Row);
            if (halfIntervalWidth >= 0)
            {
                var interval = new Interval(sensor.x - halfIntervalWidth, sensor.x + halfIntervalWidth);
                intervals = interval.MergeInto(intervals);
            }
        }
        if (part == 1)
        {
            var intervalSize = intervals.Sum(i => i.Size);
            intervalSize -= beacons.Count(b => b.y == part1Row);
            return intervalSize;
        }

        var stopWatch = Stopwatch.StartNew();

        //var interestingYValues = Enumerable.Range(0, test ? 21 : 4_000_001); // this works but takes ~4100ms
        var interestingYValues = new HashSet<int>();
        foreach (var (sensorA, sensorB) in sensors
            .SelectMany(_ => sensors, (a, b) => (a, b)) // every pair of sensors
            .Where(p => p.a != p.b)) // but don't pair a sensor with itself
        {   // Solve for all intersections of the extended border lines of the sensor pair.
            // Assume the missing beacon must be on a row with one of these intersections.
            //   2*y = A.y + B.y ± A.r ± B.r ± (A.x-B.x)   <== 8 solutions for y
            // only 2 or 0 solutions are actual intersections
            var intersections = new List<int>()
            {
                +sensorA.r + sensorB.r + sensorA.x - sensorB.x,
                +sensorA.r + sensorB.r - sensorA.x + sensorB.x,
                +sensorA.r - sensorB.r + sensorA.x - sensorB.x,
                +sensorA.r - sensorB.r - sensorA.x + sensorB.x,
                -sensorA.r + sensorB.r + sensorA.x - sensorB.x,
                -sensorA.r + sensorB.r - sensorA.x + sensorB.x,
                -sensorA.r - sensorB.r + sensorA.x - sensorB.x,
                -sensorA.r - sensorB.r - sensorA.x + sensorB.x
            }
                .Select(y => (y + sensorA.y + sensorB.y) / 2)
                .Where(y => y >= 0 && y <= (test ? 20 : 4_000_000)
                    && y >= sensorA.y - sensorA.r && y <= sensorA.y + sensorA.r
                    && y >= sensorB.y - sensorB.r && y <= sensorB.y + sensorB.r);
            foreach (var y in intersections)
                interestingYValues.Add(y);
        }

        var borders = new List<Interval>() { // our missing beacon might be on the edge! it's not though.
            new(-1, -1),
            test ? new(21, 21) : new(4_000_001, 4_000_001)
        };
        // Instead of returning early (smart, faster), I want to search the whole puzzle
        // space in order to get a better sense of my optimizations. That is, make sure we
        // really are faster, and don't just happen to get lucky and hit the right row first.
        var solutions = new List<(int x, int y)>();
        foreach (var row in interestingYValues)
        {
            intervals = borders;
            foreach (var (sensorX, sensorY, radius) in sensors)
            {
                var halfIntervalWidth = radius - Math.Abs(sensorY - row);
                if (halfIntervalWidth >= 0)
                {
                    var interval = new Interval(sensorX - halfIntervalWidth, sensorX + halfIntervalWidth);
                    intervals = interval.MergeInto(intervals);
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
        var solution = solutions.SingleOrDefault();
        Console.Write($"{solution.x},{solution.y}: examined {interestingYValues.Count} rows, ");
        Console.WriteLine($"{stopWatch.ElapsedMilliseconds}ms elapsed");
        return 4_000_000L * solution.x + solution.y;
    }
    public static int Day16(int part)
    {
        var valveRates = new Dictionary<string, int>();
        var valveTunnels = new Dictionary<string, ISet<string>>();
        var pattern = new Regex(@"^Valve (..) .* rate=(\d+);.* valves? (.+)$");
        for (var line = Console.ReadLine(); line is not null; line = Console.ReadLine())
        {
            var match = pattern.Match(line);
            var (valve, rate) = (match.Groups[1].Value, int.Parse(match.Groups[2].Value));
            var tunnels = match.Groups[3].Value.Split(", ").ToHashSet();
            valveRates[valve] = rate;
            valveTunnels[valve] = tunnels;
        }

        // all the valves start at 0. When we open one, it scores
        // timeRemaining * valveRate points. We want to maximize points.
        var zeroScore = valveRates
            .Where(kv => kv.Value > 0)
            .ToDictionary(kv => kv.Key, _ => 0);
        // We only ever want to move to valves with positive rate, so compute
        // the least distance from every valve>0 (plus AA), to every different
        // valve>0.  We make a Dictionary like { (from: "BB", to: "GG"), 5 }
        var valveDistance = valveRates
            .Where(kv => kv.Value > 0 || kv.Key == "AA")
            .SelectMany(x =>
                valveRates.Where(y => x.Key != y.Key && y.Value > 0),
                (a, b) => (from: a.Key, to: b.Key))
            .ToDictionary(pair => pair, pair => ShortestPath(pair.from, pair.to, valveTunnels));
        var cache = new Dictionary<(string, int, string), (int, IEnumerable<string>)>();

        var stopWatch = Stopwatch.StartNew();
        if (part == 1)
        {
            var scores = SlowBestScore("AA", 30, zeroScore, Enumerable.Empty<string>().ToHashSet());
            Console.WriteLine(stopWatch.Elapsed.ToString());
            foreach (var (room, score) in scores.OrderBy(kv => kv.Key))
                Console.WriteLine($"{room}: {score} ({valveRates[room]}*{score / valveRates[room]})");
            return scores.Values.Sum();
        }
        else
        {
            var valvePowerSet = valveRates.Where(kv => kv.Value > 0).Select(kv => kv.Key)
                .Aggregate(Enumerable.Empty<IEnumerable<string>>().Append([]),
                    (acc, term) => acc.Concat(acc.Select(inner => inner.Append(term)))
                );
            var subsetScores = valvePowerSet
                .Where(x => x.Count() == 7 || x.Count() == 8)
                .ToDictionary(
                    x => string.Join(" ", x.Order()),
                    x => BestScore2("AA", 26, x.ToHashSet())
                );
            var complementPairedSubsetScores = subsetScores
                .Select(kv =>
                {
                    var complement = string.Join(" ", zeroScore
                        .Keys.Where(x => !kv.Key.Contains(x)).Order());
                    return (me: kv.Value, elephant: subsetScores[complement]);
                });
            var scores = complementPairedSubsetScores
                .MaxBy(pair => pair.me.score + pair.elephant.score);
            Console.WriteLine(stopWatch.Elapsed.ToString());
            Console.WriteLine($"{scores.me.score}: {string.Join(' ', scores.me.path)}");
            Console.WriteLine($"{scores.elephant.score}: {string.Join(' ', scores.elephant.path)}");
            return scores.me.score + scores.elephant.score;
        }
        /*
            00:01:40.5643280 elapsed
            EU: 0 (10*0) 80 (10*8)
            FF: 24 (12*2) 0 (12*0)
            KF: 35 (7*5) 0 (7*0)
            MH: 330 (15*22) 0 (15*0)
            NQ: 0 (25*0) 375 (25*15)
            NT: 104 (13*8) 0 (13*0)
            NW: 0 (3*0) 0 (3*0)
            QL: 0 (9*0) 45 (9*5)
            QM: 0 (8*0) 176 (8*22)
            QW: 437 (23*19) 0 (23*0)
            SX: 0 (21*0) 231 (21*11)
            TZ: 0 (17*0) 34 (17*2)
            VC: 256 (16*16) 0 (16*0)
            XY: 0 (18*0) 324 (18*18)
            ZU: 228 (19*12) 0 (19*0)
            2679
        */

        static int ShortestPath(string from, string goal, Dictionary<string, ISet<string>> tunnels)
        {   // Breadth-first search (I already wrote this for Day12)
            var toSearch = new Queue<string>();
            toSearch.Enqueue(from);
            var visitedFrom = new Dictionary<string, string?>() { { from, null } };
            while (toSearch.Count != 0)
            {
                var room = toSearch.Dequeue();
                foreach (var neighbor in tunnels[room].Where(s => !visitedFrom.ContainsKey(s)))
                {
                    visitedFrom[neighbor] = room;
                    toSearch.Enqueue(neighbor);
                    if (neighbor == goal)
                    {
                        var path = new List<string>();
                        for (var s = neighbor; s is not null; s = visitedFrom[s])
                            path.Add(s);
                        return path.Count - 1;
                    }
                }
            }
            return 99999;
        }
        (int score, IEnumerable<string> path) BestScore2(string room, int timeLeft, ISet<string> ignoreValves)
        {
            var valveCode = string.Join(string.Empty, ignoreValves.Order());
            if (cache.TryGetValue((room, timeLeft, valveCode), out var memo))
                return memo;

            var newIgnore = new HashSet<string>(ignoreValves);
            var score = 0;
            if (valveRates[room] > 0 && timeLeft > 0)
            {
                newIgnore.Add(room);
                score = timeLeft * valveRates[room];
            }
            var paths = valveDistance
                .Where(x => x.Key.from == room && !newIgnore.Contains(x.Key.to) && x.Value < timeLeft - 1);
            if (paths.Any())
            {
                var best = paths
                    .Select(path => BestScore2(path.Key.to, timeLeft - 1 - path.Value, newIgnore))
                    .MaxBy(x => x.score);
                best.score += score;
                best.path = best.path.Append(room);
                return cache[(room, timeLeft, valveCode)] = best;
            }
            else
            {
                var path = new List<string>() { room };
                return cache[(room, timeLeft, valveCode)] = (score, path);
            }
        }
        Dictionary<string, int> SlowBestScore(string room, int timeLeft, Dictionary<string, int> score, ISet<string> ignoreValves)
        {   // Depth-first search (recursive)
            var newScore = new Dictionary<string, int>(score);
            if (valveRates[room] > 0 && timeLeft > 0)
                newScore[room] = timeLeft * valveRates[room];
            var paths = valveDistance
                .Where(x => x.Key.from == room && !ignoreValves.Contains(x.Key.to)
                    && newScore[x.Key.to] == 0 && x.Value < timeLeft - 1);
            if (paths.Any())
                return paths
                    .Select(path => SlowBestScore(path.Key.to, timeLeft - 1 - path.Value, newScore, ignoreValves))
                    .MaxBy(x => x.Values.Sum())!;
            else
                return newScore;
        }
        Dictionary<string, int> SlowBestScoreWithElephant((string room, int timeLeft) me, (string room, int timeLeft) elephant, Dictionary<string, int> valveScore)
        {
            var newScore = new Dictionary<string, int>(valveScore);
            if (me.timeLeft >= elephant.timeLeft)
            {
                if (valveRates[me.room] > 0 && me.timeLeft > 0)
                    newScore[me.room] = me.timeLeft * valveRates[me.room];
                var paths = valveDistance
                    .Where(x => x.Key.from == me.room && newScore[x.Key.to] == 0
                        && x.Value < me.timeLeft - 1 && x.Key.to != elephant.room);
                if (paths.Any())
                    return paths
                        .Select(path => SlowBestScoreWithElephant((path.Key.to, me.timeLeft - 1 - path.Value), elephant, newScore))
                        .MaxBy(x => x.Values.Sum())!;
                else
                    return SlowBestScoreWithElephant((string.Empty, 0), elephant, newScore); // let the elephant complete its final move
            }
            else
            {
                if (valveRates[elephant.room] > 0 && elephant.timeLeft > 0)
                    newScore[elephant.room] = elephant.timeLeft * valveRates[elephant.room];
                var paths = valveDistance
                    .Where(x => x.Key.from == elephant.room && newScore[x.Key.to] == 0
                    && x.Value < elephant.timeLeft - 1 && x.Key.to != me.room);
                if (paths.Any())
                    return paths
                        .Select(path => SlowBestScoreWithElephant(me, (path.Key.to, elephant.timeLeft - 1 - path.Value), newScore))
                        .MaxBy(x => x.Values.Sum())!;
                else
                    return newScore; //.Values.Sum();
            }
        }
    }
    internal class Rock
    {
        private Rock(bool[,] squares) =>
            (_Squares, XSize, YSize) = (squares, squares.GetLength(1), squares.GetLength(0));
        private readonly bool[,] _Squares;
        internal readonly int XSize, YSize;
        private bool this[int x, int y] => _Squares[y, x];
        private static readonly List<Rock> _Rocks = new()
        {
            new(new bool[,] { // —
                { true, true, true, true } }),
            new(new bool[,] { // +
                { false, true, false },
                { true, true, true },
                { false, true, false } }),
            new(new bool[,] { // ⅃
                { false, false, true },
                { false, false, true },
                { true, true, true } }),
            new(new bool[,] { // |
                { true }, { true }, { true }, { true } }),
            new(new bool[,] { // □
                { true, true },
                { true, true } })
        };
        internal static IEnumerable<Rock> Next()
        {
            while (true)
            {
                yield return _Rocks[0];
                yield return _Rocks[1];
                yield return _Rocks[2];
                yield return _Rocks[3];
                yield return _Rocks[4];
            }
        }
        internal bool FitsAt(List<bool[]> board, int x, int y)
        {
            for (var ix = 0; ix < XSize; ix++)
                for (var iy = 0; iy < YSize; iy++)
                    if (x + ix < 0 || x + ix >= 7 || this[ix, iy] && board[y - iy][x + ix])
                        return false;
            return true;
        }
        internal void Settle(List<bool[]> board, int x, int y)
        {
            for (var ix = 0; ix < XSize; ix++)
                for (var iy = 0; iy < YSize; iy++)
                    if (this[ix, iy])
                        board[y - iy][x + ix] = true;
        }
    }
    public static long Day17(int part)
    {
        var line = Console.ReadLine();
        var jets = line.Select(c => c == '<' ? -1 : 1).ToList();

        var board = new List<bool[]>() { new bool[] { true, true, true, true, true, true, true } };
        var (highestRock, pieceCount, jetIndex) = (0, 0, 0);
        var (lastPieceCountOnReset, lastHeightOnReset) = (0, 0);
        foreach (var piece in Rock.Next())
        {
            var (x, y) = (2, highestRock + 3 + piece.YSize);
            // extend the board, if needed
            while (board.Count <= y)
                board.Add(new bool[7]);

            while (true) // rockfall loop
            {
                var jet = jets[jetIndex++ % jets.Count];
                if (piece.FitsAt(board, x + jet, y)) x += jet;
                if (piece.FitsAt(board, x, y - 1)) y--;
                else
                {
                    piece.Settle(board, x, y);
                    highestRock = y > highestRock ? y : highestRock;
                    pieceCount++;
                    break;
                }
            }
            if (part == 2 && jetIndex >= jets.Count)
            {
                jetIndex -= jets.Count;
                Console.Write($"{jetIndex} {pieceCount} +{pieceCount - lastPieceCountOnReset}");
                Console.WriteLine($" @ {highestRock} +{highestRock - lastHeightOnReset}");
                lastPieceCountOnReset = pieceCount;
                lastHeightOnReset = highestRock;
            }

            if (part == 1 && pieceCount >= 2022)
                return highestRock;

            // After 1719 rocks have fallen, we (manually) observe a pattern of
            // every 1725 rocks increasing the height by 2728.  Doesn't work for
            // the test case since the pattern is different for different inputs.
            // TODO: implement automatic pattern detector?  :(
            if (part == 2 && pieceCount == 1719 + (1_000_000_000_000L - 1719) % 1725)
                return highestRock + 2728 * (1_000_000_000_000L - 1719) / 1725;
        }
        return 0;
    }
    public static int Day18(int part)
    {
        var cubes = new HashSet<(int x, int y, int z)>();
        var neighbors = new List<(int x, int y, int z)>()
        {
            ( -1, 0, 0 ), ( 0, -1, 0 ), ( 0, 0, -1 ),
            (  1, 0, 0 ), ( 0,  1, 0 ), ( 0, 0,  1 )
        };
        var (min, max) = (100, 0);
        for (var line = Console.ReadLine(); line is not null; line = Console.ReadLine())
        {
            var xyz = line.Split(',').Select(int.Parse).ToArray();
            cubes.Add((xyz[0], xyz[1], xyz[2]));
            min = min < xyz.Min() - 1 ? min : xyz.Min() - 1;
            max = max > xyz.Max() + 1 ? max : xyz.Max() + 1;
        }
        if (part == 1)
            return cubes.SelectMany(_ => neighbors,
                (cube, dxyz) => cubes.Contains(TupleAdd(cube, dxyz)) ? 0 : 1)
                .Sum();

        var isExternal = new Dictionary<(int x, int y, int z), bool>();
        FloodFill((min, min, min));

        return cubes.SelectMany(_ => neighbors,
            (cube, dxyz) => !cubes.Contains(TupleAdd(cube, dxyz))
                && isExternal.TryGetValue(TupleAdd(cube, dxyz), out bool value) && value ? 1 : 0)
            .Sum();

        static (int x, int y, int z) TupleAdd((int x, int y, int z) a, (int x, int y, int z) b) =>
            (a.x + b.x, a.y + b.y, a.z + b.z);
        void FloodFill((int x, int y, int z) start)
        {   // a little different from BFS, but same idea.
            var toFlood = new Queue<(int, int, int)>();
            toFlood.Enqueue(start);
            isExternal[start] = true;
            while (toFlood.Count != 0)
            {
                var cube = toFlood.Dequeue();
                neighbors.Select(o => TupleAdd(cube, o))
                    .Where(n => !cubes.Contains(n) && !isExternal.ContainsKey(n))
                    .Where(n => n.x >= min && n.y >= min && n.z >= min)
                    .Where(n => n.x <= max && n.y <= max && n.z <= max)
                    .ToList().ForEach(x => { toFlood.Enqueue(x); isExternal[x] = true; });
            }
        }
    }
    internal class GameState : IComparable<GameState>
    {
        private readonly int[] _resources;
        private readonly int[] _robots;
        private readonly GameState? PrevState;
        public int TimeLeft { get; }
        public int Geodes => _resources[3];
        private int MaxGeodes =>
            // If we keep cracking geodes at our current rate, AND somehow make a new geode
            // cracker every turn from now on, what's the most geodes can we possibly hope for?
            _resources[3] + TimeLeft * _robots[3] + (TimeLeft - 1) * TimeLeft / 2;
        public GameState(int[] resources, int[] robots, int timeLeft, GameState? prevState = null) =>
            (_resources, _robots, TimeLeft, PrevState) = (resources, robots, timeLeft, prevState);
        public int CompareTo(GameState? other)
        {
            if (other is null) return 1;
            var criteria = new List<Func<GameState, int>>() {
                // Compare these properties, in this priority:
                x => x.MaxGeodes,
                x => x._robots[3], x => x._resources[3], // geodes
                x => x._robots[2], x => x._resources[2], // obsidian
                x => x._robots[1], x => x._resources[1], // clay
                x => x._robots[0], x => x._resources[0], // ore
            };
            return criteria.Select(f => f(this) - f(other)).FirstOrDefault(x => x != 0, 0);
        }
        public void PrintHistory()
        {
            for (var state = this; state is not null; state = state.PrevState)
                Console.WriteLine($"t:{24 - state.TimeLeft}\t" +
                    $"res: {string.Join(' ', state._resources)}\t" +
                    $"bots: {string.Join(' ', state._robots)}");
        }
        private GameState ProduceAndPayFor(int[] blueprint) => new(
            _resources.Zip(_robots, blueprint).Select(x => x.First + x.Second - x.Third).ToArray(),
            _robots.ToArray(), TimeLeft - 1, this);
        public bool TryToMakeRobot(int robotId, int[] blueprint, out GameState newState)
        {
            newState = default!;
            if (_resources.Zip(blueprint).Any(x => x.First < x.Second))
                return false;
            newState = ProduceAndPayFor(blueprint);
            if (robotId < 4) newState._robots[robotId]++; // 4 is the dummy 0,0,0,0 no-robot move
            return true;
        }
    }
    public static int Day19(int part)
    {
        var allBlueprints = new List<List<int[]>>();
        var pattern = new Regex(@"Blueprint (\d+).*(\d+) ore.*(\d+) ore.*(\d+) ore and (\d+) clay.*(\d+) ore and (\d+) obsidian.");
        for (var line = Console.ReadLine(); line is not null; line = Console.ReadLine())
        {
            var match = pattern.Match(line);
            allBlueprints.Add(
            [
                [int.Parse(match.Groups[2].Value), 0, 0, 0], // ore robot
                [int.Parse(match.Groups[3].Value), 0, 0, 0], // clay robot
                [int.Parse(match.Groups[4].Value), int.Parse(match.Groups[5].Value), 0, 0], // obsidian robot
                [int.Parse(match.Groups[6].Value), 0, int.Parse(match.Groups[7].Value), 0], // geode robot
                [0, 0, 0, 0] // no robot
            ]);
        }

        var stopWatch = Stopwatch.StartNew();
        if (part == 1)
        {
            var initialState = new GameState([0, 0, 0, 0], [1, 0, 0, 0], 24);
            var geodes = allBlueprints.Select(blueprints => MineGeodes(initialState, blueprints));
            var totalQuality = geodes.Select((x, i) => x * (i + 1)).Sum();
            Console.WriteLine($"{stopWatch.ElapsedMilliseconds}ms elapsed");
            return totalQuality;
        }
        else
        {
            var initialState = new GameState([0, 0, 0, 0], [1, 0, 0, 0], 32);
            var geodes = allBlueprints.Take(3).Select(blueprints => MineGeodes(initialState, blueprints));
            var product = geodes.Aggregate((prod, term) => prod * term);
            Console.WriteLine($"{stopWatch.ElapsedMilliseconds}ms elapsed");
            return product;
        }

        static int MineGeodes(GameState initialState, List<int[]> blueprints)
        {
            var nextStep = new SortedSet<GameState>() { initialState };
            for (var thisStep = nextStep; nextStep.First().TimeLeft > 0; thisStep = nextStep)
            {
                nextStep = [];
                foreach (var state in thisStep.TakeLast(100))
                    for (int i = 0; i < blueprints.Count; i++)
                        if (state.TryToMakeRobot(i, blueprints[i], out var newState))
                            nextStep.Add(newState);
            }
            var best = nextStep.Last(); // set is sorted, Last is best!
            //best.PrintHistory();
            return best.Geodes;
        }
    }
    internal class Node<T>
    {
        public T Value { get; private set; }
        public Node<T> Next { get; private set; }
        public Node<T> Prev { get; private set; }
        public Node(T value) => (Value, Prev, Next) = (value, this, this);
        public Node<T> NNext(long n, int size)
        {
            var node = this;
            var steps = n % size;
            if (steps > 0)
                for (var i = 0; i < steps; i++)
                    node = node.Next;
            else if (steps < 0)
                for (var i = 0; i > steps; i--)
                    node = node.Prev;
            return node;
        }
        public Node<T> Remove()
        {
            (Prev.Next, Next.Prev) = (Next, Prev);
            (Next, Prev) = (null, null);
            return this;
        }
        public Node<T> InsertAfter(Node<T> newNext)
        {
            if (this == newNext || Next == newNext) return this;
            newNext.Prev = this;
            newNext.Next = Next;
            Next.Prev = newNext;
            Next = newNext;
            return this;
        }
        public static Node<T> NewTail()
        {
            var tail = new Node<T>(default);
            tail.Next = tail.Prev = tail;
            return tail;
        }
    }
    public static long Day20(int part)
    {
        var tail = Node<long>.NewTail(); // dummy
        var nodes = new List<Node<long>>();
        var zeroNode = tail;
        for (var line = Console.ReadLine(); line is not null; line = Console.ReadLine())
        {
            var node = new Node<long>(long.Parse(line) * part == 1 ? 1 : 811589153);
            nodes.Add(node);
            tail.InsertAfter(node);
            tail = node;
            if (node.Value == 0) zeroNode = node;
        }
        tail.Next?.Remove(); // remove the dummy tail

        var stopWatch = Stopwatch.StartNew();
        for (var mixes = 0; mixes < (part == 1 ? 1 : 10); mixes++)
            foreach (var node in nodes)
            {
                var anchor = node.Prev;
                node.Remove(); // Apparently the node gets removed BEFORE mixing it
                var destination = anchor.NNext(node.Value, nodes.Count - 1);
                if (node != destination)
                    destination.InsertAfter(node);
            }

        Console.WriteLine(stopWatch.Elapsed.ToString());
        return zeroNode.NNext(1000, nodes.Count).Value
             + zeroNode.NNext(2000, nodes.Count).Value
             + zeroNode.NNext(3000, nodes.Count).Value;

        /*
        static void PrintCLL(Node<long> tail)
        {
            for (var node = tail.Next; node != tail; node = node.Next)
                Console.Write($"{node.Value} ");
            Console.WriteLine(tail.Value);
        }
        */
    }
    internal class Algebraic
    {
        public long? N { get; private set; }
        private readonly Stack<(string op, long other)> Operations;
        public Algebraic(long? value) => (N, Operations) = (value, new());
        private Algebraic(Algebraic copy) =>
            (N, Operations) = (copy.N, new(copy.Operations.Reverse())); // Did you know Stacks copy in reverse? I do now!
        public Algebraic Operate(string op, Algebraic b)
        {
            if (N is null && b.N is null)
                throw new ArgumentNullException(nameof(b), "Only one unknown supported");
            else if (N is null)
            {
                var c = new Algebraic(this);
                c.Operations.Push((op, b.N!.Value));
                return c;
            }
            else if (b.N is null)
            {
                var c = new Algebraic(b);
                switch (op)
                {
                    case "+":
                    case "*":
                        c.Operations.Push((op, N.Value)); break;
                    case "-":
                        c.Operations.Push(("*", -1));
                        c.Operations.Push(("+", N.Value)); break;
                    case "/":
                        throw new NotSupportedException("reciprocal operation not supported");
                }
                return c;
            }
            else
                return op switch
                {
                    "+" => new Algebraic(N + b.N),
                    "-" => new Algebraic(N - b.N),
                    "*" => new Algebraic(N * b.N),
                    "/" => new Algebraic(N / b.N),
                    _ => throw new NotSupportedException($"{op} operation not supported")
                };
        }
        public override string ToString()
        {
            if (N is not null) return N.Value.ToString();
            var sb = new StringBuilder("X");
            foreach (var (op, b) in Operations)
                sb.Append($" {op} {b}");
            return sb.ToString();
        }
        public static Algebraic Solve(Algebraic a, Algebraic b)
        {
            if (a.N is null && b.N is null)
                throw new ArgumentNullException(nameof(a), "Only one unknown supported");
            if (b.N is null)
                return Solve(b, a);
            var sum = new Algebraic(b);
            while (a.Operations.TryPop(out var pair))
            {
                var undo = pair.op switch
                {
                    "+" => "-",
                    "-" => "+",
                    "*" => "/",
                    "/" => "*",
                    _ => throw new ArgumentException($"{pair.op} operation not supported"),
                };
                sum = sum.Operate(undo, new Algebraic(pair.other));
            }
            return sum;
        }
    }
    public static long Day21(int part)
    {
        var monkeyNums = new Dictionary<string, long>();
        var monkeyOps = new Dictionary<string, (string a, string b, string op)>();
        var pattern = new Regex(@"(\w+): (?:(-?\d+)|(\w+) ([-+*/]) (\w+))");
        for (var line = Console.ReadLine(); line is not null; line = Console.ReadLine())
        {
            var match = pattern.Match(line);
            if (match.Groups[2].Success)
                monkeyNums[match.Groups[1].Value] = int.Parse(match.Groups[2].Value);
            else
                monkeyOps[match.Groups[1].Value] =
                    (match.Groups[3].Value, match.Groups[5].Value, match.Groups[4].Value);
        }
        return MonkeySolve("root").N.GetValueOrDefault();

        Algebraic MonkeySolve(string monkey)
        {
            if (part == 2 && monkey == "humn")
                return new(null);
            if (monkeyNums.TryGetValue(monkey, out var value))
                return new(value);
            var (a, b, op) = monkeyOps[monkey];
            if (part == 2 && monkey == "root")
                return Algebraic.Solve(MonkeySolve(a), MonkeySolve(b));
            return MonkeySolve(a).Operate(op, MonkeySolve(b));
        }
    }
    internal class BoardWrap
    {
        public BoardWrap(char[,] board, (int, int) location) =>
            (Board, Location, Size) = (board, location, board.GetLength(0));
        private readonly char[,] Board;
        private readonly int Size;
        public (int x, int y) Location { get; private set; }
        private BoardWrap? WrapLeft, WrapRight, WrapUp, WrapDown;
        private bool ReverseLeft, ReverseRight, ReverseUp, ReverseDown;
        private Facing NewLeft, NewRight, NewUp, NewDown;
        public BoardWrap Stitch(Facing o, BoardWrap other)
        {
            switch (o)
            {
                case Facing.Right: (WrapRight, other.WrapLeft) = (other, this); break;
                case Facing.Left: (WrapLeft, other.WrapRight) = (other, this); break;
                case Facing.Up: (WrapUp, other.WrapDown) = (other, this); break;
                case Facing.Down: (WrapDown, other.WrapUp) = (other, this); break;
            }
            return this;
        }
        public BoardWrap Stitch3d(Facing o1, BoardWrap other, Facing o2, bool reversed = false)
        {
            switch (o1)
            {
                case Facing.Right:
                    (WrapRight, ReverseRight, NewRight) = (other, reversed, o2); break;
                case Facing.Left:
                    (WrapLeft, ReverseLeft, NewLeft) = (other, reversed, o2); break;
                case Facing.Up:
                    (WrapUp, ReverseUp, NewUp) = (other, reversed, o2); break;
                case Facing.Down:
                    (WrapDown, ReverseDown, NewDown) = (other, reversed, o2); break;
            }
            switch (o2)
            {
                case Facing.Right:
                    (other.WrapRight, other.ReverseRight, other.NewRight) = (this, reversed, o1); break;
                case Facing.Left:
                    (other.WrapLeft, other.ReverseLeft, other.NewLeft) = (this, reversed, o1); break;
                case Facing.Up:
                    (other.WrapUp, other.ReverseUp, other.NewUp) = (this, reversed, o1); break;
                case Facing.Down:
                    (other.WrapDown, other.ReverseDown, other.NewDown) = (this, reversed, o1); break;
            }
            return this;
        }
        public enum Facing { Right = 0, Down, Left, Up }
        public (int x, int y, BoardWrap board, Facing facing) Step(int x, int y, Facing facing)
        {
            var (newX, newY, newBoard, newFacing) = (x, y, this, facing);
            var (reverseX, reverseY) = (false, false);
            var (fromFacing, toFacing) = (facing, facing);
            switch (facing)
            {
                case Facing.Left: newX--; break;
                case Facing.Right: newX++; break;
                case Facing.Up: newY--; break;
                case Facing.Down: newY++; break;
            }
            if (newX < 0)
            {
                newX += Size;
                (newBoard, reverseY) = (WrapLeft, ReverseLeft);
                (fromFacing, toFacing) = (Facing.Left, NewLeft);
                newFacing = (Facing)(((int)NewLeft + 2) % 4);
            }
            if (newX >= Size)
            {
                newX -= Size;
                (newBoard, reverseY) = (WrapRight, ReverseRight);
                (fromFacing, toFacing) = (Facing.Right, NewRight);
                newFacing = (Facing)(((int)NewRight + 2) % 4);
            }
            if (newY < 0)
            {
                newY += Size;
                (newBoard, reverseX) = (WrapUp, ReverseUp);
                (fromFacing, toFacing) = (Facing.Up, NewUp);
                newFacing = (Facing)(((int)NewUp + 2) % 4);
            }
            if (newY >= Size)
            {
                newY -= Size;
                (newBoard, reverseX) = (WrapDown, ReverseDown);
                (fromFacing, toFacing) = (Facing.Down, NewDown);
                newFacing = (Facing)(((int)NewDown + 2) % 4);
            }
            var horizontals = new List<Facing>() { Facing.Right, Facing.Left };
            if (this != newBoard)
            {
                if (reverseX) newX = Size - newX - 1;
                if (reverseY) newY = Size - newY - 1;
                if (horizontals.Contains(fromFacing) && !horizontals.Contains(toFacing)
                    || horizontals.Contains(toFacing) && !horizontals.Contains(fromFacing))
                    (newX, newY) = (newY, newX);
                if (toFacing == Facing.Down) newY = Size - 1;
                if (toFacing == Facing.Right) newX = Size - 1;
                if (toFacing == Facing.Up) newY = 0;
                if (toFacing == Facing.Left) newX = 0;
            }
            if (newBoard!.Board[newX, newY] == '#') // blocked!
                return (x, y, this, facing);
            else
                return (newX, newY, newBoard, newFacing);
        }
        public static void Print(BoardWrap?[,] boards, int myX, int myY, BoardWrap myBoard)
        {
            for (var y = 0; y < 4 * 4; y++)
            {
                for (var x = 0; x < 4 * 4; x++)
                {
                    var drawBoard = boards[x / 4, y / 4];
                    if (drawBoard == null) { Console.Write(" "); continue; }
                    else if (drawBoard == myBoard && y % 4 == myY && x % 4 == myX)
                        Console.Write("@");
                    else
                        Console.Write(drawBoard.Board[x % 4, y % 4]);
                }
                Console.WriteLine();
            }
        }
    }
    public static int Day22(int part)
    {
        var lines = new List<string>();
        for (var line = Console.ReadLine(); !string.IsNullOrEmpty(line); line = Console.ReadLine())
            lines.Add(line);
        var instructions = Regex.Matches(Console.ReadLine(), @"(\d+)([LR]|$)")
            .Select(match => (go: int.Parse(match.Groups[1].Value), turn: match.Groups[2].Value));
        var size = lines.Count > lines.Max(x => x.Length)
            ? lines.Count / 4 : lines.Max(x => x.Length) / 4;
        var boards = new BoardWrap?[4, 4];
        for (var boardY = 0; boardY < 4; boardY++)
            for (var boardX = 0; boardX < 4; boardX++)
            {
                if (boardY * size >= lines.Count || boardX * size >= lines[size * boardY].Length
                    || lines[boardY * size][boardX * size] == ' ')
                    continue;
                var board = new char[size, size];
                for (var y = 0; y < size; y++)
                    for (var x = 0; x < size; x++)
                        board[x, y] = lines[y + size * boardY][x + size * boardX];
                boards[boardX, boardY] = new(board, (boardX, boardY));
            }
        if (part == 1)
        {
            for (var y = 0; y < 4; y++)
                for (var x = 0; x < 4; x++)
                    if (boards[x, y] is not null)
                        boards[x, y]!
                            .Stitch(BoardWrap.Facing.Right,
                                boards[(x + 1) % 4, y] ?? boards[(x + 2) % 4, y] ??
                                boards[(x + 3) % 4, y] ?? boards[x, y]!)
                            .Stitch(BoardWrap.Facing.Down,
                                boards[x, (y + 1) % 4] ?? boards[x, (y + 2) % 4] ??
                                boards[x, (y + 3) % 4] ?? boards[x, y]!);
        }
        else if (size == 4)
        {
            boards[2, 0]
                .Stitch3d(BoardWrap.Facing.Left, boards[1, 1], BoardWrap.Facing.Up)
                .Stitch3d(BoardWrap.Facing.Up, boards[0, 1], BoardWrap.Facing.Up, true)
                .Stitch3d(BoardWrap.Facing.Right, boards[3, 2], BoardWrap.Facing.Right, true)
                .Stitch3d(BoardWrap.Facing.Down, boards[2, 1], BoardWrap.Facing.Up);
            boards[0, 1]
                .Stitch3d(BoardWrap.Facing.Left, boards[3, 2], BoardWrap.Facing.Down, true)
                .Stitch3d(BoardWrap.Facing.Down, boards[2, 2], BoardWrap.Facing.Down, true)
                .Stitch3d(BoardWrap.Facing.Right, boards[1, 1], BoardWrap.Facing.Left);
            boards[1, 1]
                .Stitch3d(BoardWrap.Facing.Down, boards[2, 2], BoardWrap.Facing.Left, true)
                .Stitch3d(BoardWrap.Facing.Right, boards[2, 1], BoardWrap.Facing.Left);
            boards[2, 1]
                .Stitch3d(BoardWrap.Facing.Right, boards[3, 2], BoardWrap.Facing.Up, true)
                .Stitch3d(BoardWrap.Facing.Down, boards[2, 2], BoardWrap.Facing.Up);
            boards[2, 2]
                .Stitch3d(BoardWrap.Facing.Right, boards[3, 2], BoardWrap.Facing.Left);
        }
        else
        {
            boards[1, 0]
                .Stitch3d(BoardWrap.Facing.Left, boards[0, 2], BoardWrap.Facing.Left, true)
                .Stitch3d(BoardWrap.Facing.Up, boards[0, 3], BoardWrap.Facing.Left)
                .Stitch3d(BoardWrap.Facing.Right, boards[2, 0], BoardWrap.Facing.Left)
                .Stitch3d(BoardWrap.Facing.Down, boards[1, 1], BoardWrap.Facing.Up);
            boards[2, 0]
                .Stitch3d(BoardWrap.Facing.Up, boards[0, 3], BoardWrap.Facing.Down)
                .Stitch3d(BoardWrap.Facing.Right, boards[1, 2], BoardWrap.Facing.Right, true)
                .Stitch3d(BoardWrap.Facing.Down, boards[1, 1], BoardWrap.Facing.Right);
            boards[1, 1]
                .Stitch3d(BoardWrap.Facing.Left, boards[0, 2], BoardWrap.Facing.Up)
                .Stitch3d(BoardWrap.Facing.Down, boards[1, 2], BoardWrap.Facing.Up);
            boards[0, 2]
                .Stitch3d(BoardWrap.Facing.Right, boards[1, 2], BoardWrap.Facing.Left)
                .Stitch3d(BoardWrap.Facing.Down, boards[0, 3], BoardWrap.Facing.Up);
            boards[1, 2]
                .Stitch3d(BoardWrap.Facing.Down, boards[0, 3], BoardWrap.Facing.Right);
        }

        var (myX, myY, facing) = (0, 0, BoardWrap.Facing.Right);
        var myBoard = boards[0, 0] ?? boards[1, 0] ?? boards[2, 0] ?? boards[3, 0];

        foreach (var (go, turn) in instructions)
        {
            for (var n = 0; n < go; n++)
                (myX, myY, myBoard, facing) = myBoard.Step(myX, myY, facing);
            if (turn == "L") facing += 3;
            if (turn == "R") facing += 1;
            if ((int)facing > 3) facing -= 4;
        }
        return 1000 * (1 + myY + size * myBoard.Location.y)
            + 4 * (1 + myX + size * myBoard.Location.x) + (int)facing;
    }
    public static int Day23(int part)
    {
        var elves = new HashSet<(int x, int y)>();
        var y = 0;
        for (var line = Console.ReadLine(); line is not null; line = Console.ReadLine())
        {
            line.Select((x, i) => (x, i)).Where(pair => pair.x == '#').ToList()
                .ForEach(pair => elves.Add((pair.i, y)));
            y++;
        }
        for (var round = 0; round < 10 || part == 2; round++)
        {
            var consider = elves
                .Where(elf => Neighbors(elf).Any(elves.Contains)) // get elves with neighbor elves
                .GroupBy(elf => Consider(elf, elves, round)) // get each elf's proposed move (if any)
                .Where(group => group.Count() == 1) // get those moves only proposed by one elf
                .Select(group => (group.Key, group.Single())) // pair each such move with its elf
                .Where(pair => pair.Item1 != pair.Item2); // filter out moves to the same spot
            foreach (var (to, from) in consider)
            {
                elves.Remove(from);
                elves.Add(to);
            }
            if (!consider.Any() && part == 2)
                return round + 2;
        }
        var minX = elves.Min(elf => elf.x);
        var maxX = elves.Max(elf => elf.x);
        var minY = elves.Min(elf => elf.y);
        var maxY = elves.Max(elf => elf.y);
        return (maxX - minX + 1) * (maxY - minY + 1) - elves.Count;

        static void PrintBoard(ISet<(int x, int y)> elves)
        {
            var minX = elves.Min(elf => elf.x);
            var maxX = elves.Max(elf => elf.x);
            var minY = elves.Min(elf => elf.y);
            var maxY = elves.Max(elf => elf.y);
            for (var y = minY; y <= maxY; y++)
            {
                for (var x = minX; x <= maxX; x++)
                    Console.Write(elves.Contains((x, y)) ? '#' : '.');
                Console.WriteLine();
            }
        }
        static (int x, int y) Consider((int x, int y) elf, ISet<(int x, int y)> elves, int step)
        {
            var considers = new List<(int x, int y)?>()
            {
                Look(elves, elf.x, elf.y, 0),
                Look(elves, elf.x, elf.y, 1),
                Look(elves, elf.x, elf.y, 2),
                Look(elves, elf.x, elf.y, 3),
                Look(elves, elf.x, elf.y, 0),
                Look(elves, elf.x, elf.y, 1),
                Look(elves, elf.x, elf.y, 2)
            }.Skip(step % 4).Take(4);
            return considers.FirstOrDefault(x => x is not null, (elf.x, elf.y))!.Value;
        }
        static (int x, int y)? Look(ISet<(int x, int y)> elves, int x, int y, int direction) =>
            direction switch
            {
                0 => Neighbors((x, y)).Any(elf => elf.y == y - 1 && elves.Contains(elf)) ? null : (x, y - 1),
                1 => Neighbors((x, y)).Any(elf => elf.y == y + 1 && elves.Contains(elf)) ? null : (x, y + 1),
                2 => Neighbors((x, y)).Any(elf => elf.x == x - 1 && elves.Contains(elf)) ? null : (x - 1, y),
                3 => Neighbors((x, y)).Any(elf => elf.x == x + 1 && elves.Contains(elf)) ? null : (x + 1, y),
                _ => null
            };
        static IEnumerable<(int x, int y)> Neighbors((int x, int y) elf) =>
            new List<(int x, int y)>() {
                (elf.x-1, elf.y-1), (elf.x, elf.y-1), (elf.x+1, elf.y-1),
                (elf.x-1, elf.y),                     (elf.x+1, elf.y),
                (elf.x-1, elf.y+1), (elf.x, elf.y+1), (elf.x+1, elf.y+1),
            };
    }
    internal class Blizzard
    {
        private readonly int _x, _y;
        private readonly char _direction;
        public Blizzard(int x, int y, char direction) => (_x, _y, _direction) = (x, y, direction);
        public (int x, int y) At(int step) => _direction switch
        {
            '>' => ((_x - 1 + step) % XSize + 1, _y),
            '<' => ((_x - 1 + step * (XSize - 1)) % XSize + 1, _y),
            'v' => (_x, (_y - 1 + step) % YSize + 1),
            '^' => (_x, (_y - 1 + step * (YSize - 1)) % YSize + 1),
            _ => (0, 0)
        };

        public static int XSize = 1, YSize = 1;
    }
    public static int Day24(int part)
    {
        var lines = new List<string>();
        for (var line = Console.ReadLine(); !string.IsNullOrEmpty(line); line = Console.ReadLine())
            lines.Add(line);
        var (xSize, ySize) = (lines.First().Length, lines.Count);
        (Blizzard.XSize, Blizzard.YSize) = (xSize - 2, ySize - 2);
        var (startX, startY) = (lines.First().IndexOf('.'), 0);
        var (goalX, goalY) = (lines.Last().IndexOf('.'), ySize - 1);
        var xBlizzards = Enumerable.Range(0, ySize).Select(_ => new List<Blizzard>()).ToArray();
        var yBlizzards = Enumerable.Range(0, xSize).Select(_ => new List<Blizzard>()).ToArray();
        var walls = new HashSet<(int x, int y)>() { (startX, startY - 1), (goalX, goalY + 1) };
        for (var y = 0; y < ySize; y++)
            for (var x = 0; x < xSize; x++)
                switch (lines[y][x])
                {
                    case '#':
                        walls.Add((x, y)); break;
                    case '>':
                    case '<':
                        xBlizzards[y].Add(new Blizzard(x, y, lines[y][x])); break;
                    case 'v':
                    case '^':
                        yBlizzards[x].Add(new Blizzard(x, y, lines[y][x])); break;
                }

        if (part == 1)
            return ShortestPath(startX, startY, 0, goalX, goalY);

        var to = ShortestPath(startX, startY, 0, goalX, goalY);
        var from = ShortestPath(goalX, goalY, to, startX, startY);
        return ShortestPath(startX, startY, from, goalX, goalY);

        int ShortestPath(int startX, int startY, int startStep, int goalX, int goalY)
        {
            var nextSearch = new HashSet<(int x, int y, int step)>() { (startX, startY, startStep) };
            while (nextSearch.Count != 0)
            {
                var thisSearch = nextSearch;
                nextSearch = [];
                foreach (var (x, y, step) in thisSearch)
                {
                    if (x == goalX && y == goalY)
                        return step;
                    if (walls.Contains((x, y)) ||
                        xBlizzards[y].Exists(bliz => bliz.At(step).x == x) ||
                        yBlizzards[x].Exists(bliz => bliz.At(step).y == y))
                        continue;
                    nextSearch.Add((x + 1, y, step + 1));
                    nextSearch.Add((x, y + 1, step + 1));
                    nextSearch.Add((x, y, step + 1));
                    nextSearch.Add((x - 1, y, step + 1));
                    nextSearch.Add((x, y - 1, step + 1));
                }
            }
            return -1;
        }
    }
    internal class Snafu
    {
        public long Value { get; private set; }
        public Snafu(long base10) => Value = base10;
        public Snafu(string snafu) => Value = snafu
            .Reverse()
            .Zip(_powers5)
            .Select(pair => pair.Second * pair.First switch
            { '2' => 2, '1' => 1, '-' => -1, '=' => -2, _ => 0 })
            .Sum();
        public override string ToString()
        {
            var base5 = _powers5.Reverse()
                .Aggregate((remainder: Value, digits: Enumerable.Empty<long>()),
                    (acc, term) => (remainder: acc.remainder % term, digits: acc.digits.Append(acc.remainder / term)))
                .digits;
            // add 2 to every digit, with carry
            var (carry, digits) = base5.SkipWhile(x => x == 0).Reverse()
                .Aggregate((carry: 0L, digits: Enumerable.Empty<long>()),
                    (acc, term) => (carry: (acc.carry + term + 2) / 5, digits: acc.digits.Append((acc.carry + term + 2) % 5)));
            // subtract 2 from each digit, except for final carry (if any)
            var snafu = digits
                .Select(x => x switch { 4 => '2', 3 => '1', 2 => '0', 1 => '-', 0 => '=', _ => 'X' });
            if (carry > 0) snafu = snafu.Append('1');
            return string.Join(string.Empty, snafu.Reverse());
        }
        public static Snafu operator +(Snafu a, Snafu b) => new(a.Value + b.Value);
        private static readonly IEnumerable<long> _powers5 =
            Enumerable.Range(0, 22).Select(n => (long)Math.Pow(5, n));
    }
    public static string Day25(int part)
    {
        var nums = new List<Snafu>();
        for (var line = Console.ReadLine(); !string.IsNullOrEmpty(line); line = Console.ReadLine())
            nums.Add(new Snafu(line));
        return nums.Aggregate((sum, term) => sum + term).ToString();
    }
}
