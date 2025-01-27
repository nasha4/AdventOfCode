namespace Advent_of_Code.Advent2022;

public class Day13(bool isPart1) : IAdventPuzzle
{
    private readonly record struct Packet : IComparable<Packet>
    {
        private Packet(int asInt) => (AsInt, List, IsInt) = (asInt, null, true);
        private Packet(List<Packet> asList) => (AsInt, List, IsInt) = (0, asList, false);
        private readonly int AsInt;
        private readonly List<Packet>? List;
        private readonly bool IsInt;
        private List<Packet> AsList => List ?? [this];
        public override string ToString() => IsInt ? AsInt.ToString() : $"[{string.Join(',', AsList.Select(x => x.ToString()))}]";

        public static Packet Parse(string text)
        {
            ArgumentException.ThrowIfNullOrEmpty(text);
            if (text[0] == '[')
            {
                var list = new List<Packet>();
                var (braceCount, prevComma) = (1, 1);
                for (int i = 1; i < text.Length; i++)
                {
                    if ((text[i] == ',' || text[i] == ']') && braceCount == 1)
                    {
                        var item = text[prevComma..i];
                        if (!string.IsNullOrEmpty(item))
                            list.Add(Parse(item));
                        prevComma = i + 1;
                    }
                    braceCount += text[i] switch { '[' => 1, ']' => -1, _ => 0 };
                }
                return new(list);
            }
            return new(int.Parse(text));
        }

        public static bool operator <=(Packet left, Packet right) => left.CompareTo(right) <= 0;
        public static bool operator >=(Packet left, Packet right) => left.CompareTo(right) >= 0;
        public int CompareTo(Packet other)
        {
            if (IsInt && other.IsInt) return AsInt.CompareTo(other.AsInt);
            var (lList, rList) = (AsList, other.AsList);
            for (int i = 0; i < lList.Count || i < rList.Count; i++)
            {
                if (i >= lList.Count) return -1;
                if (i >= rList.Count) return 1;
                if (lList[i].CompareTo(rList[i]) is var cmp and not 0) return cmp;
            }
            return 0;
        }
    }

    public string Solve(InputHelper inputHelper)
    {
        var packets = inputHelper.EachLine().Where(s => !string.IsNullOrEmpty(s)).Select(Packet.Parse).ToList();

        if (isPart1)
            return packets.Chunk(2)
                .Select((pair, i) => (cmp: pair[0] <= pair[1], i ))
                .Where(x => x.cmp)
                .Sum(x => x.i + 1) // 1-based index
                .ToString();

        var divider2 = Packet.Parse("[[2]]");
        var divider6 = Packet.Parse("[[6]]");
        packets.AddRange(divider2, divider6);
        packets.Sort();
        return $"{(packets.IndexOf(divider2) + 1) * (packets.IndexOf(divider6) + 1)}";
    }
}