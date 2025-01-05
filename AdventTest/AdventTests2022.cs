using Advent_of_Code.Advent2022;

namespace AdventTest;

public class AdventSolutions2022 : AdventSolutions
{
    protected override Dictionary<Type, IEnumerable<string>> Solutions => solutions;

    private static readonly Dictionary<Type, IEnumerable<string>> solutions = new()
    {
        [typeof(Day01)] = ["66616", "199172"],
        [typeof(Day02)] = ["13924", "13448"],
        [typeof(Day03)] = ["7581", "2525"],
        [typeof(Day04)] = ["547", "843"],
        [typeof(Day05)] = ["QNHWJVJZW", "BPCZJLFJW"],
        [typeof(Day06)] = ["1140", "3495"],
        [typeof(Day07)] = ["1490523", "12390492"],
        [typeof(Day08)] = ["1825", "235200"],
        [typeof(Day09)] = ["5907", "2303"],
        [typeof(Day10)] = ["13440", """
            ###..###..####..##..###...##..####..##..
            #..#.#..#....#.#..#.#..#.#..#....#.#..#.
            #..#.###....#..#....#..#.#..#...#..#..#.
            ###..#..#..#...#.##.###..####..#...####.
            #....#..#.#....#..#.#.#..#..#.#....#..#.
            #....###..####..###.#..#.#..#.####.#..#.
            """],
        [typeof(Day11)] = ["99840", "20683044837"],
        [typeof(Day12)] = ["468", "459"],
        [typeof(Day13)] = ["5588", "23958"],
        [typeof(Day14)] = ["805", "25161"],
        [typeof(Day15)] = ["5181556", "12817603219131"],
        [typeof(Day16)] = [string.Empty, string.Empty],
        [typeof(Day17)] = [string.Empty, string.Empty],
        [typeof(Day18)] = [string.Empty, string.Empty],
        [typeof(Day19)] = [string.Empty, string.Empty],
        [typeof(Day20)] = [string.Empty, string.Empty],
        [typeof(Day21)] = [string.Empty, string.Empty],
        [typeof(Day22)] = [string.Empty, string.Empty],
        [typeof(Day23)] = [string.Empty, string.Empty],
        [typeof(Day24)] = [string.Empty, string.Empty],
        [typeof(Day25)] = ["2=0--0---11--01=-100"]
    };
}
