using System.Text.RegularExpressions;

namespace Advent_of_Code.Advent2022;

public partial class Day19(bool isPart1) : IAdventPuzzle
{
    private readonly record struct GameState(int Ore = 0, int Clay = 0, int Obsidian = 0, int Geodes = 0, int OreBots = 0, int ClayBots = 0, int ObsidianBots = 0, int GeodeBots = 0, int TimeLeft = 0)
        : IComparable<GameState>
    {
        private readonly int[] Resources => [Ore, Clay, Obsidian, Geodes];
        private readonly int[] Robots => [OreBots, ClayBots, ObsidianBots, GeodeBots];
        private readonly int MaxGeodes =>
            // If we keep cracking geodes at our current rate, AND somehow make a new geode
            // cracker every turn from now on, what's the most geodes can we possibly hope for?
            Geodes + TimeLeft * GeodeBots + (TimeLeft - 1) * TimeLeft / 2;
        private GameState(int[] resources, int[] robots, int timeLeft) : this(resources[0], resources[1], resources[2], resources[3], robots[0], robots[1], robots[2], robots[3], timeLeft) { }

        private readonly IEnumerable<int> Criteria => [ MaxGeodes, GeodeBots, Geodes, ObsidianBots, Obsidian, ClayBots, Clay, OreBots, Ore ];
        public readonly int CompareTo(GameState other) => Criteria.Zip(other.Criteria, (a, b) => a.CompareTo(b)).FirstOrDefault(x => x != 0, 0);

        public readonly bool CanAfford(int[] blueprint) => Resources.Zip(blueprint, (a, b) => a >= b).All(x => x);
        public readonly GameState MakeRobot(int[] blueprint) => new(
            Resources.Zip(Robots, blueprint).Select(x => x.First + x.Second - x.Third).ToArray(),
            Robots.Select((x, i) => i == blueprint[^1] ? x + 1 : x).ToArray(),
            TimeLeft - 1);
    }

    public string Solve(InputHelper inputHelper)
    {
        var allBlueprints = inputHelper.EachMatchGroup(Integer, x => int.Parse(x[0]))
            .Chunk(7)
            .Select(x => new[] {
                x[1], 0, 0, 0, 0, // ore robot
                x[2], 0, 0, 0, 1, // clay robot
                x[3], x[4], 0, 0, 2, // obsidian robot
                x[5], 0, x[6], 0, 3, // geode robot
                0, 0, 0, 0, -1 // no robot
            }.Chunk(5));

        if (isPart1)
        {
            var initialState = new GameState(OreBots: 1, TimeLeft: 24);
            var geodes = allBlueprints.Select(blueprints => MineGeodes(initialState, blueprints));
            return geodes.Select((x, i) => x * (i + 1)).Sum().ToString();
        }
        else
        {
            var initialState = new GameState(OreBots: 1, TimeLeft: 32);
            var geodes = allBlueprints.Take(3).Select(blueprints => MineGeodes(initialState, blueprints));
            return geodes.Aggregate((prod, term) => prod * term).ToString();
        }
    }
    private static int MineGeodes(GameState initialState, IEnumerable<int[]> blueprints)
    {
        var nextStep = new SortedSet<GameState>() { initialState };
        for (var thisStep = nextStep; nextStep.First().TimeLeft > 0; thisStep = nextStep)
        {
            nextStep = new(thisStep.TakeLast(100)
                .SelectMany(state => blueprints.Where(state.CanAfford), (state, blueprint) => state.MakeRobot(blueprint)));
        }
        return nextStep.Last().Geodes; // set is sorted, Last is best!
    }

    [GeneratedRegex(@"\d+")]
    private static partial Regex Integer { get; }
}