using System.Text.RegularExpressions;

namespace Advent;

internal static class Advent2021
{
    public static int Day1(int part)
    {
        var depths = new List<int>();
        for (var line = Console.ReadLine(); line is not null; line = Console.ReadLine())
            depths.Add(int.Parse(line));

        if (part == 2)
            depths = depths.Zip(depths.Skip(1), depths.Skip(2)).Select(x => x.First + x.Second + x.Third).ToList();
        var differences = depths.Skip(1).Zip(depths, (a, b) => a - b);
        var increases = differences.Count(x => x > 0);
        return increases;
    }
    public static int Day2(int part)
    {
        var (depth, horizontal, aim) = (0, 0, 0);
        var instructionRx = new Regex(@"(\w+) (\d+)");
        for (var line = Console.ReadLine(); line is not null; line = Console.ReadLine())
        {
            var match = instructionRx.Match(line);
            var instruction = match.Groups[1].Value;
            var n = int.Parse(match.Groups[2].Value);
            if (part == 1)
            {
                switch (instruction)
                {
                    case "forward": horizontal += n; break;
                    case "down": depth += n; break;
                    case "up": depth -= n; break;
                }
            }
            else if (part == 2)
            {
                switch (instruction)
                {
                    case "forward":
                        horizontal += n;
                        depth += aim * n;
                        break;
                    case "down": aim += n; break;
                    case "up": aim -= n; break;
                }
            }
        }
        return depth * horizontal;
    }
    public static int Day3(int part)
    {
        var line = Console.ReadLine();
        var bitCounts = new int[line.Length];
        var count = 0;
        var readings = new List<string>();
        while (line is not null)
        {
            readings.Add(line);
            line.Select((c, i) => new { c, i }).ToList()
                .ForEach(x => bitCounts[x.i] += x.c == '1' ? 1 : 0);
            count++;
            line = Console.ReadLine();
        }
        if (part == 1)
        {
            var gammaString = new string(bitCounts.Select(n => n > count - n ? '0' : '1').ToArray());
            var epsilonString = new string(bitCounts.Select(n => n > count - n ? '1' : '0').ToArray());
            var gammaRate = Convert.ToInt32(gammaString, 2);
            var epsilonRate = Convert.ToInt32(epsilonString, 2);

            return gammaRate * epsilonRate;
        }
        var candidates = readings;
        var bit = 0;
        while (candidates.Count > 1 && bit < bitCounts.Length)
        {
            var oneCount = candidates.Count(cand => cand[bit] == '1');
            var mostCommonBit = oneCount >= candidates.Count - oneCount ? '1' : '0';
            candidates = candidates.Where(cand => cand[bit] == mostCommonBit).ToList();
            bit++;
        }
        var O2Rating = Convert.ToInt32(candidates.SingleOrDefault(), 2);
        candidates = readings;
        bit = 0;
        while (candidates.Count > 1 && bit < bitCounts.Length)
        {
            var oneCount = candidates.Count(cand => cand[bit] == '1');
            var leastCommonBit = oneCount < candidates.Count - oneCount ? '1' : '0';
            candidates = candidates.Where(cand => cand[bit] == leastCommonBit).ToList();
            bit++;
        }
        var CO2Rating = Convert.ToInt32(candidates.SingleOrDefault(), 2);
        return O2Rating * CO2Rating;
    }
    public static int Day4(int part)
    {
        var line = Console.ReadLine();
        var calledNumbers = line!.Split(',').Select(x => Convert.ToInt32(x));
        var boards = new List<int[,]>();
        var lookup = new Dictionary<int, List<(int board, int row, int col)>>();
        var rx = new Regex(@"(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)");
        var row = 0;
        for (line = Console.ReadLine(); line is not null; line = Console.ReadLine())
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                boards.Add(new int[5, 5]);
                row = 0;
            }
            else
            {
                var match = rx.Match(line);
                for (var col = 0; col < 5; col++)
                {
                    var n = Convert.ToInt32(match.Groups[col + 1].Value);
                    if (!lookup.ContainsKey(n))
                        lookup.Add(n, new());
                    boards[^1][row, col] = n;
                    lookup[n].Add((boards.Count - 1, row, col));
                }
                row++;
            }
        }
        bool isWinner(int[,] board)
        {
            for (int i = 0; i < 5; i++)
                if (completeRow(board, i) || completeColumn(board, i))
                    return true;
            return false;
        }
        bool completeRow(int[,] board, int row)
        {
            for (int i = 0; i < 5; i++)
                if (board[row, i] != -1)
                    return false;
            return true;
        }
        bool completeColumn(int[,] board, int col)
        {
            for (int i = 0; i < 5; i++)
                if (board[i, col] != -1)
                    return false;
            return true;
        }
        int sumOfUnmarked(int[,] board)
        {
            var sum = 0;
            for (var row = 0; row < 5; row++)
                for (var col = 0; col < 5; col++)
                    if (board[row, col] != -1)
                        sum += board[row, col];
            return sum;
        }
        int[,]? lastBoard = null;
        foreach (var call in calledNumbers)
        {
            lookup[call].ForEach(brc => boards[brc.board][brc.row, brc.col] = -1);
            if (part == 1)
            {
                var winningBoard = boards.Where(isWinner);
                if (winningBoard.Count() == 1)
                    return call * sumOfUnmarked(winningBoard.Single());
            }
            else if (part == 2)
            {
                var losingBoards = boards.Where(b => !isWinner(b));
                if (losingBoards.Count() == 1)
                    lastBoard = losingBoards.Single();
                if (!losingBoards.Any())
                    return call * sumOfUnmarked(lastBoard);
            }
        }
        return 0;
    }
    public static long Day5(int part)
    {
        var points = new Dictionary<(int x, int y), int>();
        for (var line = Console.ReadLine(); line is not null; line = Console.ReadLine())
        {
            var coords = Regex.Matches(line, @"\d+").Select(x => int.Parse(x.Value)).ToList();
            if (coords[0] == coords[2])
            {
                var (from, to) = coords[1] > coords[3] ? (coords[3], coords[1]) : (coords[1], coords[3]);
                foreach (var y in Enumerable.Range(from, to - from + 1))
                    if (points.ContainsKey((coords[0], y)))
                        points[(coords[0], y)]++;
                    else
                        points[(coords[0], y)] = 1;
            }
            else if (coords[1] == coords[3])
            {
                var (from, to) = coords[0] > coords[2] ? (coords[2], coords[0]) : (coords[0], coords[2]);
                foreach (var x in Enumerable.Range(from, to - from + 1))
                    if (points.ContainsKey((x, coords[1])))
                        points[(x, coords[1])]++;
                    else
                        points[(x, coords[1])] = 1;
            }

        }
        return points.Values.Count(x => x >= 2);
    }
    public static long Day6(int part)
    {
        var fish = Console.ReadLine().Split(',').Select(int.Parse)
            .GroupBy(x => x).ToDictionary(x => x.Key, x => x.LongCount());
        for (var day = 0; day < (part == 1 ? 80 : 256); day++)
        {
            var oldFish = fish;
            fish = new();
            foreach (var n in Enumerable.Range(1, 8))
                fish[n - 1] = oldFish.GetValueOrDefault(n, 0);
            fish[6] += oldFish.GetValueOrDefault(0, 0);
            fish[8] = oldFish.GetValueOrDefault(0, 0);
        }
        return fish.Values.Sum();
    }
    public static int Day7(int part)
    {
        var crabs = Console.ReadLine().Split(',').Select(int.Parse);
        var fuel = (int a, int b) => Math.Abs(a - b);
        if (part == 2) fuel = (int a, int b) => Math.Abs(a - b) * (Math.Abs(a - b) + 1) / 2;
        var alignment = Enumerable.Range(0, crabs.Max())
            .Select(x => crabs.Select(crab => fuel(crab, x)).Sum());
        return alignment.Min();
    }
    public static int Day8(int part)
    {
        var inputs = new List<IEnumerable<string>>();
        var outputs = new List<IEnumerable<string>>();
        for (var line = Console.ReadLine(); line is not null; line = Console.ReadLine())
        {
            var inOut = line.Split(" | ");
            inputs.Add(inOut[0].Split(' ').Select(x => new string(x.Order().ToArray())));
            outputs.Add(inOut[1].Split(' ').Select(x => new string(x.Order().ToArray())));
        }
        if (part == 1)
            return outputs.Sum(x => x.Count(y => y.Length == 2 || y.Length == 3 || y.Length == 4 || y.Length == 7));
        else
            return inputs.Zip(outputs).Select(pair => Decode(pair.Second, GetDecoder(pair.First))).Sum();

        static int Decode(IEnumerable<string> encoded, IDictionary<string, int> decoder) =>
            encoded.Select((x, i) => decoder[x] * (int)Math.Pow(10, 3 - i)).Sum();

        static IDictionary<string, int> GetDecoder(IEnumerable<string> input)
        {
            var segmentCounts = input.SelectMany(x => x)
                .GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count());
            var digit1 = input.Single(x => x.Length == 2);
            var digit4 = input.Single(x => x.Length == 4);
            var digit7 = input.Single(x => x.Length == 3);
            var a = digit7.Except(digit1).Single();
            var b = segmentCounts.Single(kv => kv.Value == 6).Key;
            var c = segmentCounts.Single(kv => kv.Value == 8 && kv.Key != a).Key;
            var d = digit4.Except(digit1).Single(x => x != b);
            var e = segmentCounts.Single(kv => kv.Value == 4).Key;
            var f = segmentCounts.Single(kv => kv.Value == 9).Key;
            var g = segmentCounts.Single(kv => kv.Value == 7 && kv.Key != d).Key;
            return new[] {
                new[] {a,b,c,e,f,g}, //     a:8*
                new[] {c,f},         // b:6     c:8*
                new[] {a,c,d,e,g},   //     d:7*
                new[] {a,c,d,f,g},   // e:4     f:9
                new[] {b,c,d,f},     //     g:7*
                new[] {a,b,d,f,g},
                new[] {a,b,d,e,f,g},
                new[] {a,c,f},
                new[] {a,b,c,d,e,f,g},
                new[] {a,b,c,d,f,g},
            }.Select((x, i) => (k: new string(x.Order().ToArray()), i))
            .ToDictionary(pair => pair.k, pair => pair.i);
        }
    }
    public static long Day10(int part)
    {
        var braces = new Regex(@"\[\]|\(\)|{}|<>");
        var lines = new List<string>();
        for (var line = Console.ReadLine(); line is not null; line = Console.ReadLine())
        {
            while (braces.IsMatch(line))
                line = braces.Replace(line, string.Empty);
            lines.Add(line);
        }
        var syntaxScore = 0L;
        var autocompleteScores = new List<long>();
        foreach (var line in lines)
        {
            var endBraces = Regex.Match(line, @"\)|\]|}|>");
            if (endBraces.Success) // corrupted
                syntaxScore += endBraces.Value switch
                {
                    ")" => 3,
                    "]" => 57,
                    "}" => 1197,
                    ">" => 25137,
                    _ => 0
                };
            else
            { // incomplete
                var acScore = 0L;
                foreach (var c in line.Reverse())
                    acScore = acScore * 5 + "x([{<".IndexOf(c);
                autocompleteScores.Add(acScore);
            }
        }

        if (part == 1)
            return syntaxScore;
        return autocompleteScores.Order().ToList()[autocompleteScores.Count / 2];
    }
    public static long Day14(int part)
    {
        var (initial, _) = (Console.ReadLine(), Console.ReadLine());
        var rules = new Dictionary<(char a, char b), char>();
        for (var line = Console.ReadLine(); line is not null; line = Console.ReadLine())
            rules[(line[0], line[1])] = line[6];

        var allPairs = rules.Values.Distinct().SelectMany(x => rules.Values.Distinct(), (a, b) => (a, b));
        var polymer = allPairs.ToDictionary(x => x, x => 0L);
        for (var i = 0; i < initial!.Length - 1; i++)
            polymer[(initial[i], initial[i + 1])]++;

        for (var step = 0; step < (part == 1 ? 10 : 40); step++)
        {
            var newPolymer = allPairs.ToDictionary(x => x, x => 0L);
            foreach (var (pair, n) in polymer)
            {
                var rule = rules[pair];
                newPolymer[(pair.a, rule)] += n;
                newPolymer[(rule, pair.b)] += n;
            }
            polymer = newPolymer;
        }
        var counts = polymer
            .GroupBy(kvp => kvp.Key.a, kvp => kvp.Value)
            .ToDictionary(x => x.Key, x => x.Sum());
        counts[initial.Last()]++; // the last char will always still be last, and uncounted
        return counts.Values.Max() - counts.Values.Min();
    }
    public static long Day20(int part)
    {
        var pixels = new HashSet<(int x, int y)>();
        var (algorithm, _) = (Console.ReadLine()!, Console.ReadLine()!);
        var y = 0;
        for (var line = Console.ReadLine(); line != null; line = Console.ReadLine())
        {
            foreach (var x in line.Select((c, i) => (c, i)))
                if (x.c == '#') pixels.Add((x.i, y));
            y++;
        }
        PrintBoard(pixels);
        for (var times = 0; times < 2; times++)
        {
            var consider = pixels.SelectMany(Neighborhood).ToHashSet();
            var oldPixels = pixels;
            pixels = new();
            foreach (var p in consider)
                if (Enhance(p, algorithm, oldPixels, times % 2 == 0) == (times % 2 == 1))
                    pixels.Add(p);
            PrintBoard(pixels);
        }

        return pixels.Count;

        static void PrintBoard(ISet<(int x, int y)> pixels)
        {
            var (minX, minY, maxX, maxY) = pixels.Aggregate(
                (minX: 99999, minY: 99999, maxX: -99999, maxY: -99999),
                (acc, p) => (acc.minX < p.x ? acc.minX : p.x, acc.minY < p.y ? acc.minY : p.y,
                    acc.maxX > p.x ? acc.maxX : p.x, acc.maxY > p.y ? acc.maxY : p.y));
            for (var y = minY; y <= maxY; y++)
            {
                for (var x = minX; x <= maxX; x++)
                    Console.Write(pixels.Contains((x, y)) ? "#" : ".");
                Console.WriteLine();
            }
        }
        static bool Enhance((int x, int y) p, string algorithm, ISet<(int, int)> points, bool invert)
        {
            var index = Neighborhood(p)
                .Select((c, i) => (points.Contains(c), (int)Math.Pow(2, 8 - i)))
                .Where(pair => pair.Item1 != invert)
                .Select(pair => pair.Item2)
                .Sum();
            return algorithm[index] == '#';
        }
        static IEnumerable<(int x, int y)> Neighborhood((int x, int y) p) =>
            new List<(int, int)>() {
                (p.x-1, p.y-1), (p.x, p.y-1), (p.x+1, p.y-1),
                (p.x-1, p.y),   (p.x, p.y),   (p.x+1, p.y),
                (p.x-1, p.y+1), (p.x, p.y+1), (p.x+1, p.y+1),
            };
    }
    internal class Die
    {
        public int Rolls { get; private set; } = 0;
        public int Roll() => Rolls++ % 100 + 1;
    }
    public static long Day21(int part)
    {
        var pos1 = int.Parse(Console.ReadLine()!.Split(' ')[4]);
        var pos2 = int.Parse(Console.ReadLine()!.Split(' ')[4]);
        var (score1, score2) = (0, 0);
        var die = new Die();

        while (true)
        {
            var advance = die.Roll() + die.Roll() + die.Roll();
            pos1 = (pos1 + advance - 1) % 10 + 1;
            score1 += pos1;
            if (score1 >= 1000)
                return score2 * die.Rolls;

            advance = die.Roll() + die.Roll() + die.Roll();
            pos2 = (pos2 + advance - 1) % 10 + 1;
            score2 += pos2;
            if (score2 >= 1000)
                return score1 * die.Rolls;
        }
    }
}
