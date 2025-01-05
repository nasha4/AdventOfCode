using System.Numerics;
using System.Text.RegularExpressions;

namespace Advent_of_Code.Advent2023;

static internal class Advent2023
{
    public static int Day1(int part)
    {
        var numbers = new string[] { "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" }
            .Select((n, i) => (name: n, value: i + 1))
            .ToDictionary(kvp => kvp.name, kvp => kvp.value);
        foreach (var numeral in "123456789")
        {
            numbers[numeral.ToString()] = numeral - '0';
        }

        var lines = new List<IEnumerable<int>>();
        for (string? line = Console.ReadLine(); line is not null; line = Console.ReadLine())
        {
            if (part == 1)
            {
                lines.Add(line.ToCharArray().Where("0123456789".Contains).Select(x => x - '0'));
            }
            else
            {
                var regex = string.Join("|", numbers.Keys);
                var first = Regex.Match(line, regex);
                var last = Regex.Match(line, regex, RegexOptions.RightToLeft);
                var matches = new Match[] { first, last };
                var digits = matches.Select(x => numbers[x.Value]);
                lines.Add(digits);
            }
        }
        return lines.Sum(x => x.First() * 10 + x.Last());
    }

    public static long Day2(int part)
    {
        static (int r, int g, int b) ParseRound(string round)
        {
            var rgb = new string[] { "red", "green", "blue" }
                .Select(x => Regex.Match(round, $"(\\d+) {x}"))
                .Select(x => x.Success ? int.Parse(x.Groups[1].Value) : 0)
                .ToArray();
            return (rgb[0], rgb[1], rgb[2]);
        }

        var games = new List<IEnumerable<(int r, int g, int b)>>();

        for (string? line = Console.ReadLine(); line is not null; line = Console.ReadLine())
        {
            var gameNumber = line.Split(':');
            var rounds = gameNumber.Last().Split(";");
            games.Add(rounds.Select(ParseRound));
        }

        if (part == 1)
            return games.Select((x, i) => (x, i + 1))
                .Where(t => !t.x.Any(rgb => rgb.r > 12 || rgb.g > 13 || rgb.b > 14))
                .Sum(t => t.Item2);

        return games.Select(x => x.Aggregate((a, b) => (int.Max(a.r, b.r), int.Max(a.g, b.g), int.Max(a.b, b.b))))
            .Sum(rgb => rgb.r * rgb.g * rgb.b);

    }

    public static int Day3(int part)
    {
        var engine = new List<string>();
        var numbers = new List<int>();
        var rectangles = new List<(int x0, int y0, int w)>();
        var symbols = new List<(char s, int x, int y)>();
        for (var line = Console.ReadLine(); line is not null; line = Console.ReadLine())
        {
            foreach (var match in Regex.Matches(line, @"\d+").Cast<Match>())
            {
                numbers.Add(int.Parse(match.Value));
                rectangles.Add((match.Index - 1, engine.Count - 1, match.Length + 2));
            }
            engine.Add(line);
        }
        foreach (var (x0, y0, w) in rectangles)
        {
            symbols.Add(Enumerable.Range(x0, w)
                .Where(x => x >= 0 && x < engine.First().Length)
                .SelectMany(_ => Enumerable.Range(y0, 3).Where(y => y >= 0 && y < engine.Count), (x, y) => (engine[y][x], x, y))
                .SingleOrDefault(c => !"01234566789.".Contains(c.Item1)));
        }
        if (part == 1)
            return numbers.Zip(symbols).Where(p => p.Second.s > '\0').Sum(p => p.First);

        return numbers
            .Zip(symbols)
            .Where(p => p.Second.s == '*')
            .GroupBy(p => p.Second)
            .Where(g => g.Count() == 2)
            .Sum(g => g.Aggregate(1, (acc, term) => acc * term.First));
    }

    public static long Day4(int part)
    {
        var memo = new Dictionary<int, long>();
        var matches = new List<int>();
        long Copies(int ticket)
        {
            if (!memo.ContainsKey(ticket))
                memo[ticket] = 1 + Enumerable.Range(ticket + 1, matches[ticket - 1]).Sum(Copies);
            return memo[ticket];
        }

        for (string? line = Console.ReadLine(); line is not null; line = Console.ReadLine())
        {
            var cardNumber = line.Split(':');
            var parts = cardNumber.Last().Split("|");
            var winners = Regex.Matches(parts.First(), @"\d+").Select(x => x.Value);
            var youHave = Regex.Matches(parts.Last(), @"\d+").Select(x => x.Value);
            matches.Add(youHave.Count(x => winners.Contains(x)));
        }
        if (part == 1)
        {
            return matches.Sum(x => x == 0 ? 0 : (long)Math.Pow(2, x - 1));
        }
        return Enumerable.Range(1, matches.Count).Sum(Copies);
    }

    public static long Day5(int part)
    {
        static long Convert(IEnumerable<(long, long, long)> table, long value)
        {
            foreach (var (dest, source, length) in table)
            {
                if (value >= source && value < source + length)
                    return dest + value - source;
            }
            return value;
        }
        static long Unconvert(IEnumerable<(long, long, long)> table, long value)
        {
            foreach (var (dest, source, length) in table)
            {
                if (value >= dest && value < dest + length)
                    return source + value - dest;
            }
            return value;
        }

        var conversions = new List<List<(long d, long s, long l)>>();
        var line = Console.ReadLine();
        var seeds = line!.Split(": ")[1].Split(' ').Select(long.Parse);

        for (line = Console.ReadLine(); line is not null; line = Console.ReadLine())
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            if (line.EndsWith(" map:")) { conversions.Add([]); continue; }
            var n = line.Split(' ').Select(long.Parse).ToArray();
            conversions.Last().Add((n[0], n[1], n[2]));
        }

        if (part == 1)
        {
            var locations = seeds.Select(seed => conversions.Aggregate(seed, (acc, table) => Convert(table, acc)));
            return locations.Min();
        }
        var seedRanges = seeds.Chunk(2);
        for (long loc = 0; ; loc++)
        {
            var seed = conversions.AsEnumerable().Reverse().Aggregate(loc, (acc, table) => Unconvert(table, acc));
            if (seedRanges.Any(range => seed >= range[0] && seed < range[0] + range[1]))
                return loc;
        }
    }

    public static long Day6(int part)
    {
        IEnumerable<long> times, dists;
        if (part == 1)
        {
            times = Regex.Split(Console.ReadLine()!, @"\s+").Skip(1).Select(long.Parse);
            dists = Regex.Split(Console.ReadLine()!, @"\s+").Skip(1).Select(long.Parse);
        }
        else
        {
            times = [long.Parse(string.Join("", Regex.Split(Console.ReadLine()!, @"\s+").Skip(1)))];
            dists = [long.Parse(string.Join("", Regex.Split(Console.ReadLine()!, @"\s+").Skip(1)))];
        }
        var races = times.Select(ms => Enumerable.Range(0, (int)ms).Select(x => x * (ms - x)));
        var beats = dists.Zip(races).Select(x => x.Second.Count(mm => mm > x.First));
        return beats.Aggregate((acc, term) => term * acc);
    }

    internal class CamelHand : IComparable<CamelHand>
    {
        public string Hand { get; protected set; }
        public int Bid { get; protected set; }
        protected readonly Dictionary<char, int> CardCounts;
        public CamelHand(string hand, int bid)
        {
            (Hand, Bid) = (hand, bid);
            CardCounts = Hand.GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count());
        }
        public int CompareTo(CamelHand? other)
        {
            if (other == null) return 1;
            if (Strength != other.Strength) return Strength - other.Strength;
            foreach (var (first, second) in CardRanks.Zip(other.CardRanks))
            {
                if (first != second) return first - second;
            }
            return 0;
        }
        protected virtual int Strength => (CardCounts.Values.Max(), CardCounts.Count) switch
        {
            (5, _) => 6, // five of a kind
            (4, _) => 5, // four of a kind
            (3, 2) => 4, // full house
            (3, _) => 3, // three of a kind
            (2, 3) => 2, // two pair
            (2, 4) => 1, // pair
            (1, 5) => 0, // nothing
            (_, _) => throw new ArgumentOutOfRangeException(Hand, nameof(Hand))
        };
        protected IEnumerable<int> CardRanks => Hand.Select(x => CardOrder.IndexOf(x));
        protected virtual string CardOrder => "23456789TJQKA";
    }
    internal class JokerHand : CamelHand
    {
        public JokerHand(string hand, int bid) : base(hand, bid)
        {
            var jokers = CardCounts.GetValueOrDefault('J', 0);
            if (jokers > 0 && jokers < 5)
            {
                CardCounts.Remove('J');
                var most = CardCounts.MaxBy(x => x.Value).Key;
                CardCounts[most] += jokers;
            }
        }
        protected override string CardOrder => "J23456789TQKA";
    }
    public static long Day7(int part)
    {
        var hands = new List<CamelHand>();
        for (var line = Console.ReadLine(); line is not null; line = Console.ReadLine())
        {
            var split = line.Split(' ');
            if (part == 1) hands.Add(new CamelHand(split[0], int.Parse(split[1])));
            if (part == 2) hands.Add(new JokerHand(split[0], int.Parse(split[1])));
        }
        return hands.Order().Select((x, i) => x.Bid * (i + 1)).Sum();
    }

    public static long Day8(int part)
    {
        var lrSequence = Console.ReadLine()!.ToCharArray();
        var tree = new Dictionary<string, (string l, string r)>();

        for (var line = Console.ReadLine(); line is not null; line = Console.ReadLine())
        {
            var match = Regex.Match(line, @"(...) = \((...), (...)\)");
            if (match.Success)
            {
                tree[match.Groups[1].Value] = (match.Groups[2].Value, match.Groups[3].Value);
            }
        }
        long steps = 0;
        if (part == 1)
        {
            for (var node = "AAA"; node != "ZZZ"; steps++)
            {
                var direction = lrSequence[(int)(steps % lrSequence.Length)];
                node = direction == 'L' ? tree[node].l : tree[node].r;
            }
            return steps;
        }
        else
        {
            var nodes = tree.Keys.Where(x => x.EndsWith('A'));
            var history = nodes.Select(_ => new List<long>()).ToArray();
            while (history.Any(x => x.Count < 3))
            {
                var direction = lrSequence[(int)(steps % lrSequence.Length)];
                nodes = nodes.Select(x => direction == 'L' ? tree[x].l : tree[x].r).ToArray();
                var zs = nodes.Select((node, i) => (node, i)).Where(x => x.node.EndsWith('Z')).Select(x => x.i).ToList();
                zs.ForEach(z => history[z].Add(steps));
                steps++;
            }
            var periods = history.Select(ghost => ghost.Last() - ghost.SkipLast(1).Last());
            return periods.Aggregate((acc, term) => acc * term / Gcd(acc, term));
        }

        static long Gcd(long a, long b)
        {
            while (b > 0)
                (a, b) = (b, a % b);
            return a;
        }
    }

    public static long Day9(int part)
    {
        static long Extrapolate(IEnumerable<int> seq)
        {
            var extrapolated = 0;
            for (var deltas = seq; !deltas.All(x => x == 0); deltas = deltas.Skip(1).Zip(deltas, (a, b) => a - b))
            {
                extrapolated += deltas.Last();
            }
            return extrapolated;
        }

        static long Baxtrapolate(IEnumerable<int> seq)
        {
            var firsts = new Stack<int>();
            for (var deltas = seq; !deltas.All(x => x == 0); deltas = deltas.Skip(1).Zip(deltas, (a, b) => a - b))
            {
                firsts.Push(deltas.First());
            }
            var extrapolated = 0;
            while (firsts.TryPop(out var first))
            {
                extrapolated = first - extrapolated;
            }
            return extrapolated;
        }

        var seqs = new List<IEnumerable<int>>();
        for (var line = Console.ReadLine(); line is not null; line = Console.ReadLine())
        {
            seqs.Add(line.Split(' ').Select(int.Parse));
        }

        return part switch
        {
            1 => seqs.Sum(Extrapolate),
            2 => seqs.Sum(Baxtrapolate),
            _ => 0
        };
    }

    public static int Day10(int part)
    {
        var creature = (x: 0, y: 0);
        var field = new List<char[]>();
        for (var line = Console.ReadLine(); line is not null; line = Console.ReadLine())
        {
            var s = line.IndexOf('S');
            if (s >= 0) creature = (s, field.Count);
            field.Add(line.ToCharArray());
        }
        var connections = new (int x, int y)[] { (0, -1), (1, 0), (0, 1), (-1, 0) }
            .Select(p => (x: p.x + creature.x, y: p.y + creature.y))
            .Select(p => p.x < 0 || p.x >= field.First().Length || p.y < 0 || p.y >= field.Count ? '.' : field[p.y][p.x])
            .Zip(new[] { "7|F", "J-7", "J|L", "L-F" }, (c, s) => s.Contains(c))
            .ToArray();

        field[creature.y][creature.x] = connections switch
        {
        [true, true, false, false] => 'L',
        [true, false, true, false] => '|',
        [true, false, false, true] => 'J',
        [false, true, true, false] => 'F',
        [false, true, false, true] => '-',
        [false, false, true, true] => '7',
            _ => throw new ArgumentException($"Pipe does not connect uniquely at {creature}")
        };

        var go = (x: 0, y: 0);
        var loop = new HashSet<(int, int)>();
        for (var at = creature; at != creature || loop.Count == 0; at = (at.x + go.x, at.y + go.y))
        {
            loop.Add(at);
            var (dir1, dir2) = field[at.y][at.x] switch
            {
                '|' => ((x: 0, y: 1), (x: 0, y: -1)),
                '-' => ((x: 1, y: 0), (x: -1, y: 0)),
                'L' => ((x: 1, y: 0), (x: 0, y: -1)),
                'J' => ((x: -1, y: 0), (x: 0, y: -1)),
                '7' => ((x: -1, y: 0), (x: 0, y: 1)),
                'F' => ((x: 1, y: 0), (x: 0, y: 1)),
                _ => throw new ArgumentException($"Unexpected pipe symbol {field[at.y][at.x]} at {at}")
            };
            go = (dir1.x + go.x, dir1.y + go.y) == (0, 0) ? dir2 : dir1; // don't go backward
        }
        if (part == 1)
            return loop.Count / 2;

        var insides = new HashSet<(int, int)>();
        for (var y = 0; y < field.Count; y++)
        {
            var lastFL = '.';
            var inside = false;
            for (var x = 0; x < field[y].Length; x++)
            {
                if (loop.Contains((x, y)))
                {
                    switch (field[y][x])
                    {
                        case 'F': case 'L': lastFL = field[y][x]; break;
                        case 'J': inside ^= lastFL == 'F'; break;
                        case '7': inside ^= lastFL == 'L'; break;
                        case '|': inside ^= true; break;
                    }
                }
                else if (inside)
                {
                    insides.Add((x, y));
                }
            }
        }
        return insides.Count;
    }

    public static long Day11(int part)
    {
        var expansion = part == 1 ? 2 : 1_000_000;
        long distances = 0;
        var galaxies = new List<(int x, int y)>();
        var universe = new List<string>();
        for (var line = Console.ReadLine(); line is not null; line = Console.ReadLine())
        {
            galaxies.AddRange(line.Select((x, i) => (x, i)).Where(p => p.x == '#').Select(p => (universe.Count, p.i)));
            universe.Add(line);
        }
        var xIsEmpty = universe.Select((x, i) => (x, i)).Where(p => !p.x.Contains('#')).Select(p => p.i).ToHashSet();
        var yIsEmpty = Enumerable.Range(0, universe.First().Length).Where(x => universe.All(y => y[x] == '.')).ToHashSet();
        foreach (var pair in galaxies.SelectMany((x, i) => galaxies.Skip(i + 1), (a, b) => (a, b)))
        {
            var xRange = Enumerable.Range(Math.Min(pair.a.x, pair.b.x), Math.Abs(pair.a.x - pair.b.x));
            var yRange = Enumerable.Range(Math.Min(pair.a.y, pair.b.y), Math.Abs(pair.a.y - pair.b.y));
            var xDist = xRange.Sum(x => xIsEmpty.Contains(x) ? expansion : 1);
            var yDist = yRange.Sum(y => yIsEmpty.Contains(y) ? expansion : 1);
            distances += xDist + yDist;
        }
        return distances;
    }

    public static long Day12(int part)
    {
        Dictionary<string, long> memo;
        Dictionary<string, long> cache = [];
        long count = 0;
        for (var line = Console.ReadLine(); line is not null; line = Console.ReadLine())
        {
            var parts = line.Split(' ');
            var (pattern, groups) = (parts[0], parts[1].Split(',').Select(int.Parse).ToArray());
            if (part == 0)
            {
                var spread = groups.Aggregate(new List<int>() { 0 }.AsEnumerable(),
                    (acc, term) => acc.Append(acc.Last() + 1 + term)).SkipLast(1);
                Console.WriteLine($"{pattern} {parts[1]} {Check(pattern.ToCharArray(), groups, spread)}");
            }
            else if (part == 1)
            {
                memo = new() { ["hits"] = 0 };
                Console.WriteLine($"{pattern} {parts[1]} {Solve(pattern.ToCharArray(), groups, [])}");
                //counts.Add(Solve(pattern.ToCharArray(), groups, []));
            }
            else
            {
                pattern = $"{pattern}?{pattern}?{pattern}?{pattern}?{pattern}";
                groups = [.. groups, .. groups, .. groups, .. groups, .. groups];
                memo = new() { ["hits"] = 0 };
                Console.Write($"{pattern} {parts[1]} ");
                var solve = Solve(pattern.ToCharArray(), groups, []);
                var cheat = Calculate(pattern, [.. groups]);
                Console.WriteLine($"{memo["hits"]}/{solve}/{cheat}");
                count += cheat;
            }
        }
        return count;

        static bool Check(char[] chars, int[] groups, IEnumerable<int> spread)
        {
            var length = spread.Any() ? spread.Last() + groups[spread.Count() - 1] + 1 : 0;
            var toCheck = Enumerable.Repeat('.', length).ToArray();
            groups.Zip(spread)
                .SelectMany(p => Enumerable.Range(p.Second, p.First))
                .ToList().ForEach(x => toCheck[x] = '#');
            return !chars.Zip(toCheck).Any(p => p.First != '?' && p.First != p.Second);
        }
        long Solve(char[] chars, int[] groups, IEnumerable<int> spread)
        {
            if (!Check(chars, groups, spread))
            {
                return 0;
            }
            if (spread.Count() == groups.Length)
            {
                return 1;
            }

            var length = spread.Any() ? spread.Last() + groups[spread.Count() - 1] : 0;
            var key = $"{spread.Count()} {length}";

            if (memo.ContainsKey(key) && false)
            {
                memo["hits"]++;
            }
            else
            {
                memo[key] = 0;
                var lastGroup = spread.Any() ? groups[spread.Count() - 1] : -2;
                var tailGroups = groups.Skip(spread.Count());
                var tailSize = tailGroups.Sum(x => x + 1) - (tailGroups.Count() > 1 ? 1 : 0);
                var lastSpread = spread.LastOrDefault(1);

                for (var thisSpread = lastGroup + lastSpread + 1; thisSpread < chars.Length - tailSize + 2; thisSpread++)
                {
                    memo[key] += Solve(chars, groups, spread.Append(thisSpread));
                }
            }

            return memo[key];
        }

        long Calculate(string springs, List<int> groups)
        {
            var key = $"{springs},{string.Join(',', groups)}";  // Cache key: spring pattern + group lengths

            if (cache.TryGetValue(key, out var value))
            {
                return value;
            }

            value = GetCount(springs, groups);
            cache[key] = value;

            return value;
        }
        long GetCount(string springs, List<int> groups)
        {
            while (true)
            {
                if (groups.Count == 0)
                {
                    return springs.Contains('#') ? 0 : 1; // No more groups to match: if there are no springs left, we have a match
                }

                if (string.IsNullOrEmpty(springs))
                {
                    return 0; // No more springs to match, although we still have groups to match
                }

                if (springs.StartsWith('.'))
                {
                    springs = springs.Trim('.'); // Remove all dots from the beginning
                    continue;
                }

                if (springs.StartsWith('?'))
                {
                    return Calculate("." + springs[1..], groups) + Calculate("#" + springs[1..], groups); // Try both options recursively
                }

                if (springs.StartsWith('#')) // Start of a group
                {
                    if (groups.Count == 0)
                    {
                        return 0; // No more groups to match, although we still have a spring in the input
                    }

                    if (springs.Length < groups[0])
                    {
                        return 0; // Not enough characters to match the group
                    }

                    if (springs[..groups[0]].Contains('.'))
                    {
                        return 0; // Group cannot contain dots for the given length
                    }

                    if (groups.Count > 1)
                    {
                        if (springs.Length < groups[0] + 1 || springs[groups[0]] == '#')
                        {
                            return 0; // Group cannot be followed by a spring, and there must be enough characters left
                        }

                        springs = springs[(groups[0] + 1)..]; // Skip the character after the group - it's either a dot or a question mark
                        groups = groups[1..];
                        continue;
                    }

                    springs = springs[groups[0]..]; // Last group, no need to check the character after the group
                    groups = groups[1..];
                    continue;
                }

                throw new Exception("Invalid input");
            }
        }
    }

    public static int Day13(int part)
    {
        var patterns = new List<List<string>>() { new() };
        for (var line = Console.ReadLine(); line is not null; line = Console.ReadLine())
            if (string.IsNullOrEmpty(line))
                patterns.Add([]);
            else
                patterns.Last().Add(line);

        int summary = 0, smudges = part == 1 ? 0 : 1;
        foreach (var pattern in patterns)
        {
            var xSize = pattern.First().Length;
            var ySize = pattern.Count;
            var foldedX = Enumerable.Range(1, xSize - 1)
                .Select(n => Enumerable.Range(0, n).Reverse().Zip(Enumerable.Range(n, xSize - n)))
                .Select((a, i) => (diffs: a.Sum(p => pattern.Count(y => y[p.First] != y[p.Second])), left: i + 1))
                .Where(p => p.diffs == smudges)
                .Select(p => p.left);
            var foldedY = Enumerable.Range(1, ySize - 1)
                .Select(n => Enumerable.Range(0, n).Reverse().Zip(Enumerable.Range(n, ySize - n)))
                .Select((a, i) => (diffs: a.Sum(p => pattern[p.First].Zip(pattern[p.Second]).Count(z => z.First != z.Second)), up: i + 1))
                .Where(p => p.diffs == smudges)
                .Select(p => p.up);

            summary += foldedX.Sum() + 100 * foldedY.Sum();
        }
        return summary;
    }

    public static int Day14(int part)
    {
        var platform = new List<char[]>();
        for (var line = Console.ReadLine(); line is not null; line = Console.ReadLine())
        {
            platform.Add(line.ToCharArray());
        }
        if (part == 1)
        {
            RollNorth(platform);
            return platform.AsEnumerable()
                .Reverse()
                .Select((row, i) => (row, i))
                .Sum(p => (p.i + 1) * p.row.Count(c => c == 'O'));
        }
        else
        {
            var history = new List<HashSet<(int, int)>>();
            var loads = new List<int>();
            var now = ToSet(platform);
            while (!history.Any(x => now.SetEquals(x)))
            {
                RollNorth(platform);
                RollWest(platform);
                RollSouth(platform);
                RollEast(platform);
                history.Add(now);
                now = ToSet(platform);
                loads.Add(platform.AsEnumerable().Reverse().Select((row, i) => (row, i)).Sum(p => (p.i + 1) * p.row.Count(c => c == 'O')));
            }
            var loopStart = history.FindIndex(x => x.SetEquals(now));
            var loopSize = history.Count - loopStart;
            var billion = (1_000_000_000 - loopStart) % loopSize + loopStart - 1;
            Console.WriteLine($"loopstart {loopStart}, loopsize {loopSize}: billionth load = {loads[billion]}");
            return loads[billion];
        }

        static HashSet<(int, int)> ToSet(List<char[]> platform) =>
            Enumerable.Range(0, platform.First().Length)
                .SelectMany(_ => Enumerable.Range(0, platform.Count),
                    (x, y) => platform[y][x] == 'O' ? (x, y) : (-1, -1))
                .ToHashSet();

        static void RollNorth(List<char[]> platform)
        {
            foreach (var (row, y) in platform.Select((row, i) => (row, i)))
                foreach (var (c, x) in row.Select((c, i) => (c, i)))
                    if (c == 'O')
                    {
                        var rollToY = 1 +
                            Enumerable.Range(0, y).Reverse()
                            .FirstOrDefault(n => platform[n][x] != '.', -1);
                        platform[y][x] = '.';
                        platform[rollToY][x] = 'O';
                    }
        }
        static void RollSouth(List<char[]> platform)
        {
            foreach (var (row, y) in platform.Select((row, i) => (row, i)).Reverse())
                foreach (var (c, x) in row.Select((c, i) => (c, i)))
                    if (c == 'O')
                    {
                        var rollToY = -1 +
                            Enumerable.Range(y + 1, platform.Count - y - 1)
                            .FirstOrDefault(n => platform[n][x] != '.', platform.Count);
                        platform[y][x] = '.';
                        platform[rollToY][x] = 'O';
                    }
        }
        static void RollEast(List<char[]> platform)
        {
            foreach (var x in Enumerable.Range(0, platform.First().Length).Reverse())
                foreach (var y in Enumerable.Range(0, platform.Count))
                    if (platform[y][x] == 'O')
                    {
                        var rollToX = -1 +
                            Enumerable.Range(x + 1, platform.First().Length - x - 1)
                            .FirstOrDefault(n => platform[y][n] != '.', platform.First().Length);
                        platform[y][x] = '.';
                        platform[y][rollToX] = 'O';
                    }
        }
        static void RollWest(List<char[]> platform)
        {
            foreach (var x in Enumerable.Range(0, platform.First().Length))
                foreach (var y in Enumerable.Range(0, platform.Count))
                    if (platform[y][x] == 'O')
                    {
                        var rollToX = 1 +
                            Enumerable.Range(0, x).Reverse()
                            .FirstOrDefault(n => platform[y][n] != '.', -1);
                        platform[y][x] = '.';
                        platform[y][rollToX] = 'O';
                    }
        }
    }

    public static int Day15(int part)
    {
        var steps = Console.ReadLine().Split(',');
        if (part == 1)
            return steps.Sum(step => step.Aggregate(0, (acc, c) => (acc + c) * 17 % 256));

        var boxes = Enumerable.Range(0, 256).Select(_ => new List<(string label, int focalLength)>()).ToArray();
        foreach (var step in steps)
        {
            var (label, focalLength) = step.EndsWith('-') ?
                (new string(step.SkipLast(1).ToArray()!), 0) :
                (new string(step.SkipLast(2).ToArray()!), step.Last() - '0');
            var box = boxes[label.Aggregate(0, (acc, c) => (acc + c) * 17 % 256)];
            if (focalLength == 0)
            {
                box.RemoveAll(x => x.label == label);
            }
            else
            {
                var index = box.FindIndex(x => x.label == label);
                if (index < 0)
                    box.Add((label, focalLength));
                else
                    box[index] = (label, focalLength);
            }
        }
        return boxes.Select((box, i1) => (i1 + 1) * box.Select((lens, i2) => (i2 + 1) * lens.focalLength).Sum()).Sum();
    }

    public class MirrorTile
    {
        public MirrorTile(char tile) => (Tile, Up, Down, Left, Right) = (tile, false, false, false, false);
        public readonly char Tile;
        public bool Up { get; set; }
        public bool Down { get; set; }
        public bool Left { get; set; }
        public bool Right { get; set; }
    }
    public class Beam
    {
        public Beam(int x, int y, int dx, int dy) => (X, Y, Dx, Dy) = (x, y, dx, dy);
        public IEnumerable<Beam> Next(char c)
        {
            switch (c, Dx, Dy)
            {
                case ('.', _, _):
                case ('-', _, 0):
                case ('|', 0, _):
                    X += Dx; Y += Dy; break;
                case ('\\', _, _):
                    (Dx, Dy) = (Dy, Dx); X += Dx; Y += Dy; break;
                case ('/', _, _):
                    (Dx, Dy) = (-Dy, -Dx); X += Dx; Y += Dy; break;
                case ('|', _, 0):
                    Y -= 1; (Dx, Dy) = (0, -1);
                    return [new(X, Y + 1, 0, 1)];
                case ('-', 0, _):
                    X -= 1; (Dx, Dy) = (-1, 0);
                    return [new(X + 1, Y, 1, 0)];
            }
            return [];
        }
        public int X { get; private set; }
        public int Y { get; private set; }
        public int Dx { get; private set; }
        public int Dy { get; private set; }
    }
    public static int Day16(int part)
    {
        var contraption = new List<MirrorTile[]>();
        for (var line = Console.ReadLine(); line is not null; line = Console.ReadLine())
        {
            contraption.Add(line.Select(c => new MirrorTile(c)).ToArray());
        }
        if (part == 1)
            return ShootBeam(contraption, new(0, 0, 1, 0));

        return Enumerable.Range(0, contraption.First().Length).Select(x => new Beam(x, 0, 0, 1))
            .Concat(Enumerable.Range(0, contraption.First().Length).Select(x => new Beam(x, contraption.Count - 1, 0, -1)))
            .Concat(Enumerable.Range(0, contraption.Count).Select(y => new Beam(0, y, 1, 0)))
            .Concat(Enumerable.Range(0, contraption.Count).Select(y => new Beam(contraption.First().Length - 1, y, -1, 0)))
            .Select(beam => ShootBeam(contraption, beam))
            .Max();

        static int ShootBeam(List<MirrorTile[]> contraption, Beam initialBeam)
        {
            var beams = new Stack<Beam>();
            beams.Push(initialBeam);
            contraption.SelectMany(x => x).ToList().ForEach(x => x.Left = x.Right = x.Up = x.Down = false);

            while (beams.Count > 0)
            {
                var beam = beams.Peek();
                if (beam.X < 0 || beam.X >= contraption.First().Length ||
                    beam.Y < 0 || beam.Y >= contraption.Count ||
                    beam.Dx == 1 && contraption[beam.Y][beam.X].Right ||
                    beam.Dx == -1 && contraption[beam.Y][beam.X].Left ||
                    beam.Dy == 1 && contraption[beam.Y][beam.X].Down ||
                    beam.Dy == -1 && contraption[beam.Y][beam.X].Up)
                {
                    beams.Pop();
                    continue;
                }
                switch (beam.Dx, beam.Dy)
                {
                    case (0, -1): contraption[beam.Y][beam.X].Up = true; break;
                    case (0, 1): contraption[beam.Y][beam.X].Down = true; break;
                    case (-1, 0): contraption[beam.Y][beam.X].Left = true; break;
                    case (1, 0): contraption[beam.Y][beam.X].Right = true; break;
                }
                foreach (var split in beam.Next(contraption[beam.Y][beam.X].Tile))
                    beams.Push(split);
            }
            return contraption.SelectMany(x => x).Count(x => x.Left || x.Right || x.Up || x.Down);
        }
    }

    public static long Day18(int part)
    {
        (long x, long y) my = (0, 0);
        var grid = new HashSet<(long x, long y)>() { my };
        var verts = new List<(long x, long y)>();
        long edge = 2;
        for (var line = Console.ReadLine(); line is not null; line = Console.ReadLine())
        {
            var parts = line.Split(' ');
            var (n, color) = (int.Parse(parts[1]), parts[2]);
            if (part == 1)
            {
                var dir = parts[0] switch
                {
                    "U" => (x: 0, y: -1),
                    "D" => (x: 0, y: 1),
                    "L" => (x: -1, y: 0),
                    "R" => (x: 1, y: 0),
                    _ => (x: 0, y: 0),
                };
                for (var i = 0; i < n; i++)
                {
                    my.x += dir.x; my.y += dir.y;
                    grid.Add(my);
                }
            }
            else
            {
                var length = int.Parse(color.Substring(2, 5), System.Globalization.NumberStyles.HexNumber);
                var dir = color[7] switch
                {
                    '0' => (x: 1, y: 0),
                    '1' => (x: 0, y: 1),
                    '2' => (x: -1, y: 0),
                    '3' => (x: 0, y: -1),
                    _ => (x: 0, y: 0),
                };

                edge += length;

                Console.WriteLine($"  going from {my} to {dir}*{length}");
                my.x += dir.x * length;
                my.y += dir.y * length;
                verts.Add(my);
            }
        }
        if (part == 1)
        {
            var corner = grid.OrderBy(p => p.x).ThenBy(p => p.y).First();
            FloodFill(grid, corner.x + 1, corner.y + 1);
            return grid.Count;
        }
        return verts.Zip(verts.Skip(1).Append(verts.First())).Aggregate(edge, (acc, term) => acc + term.First.x * term.Second.y - term.Second.x * term.First.y) / 2;

        static void FloodFill(HashSet<(long x, long y)> grid, long x, long y)
        {
            var toFill = new Queue<(long x, long y)>();
            toFill.Enqueue((x, y));
            while (toFill.Count > 0)
            {
                (x, y) = toFill.Dequeue();
                if (!grid.Contains((x, y)))
                {
                    grid.Add((x, y));
                    toFill.Enqueue((x + 1, y));
                    toFill.Enqueue((x - 1, y));
                    toFill.Enqueue((x, y + 1));
                    toFill.Enqueue((x, y - 1));
                }
            }
        }
    }

    public record XmasRule(char C, char Cmp, int N, string WorkFlow)
    {
        public static XmasRule FromString(string s)
        {
            var match = Regex.Match(s, @"(.)(.)(\d+):(.+)");
            return match.Success switch
            {
                true => new(match.Groups[1].Value[0], match.Groups[2].Value[0], int.Parse(match.Groups[3].Value), match.Groups[4].Value),
                false => new(' ', '=', 0, s)
            };
        }
        public static bool operator &(XmasRule rule, XmasPart part)
        {
            var val = rule.C switch
            {
                'x' => part.X,
                'm' => part.M,
                'a' => part.A,
                's' => part.S,
                ' ' => 0,
                _ => throw new ArgumentException($"Unexpected XMAS rule: {rule.C}")
            };
            return rule.Cmp switch
            {
                '>' => val > rule.N,
                '<' => val < rule.N,
                _ => true
            };
        }
        public static (XmasSpace? inc, XmasSpace? exc) operator &(XmasRule rule, XmasSpace space)
        {
            var (inc, exc) = (rule.C, rule.Cmp) switch
            {
                (_, '=') => (space, XmasSpace.Empty),
                ('x', '<') => (space with { X1 = rule.N }, space with { X0 = rule.N }), // 100-200 <150:  100-150 150-200
                ('m', '<') => (space with { M1 = rule.N }, space with { M0 = rule.N }),
                ('a', '<') => (space with { A1 = rule.N }, space with { A0 = rule.N }),
                ('s', '<') => (space with { S1 = rule.N }, space with { S0 = rule.N }),
                ('x', '>') => (space with { X0 = rule.N + 1 }, space with { X1 = rule.N + 1 }), // 100-200 >150:  151-200 100-151
                ('m', '>') => (space with { M0 = rule.N + 1 }, space with { M1 = rule.N + 1 }),
                ('a', '>') => (space with { A0 = rule.N + 1 }, space with { A1 = rule.N + 1 }),
                ('s', '>') => (space with { S0 = rule.N + 1 }, space with { S1 = rule.N + 1 }),
                _ => throw new ArgumentException($"Unexpected XMAS rule: {rule.C} {rule.Cmp}")
            };
            return (inc, exc) switch
            {
                ({ Size: > 0 }, { Size: > 0 }) => (inc, exc),
                (_, { Size: > 0 }) => (null, space),
                _ => (space, null)
            };
        }
    }
    public record XmasPart(int X, int M, int A, int S);
    public record XmasSpace(int X0, int X1, int M0, int M1, int A0, int A1, int S0, int S1)
    {
        public static XmasSpace Empty = new(0, 0, 0, 0, 0, 0, 0, 0);
        public long Size => (long)(X1 - X0) * (M1 - M0) * (A1 - A0) * (S1 - S0);
    }

    public static long Day19(int part)
    {
        var rules = new Dictionary<string, IEnumerable<XmasRule>>() { ["A"] = [], ["R"] = [] };
        var parts = new List<XmasPart>();
        for (var line = Console.ReadLine(); line is not null; line = Console.ReadLine())
        {
            if (string.IsNullOrEmpty(line)) continue;
            var partMatch = Regex.Match(line, @"{x=(\d+),m=(\d+),a=(\d+),s=(\d+)}");
            if (partMatch.Success)
            {
                var xmas = partMatch.Groups.Values.Skip(1).Select(x => int.Parse(x.Value)).ToArray();
                parts.Add(new(xmas[0], xmas[1], xmas[2], xmas[3]));
            }
            else
            {
                var ruleMatch = Regex.Match(line, @"(.+){(.+)}");
                var key = ruleMatch.Groups[1].Value;
                rules[key] = ruleMatch.Groups[2].Value.Split(',').Select(XmasRule.FromString);
            }
        }

        if (part == 1)
        {
            var accepted = new List<XmasPart>();
            foreach (var xmasPart in parts)
            {
                var workflow = "in";
                while (rules[workflow].Any())
                {
                    var firstRule = rules[workflow].First(rule => rule & xmasPart);
                    workflow = firstRule.WorkFlow;
                }
                if (workflow == "A") accepted.Add(xmasPart);
            }
            return accepted.Sum(p => p.X + p.M + p.A + p.S);
        }
        else
        {
            var accepted = new List<XmasSpace>();
            var spaces = new Queue<(string, XmasSpace)>();
            spaces.Enqueue(("in", new(1, 4001, 1, 4001, 1, 4001, 1, 4001)));
            while (spaces.TryDequeue(out var pair))
            {
                var (workflow, space) = pair;
                if (workflow == "A") accepted.Add(space);
                foreach (var rule in rules[workflow])
                {
                    var (inc, exc) = rule & space;
                    if (inc is not null) spaces.Enqueue((rule.WorkFlow, inc));
                    if (exc is null) break;
                    space = exc;
                }
            }

            return accepted.Aggregate(0L, (acc, term) => acc + term.Size);
        }
    }

    public class Module
    {
        public static int High { get; protected set; } = 0;
        public static int Low { get; protected set; } = 0;
        public Module(string line)
        {

        }
        public enum ModuleType { Broadcast, FlipFlop, Conjunction }
        public enum Pulse { High, Low }
        public ModuleType Type { get; protected set; }
        public IEnumerable<Pulse>? Remembered { get; protected set; }
        public bool IsOn { get; protected set; }

    }
    public static long Day20(int part)
    {
        return 0;
    }

    public record Plot(int X, int Y)
    {
        public static Plot operator +(Plot a, Plot b) => new(a.X + b.X, a.Y + b.Y);
    }
    public static int Day21(int part)
    {
        const int STEPS = 64;
        var start = (x: 0, y: 0);
        var garden = new List<string>();
        for (var line = Console.ReadLine(); line is not null; line = Console.ReadLine())
        {
            var s = line.IndexOf('S');
            if (s >= 0) start = (s, garden.Count);
            garden.Add(line);
        }
        var plots = new HashSet<Plot>[STEPS + 1];
        plots[0] = [new(start.x, start.y)];
        for (var steps = 0; steps < STEPS; steps++)
        {
            plots[steps + 1] = [];
            foreach (var from in plots[steps])
            {
                var nextSteps = new Plot[] { new(0, 1), new(0, -1), new(1, 0), new(-1, 0) }
                    .Select(p => p + from)
                    .Where(p => p.X >= 0 && p.Y >= 0 && p.X < garden.First().Length && p.Y < garden.Count && garden[p.Y][p.X] != '#');
                foreach (var next in nextSteps)
                {
                    plots[steps + 1].Add(next);
                }
            }
        }
        return plots[STEPS].Count;
    }

    public class SandBrick : IComparable<SandBrick>
    {
        public int X0 { get; set; }
        public int X1 { get; set; }
        public int Y0 { get; set; }
        public int Y1 { get; set; }
        public int Z0 { get; set; }
        public int Z1 { get; set; }
        public override string ToString() => $"{X0},{Y0},{Z0}~{X1},{Y1},{Z1}";
        public SandBrick(int x0, int y0, int z0, int x1, int y1, int z1)
        {
            if (x0 > x1) (x0, x1) = (x1, x0);
            if (y0 > y1) (y0, y1) = (y1, y0);
            if (z0 > z1) (z0, z1) = (z1, z0);
            (X0, Y0, Z0, X1, Y1, Z1) = (x0, y0, z0, x1, y1, z1);
        }
        public IEnumerable<(int x, int y, int z)> BottomBlocks =>
            Enumerable.Range(X0, X1 - X0 + 1)
                .SelectMany(_ => Enumerable.Range(Y0, Y1 - Y0 + 1), (a, b) => (a, b, Z0));
        public IEnumerable<(int x, int y, int z)> Blocks =>
            Enumerable.Range(X0, X1 - X0 + 1)
                .SelectMany(_ => Enumerable.Range(Y0, Y1 - Y0 + 1), (a, b) => (a, b))
                .SelectMany(_ => Enumerable.Range(Z0, Z1 - Z0 + 1), (p, c) => (p.a, p.b, c));

        public int CompareTo(SandBrick? other)
        {
            if (other is null) return 1;
            if (Z0 != other.Z0) return Z0.CompareTo(other.Z0);
            if (Y0 != other.Y0) return Y0.CompareTo(other.Y0);
            if (X0 != other.X0) return X0.CompareTo(other.X0);
            return 0;
        }
        public int FallInto(HashSet<(int x, int y, int z)> grid)
        {
            int dz;
            var xys = Enumerable.Range(X0, X1 - X0 + 1).SelectMany(_ => Enumerable.Range(Y0, Y1 - Y0 + 1), (x, y) => (x, y));
            foreach (var p in Blocks) grid.Remove(p);

            for (dz = 0; Z0 + dz > 0 && !xys.Any(p => grid.Contains((p.x, p.y, Z0 + dz))); dz--) ;
            Z1 += dz + 1;
            Z0 += dz + 1;

            foreach (var p in Blocks) grid.Add(p);

            return dz + 1;
        }
    }
    public static int Day22(int part)
    {
        var grid = new HashSet<(int x, int y, int z)>();
        var bricks = new List<SandBrick>();
        for (var line = Console.ReadLine(); line is not null; line = Console.ReadLine())
        {
            var match = Regex.Match(line, @"(\d+),(\d+),(\d+)~(\d+),(\d+),(\d+)");
            var n = match.Groups.Values.Skip(1).Select(x => int.Parse(x.Value)).ToArray();
            bricks.Add(new(n[0], n[1], n[2], n[3], n[4], n[5]));
        }
        bricks.Sort();
        foreach (var block in bricks.SelectMany(x => x.Blocks)) grid.Add(block);
        foreach (var brick in bricks) brick.FallInto(grid);

        var blockByGrid = bricks.SelectMany(x => x.Blocks.Select(y => (y, x))).ToDictionary(p => p.y, p => p.x);
        var restsOn = bricks.ToDictionary(a => a, a =>
            a.BottomBlocks.SelectMany(b => blockByGrid.ContainsKey((b.x, b.y, b.z - 1))
                ? [blockByGrid[(b.x, b.y, b.z - 1)]]
                : Enumerable.Empty<SandBrick>()).Distinct());

        if (part == 1)
        {
            var singleSupport = restsOn.Where(kvp => kvp.Value.Count() == 1).Select(kvp => kvp.Value.Single()).ToHashSet();
            return bricks.Count - singleSupport.Count;
        }

        var sum = 0;
        foreach (var b0 in bricks)
        {
            var falling = new HashSet<SandBrick>() { b0 };
            while (true)
            {
                var supportedOnlyByFalling = restsOn.Where(kvp => !falling.Contains(kvp.Key) && kvp.Value.Any() && kvp.Value.All(x => falling.Contains(x)));
                if (!supportedOnlyByFalling.Any()) break;
                foreach (var b in supportedOnlyByFalling) falling.Add(b.Key);
            }
            sum += falling.Count - 1;
        }
        return sum;
    }

    public static int Day23(int part)
    {
        var map = new List<string>();
        var slopes = new List<(int x0, int y0, int x1, int y1)>();
        var trails = new Dictionary<(int x, int y), IEnumerable<(int x, int y)>>();
        for (var line = Console.ReadLine(); line is not null; line = Console.ReadLine())
        {
            foreach (var slope in Regex.Matches(line, @"[v^<>]").ToArray())
            {
                var (x, y) = (slope.Index, map.Count);
                slopes.Add(slope.Value switch
                {
                    "^" => (x, y + 1, x, y - 1),
                    "v" => (x, y - 1, x, y + 1),
                    ">" => (x - 1, y, x + 1, y),
                    "<" => (x + 1, y, x - 1, y),
                    _ => (0, 0, 0, 0)
                });
            }
            map.Add(line);
        }
        var start = (map.First().IndexOf('.'), 0);
        var finish = (map.Last().IndexOf('.'), map.Count - 1);
        var graph = new Dictionary<(int x, int y), HashSet<(int x, int y)>>();
        foreach (var trailHead in slopes.Select(a => (a.x1, a.y1)).Append(start).Distinct())
        {
            var trail = new List<(int x, int y)>() { trailHead };
            while (true)
            {
                var (x, y) = trail.Last();
                var steps = new[] { (x: x - 1, y), (x: x + 1, y), (x, y: y - 1), (x, y: y + 1) }
                    .Where(p => p.x >= 0 && p.x < map.First().Length && p.y >= 0 && p.y < map.Count)
                    .Where(p => map[p.y][p.x] == '.' && !trail.Contains(p));
                if (!steps.Any()) break;
                trail.Add(steps.Single());
            }
            trails[trailHead] = trail;
            graph[trailHead] = slopes.Where(p => (p.x0, p.y0) == trail.Last()).Select(p => (p.x1, p.y1)).ToHashSet();
        }
        if (part == 2)
        {
            foreach (var (from, to) in graph.SelectMany(kvp => kvp.Value, (a, b) => (from: a.Key, to: b)))
            {
                graph[to].Add(from);
            }

        }

        return HikeFrom(start, [start]) - 1;

        int HikeFrom((int x, int y) head, HashSet<(int x, int y)> visitedTrails)
        {
            if (trails[head].Last() == finish) return trails[head].Count();
            var paths = graph[head].Where(x => !visitedTrails.Contains(x));
            if (!paths.Any()) return -9999;
            return trails[head].Count() + 1 + paths.Max(p => HikeFrom(p, [.. visitedTrails, p]));
        }
    }

    public class XmasGraph
    {
        protected Dictionary<string, HashSet<string>> _adjacency = [];
        protected Dictionary<string, Dictionary<string, IEnumerable<string>>>? _shortestPath = null;
        public void Add(string node0, string node1)
        {
            if (!_adjacency.ContainsKey(node0)) _adjacency[node0] = [];
            if (!_adjacency.ContainsKey(node1)) _adjacency[node1] = [];
            _adjacency[node0].Add(node1);
            _adjacency[node1].Add(node0);
        }
        public int Count => _adjacency.Count;
        public int EdgeCount => _adjacency.Sum(x => x.Value.Count) / 2;
        public IEnumerable<string> Nodes => _adjacency.Keys;
        protected void FindShortestPaths()
        {
            var open = new Queue<IEnumerable<string>>();
            _shortestPath = [];
            foreach (var node in _adjacency.Keys)
            {
                _shortestPath[node] = new() { [node] = [node] };
                open.Enqueue(_shortestPath[node][node]);
            }
            while (open.TryDequeue(out var path))
            {
                foreach (var next in _adjacency[path.Last()].Where(x => !path.Contains(x)))
                {
                    if (!_shortestPath[path.First()].ContainsKey(next))
                    {
                        _shortestPath[path.First()][next] = path.Append(next);
                        open.Enqueue(_shortestPath[path.First()][next]);
                    }
                }
            }
        }
        public IEnumerable<string> Path(string from, string to)
        {
            if (!_adjacency.ContainsKey(from)) throw new ArgumentException($"Node not present: {from}", nameof(from));
            if (!_adjacency.ContainsKey(to)) throw new ArgumentException($"Node not present: {to}", nameof(to));
            if (_shortestPath is null) FindShortestPaths();
            return _shortestPath![from][to];
        }
    }
    public static int Day25(int part)
    {
        var adj = new XmasGraph();
        for (var line = Console.ReadLine(); line is not null; line = Console.ReadLine())
        {
            var split = line.Split(": ");
            var src = split[0];
            foreach (var dest in split[1].Split(" "))
            {
                adj.Add(src, dest);
            }
        }
        var pathsThrough = adj.Nodes.ToDictionary(x => x, _ => 0);
        var everyPath = adj.Nodes.SelectMany(_ => adj.Nodes, adj.Path);
        foreach (var node in everyPath.SelectMany(x => x))
            pathsThrough[node]++;
        var ordered = pathsThrough.OrderBy(x => x.Value).Select(kvp => kvp.Key);
        var least = ordered.First();
        var most6 = ordered.TakeLast(6);
        var side = most6.ToDictionary(x => x, _ => 3);
        foreach (var node in adj.Nodes.Where(x => !most6.Contains(x)))
        {
            var path = adj.Path(least, node);
            side[node] = path.Join(most6, a => a, b => b, (a, _) => a).Any() ? 1 : 0;
        }

        return (side.Count(kvp => kvp.Value == 0) + 3) * (side.Count(kvp => kvp.Value == 1) + 3);
    }
}