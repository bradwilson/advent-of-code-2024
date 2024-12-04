using System.Text.RegularExpressions;

namespace Day3;

public static partial class Computer
{
    static Regex regex = MakeRegex();

    public static int Evaluate(bool supportConditionals, params string[] instructions)
    {
        var result = 0;
        var enabled = true;

        foreach (var instruction in instructions)
            foreach (Match match in regex.Matches(instruction))
                if (match.Groups[1].Value == "mul" && enabled)
                    result += int.Parse(match.Groups[2].Value) * int.Parse(match.Groups[3].Value);
                else if (supportConditionals && match.Groups[4].Value == "do")
                    enabled = true;
                else if (supportConditionals && match.Groups[5].Value == "don't")
                    enabled = false;

        return result;
    }

    [GeneratedRegex(@"(mul)\((\d\d?\d?),(\d\d?\d?)\)|(do)\(\)|(don't)\(\)", RegexOptions.Compiled)]
    private static partial Regex MakeRegex();
}

public class ComputerTests
{
    [Fact]
    public void CanEvaluateSingleMul()
    {
        var result = Computer.Evaluate(false, "xmul(1,2)y");

        Assert.Equal(2, result);
    }

    [Fact]
    public void CanEvaluateMultipleMul()
    {
        var result = Computer.Evaluate(false, "xmul(2,4)&mul[3,7]!^don't()_mul(5,5)+mul(32,64](mul(11,8)undo()?mul(8,5))");

        Assert.Equal(161, result);
    }

    [Fact]
    public void CanEvaluateMultipleMulWithConditionals()
    {
        var result = Computer.Evaluate(true, "xmul(2,4)&mul[3,7]!^don't()_mul(5,5)+mul(32,64](mul(11,8)undo()?mul(8,5))");

        Assert.Equal(48, result);
    }
}

public class Tests
{
    [Theory]
    [InlineData("Data1.txt", 161)]
    [InlineData("Data2.txt", 168_539_636)]
    public void Part1(string filename, int expected)
    {
        var result = Computer.Evaluate(false, File.ReadAllLines(filename));

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("Data1.txt", 48)]
    [InlineData("Data2.txt", 97_529_391)]
    public void Part2(string filename, int expected)
    {
        var result = Computer.Evaluate(true, File.ReadAllLines(filename));

        Assert.Equal(expected, result);
    }
}
