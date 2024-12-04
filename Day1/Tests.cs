namespace Day1;

public class DistanceComputer
{
    readonly List<int> left;
    readonly List<int> right;

    public DistanceComputer(string filename)
    {
        left = [];
        right = [];

        foreach (var line in File.ReadAllLines(filename))
        {
            var pieces = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            left.Add(int.Parse(pieces[0]));
            right.Add(int.Parse(pieces[1]));
        }
    }

    public int ComputeDistance()
    {
        var sortedLeft = left.OrderBy(x => x).ToList();
        var sortedRight = right.OrderBy(x => x).ToList();
        var result = 0;

        for (int idx = 0; idx < sortedLeft.Count; ++idx)
            result += Math.Abs(sortedLeft[idx] - sortedRight[idx]);

        return result;
    }

    public int ComputeSimilarity()
    {
        var result = 0;

        foreach (var leftItem in left)
        {
            var occurance = 0;

            foreach (var rightItem in right)
                if (rightItem == leftItem)
                    occurance++;

            result += leftItem * occurance;
        }

        return result;
    }
}

public class Tests
{
    static readonly DistanceComputer data1 = new("Data1.txt");
    static readonly DistanceComputer data2 = new("Data2.txt");

    public static IEnumerable<TheoryDataRow<DistanceComputer, int>> Part1Data =>
        [
            new(data1, 11),
            new(data2, 3_569_916),
        ];

    [Theory]
    [MemberData(nameof(Part1Data))]
    public void Part1(DistanceComputer computer, int expectedDistance)
    {
        var actualDistance = computer.ComputeDistance();

        Assert.Equal(expectedDistance, actualDistance);
    }

    public static IEnumerable<TheoryDataRow<DistanceComputer, int>> Part2Data =>
        [
            new(data1, 31),
            new(data2, 26_407_426)
        ];

    [Theory]
    [MemberData(nameof(Part2Data))]
    public void Part2(DistanceComputer computer, int expectedSimilarity)
    {
        var actualSimilarity = computer.ComputeSimilarity();

        Assert.Equal(expectedSimilarity, actualSimilarity);
    }
}
