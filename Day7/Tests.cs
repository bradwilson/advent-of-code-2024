using System.Text.RegularExpressions;

namespace Day7;

public class Calculator
{
    public static bool IsCorrect(bool supportConcat, long target, long[] values) =>
        IsCorrect(supportConcat, target, values, values[0], 1);

    static bool IsCorrect(bool supportConcat, long target, long[] values, long firstValue, int restIdx)
    {
        var sum = firstValue + values[restIdx];
        var product = firstValue * values[restIdx];
        var concat = long.Parse(firstValue.ToString() + values[restIdx].ToString());

        if (restIdx + 1 == values.Length)
            return sum == target || product == target || (supportConcat && concat == target);

        return
            (sum <= target && IsCorrect(supportConcat, target, values, sum, restIdx + 1)) ||
            (product <= target && IsCorrect(supportConcat, target, values, product, restIdx + 1)) ||
            (supportConcat && concat <= target && IsCorrect(supportConcat, target, values, concat, restIdx + 1));
    }

    public static (long Target, long[] Values) Parse(string equation)
    {
        var regex = new Regex(@"(\d+): (\d+)+(\s(\d+))+");
        var match = regex.Match(equation ?? string.Empty);
        if (!match.Success)
            throw new ArgumentException("Did not match equation pattern", nameof(equation));

        var target = long.Parse(match.Groups[1].Value);
        var first = long.Parse(match.Groups[2].Value);
        var rest = match.Groups[4].Captures.Cast<Capture>().Select(c => long.Parse(c.Value)).ToArray();

        return (target, [first, .. rest]);
    }
}

public class CalculatorTests
{
    public class Parse
    {
        [Theory]
        [InlineData(default(string))]
        [InlineData("")]
        [InlineData("142 12")]
        [InlineData("42:")]
        [InlineData("42: 2112")]
        [InlineData("42: 2112 ABC")]
        public void MustHaveValues(string? equation)
        {
            var ex = Record.Exception(() => Calculator.Parse(equation!));

            Assert.NotNull(ex);
            var argEx = Assert.IsType<ArgumentException>(ex);
            Assert.Equal("equation", argEx.ParamName);
            Assert.StartsWith("Did not match equation pattern", argEx.Message);
        }

        [Theory]
        [InlineData("190: 10 19", 190L, new long[] { 10L, 19L })]
        [InlineData("3267: 81 40 27", 3267L, new long[] { 81, 40, 27 })]
        public void CanParse(string equation, long target, long[] values)
        {
            var result = Calculator.Parse(equation);

            Assert.Equal(target, result.Target);
            Assert.Equal(values, result.Values);
        }
    }

    public class IsCorrect
    {
        [Theory]
        [InlineData("190: 10 19", true)]
        [InlineData("161011: 16 10 13", false)]
        [InlineData("156: 15 6", false)]
        public void CanComputeIsCorrect(string equation, bool expected)
        {
            var parsed = Calculator.Parse(equation);

            Assert.Equal(expected, Calculator.IsCorrect(false, parsed.Target, parsed.Values));
        }

        [Theory]
        [InlineData("190: 10 19", true)]
        [InlineData("161011: 16 10 13", false)]
        [InlineData("156: 15 6", true)]
        [InlineData("7290: 6 8 6 15", true)]
        [InlineData("192: 17 8 14", true)]
        public void CanComputeWithConcatenation(string equation, bool expected)
        {
            var parsed = Calculator.Parse(equation);

            Assert.Equal(expected, Calculator.IsCorrect(true, parsed.Target, parsed.Values));
        }
    }
}

public class Tests
{
    [Theory]
    [InlineData("Data1.txt", 3_749L)]
    [InlineData("Data2.txt", 1_260_333_054_159L)]
    public void Part1(string filename, long expected)
    {
        var result = 0L;

        foreach (var equation in File.ReadAllLines(filename))
        {
            var parsed = Calculator.Parse(equation);
            if (Calculator.IsCorrect(false, parsed.Target, parsed.Values))
                result += parsed.Target;
        }

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("Data1.txt", 11_387L)]
    [InlineData("Data2.txt", 162_042_343_638_683L)]
    public void Part2(string filename, long expected)
    {
        var result = 0L;

        foreach (var equation in File.ReadAllLines(filename))
        {
            var parsed = Calculator.Parse(equation);
            if (Calculator.IsCorrect(true, parsed.Target, parsed.Values))
                result += parsed.Target;
        }

        Assert.Equal(expected, result);
    }
}
