using System.Text;
using System.Text.RegularExpressions;

namespace Advent;

internal static class Advent2020
{
    public static int Day1(int part)
    {
        var expenses = new List<int>();
        for (var line = Console.ReadLine(); line is not null; line = Console.ReadLine())
            expenses.Add(int.Parse(line));

        var pairs = expenses
            .Join(expenses, a => 0, b => 0, (a, b) => new { a, b })
            .Where(x => x.a != x.b && x.a + x.b == 2020);

        var triples = expenses
            .Join(expenses, a => 0, b => 0, (a, b) => new { a, b })
            .Join(expenses, x => 0, c => 0, (x, c) => new { x.a, x.b, c })
            .Where(x => x.a + x.b + x.c == 2020);

        return part switch
        {
            1 => pairs.Take(1).Select(x => x.a * x.b).First(),
            2 => triples.Take(1).Select(x => x.a * x.b * x.c).First(),
            _ => 0
        };
    }
    public static int Day2(int part)
    {
        var validCount1 = 0;
        var validCount2 = 0;
        for (var line = Console.ReadLine(); line is not null; line = Console.ReadLine())
        {
            var rx = new Regex(@"(\d+)-(\d+) (.): (.*)");
            var match = rx.Match(line);
            if (match.Success)
            {
                var min = int.Parse(match.Groups[1].Value);
                var max = int.Parse(match.Groups[2].Value);
                var character = match.Groups[3].Value[0];
                var password = match.Groups[4].Value;
                var occurrences = password.ToCharArray()
                    .Count(x => x == character);
                if (occurrences >= min && occurrences <= max)
                    validCount1++;
                if (password[min-1] == character ^ password[max-1] == character)
                    validCount2++;
            }
        }

        return part switch
        {
            1 => validCount1,
            2 => validCount2,
            _ => 0
        };
    }
    public static int Day3(int part)
    {
        var treeRows = new List<bool[]>();
        for (var line = Console.ReadLine(); line is not null; line = Console.ReadLine())
            treeRows.Add(line.ToCharArray().Select(c => c == '#').ToArray());
        
        var slopes = new List<(int, int)>();
        if (part == 1)
            slopes.Add((3, 1));
        else if (part == 2)
        {
            slopes.AddRange(new List<(int, int)>() {
                (1,1), (3,1), (5,1), (7,1), (1,2)
            });
        }
        var treeCounts = new Dictionary<(int, int), int>();
        foreach (var slope in slopes)
        {
            var (dx, dy) = slope;
            treeCounts.Add(slope, 0);
            for (int x = 0, y = 0; y < treeRows.Count; x += dx, y += dy)
                if (treeRows[y][x % treeRows[y].Length])
                    treeCounts[slope]++;
        }

        return treeCounts.Aggregate(1, (product, trees) => product * trees.Value);
    }
    public static int Day4(int part)
    {
        var passports = new List<Dictionary<string, string>>();
        for (string? line = string.Empty; line is not null; line = Console.ReadLine())
        {
            if (string.IsNullOrWhiteSpace(line))
                passports.Add(new());
            else
                foreach (var field in line.Split(' '))
                {
                    var rx = new Regex(@"(.*):(.*)");
                    var match = rx.Match(field);
                    passports[^1].Add(match.Groups[1].Value, match.Groups[2].Value);
                }
        }

        var required = new List<string>() {
            "byr", "iyr", "eyr", "hgt", "hcl", "ecl", "pid" };
        var validators = new Dictionary<string, Predicate<string>>()
        {
            {"byr", byr => {
                var i = int.Parse(byr);
                return i >= 1920 && i <= 2002;
            } },
            {"iyr", iyr => {
                var i = int.Parse(iyr);
                return i >= 2010 && i <= 2020;
            } },
            {"eyr", eyr => {
                var i = int.Parse(eyr);
                return i >= 2020 && i <= 2030;
            } },
            {"hgt", hgt => {
                var hgtRx = new Regex(@"^(\d+)(cm|in)$");
                var match = hgtRx.Match(hgt);
                if (!match.Success) return false;
                var height = int.Parse(match.Groups[1].Value);
                var units = match.Groups[2].Value;
                return units == "cm" && height >= 150 && height <= 193
                    || units == "in" && height >=  59 && height <=  76;
            } },
            {"hcl", hcl => {
                var rx = new Regex(@"^\#[0-9a-f]{6}$");
                return rx.IsMatch(hcl);
            } },
            {"ecl", ecl => {
                var validEcls = new List<string>() {
                    "amb", "blu", "brn", "gry", "grn", "hzl", "oth" };
                return validEcls.Any(x => x == ecl);
            } },
            {"pid", pid => { 
                var rx = new Regex(@"^[0-9]{9}$");
                return rx.IsMatch(pid);
            } },
            {"cid", cid => true }
        };

        var valid1 = passports.Where(x => required.All(r => x.ContainsKey(r)));
        var valid2 = valid1.Where(x => x.All(kv => validators[kv.Key](kv.Value)));

        return part switch
        {
            1 => valid1.Count(),
            2 => valid2.Count(),
            _ => 0
        };
    }
    public static int Day5(int part)
    {
        var seats = new List<(int row, int col)>();
        for (var line = Console.ReadLine(); line is not null; line = Console.ReadLine())
        {
            int row = 0, col = 0;
            foreach (var c in line) {
                switch (c)
                {
                    case 'B': row *= 2; row++; break;
                    case 'F': row *= 2;        break;
                    case 'R': col *= 2; col++; break;
                    case 'L': col *= 2;        break;
                }
            }
            seats.Add((row, col));
        }
        var seatCodes = seats.Select(s => s.row * 8 + s.col);

        return part switch
        {
            1 => seatCodes.Max(),
            2 => Enumerable.Range(0, seatCodes.Max() + 1)
                .Where(x => !seatCodes.Any(sc => sc == x))
                .Max(),
            _ => 0
        };
    }
    public static int Day6(int part)
    {
        var groups = new List<Dictionary<char, int>>();
        for (string? line = string.Empty; line is not null; line = Console.ReadLine())
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                groups.Add(new() { { '*', 0 } }); // initialize size ('*' key) to 0
            }
            else
            {
                groups[^1]['*']++; // increment size of group
                foreach (var c in line)
                {
                    if (groups[^1].ContainsKey(c))
                        groups[^1][c]++;
                    else
                        groups[^1][c] = 1;
                }
            }
        }

        return part switch
        {
            1 => groups.Select(g => g.Count - 1).Sum(),
            2 => groups.Select(g => g.Count(kv => kv.Value == g['*']) - 1).Sum(),
            _ => 0
        };
    }
    public static int Day7(int part)
    {
        var canBeContainedBy = new Dictionary<string, List<string>>();
        var canContain = new Dictionary<string, List<(int n, string bag)>>();
        var containRx = new Regex(@"^(.+) bags contain (.+)\.$");
        var nBagsRx = new Regex(@"^(\d+) (.+) bags?$");
        for (var line = Console.ReadLine(); line is not null; line = Console.ReadLine())
        {
            var match = containRx.Match(line);
            var container = match.Groups[1].Value;
            canContain[container] = new();
            foreach (var content in match.Groups[2].Value.Split(", "))
            {
                var match2 = nBagsRx.Match(content);
                if (!match2.Success) continue; // "no other bags"
                var nContents = int.Parse(match2.Groups[1].Value);
                var contents = match2.Groups[2].Value;
                canContain[container].Add((nContents, contents));
                if (!canBeContainedBy.ContainsKey(contents))
                    canBeContainedBy[contents] = new();
                canBeContainedBy[contents].Add(container);
            }
        }

        ISet<string> traverseUp(string node)
        {
            var rv = new HashSet<string>();
            if (canBeContainedBy.ContainsKey(node))
            {
                canBeContainedBy[node].ForEach(s => rv.Add(s));
                foreach (var child in canBeContainedBy[node])
                    rv.UnionWith(traverseUp(child));
            }
            return rv;
        }
        int traverseDown(string node)
        {
            var rv = canContain[node].Select(x => x.n).Sum();
            canContain[node].ForEach(x => rv += x.n * traverseDown(x.bag));
            return rv;
        }
        var containers = traverseUp("shiny gold");
        var totalContents = traverseDown("shiny gold");

        return part switch
        {
            1 => containers.Count,
            2 => totalContents,
            _ => 0
        };
    }
    public static int Day8(int part)
    {
        var rx = new Regex(@"^(acc|nop|jmp) \+?(-?\d+)$");
        var instructions = new List<(string cmd, int n)>();
        for (var line = Console.ReadLine(); line is not null; line = Console.ReadLine())
        {
            var match = rx.Match(line);
            instructions.Add((match.Groups[1].Value, int.Parse(match.Groups[2].Value)));
        }

        static (bool terminated, int acc) runProgram(List<(string, int)> instructions)
        {
            int acc = 0, ip = 0;
            var visited = new HashSet<int>();
            while (ip < instructions.Count && !visited.Contains(ip))
            {
                var (cmd, n) = instructions[ip];
                visited.Add(ip);
                switch (cmd)
                {
                    case "acc": acc += n; ip++; break;
                    case "jmp": ip += n; break;
                    case "nop": ip++; break;
                }
            }
            return (ip >= instructions.Count, acc);
        }

        if (part == 1)
        {
            var (_, acc) = runProgram(instructions);
            return acc;
        }

        for (var i = 0; i < instructions.Count; i++)
        {
            var (cmd, n) = instructions[i];
            var patched = new List<(string, int)>(instructions);
            switch(cmd)
            {
                case "acc": continue;
                case "nop": patched[i] = ("jmp", n); break;
                case "jmp": patched[i] = ("nop", n); break;
            }
            var (terminated, acc) = runProgram(patched);
            if (terminated) return acc;
        }
        return 0;
    }
    public static long Day9(int part)
    {
        var codes = new List<long>();
        for (var line = Console.ReadLine(); line is not null; line = Console.ReadLine())
            codes.Add(long.Parse(line));

        var preamble = 25;
        long weakness = 0;
        for (var i = preamble; i < codes.Count; i++)
        {
            var addends = new List<long>(codes.Skip(i - preamble).Take(preamble));
            var pairSums = addends
                .SelectMany(x => addends, (a, b) => new { a, b })
                .Where(x => x.a != x.b)
                .Select(x => x.a + x.b);
            if (!pairSums.Any(s => s == codes[i]))
            {
                weakness = codes[i];
                break;
            }
        }
        if (part == 1) return weakness;

        for (var skip = 0; skip < codes.Count; skip++)
        {
            for (var take = 2; take < codes.Count; take++)
            {
                var seq = codes.Skip(skip).Take(take);
                if (seq.Sum() == weakness)
                    return seq.Max() + seq.Min();
                else if (seq.Sum() > weakness)
                    take = codes.Count;
            }
        }

        return -1;
    }
    public static long Day10(int part)
    {
        var jolts = new HashSet<int>() { 0 };
        for (var line = Console.ReadLine(); line is not null; line = Console.ReadLine())
            jolts.Add(int.Parse(line));
        var maxJoltage = jolts.Max() + 3;
        jolts.Add(maxJoltage);

        if (part == 1)
        {
            var sortedJolts = jolts.Order().ToList();
            int[] gapCount = { 0, 0, 0, 0 };
            for (var i = 1; i < sortedJolts.Count; i++)
                gapCount[sortedJolts[i] - sortedJolts[i - 1]]++;

            return gapCount[1] * gapCount[3];
        }

        var pathsFromMemo = new Dictionary<int, long>() { { maxJoltage, 1 } };
        long pathsFrom(int joltage)
        {
            if (pathsFromMemo.TryGetValue(joltage, out var paths))
                return paths;
            else if (!jolts.Contains(joltage))
                return pathsFromMemo[joltage] = 0;
            else
                return pathsFromMemo[joltage] =
                    pathsFrom(joltage + 1) +
                    pathsFrom(joltage + 2) +
                    pathsFrom(joltage + 3);
        }

        return pathsFrom(0);
    }
    public static int Day11(int part)
    {
        static char NextSeat(char[,] board, int x, int y)
        {
            if (board[x, y] == '.') return '.';
            var adj = new List<int>() { -1, 0, 1 };
            var occupiedAdj = 0;
            var neighbors = adj.SelectMany(n => adj, (x, y) => (x, y))
                .Where(d => d.x != 0 || d.y != 0)
                .Where(d => x + d.x >= 0 && x + d.x < board.GetLength(0))
                .Where(d => y + d.y >= 0 && y + d.y < board.GetLength(1));
            foreach (var d in neighbors)
                if (board[x + d.x, y + d.y] == '#')
                    occupiedAdj++;
            if (board[x, y] == 'L' && occupiedAdj == 0) return '#';
            if (board[x, y] == '#' && occupiedAdj >= 4) return 'L';
            return board[x, y];
        }
        var lines = new List<string>();
        for (var line = Console.ReadLine(); line is not null; line = Console.ReadLine())
            lines.Add(line);

        var (xSize, ySize) = (lines[0].Length, lines.Count);
        var board = new char[xSize, ySize];
        for (var y = 0; y < ySize; y++)
            for (var x = 0; x < xSize; x++)
                board[x, y] = lines[y][x];
        var changed = true;
        while (changed)
        {
            var newBoard = new char[xSize, ySize];
            changed = false;
            for (var y = 0; y < ySize; y++)
                for (var x = 0; x < xSize; x++) {
                    newBoard[x, y] = NextSeat(board, x, y);
                    changed = changed || newBoard[x, y] != board[x, y];
                }
            board = newBoard;
        }
        var occupied = 0;
        for (var y = 0; y < ySize; y++)
            for (var x = 0; x < xSize; x++)
                if (board[x, y] == '#')
                    occupied++;

        return occupied;
    }
    public static int Day12(int part)
    {
        var (facing, x, y, wpx, wpy) = (0, 0, 0, 10, 1);
        var cmdRx = new Regex(@"([NSEWLRF])(\d+)");
        for (var line = Console.ReadLine(); line is not null; line = Console.ReadLine())
        {
            var match = cmdRx.Match(line);
            var (cmd, val) = (match.Groups[1].Value, int.Parse(match.Groups[2].Value));
            if (cmd == "R") // convert cw rotation to ccw
                (cmd, val) = ("L", 360 - val);
            if (part == 1)
            {
                switch (cmd)
                {
                    case "E": x += val; break;
                    case "N": y += val; break;
                    case "W": x -= val; break;
                    case "S": y -= val; break;
                    case "L": facing = (facing + val) % 360; break;
                    case "F":
                        switch (facing)
                        {
                            case 0: x += val; break;
                            case 90: y += val; break;
                            case 180: x -= val; break;
                            case 270: y -= val; break;
                        }
                        break;
                }
            }
            else if (part == 2)
            {
                switch (cmd)
                {
                    case "E": wpx += val; break;
                    case "N": wpy += val; break;
                    case "W": wpx -= val; break;
                    case "S": wpy -= val; break;
                    case "F":
                        x += wpx * val;
                        y += wpy * val;
                        break;
                    case "L":
                        switch (val)
                        {
                            case  90: (wpx, wpy) = (-wpy, wpx); break;
                            case 180: (wpx, wpy) = (-wpx, -wpy); break;
                            case 270: (wpx, wpy) = (wpy, -wpx); break;
                        }
                        break;
                }
            }
        }

        return Math.Abs(x) + Math.Abs(y);
    }
    public static int Day13(int part)
    {
        var busTimes = new HashSet<int>();
        var line = Console.ReadLine();
        var startTime = int.Parse(line!);
        line = Console.ReadLine();
        foreach (var bus in line!.Split(','))
        {
            if (bus == "x") continue;
            busTimes.Add(int.Parse(bus));
        }
        if (part == 1)
        {
            var bestBus = busTimes.MinBy(x => x - startTime % x);
            return bestBus * (bestBus - startTime % bestBus);
        }
        return 0;
    }
    public static long Day14(int part)
    {
        var memory = new Dictionary<long, long>();
        long andMask = -1, orMask = 0;
        var xMask = new HashSet<int>();
        var assignment = new Regex(@"mem\[(\d+)\] = (\d+)");
        for (var line = Console.ReadLine(); line is not null; line = Console.ReadLine())
        {
            if (line[1] == 'a') // mask = XXXXXXX...
            {
                var andMaskString = new string('1', 36).ToCharArray();
                var orMaskString = new string('0', 36).ToCharArray();
                xMask.Clear();
                var mask = line.Split(' ')[2];
                mask.Select((c, i) => new { c, i }).ToList().ForEach(x => {
                    if (x.c == '1') orMaskString[x.i] = '1';
                    if (x.c == '0') andMaskString[x.i] = '0';
                    if (x.c == 'X') xMask.Add(x.i);
                });
                andMask = Convert.ToInt64(new string(andMaskString), 2);
                orMask = Convert.ToInt64(new string(orMaskString), 2);
            }
            else // mem[n] = value
            {
                var match = assignment.Match(line);
                var address = long.Parse(match.Groups[1].Value);
                var value = long.Parse(match.Groups[2].Value);
                if (part == 1)
                {
                    memory[address] = value & andMask | orMask;
                }
                else if (part == 2)
                {
                    var firstAddress = Convert.ToString(address | orMask, 2).PadLeft(36, '0');
                    var addresses = new HashSet<char[]>() { firstAddress.ToCharArray() };
                    foreach (var bit in xMask)
                    {
                        var tempAddresses = new HashSet<char[]>(addresses);
                        foreach (var a in tempAddresses)
                        {
                            for (var n = '0'; n <= '1'; n++)
                            {
                                var x = a.ToArray();
                                x[bit] = n;
                                addresses.Add(x);
                            }
                        }
                    }
                    foreach (var a in addresses)
                        memory[Convert.ToInt64(new string(a), 2)] = value;
                }
            }
        }

        return memory.Values.Sum();
    }
    public static int Day15(int part)
    {
        var lastSaid = new Dictionary<int, int>();
        var (turn, previousNumber, number) = (1, -1, -1);
        foreach (var starterNumber in Console.ReadLine()!.Split(','))
        {
            previousNumber = number;
            number = int.Parse(starterNumber);
            lastSaid[previousNumber] = turn++;
        }
        var maxTurns = part == 1 ? 2020 : 30000000;
        while (turn <= maxTurns)
        {
            previousNumber = number;
            if (lastSaid.TryGetValue(number, out var last))
                number = turn - last;
            else
                number = 0;
            lastSaid[previousNumber] = turn++;
        }
        return number;
    }
    public static int Day17Part1()
    {
        var board = new HashSet<(int, int, int)>();
        var row = 0;
        for (var line = Console.ReadLine(); line is not null; line = Console.ReadLine())
        {
            for (var col = 0; col < line.Length; col++)
                if (line[col] == '#')
                    board.Add((col, row, 0));
            row++;
        }
        static HashSet<(int, int, int)> Iterate3d(HashSet<(int, int, int)> board)
        {
            static (int, int, int) TripleAdd((int, int, int) a, (int, int, int) b) =>
                (a.Item1 + b.Item1, a.Item2 + b.Item2, a.Item3 + b.Item3);
            var newBoard = new HashSet<(int, int, int)>();
            var plusMinus = Enumerable.Range(-1, 3);
            var dxyz = plusMinus
                .SelectMany(_ => plusMinus, (a, b) => (a, b))
                .SelectMany(_ => plusMinus, (t, c) => (dx: t.a, dy: t.b, dz: c));
            var dNeighbors = dxyz.Where(t => t != (0, 0, 0));
            foreach (var activeCell in board)
            {
                // Consider every cell adjacent to an active cell, maybe multiple times
                var localCube = dxyz.Select(d => TripleAdd(activeCell, d));
                foreach (var cell in localCube)
                {
                    var neighbors = dNeighbors.Select(d => TripleAdd(cell, d));
                    var activeNeighbors = neighbors.Count(board.Contains);
                    if (board.Contains(cell) && (activeNeighbors == 2 || activeNeighbors == 3)
                        || activeNeighbors == 3)
                        newBoard.Add(cell);
                }
            }
            return newBoard;
        }
        board = Iterate3d(board);
        board = Iterate3d(board);
        board = Iterate3d(board);
        board = Iterate3d(board);
        board = Iterate3d(board);
        board = Iterate3d(board);

        return board.Count;
    }
    public static int Day17Part2()
    {
        var board = new HashSet<(int, int, int, int)>();
        var row = 0;
        for (var line = Console.ReadLine(); line is not null; line = Console.ReadLine())
        {
            for (var col = 0; col < line.Length; col++)
                if (line[col] == '#')
                    board.Add((0, col, row, 0));
            row++;
        }
        static HashSet<(int, int, int, int)> Iterate4d(HashSet<(int, int, int, int)> board)
        {
            static (int, int, int, int) QuadAdd((int, int, int, int) a, (int, int, int, int) b) =>
                (a.Item1 + b.Item1, a.Item2 + b.Item2, a.Item3 + b.Item3, a.Item4 + b.Item4);
            var newBoard = new HashSet<(int, int, int, int)>();
            var plusMinus = Enumerable.Range(-1, 3);
            var dwxyz = plusMinus
                .SelectMany(_ => plusMinus, (a, b) => (a, b))
                .SelectMany(_ => plusMinus, (t, c) => (t.a, t.b, c))
                .SelectMany(_ => plusMinus, (t, d) => (t.a, t.b, t.c, d));
            var dNeighbors = dwxyz.Where(t => t != (0, 0, 0, 0));
            foreach (var activeCell in board)
            {
                // Consider every cell adjacent to an active cell, maybe multiple times
                var localCube = dwxyz.Select(d => QuadAdd(activeCell, d));
                foreach (var cell in localCube)
                {
                    var neighbors = dNeighbors.Select(d => QuadAdd(cell, d));
                    var activeNeighbors = neighbors.Count(board.Contains);
                    if (board.Contains(cell) && (activeNeighbors == 2 || activeNeighbors == 3)
                        || activeNeighbors == 3)
                        newBoard.Add(cell);
                }
            }
            return newBoard;
        }
        board = Iterate4d(board);
        board = Iterate4d(board);
        board = Iterate4d(board);
        board = Iterate4d(board);
        board = Iterate4d(board);
        board = Iterate4d(board);

        return board.Count;
    }
    public static long Day18(int part)
    {
        static long EvalSimple(string expression)
        {
            var tokens = expression.Split(' ');
            var total = long.Parse(tokens[0]);
            for (var i = 1; i < tokens.Length; i += 2)
            {
                var n = long.Parse(tokens[i + 1]);
                if (tokens[i] == "*") total *= n;
                if (tokens[i] == "+") total += n;
            }
            return total;
        }
        static long EvalPlusFirst(string expression)
        {
            var addition = new Regex(@"\d+ \+ \d+");
            var simplified = expression;
            while (simplified.Contains('+'))
                simplified = addition.Replace(simplified, x =>
                    EvalSimple(x.Groups[0].Value).ToString());
            return EvalSimple(simplified);
        }
        long sum = 0;
        for (var line = Console.ReadLine(); line is not null; line = Console.ReadLine())
        {
            Func<string, long> Evaluator = part == 1 ? EvalSimple : EvalPlusFirst;
            var parenExpression = new Regex(@"\(([0-9 *+]+)\)");
            var simplified = line;
            while (simplified.Contains('('))
                simplified = parenExpression.Replace(simplified, x =>
                    Evaluator(x.Groups[1].Value).ToString());
            sum += Evaluator(simplified);
        }
        return sum;
    }
    public static long Day20(int part)
    {
        static bool[][] GetTileBorders()
        {
            var line = Console.ReadLine();
            var (borderE, borderW) = (new bool[10], new bool[10]);
            var borderN = line.Select(x => x == '#').ToArray();
            borderE[0] = line.Last() == '#';
            borderW[0] = line.First() == '#';
            for (var i = 1; i < 9; i++)
            {
                line = Console.ReadLine();
                borderE[i] = line.Last() == '#';
                borderW[i] = line.First() == '#';
            }
            line = Console.ReadLine();
            var borderS = line.Select(x => x == '#').ToArray();
            borderE[9] = line.Last() == '#';
            borderW[9] = line.First() == '#';
            var borders = new bool[][] { borderN, borderE, borderS, borderW };
            var flippedBorders = borders.Select(x => x.Reverse().ToArray()).ToArray();
            return borders.Concat(flippedBorders).ToArray();
        }
        static void AddTilesByBorder(Dictionary<string, List<(int, int)>> tilesByBorder, int tileNumber, bool[][] borders)
        {
            for (var i = 0; i < 8; i++)
            {
                var key = KeyOf(borders[i]);
                if (!tilesByBorder.ContainsKey(key))
                    tilesByBorder[key] = new();
                tilesByBorder[key].Add((tileNumber, i));
            }
        }
        static string KeyOf(bool[] border) =>
            string.Join(string.Empty, border.Select(x => x ? '.' : '#'));

        var bordersByTile = new Dictionary<int, List<bool[]>>();
        var tilesByBorder = new Dictionary<string, List<(int tile, int randr)>>();
        var newTilePattern = new Regex(@"Tile (\d+):");
        for (var line = Console.ReadLine(); line is not null; line = Console.ReadLine())
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            var match = newTilePattern.Match(line);
            if (!match.Success) continue;
            var tileNumber = int.Parse(match.Groups[1].Value);
            var allBorders = GetTileBorders();
            bordersByTile[tileNumber] = new(allBorders);
            AddTilesByBorder(tilesByBorder, tileNumber, allBorders);
        }
        // find a corner piece to start from.  corners have exactly 4 borders with 2 tiles, I hope
        var corners = bordersByTile
            .Where(kv =>
                kv.Value.Count(border => tilesByBorder[KeyOf(border)].Count == 2) == 4)
            .Select(kv => kv.Key);
        return corners.Aggregate(1, (product, corner) => product * corner);
    }
    public static long Day21(int part)
    {
        var allFoods = new List<(List<string> ingredients, List<string> allergens)>();
        var allAllergens = new HashSet<string>();
        var allIngredients = new HashSet<string>();
        var allergenPattern = new Regex(@" \(contains (.+)\)");
        for (var line = Console.ReadLine(); line is not null; line = Console.ReadLine())
        {
            var match = allergenPattern.Match(line);
            var allergens = match.Groups[1].Value.Split(", ");
            var ingredients = allergenPattern.Replace(line, string.Empty).Split(' ');
            allAllergens = allAllergens.Union(allergens).ToHashSet();
            allIngredients = allIngredients.Union(ingredients).ToHashSet();
            allFoods.Add((new(ingredients), new(allergens)));
        }
        var couldBeIn = allAllergens.ToDictionary(x => x, x => new HashSet<string>(allIngredients));
        foreach (var (ingredients, allergens) in allFoods)
        {
            var missingIngredients = new HashSet<string>(allIngredients);
            missingIngredients.ExceptWith(ingredients);
            foreach (var ingredient in missingIngredients)
                foreach (var allergen in allergens)
                    couldBeIn[allergen].Remove(ingredient);
        }
        if (part == 1)
        {
            var count = 0;
            var couldBeAllergens = couldBeIn.SelectMany(kv => kv.Value);
            var allergenFree = allIngredients.Where(x => !couldBeAllergens.Contains(x));
            foreach (var (ingredients, allergens) in allFoods)
            {
                count += ingredients.Count(x => allergenFree.Contains(x));
            }
            return count;
        }
        while (couldBeIn.Any(kv => kv.Value.Count > 1))
        {
            var canEliminate = couldBeIn
                .Where(kv => kv.Value.Count == 1)
                .Select(kv => kv.Value.Single());
            foreach (var (allergen, ingredients) in couldBeIn)
            {
                if (ingredients.Count == 1) continue;
                canEliminate.ToList().ForEach(x => ingredients.Remove(x));
            }

        }

        Console.WriteLine(string.Join(',',
            couldBeIn.OrderBy(kv => kv.Key).Select(kv => kv.Value.Single())
        ));

        return 0;
    }
    public static int Day22(int part)
    {
        var decks = new List<int>[2] { new(), new() };
        var deck = 0;
        for (var line = Console.ReadLine(); line is not null; line = Console.ReadLine())
        {
            if (string.IsNullOrWhiteSpace(line))
                deck++;
            else if (line[0] == 'P') // "Player n:"
                continue;
            else
                decks[deck].Add(int.Parse(line));
        }
        if (part == 1)
        {
            while (decks[0].Any() && decks[1].Any())
            {
                var played = new int[2] { decks[0][0], decks[1][0] };
                decks[0].Remove(decks[0][0]);
                decks[1].Remove(decks[1][0]);
                if (played[0] > played[1])
                    decks[0].AddRange(played.OrderDescending());
                else
                    decks[1].AddRange(played.OrderDescending());
            }

            return decks.Select(deck => // compute both players' scores, one of them will have 0.
                deck.AsEnumerable().Reverse().Select((card, i) => card * (i + 1)).Sum()
            ).Max();
        }
        else if (part == 2)
        {
            static int WinnerOfSubGame(List<int> deck1, List<int> deck2)
            {
                var seenBefore = new HashSet<string>();
                while (deck1.Any() && deck2.Any()) {
                    var roundWinner = 0; // 0 is player 1

                    var deckState = new StringBuilder();
                    deckState.Append(string.Join(' ', deck1));
                    deckState.Append('|');
                    deckState.Append(string.Join(' ', deck2));
                    if (seenBefore.Contains(deckState.ToString())) return 0;
                    seenBefore.Add(deckState.ToString());

                    var played = new int[2] { deck1[0], deck2[0] };
                    deck1.Remove(deck1[0]);
                    deck2.Remove(deck2[0]);
                    if (deck1.Count >= played[0] && deck2.Count >= played[1])
                    {
                        var subdeck1 = new List<int>(deck1.Take(played[0]));
                        var subdeck2 = new List<int>(deck2.Take(played[1]));
                        roundWinner = WinnerOfSubGame(subdeck1, subdeck2);
                    }
                    else
                    {
                        roundWinner = played[0] > played[1] ? 0 : 1;
                    }

                    if (roundWinner == 0)
                        deck1.AddRange(played);
                    else
                        deck2.AddRange(played.Reverse());
                }
                return deck1.Any() ? 0 : 1;
            }
            var winner = WinnerOfSubGame(decks[0], decks[1]);
            return decks[winner].AsEnumerable().Reverse().Select((card, i) => card * (i + 1)).Sum();
        }
        return 0;
    }
    class CupNode<T>
    {
        public T Value { get; }
        public CupNode<T>? PrevInCircle { get; set; }
        public CupNode<T>? NextInCircle { get; set; }
        public CupNode<T>? PrevByValue { get; set; }
        public CupNode<T>? NextByValue { get; set; }
        public CupNode(T value) => Value = value;
        public CupNode<T> CreateAfter(T value) =>
            NextInCircle = NextByValue = new CupNode<T>(value) { PrevByValue = this, PrevInCircle = this };
        public CupNode<T> Remove()
        {
            var (before, after) = (PrevInCircle, NextInCircle);
            (PrevInCircle, NextInCircle) = (null, null);
            if (before is not null)
                before.NextInCircle = after;
            if (after is not null)
                after.PrevInCircle = before;
            return this;
        }
        public CupNode<T> InsertAfter(CupNode<T> newNode)
        {
            // this <=> this.next
            // this <=> node <=> this.next
            var next = NextInCircle;
            NextInCircle = newNode;
            newNode.NextInCircle = next;
            next.PrevInCircle = newNode;
            newNode.PrevInCircle = this;
            return newNode;
        }
    }
    public static string Day23(int part)
    {
        var test = false;
        var cupsList = (test ? "389125467" : "219748365").Select(x => x - '0');
        if (part == 2)
            cupsList = cupsList.Concat(Enumerable.Range(10, 1000000 - 9));
        var head = new CupNode<int>(0);
        var cup = head;
        foreach (var n in cupsList)
            cup = cup.CreateAfter(n);

        var firstCup = head.NextInCircle;
        cup.NextInCircle = firstCup;
        firstCup!.PrevInCircle = cup;

        var firstNine = new CupNode<int>[11];
        cup = firstCup.PrevInCircle;
        firstNine[0] = cup;
        for (var i = 1; i < 11; i++)
        {
            cup = cup!.NextInCircle!;
            firstNine[cup.Value] = cup;
        }
        head.NextByValue = firstNine[1];
        firstNine[10] ??= firstNine[1];
        for (var i = 1; i < 10; i++)
        {
            cup = firstNine[i];
            cup.PrevByValue = firstNine[cup.Value - 1];
            cup.NextByValue = firstNine[cup.Value + 1];
            firstNine[cup.Value - 1].NextByValue = cup;
            firstNine[cup.Value + 1].PrevByValue = cup;
        }

        var currentCup = head.NextInCircle;
        for (var i = 0; i < (part == 1 ? 100 : 10_000_000); i++)
        {
            var removed = new CupNode<int>[]
            {
                currentCup.NextInCircle.Remove(),
                currentCup.NextInCircle.Remove(),
                currentCup.NextInCircle.Remove()
            };
            var destinationCup = currentCup.PrevByValue;
            while (destinationCup.NextInCircle is null)
                destinationCup = destinationCup.PrevByValue;
            destinationCup
                .InsertAfter(removed[0])
                .InsertAfter(removed[1])
                .InsertAfter(removed[2]);
            currentCup = currentCup.NextInCircle;
        }
        if (part == 1)
        {
            var seq = new List<int>();
            for (cup = head.NextByValue; seq.Count < 9; cup = cup.NextInCircle)
                seq.Add(cup.Value);
            return string.Join(string.Empty, seq.Skip(1));
        }

        return ((long)head.NextByValue.NextInCircle.Value *
            head.NextByValue.NextInCircle.NextInCircle.Value)
            .ToString();
    }
    public static int Day24(int part)
    {
        static (int, int) Add((int x, int y) a, (int x, int y) b) => (a.x + b.x, a.y + b.y);
        static (int, int) HexMove((int x, int y) tile, string move) => Add(tile, move switch
        {
            "e" => (1, 0), "w" => (-1, 0),
            "se" => (0, 1), "nw" => (0, -1),
            "sw" => (-1, 1), "ne" => (1, -1),
            _ => (0, 0)
        });
        static IEnumerable<(int, int)> HexNeighbors((int, int) tile) =>
            "e w ne nw se sw".Split(' ').Select(move => HexMove(tile, move));

        var blackTiles = new HashSet<(int, int)>();
        var stepPattern = new Regex(@"e|w|se|sw|ne|nw");
        for (var line = Console.ReadLine(); line is not null; line = Console.ReadLine())
        {
            var tile = (0, 0);
            foreach (Match match in stepPattern.Matches(line))
                tile = HexMove(tile, match.Value);
            if (blackTiles.Contains(tile))
                blackTiles.Remove(tile);
            else
                blackTiles.Add(tile);
        }

        if (part == 2)
            for (var n = 0; n < 100; n++)
                blackTiles = LivingArt(blackTiles);

        return blackTiles.Count;

        static HashSet<(int, int)> LivingArt(HashSet<(int, int)> wasBlack)
        {
            var blackNeighborCount = new Dictionary<(int, int), int>();
            foreach (var blackTile in wasBlack)
            {
                if (!blackNeighborCount.ContainsKey(blackTile))
                    blackNeighborCount[blackTile] = 0;
                foreach (var neighbor in HexNeighbors(blackTile))
                {
                    if (!blackNeighborCount.ContainsKey(neighbor))
                        blackNeighborCount[neighbor] = 1;
                    else
                        blackNeighborCount[neighbor]++;
                }
            }

            return blackNeighborCount
                .Where(kv => kv.Value == 2 || kv.Value == 1 && wasBlack.Contains(kv.Key))
                .Select(kv => kv.Key)
                .ToHashSet();
        }
    }
}
