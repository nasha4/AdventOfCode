namespace Advent;

internal static class Advent2019
{
    public static int Day1(int part)
    {
        var sum = 0;
        for (var line = Console.ReadLine(); line is not null; line = Console.ReadLine())
        {
            var mass = int.Parse(line);
            if (part == 1)
                sum += mass / 3 - 2;
            else
                while (mass / 3 - 2 >= 0)
                    sum += mass = mass / 3 - 2;
        }
        return sum;
    }
    public static int Day2(int part)
    {
        var ints = Console.ReadLine().Split(',').Select(int.Parse).ToArray();
        ints[1] = 12; ints[2] = 2;
        for (var ip = 0; ip < ints.Length; ip += 4)
        {
            if (ints[ip] == 1)
                ints[ints[ip + 3]] = ints[ints[ip + 1]] + ints[ints[ip + 2]];
            else if (ints[ip] == 2)
                ints[ints[ip + 3]] = ints[ints[ip + 1]] * ints[ints[ip + 2]];
            else if (ints[ip] == 99)
                return ints[0];
        }
        return -1;
    }
}