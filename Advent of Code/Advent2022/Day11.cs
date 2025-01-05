namespace Advent_of_Code.Advent2022;

public class Day11(bool isPart1) : IAdventPuzzle
{
    private sealed record Monkey(int DivisibilityTest, int ThrowToWhenTrue, int ThrowToWhenFalse, Func<long, long> Operation, IEnumerable<long> StartingItems)
    {
        private readonly Queue<long> Items = new(StartingItems);
        public int Throw(long x) => x % DivisibilityTest == 0 ? ThrowToWhenTrue : ThrowToWhenFalse;
        public void Catch(long item) => Items.Enqueue(item);
        public long Inspected { get; private set; } = 0;
        public bool TryInspect(out long inspected)
        {
            if (Items.TryDequeue(out var item))
            {
                Inspected++;
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
                    case ["Test", var rest]: test = UglyParse(rest); break;
                    case ["If true", var rest]: throwTrue = UglyParse(rest); break;
                    case ["If false", var rest]: throwFalse = UglyParse(rest); break;
                    case ["Operation", var rest]:
                        switch (rest.Split(' '))
                        {
                            case ["new", "=", "old", "*", "old"]: op = x => x * x; break;
                            case ["new", "=", "old", "+", var s]: if (int.TryParse(s, out var m)) op = x => x + m; break;
                            case ["new", "=", "old", "*", var s]: if (int.TryParse(s, out var n)) op = x => x * n; break;
                        }
                        break;
                }
            }
            return new(test, throwTrue, throwFalse, op, items);
        }
        private static int UglyParse(string s) => int.Parse(s.Where("0123456789".Contains).ToArray());
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
                    monkeys[monkey.Throw(item)].Catch(item);

        return monkeys.Select(m => m.Inspected).Order().TakeLast(2).Aggregate((a, b) => a * b).ToString();
    }
}
