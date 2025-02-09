﻿namespace Advent_of_Code.Advent2024;

public class Day25(bool isPart1) : IAdventPuzzle
{
    public string Solve(InputHelper inputHelper)
    {
        var schematics = inputHelper.EachSection(section => new Grid.Helper(section));
        var codes = schematics.GroupBy(grid => grid[[0, 0]],
                grid => Enumerable.Range(0, 5).Select(x => grid['#'].Count(xy => xy[1] == x)))
            .ToDictionary(g => g.Key, g => g.AsEnumerable());

        return codes['#'].SelectMany(_ => codes['.'], (key, lok) => key.Zip(lok, (k, l) => k + l))
            .Count(pins => pins.All(pin => pin <= 7)).ToString();
    }
}