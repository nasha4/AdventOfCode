namespace Advent_of_Code.Advent2022;

public class Day11(bool isPart1) : IAdventPuzzle
{
    private readonly record struct Monkey(int DivisibilityTest, int ThrowToWhenTrue, int ThrowToWhenFalse, Func<long, long> Operation, Queue<long> Items)
    {
        public readonly int Throw(long x) => x % DivisibilityTest == 0 ? ThrowToWhenTrue : ThrowToWhenFalse;
        public readonly bool TryInspect(out long inspected)
        {
            if (Items.TryDequeue(out var item))
            {
                Inspected[this]++;
                inspected = WorryReducer(Operation(item));
                return true;
            }
            else
            {
                inspected = 0;
                return false;
            }
        }
        public static Func<long, long> WorryReducer { get; set; } = x => x / 3;

        public static Monkey Parse(IEnumerable<string> lines)
        {
            var (test, throwTrue, throwFalse, items) = (0, 0, 0, Enumerable.Empty<long>());
            Func<long, long> op = x => x;
            foreach (var line in lines)
            {
                switch (line.Trim().Split(": "))
                {
                    case ["Starting items", var rest]: items = rest.Split(", ").Select(long.Parse); break;
                    case ["Test", var rest]: test = int.Parse(rest.Split(' ')[^1]); break;
                    case ["If true", var rest]: throwTrue = int.Parse(rest.Split(' ')[^1]); break;
                    case ["If false", var rest]: throwFalse = int.Parse(rest.Split(' ')[^1]); break;
                    case ["Operation", var rest]: op = rest.Split(' ') switch
                        {
                            ["new", "=", "old", "*", "old"] => x => x * x,
                            ["new", "=", "old", "+", var s] when long.TryParse(s, out var y) => x => x + y,
                            ["new", "=", "old", "*", var s] when long.TryParse(s, out var y) => x => x * y,
                            _ => op
                        };
                        break;
                }
            }
            var monkey = new Monkey(test, throwTrue, throwFalse, op, new(items));
            Inspected[monkey] = 0;
            return monkey;
        }
        public static readonly Dictionary<Monkey, long> Inspected = [];
    }

    public string Solve(InputHelper inputHelper)
    {
        var monkeys = inputHelper.EachSection(Monkey.Parse).ToArray();

        if (!isPart1)
        {
            var testProduct = monkeys.Aggregate(1, (product, monkey) => product * monkey.DivisibilityTest);
            Monkey.WorryReducer = x => x % testProduct; // The secret!!!
        }

        for (int round = 0; round < (isPart1 ? 20 : 10000); round++)
            foreach (var monkey in monkeys)
                while (monkey.TryInspect(out var item))
                    monkeys[monkey.Throw(item)].Items.Enqueue(item);

        return Monkey.Inspected.Values.Order().TakeLast(2).Aggregate((a, b) => a * b).ToString();
    }
}