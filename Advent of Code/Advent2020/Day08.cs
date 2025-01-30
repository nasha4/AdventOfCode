namespace Advent_of_Code.Advent2020;

public class Day08(bool isPart1) : IAdventPuzzle
{
    public string Solve(InputHelper inputHelper)
    {
        var program = inputHelper.EachLine(line => line.Split(' '))
            .Select(p => (p[0], int.Parse(p[1]))).ToList();

        return isPart1
            ? Execute(program, -1).acc.ToString()
            : Enumerable.Range(0, program.Count)
                .Where(i => program[i] is not ("acc", _))
                .Select(i => Execute(program, i))
                .First(x => !x.loops)
                .acc.ToString();
    }

    private static (bool loops, int acc) Execute(List<(string, int)> program, int patch)
    {
        var (ip, acc) = (0, 0);
        for (var visited = new HashSet<int>();
            ip < program.Count && visited.Add(ip);
            (ip, acc) = (program[ip], patch == ip) switch
            {
                (("acc", var n), _) => (ip + 1, acc + n),
                (("jmp", var n), false) => (ip + n, acc),
                (("nop", var n), true) => (ip + n, acc),
                _ => (ip + 1, acc)
            }) ;
        return (ip < program.Count, acc);
    }
}