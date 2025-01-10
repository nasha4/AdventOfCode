namespace Advent_of_Code.Advent2020;

public class Day16(bool isPart1) : IAdventPuzzle
{
    public string Solve(InputHelper inputHelper)
    {
        var fields = inputHelper.EachLineInSection(line => line.Split(": "))
            .Select(x => KeyValuePair.Create(x[0], x[1].Split(" or ").Select(y => y.Split('-').Select(int.Parse)).Select(z => (from: z.First(), to: z.Skip(1).Single()))))
            .ToDictionary();
        var myTicket = inputHelper.EachLineInSection(line => line).Skip(1).Single().Split(',').Select(int.Parse).ToArray();
        var tickets = inputHelper.EachLineInSection(line => line).Skip(1).Select(x => x.Split(',').Select(int.Parse).ToArray()).ToList();

        if (isPart1) return tickets.SelectMany(x => x).Where(n => !fields.SelectMany(x => x.Value).Any(f => f.from <= n && f.to >= n)).Sum().ToString();

        var validTickets = tickets.Append(myTicket).Where(ticket => !ticket.Any(n => !fields.SelectMany(x => x.Value).Any(f => f.from <= n && f.to >= n)));
        var couldBe = Enumerable.Range(0, myTicket.Length).Select(i => fields.Keys.Where(k => validTickets.Select(t => t[i]).All(tv => fields[k].Any(r => r.from <= tv && r.to >= tv))).ToHashSet()).ToArray();
        Dictionary<bool, List<HashSet<string>>> solved;
        for (solved = new() { [false] = [] };
            solved.ContainsKey(false);
            solved = couldBe.GroupBy(h => h.Count == 1).ToDictionary(g => g.Key, g => g.ToList()))
            solved[false].ForEach(hs => hs.ExceptWith(solved[true].SelectMany(x => x)));
        var solution = solved[true].Select((x, i) => KeyValuePair.Create(x.Single(), i)).ToDictionary();

        return fields.Keys.Where(k => k.Contains("departure")).Select(k => myTicket[solution[k]]).Aggregate(1L, (product, term) => product * term).ToString();
    }
}
