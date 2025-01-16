using Advent_of_Code.Advent2022;

namespace AdventTest;

public class AdventSolutions2022 : AdventSolutions
{
    protected override Dictionary<Type, IEnumerable<object>> Solutions { get; } = new()
    {
        [typeof(Day01)] = [66_616, 199_172],
        [typeof(Day02)] = [13_924, 13_448],
        [typeof(Day03)] = [7_581, 2_525],
        [typeof(Day04)] = [547, 843],
        [typeof(Day05)] = ["QNHWJVJZW", "BPCZJLFJW"],
        [typeof(Day06)] = [1_140, 3_495],
        [typeof(Day07)] = [1_490_523, 12_390_492],
        [typeof(Day08)] = [1_825, 235_200],
        [typeof(Day09)] = [5_907, 2_303],
        [typeof(Day10)] = [13_440, """
            ###..###..####..##..###...##..####..##..
            #..#.#..#....#.#..#.#..#.#..#....#.#..#.
            #..#.###....#..#....#..#.#..#...#..#..#.
            ###..#..#..#...#.##.###..####..#...####.
            #....#..#.#....#..#.#.#..#..#.#....#..#.
            #....###..####..###.#..#.#..#.####.#..#.
            """],
        [typeof(Day11)] = [99_840, 20_683_044_837],
        [typeof(Day12)] = [468, 459],
        [typeof(Day13)] = [5_588, 23_958],
        [typeof(Day14)] = [805, 25_161],
        [typeof(Day15)] = [5_181_556, 12_817_603_219_131],
        [typeof(Day16)] = [1_944, 2_679],
        [typeof(Day17)] = [3_157, 1_581_449_275_319],
        [typeof(Day18)] = [3_496, 2_064],
        [typeof(Day19)] = [1_346, 7_644],
        [typeof(Day20)] = [7_584, 4_907_679_608_191],
        [typeof(Day21)] = [194_501_589_693_264, 3_887_609_741_189],
        [typeof(Day22)] = [3_590, 86_382],
        [typeof(Day23)] = [3_947, 1_012],
        [typeof(Day24)] = [230, 713],
        [typeof(Day25)] = ["2=0--0---11--01=-100"]
    };
}
