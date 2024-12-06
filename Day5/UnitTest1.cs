using Xunit.Internal;

namespace Day5;

public class Parser
{
    readonly List<List<int>> pageGroups = [];
    readonly Dictionary<int, HashSet<int>> prerequisitesByPage = [];

    public Parser(List<string> lines)
    {
        var breakIdx = lines.IndexOf(string.Empty);

        foreach (var line in lines[..breakIdx])
        {
            var pieces = line.Split('|').Select(int.Parse).ToArray();
            var prerequisites = prerequisitesByPage.AddOrGet(pieces[1]);
            prerequisites.Add(pieces[0]);
        }

        foreach (var line in lines[(breakIdx + 1)..])
            pageGroups.Add(line.Split(',').Select(int.Parse).ToList());
    }

    public int Verify(bool onlyFixable = false) =>
        pageGroups.Select(pageGroup => Verify(pageGroup, onlyFixable)).Sum();

    int Verify(List<int> pageGroup, bool onlyFixable, bool mutated = false)
    {
        for (int pageIdx = 0; pageIdx < pageGroup.Count; pageIdx++)
        {
            var page = pageGroup[pageIdx];
            if (prerequisitesByPage.TryGetValue(page, out var prerequisites))
            {
                for (var violatorIdx = pageIdx + 1; violatorIdx < pageGroup.Count; violatorIdx++)
                {
                    var violator = pageGroup[violatorIdx];
                    if (prerequisites.Contains(violator))
                    {
                        if (!onlyFixable)
                            return 0;

                        List<int> newPages = [
                            .. pageGroup[..pageIdx],
                            violator,
                            .. pageGroup[pageIdx..violatorIdx],
                            .. pageGroup[(violatorIdx + 1)..]
                        ];

                        return Verify(newPages, onlyFixable, true);
                    }
                }
            }
        }

        if (mutated || !onlyFixable)
            return pageGroup[pageGroup.Count / 2];

        return 0;
    }
}

public class ParserTests
{
    [Fact]
    public void ValidPrintRequest()
    {
        var parser = new Parser(["1|2", "", "1,2"]);

        var result = parser.Verify();

        Assert.Equal(2, result);
    }

    [Fact]
    public void InvalidPrintRequest()
    {
        var parser = new Parser(["1|2", "", "2,1,3"]);

        var result = parser.Verify();

        Assert.Equal(0, result);
    }

    [Fact]
    public void MixOfValidAndInvalid()
    {
        var parser = new Parser(["1|2", "2|3", "", "1,2,3", "2,1,3", "1,5,6"]);

        var result = parser.Verify();

        Assert.Equal(7, result);
    }

    [Fact]
    public void ValidPrintRequest_OnlyFixable()
    {
        var parser = new Parser(["1|2", "", "1,2"]);

        var result = parser.Verify(onlyFixable: true);

        Assert.Equal(0, result);
    }

    [Fact]
    public void InvalidPrintRequest_OnlyFixable()
    {
        var parser = new Parser(["1|2", "", "2,1,3"]);

        var result = parser.Verify(onlyFixable: true);

        Assert.Equal(2, result);
    }
}

public class Tests
{
    [Theory]
    [InlineData("Data1.txt", 143)]
    [InlineData("Data2.txt", 5275)]
    public void Part1(string filename, int expected)
    {
        var parser = new Parser([.. File.ReadAllLines(filename)]);

        var result = parser.Verify();

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("Data1.txt", 123)]
    [InlineData("Data2.txt", 6191)]
    public void Part2(string filename, int expected)
    {
        var parser = new Parser([.. File.ReadAllLines(filename)]);

        var result = parser.Verify(onlyFixable: true);

        Assert.Equal(expected, result);
    }
}
