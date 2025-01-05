namespace Advent_of_Code.Advent2022;

public class Day13(bool isPart1) : IAdventPuzzle
{
    private sealed class Packet
    {
        private Packet(int asInt) => (AsInt, AsList, IsInt) = (asInt, [this], true);
        private Packet(List<Packet> asList) => (AsInt, AsList, IsInt) = (0, asList, false);
        private readonly int AsInt;
        private readonly List<Packet> AsList;
        private readonly bool IsInt;
        public override string ToString() => IsInt ? AsInt.ToString() : $"[{string.Join(',', AsList.Select(x => x.ToString()))}]";

        public static Packet Parse(string text)
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
                            list.Add(Parse(item));
                        prevComma = i + 1;
                    }
                    if (text[i] == '[') braceCount++;
                    if (text[i] == ']') braceCount--;
                }
                return new(list);
            }
            else
            {
                return new(int.Parse(text));
            }
        }

        public static bool operator <=(Packet left, Packet right) => Comparer.Compare(left, right) <= 0;
        public static bool operator >=(Packet left, Packet right) => Comparer.Compare(left, right) >= 0;

        public static IComparer<Packet> Comparer { get; } = new PacketComparer();

        private sealed class PacketComparer : IComparer<Packet>
        {
            public int Compare(Packet? x, Packet? y)
            {
                switch (x,y) {
                    case (null, null): return 0;
                    case (null, _): return -1;
                    case (_, null): return 1;
                    case (_,_) when x.IsInt && y.IsInt: return x.AsInt.CompareTo(y.AsInt);
                    default:
                        var (lList, rList) = (x.AsList, y.AsList);
                        for (int i = 0; i < lList.Count || i < rList.Count; i++)
                        {
                            if (i >= lList.Count) return -1;
                            if (i >= rList.Count) return 1;
                            var cmp = Compare(lList[i], rList[i]);
                            if (cmp != 0) return cmp;
                        }
                        return 0;
                }
            }
        }
    }

    public string Solve(InputHelper inputHelper)
    {
        var packets = inputHelper.EachLine().Where(s => !string.IsNullOrEmpty(s)).Select(line => Packet.Parse(line)).ToList();

        if (isPart1)
            return packets.Chunk(2)
                .Select((pair, i) => (cmp: pair[0] <= pair[1], i ))
                .Where(x => x.cmp)
                .Sum(x => x.i + 1) // 1-based index
                .ToString();

        var divider2 = Packet.Parse("[[2]]");
        var divider6 = Packet.Parse("[[6]]");
        packets.AddRange(divider2, divider6);
        packets.Sort(Packet.Comparer);
        return $"{(packets.IndexOf(divider2) + 1) * (packets.IndexOf(divider6) + 1)}";
    }
}
