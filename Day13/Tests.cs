using System.Text.RegularExpressions;
using MathNet.Numerics.LinearAlgebra;

namespace Day13;

public class UnitTest1
{
    static readonly Regex buttonRegex = new(@"X\+(\d+), Y\+(\d+)");
    static readonly Regex prizeRegex = new(@"X=(\d+), Y=(\d+)");

    [Theory]
    [InlineData("Data1.txt", 480)]
    [InlineData("Data2.txt", 28059)]
    public void Part1(string filename, long expected)
    {
        var lines = File.ReadAllLines(filename);
        var idx = 0;
        var result = 0L;

        while (idx < lines.Length)
        {
            var buttonA = Parse(buttonRegex, lines[idx]);
            var buttonB = Parse(buttonRegex, lines[idx + 1]);
            var prize = Parse(prizeRegex, lines[idx + 2]);
            result += Solve(buttonA, buttonB, prize);

            idx += 4;
        }

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("Data1.txt", 875318608908)]
    [InlineData("Data2.txt", 102255878088512)]
    public void Part2(string filename, long expected)
    {
        var lines = File.ReadAllLines(filename);
        var idx = 0;
        var result = 0L;

        while (idx < lines.Length)
        {
            var buttonA = Parse(buttonRegex, lines[idx]);
            var buttonB = Parse(buttonRegex, lines[idx + 1]);
            var prize = Parse(prizeRegex, lines[idx + 2]);
            result += Solve(buttonA, buttonB, (prize.x + 10_000_000_000_000L, prize.y + 10_000_000_000_000L));

            idx += 4;
        }

        Assert.Equal(expected, result);
    }

    static (long x, long y) Parse(Regex regex, string text)
    {
        var match = regex.Match(text);
        Assert.True(match.Success);

        return (long.Parse(match.Groups[1].Value), long.Parse(match.Groups[2].Value));
    }

    static long Solve((long x, long y) buttonA, (long x, long y) buttonB, (long x, long y) prize)
    {
        var matrix = Matrix<double>.Build.DenseOfArray(new double[,] { { buttonA.x, buttonB.x }, { buttonA.y, buttonB.y } });
        var vector = Vector<double>.Build.DenseOfArray([prize.x, prize.y]);
        var solution = matrix.Solve(vector);

        var countA = (long)Math.Round(solution[0]);
        var countB = (long)Math.Round(solution[1]);

        if ((countA * buttonA.x + countB * buttonB.x == prize.x) && (countA * buttonA.y + countB * buttonB.y == prize.y))
            return 3 * countA + countB;

        return 0;
    }
}
