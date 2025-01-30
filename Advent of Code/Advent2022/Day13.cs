namespace Advent_of_Code.Advent2022;

public class Day13(bool isPart1) : IAdventPuzzle
{
    private interface IPacket : IComparable<IPacket>
    {
        List<IPacket> AsList { get; }

        int IComparable<IPacket>.CompareTo(IPacket? other) => (this, other) switch
        {
            (_, null) => 1,
            (Int a, Int b) => a.AsInt.CompareTo(b.AsInt),
            _ => ListCompare(AsList, other.AsList)
        };
        private static int ListCompare(List<IPacket> a, List<IPacket> b)
        {
            for (int i = 0; i < a.Count || i < b.Count; i++)
            {
                if (i >= a.Count) return -1;
                if (i >= b.Count) return 1;
                if (a[i].CompareTo(b[i]) is var cmp and not 0) return cmp;
            }
            return 0;
        }
        public static bool operator <=(IPacket left, IPacket right) => left.CompareTo(right) <= 0;
        public static bool operator >=(IPacket left, IPacket right) => left.CompareTo(right) >= 0;
        public static IPacket Parse(string text)
        {
            ArgumentException.ThrowIfNullOrEmpty(text);
            if (text[0] == '[')
            {
                var list = new List<IPacket>();
                var (braceCount, prevComma) = (1, 1);
                for (int i = 1; i < text.Length; i++)
                {
                    if (text[i] is ',' or ']' && braceCount == 1)
                    {
                        var item = text[prevComma..i];
                        if (!string.IsNullOrEmpty(item))
                            list.Add(Parse(item));
                        prevComma = i + 1;
                    }
                    braceCount += text[i] switch { '[' => 1, ']' => -1, _ => 0 };
                }
                return new List(list);
            }
            return new Int(int.Parse(text));
        }

        private readonly record struct Int(int AsInt) : IPacket
        {
            public readonly List<IPacket> AsList => [this];
            public override string ToString() => AsInt.ToString();

        }
        private readonly record struct List(List<IPacket> AsList) : IPacket
        {
            public override string ToString() => $"[{string.Join(',', AsList.Select(x => x.ToString()))}]";
        }
    }

    public string Solve(InputHelper inputHelper)
    {
        var packets = inputHelper.EachLine().Where(s => !string.IsNullOrEmpty(s)).Select(IPacket.Parse).ToList();

        if (isPart1)
            return packets.Chunk(2)
                .Select((pair, i) => (cmp: pair[0] <= pair[1], i ))
                .Where(x => x.cmp)
                .Sum(x => x.i + 1) // 1-based index
                .ToString();

        var divider2 = IPacket.Parse("[[2]]");
        var divider6 = IPacket.Parse("[[6]]");
        packets.AddRange(divider2, divider6);
        packets.Sort();
        return $"{(packets.IndexOf(divider2) + 1) * (packets.IndexOf(divider6) + 1)}";
    }
}