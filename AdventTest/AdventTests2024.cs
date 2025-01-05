using Advent_of_Code.Advent2024;

namespace AdventTest;

public class AdventSolutions2024 : AdventSolutions
{
    protected override Dictionary<Type, IEnumerable<string>> Solutions => solutions;

    private static readonly Dictionary<Type, IEnumerable<string>> solutions = new()
    {
        [typeof(Day01)] = ["2196996", "23655822"],
        [typeof(Day02)] = ["252", "324"],
        [typeof(Day03)] = ["164730528", "70478672"],
        [typeof(Day04)] = ["2517", "1960"],
        [typeof(Day05)] = ["4689", "6336"],
        [typeof(Day06)] = ["4988", "1697"],
        [typeof(Day07)] = ["5030892084481", "91377448644679"],
        [typeof(Day08)] = ["276", "991"],
        [typeof(Day09)] = ["6288707484810", "6311837662089"],
        [typeof(Day10)] = ["798", "1816"],
        [typeof(Day11)] = ["189167", "225253278506288"],
        [typeof(Day12)] = ["1477924", "841934"],
        [typeof(Day13)] = ["34787", "85644161121698"],
        [typeof(Day14)] = ["231782040", "6475"],
        [typeof(Day15)] = ["1499739", "1522215"],
        [typeof(Day16)] = ["88416", "442"],
        [typeof(Day17)] = ["7,4,2,0,5,0,5,3,7", "202991746427434"],
        [typeof(Day18)] = ["284", "51,50"],
        [typeof(Day19)] = ["240", "848076019766013"],
        [typeof(Day20)] = ["1360", "1005476"],
        [typeof(Day21)] = ["248108", "303836969158972"],
        [typeof(Day22)] = ["13004408787", "1455"],
        [typeof(Day23)] = ["1302", "cb,df,fo,ho,kk,nw,ox,pq,rt,sf,tq,wi,xz"],
        [typeof(Day24)] = ["61495910098126", "61633350099662"],
        [typeof(Day25)] = ["2950"]
    };
}
