namespace Day2;

public class LevelAnalyzer
{
    public static bool IsSafe(int[] levels)
    {
        var goingUp = levels[1] > levels[0];

        for (var idx = 1; idx < levels.Length; ++idx)
        {
            var distance = levels[idx] - levels[idx - 1];
            if (!goingUp)
                distance = -distance;

            if (distance >= 1 && distance <= 3)
                continue;

            return false;
        }

        return true;
    }

    public static bool IsSafeWithSkip(int[] levels)
    {
        if (IsSafe(levels))
            return true;

        for (var idx = 0; idx < levels.Length; ++idx)
            if (IsSafe([.. levels[0..idx], .. levels[(idx + 1)..]]))
                return true;

        return false;
    }
}

public class LevelAnalyzerTests
{
    [Theory]
    [InlineData(new[] { 7, 6, 4, 2, 1 }, true)]
    [InlineData(new[] { 1, 2, 7, 8, 9 }, false)]
    [InlineData(new[] { 9, 7, 6, 2, 1 }, false)]
    [InlineData(new[] { 1, 3, 2, 4, 5 }, false)]
    [InlineData(new[] { 8, 6, 4, 4, 1 }, false)]
    [InlineData(new[] { 1, 3, 6, 7, 9 }, true)]
    public void CanVerifySafety(int[] levels, bool isSafe)
    {
        Assert.Equal(isSafe, LevelAnalyzer.IsSafe(levels));
    }

    [Theory]
    [InlineData(new[] { 7, 6, 4, 2, 1 }, true)]
    [InlineData(new[] { 1, 2, 7, 8, 9 }, false)]
    [InlineData(new[] { 9, 7, 6, 2, 1 }, false)]
    [InlineData(new[] { 1, 3, 2, 4, 5 }, true)]
    [InlineData(new[] { 8, 6, 4, 4, 1 }, true)]
    [InlineData(new[] { 1, 3, 6, 7, 9 }, true)]
    public void CanVerifySafetyWithSkip(int[] levels, bool isSafe)
    {
        Assert.Equal(isSafe, LevelAnalyzer.IsSafeWithSkip(levels));
    }
}

public class Tests
{
    [Theory]
    [InlineData("Data1.txt", 2)]
    [InlineData("Data2.txt", 502)]
    public void Part1(string filename, int expected)
    {
        var result = 0;
        List<int[]> reports = [];

        foreach (var line in File.ReadAllLines(filename))
            reports.Add(line.Split(' ').Select(int.Parse).ToArray());

        foreach (var report in reports)
            if (LevelAnalyzer.IsSafe(report))
                result++;

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("Data1.txt", 4)]
    [InlineData("Data2.txt", 544)]
    public void Part2(string filename, int expected)
    {
        var result = 0;
        List<int[]> reports = [];

        foreach (var line in File.ReadAllLines(filename))
            reports.Add(line.Split(' ').Select(int.Parse).ToArray());

        foreach (var report in reports)
            if (LevelAnalyzer.IsSafeWithSkip(report))
                result++;

        Assert.Equal(expected, result);
    }
}
