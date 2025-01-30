namespace Advent_of_Code.Advent2022;

public class Day25(bool isPart1) : IAdventPuzzle
{
    private readonly record struct Snafu(long Value)
    {
        private const string Radix = "=-012";
        public Snafu(string snafu) : this(snafu
            .Reverse()
            .Zip(powers5, (a, b) => b * (Radix.IndexOf(a) - 2))
            .Sum())
        { }
        public override string ToString()
        {
            var base5 = powers5.Reverse()
                .Aggregate((remainder: Value, digits: Enumerable.Empty<long>()),
                    (acc, term) => (acc.remainder % term, acc.digits.Append(acc.remainder / term)))
                .digits;
            // add 2 to every digit, with carry
            var (carry, digits) = base5.SkipWhile(x => x == 0).Reverse()
                .Aggregate((carry: 0, digits: Enumerable.Empty<int>()),
                    (acc, term) => ((int)((acc.carry + term + 2) / 5), acc.digits.Append((int)((acc.carry + term + 2) % 5))));
            // subtract 2 from each digit, except for final carry (if any)
            var snafu = digits.Select(x => Radix[x]);
            if (carry > 0) snafu = snafu.Append('1');
            return string.Join(string.Empty, snafu.Reverse());
        }
        public static Snafu operator +(Snafu a, Snafu b) => new(a.Value + b.Value);
        private static readonly IEnumerable<long> powers5 =
            Enumerable.Range(0, 22).Select(static n => (long)Math.Pow(5, n));
    }

    public string Solve(InputHelper inputHelper) =>
        inputHelper.EachLine(line => new Snafu(line)).Aggregate((sum, term) => sum + term).ToString();
}