namespace Advent_of_Code.Advent2020;

public class Day23(bool isPart1) : IAdventPuzzle
{
    private sealed record CrabCup<T>(T Value)
    {
        public CrabCup<T>? PrevInCircle { get; set; }
        public CrabCup<T>? NextInCircle { get; set; }
        public CrabCup<T>? PrevByValue { get; set; }
        public CrabCup<T>? NextByValue { get; set; }

        public CrabCup<T> CreateAfter(T value) => NextInCircle = NextByValue = new CrabCup<T>(value) { PrevByValue = this, PrevInCircle = this };
        public CrabCup<T> Remove()
        {
            var (before, after) = (PrevInCircle, NextInCircle);
            (PrevInCircle, NextInCircle) = (null, null);
            if (before is not null) before.NextInCircle = after;
            if (after is not null) after.PrevInCircle = before;
            return this;
        }
        public CrabCup<T> InsertAfter(CrabCup<T> newNode)
        {
            (NextInCircle, newNode.PrevInCircle, newNode.NextInCircle, NextInCircle.PrevInCircle) = (newNode, this, NextInCircle, newNode);
            return newNode;
        }
    }

    public string Solve(InputHelper inputHelper)
    {
        var cups = inputHelper.EachLine(line => line.Select(x => x - '0')).Single();
        if (!isPart1)
            cups = cups.Concat(Enumerable.Range(10, 1_000_000 - 9));
        var head = new CrabCup<int>(0);
        var cup = head;
        foreach (var n in cups)
            cup = cup.CreateAfter(n);

        var firstCup = head.NextInCircle;
        cup.NextInCircle = firstCup;
        firstCup!.PrevInCircle = cup;

        var firstNine = new CrabCup<int>[11];
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
        for (var i = 0; i < (isPart1 ? 100 : 10_000_000); i++)
        {
            var removed = new CrabCup<int>[]
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
        if (isPart1)
        {
            var seq = new List<int>();
            for (cup = head.NextByValue; seq.Count < 9; cup = cup.NextInCircle)
                seq.Add(cup.Value);
            return string.Join(string.Empty, seq.Skip(1));
        }

        return int.BigMul(head.NextByValue.NextInCircle.Value, head.NextByValue.NextInCircle.NextInCircle.Value).ToString();
    }
}