using System.Text.RegularExpressions;

namespace Advent_of_Code.Advent2022;

public partial class Day16(bool isPart1) : IAdventPuzzle
{
    private readonly Dictionary<(string, int, string), (int, IEnumerable<string>)> cache = [];
    private readonly Dictionary<string, int> valveRates = [];
    private Dictionary<(string from, string to), int> valveDistance = [];
    public string Solve(InputHelper inputHelper)
    {
        var valveTunnels = new Dictionary<string, IReadOnlySet<string>>();
        foreach (var groups in inputHelper.EachMatch(ValveRates, match => match.Groups))
        {
            var (valve, rate) = (groups[1].Value, int.Parse(groups[2].Value));
            valveRates[valve] = rate;
            valveTunnels[valve] = groups[3].Captures.Select(c => c.Value).ToHashSet();
        }

        // all the valves start at 0. When we open one, it scores
        // timeRemaining * valveRate points. We want to maximize points.
        var zeroScore = valveRates
            .Where(kv => kv.Value > 0)
            .ToDictionary(kv => kv.Key, _ => 0);
        // We only ever want to move to valves with positive rate, so compute
        // the least distance from every valve>0 (plus AA), to every different
        // valve>0.  We make a Dictionary like { (from: "BB", to: "GG"), 5 }
        valveDistance = valveRates
            .Where(kv => kv.Value > 0 || kv.Key == "AA")
            .SelectMany(x =>
                valveRates.Where(y => x.Key != y.Key && y.Value > 0),
                (a, b) => (from: a.Key, to: b.Key))
            .ToDictionary(pair => pair, pair => ShortestPath(pair.from, pair.to, valveTunnels));

        if (isPart1)
        {
            var scores = SlowBestScore("AA", 30, zeroScore, Enumerable.Empty<string>().ToHashSet());
            return scores.Values.Sum().ToString();
        }
        else
        {
            var valvePowerSet = valveRates.Where(kv => kv.Value > 0).Select(kv => kv.Key)
                .Aggregate(Enumerable.Empty<IEnumerable<string>>().Append([]),
                    (acc, term) => acc.Concat(acc.Select(inner => inner.Append(term))));
            var subsetScores = valvePowerSet
                .Where(x => x.Count() == 7 || x.Count() == 8)
                .ToDictionary(
                    x => string.Join(" ", x.Order()),
                    x => BestScore2("AA", 26, x.ToHashSet()));
            var complementPairedSubsetScores = subsetScores
                .Select(kv =>
                {
                    var complement = string.Join(" ", zeroScore
                        .Keys.Where(x => !kv.Key.Contains(x)).Order());
                    return (me: kv.Value, elephant: subsetScores[complement]);
                });
            var (me, elephant) = complementPairedSubsetScores
                .MaxBy(pair => pair.me.score + pair.elephant.score);
            return $"{me.score + elephant.score}";
        }
    }

    private static int ShortestPath(string from, string goal, Dictionary<string, IReadOnlySet<string>> tunnels)
    {   // Breadth-first search (I already wrote this for Day12)
        var toSearch = new Queue<string>([from]);
        var visitedFrom = new Dictionary<string, string?>() { [from] = null };
        while (toSearch.TryDequeue(out var room))
        {
            foreach (var neighbor in tunnels[room].Where(s => !visitedFrom.ContainsKey(s)))
            {
                visitedFrom[neighbor] = room;
                toSearch.Enqueue(neighbor);
                if (neighbor == goal)
                {
                    var path = new List<string>();
                    for (var s = neighbor; s is not null; s = visitedFrom[s])
                        path.Add(s);
                    return path.Count - 1;
                }
            }
        }
        return 99999;
    }

    private (int score, IEnumerable<string> path) BestScore2(string room, int timeLeft, IReadOnlySet<string> ignoreValves)
    {
        var valveCode = string.Join(string.Empty, ignoreValves.Order());
        if (cache.TryGetValue((room, timeLeft, valveCode), out var memo))
            return memo;

        var newIgnore = new HashSet<string>(ignoreValves);
        var score = 0;
        if (valveRates[room] > 0 && timeLeft > 0)
        {
            newIgnore.Add(room);
            score = timeLeft * valveRates[room];
        }
        var paths = valveDistance
            .Where(x => x.Key.from == room && !newIgnore.Contains(x.Key.to) && x.Value < timeLeft - 1);
        if (paths.Any())
        {
            var best = paths
                .Select(path => BestScore2(path.Key.to, timeLeft - 1 - path.Value, newIgnore))
                .MaxBy(x => x.score);
            best.score += score;
            best.path = best.path.Append(room);
            return cache[(room, timeLeft, valveCode)] = best;
        }
        else
        {
            return cache[(room, timeLeft, valveCode)] = (score, [room]);
        }
    }

    private Dictionary<string, int> SlowBestScore(string room, int timeLeft, Dictionary<string, int> score, IReadOnlySet<string> ignoreValves)
    {   // Depth-first search (recursive)
        var newScore = new Dictionary<string, int>(score);
        if (valveRates[room] > 0 && timeLeft > 0)
            newScore[room] = timeLeft * valveRates[room];
        var paths = valveDistance
            .Where(x => x.Key.from == room && !ignoreValves.Contains(x.Key.to)
                && newScore[x.Key.to] == 0 && x.Value < timeLeft - 1);
        if (paths.Any())
            return paths
                .Select(path => SlowBestScore(path.Key.to, timeLeft - 1 - path.Value, newScore, ignoreValves))
                .MaxBy(x => x.Values.Sum())!;
        else
            return newScore;
    }

    private Dictionary<string, int> SlowBestScoreWithElephant((string room, int timeLeft) me, (string room, int timeLeft) elephant, Dictionary<string, int> valveScore)
    {
        var newScore = new Dictionary<string, int>(valveScore);
        if (me.timeLeft >= elephant.timeLeft)
        {
            if (valveRates[me.room] > 0 && me.timeLeft > 0)
                newScore[me.room] = me.timeLeft * valveRates[me.room];
            var paths = valveDistance
                .Where(x => x.Key.from == me.room && newScore[x.Key.to] == 0
                    && x.Value < me.timeLeft - 1 && x.Key.to != elephant.room);
            if (paths.Any())
                return paths
                    .Select(path => SlowBestScoreWithElephant((path.Key.to, me.timeLeft - 1 - path.Value), elephant, newScore))
                    .MaxBy(x => x.Values.Sum())!;
            else
                return SlowBestScoreWithElephant((string.Empty, 0), elephant, newScore); // let the elephant complete its final move
        }
        else
        {
            if (valveRates[elephant.room] > 0 && elephant.timeLeft > 0)
                newScore[elephant.room] = elephant.timeLeft * valveRates[elephant.room];
            var paths = valveDistance
                .Where(x => x.Key.from == elephant.room && newScore[x.Key.to] == 0
                && x.Value < elephant.timeLeft - 1 && x.Key.to != me.room);
            if (paths.Any())
                return paths
                    .Select(path => SlowBestScoreWithElephant(me, (path.Key.to, elephant.timeLeft - 1 - path.Value), newScore))
                    .MaxBy(x => x.Values.Sum())!;
            else
                return newScore;
        }
    }

    [GeneratedRegex(@"Valve (..) .* rate=(\d+);.* valves? (?:(..)(?:, )?)+")]
    private static partial Regex ValveRates { get; }
}