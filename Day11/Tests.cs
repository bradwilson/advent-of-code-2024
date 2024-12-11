namespace Day11;

public class Stones
{
    Dictionary<long, long> stones = [];

    public Stones(string stoneList)
    {
        foreach (var stone in stoneList.Split(' ').Select(long.Parse))
            Add(stones, stone, 1);
    }

    public long Count => stones.Values.Sum();

    static void Add(Dictionary<long, long> stones, long stone, long count)
    {
        if (stones.TryGetValue(stone, out var current))
            stones[stone] = current + count;
        else
            stones[stone] = count;
    }

    public void Blink()
    {
        Dictionary<long, long> newStones = [];

        foreach (var stone in stones)
        {
            if (stone.Key == 0)
                Add(newStones, 1, stone.Value);
            else
            {
                var stoneText = stone.Key.ToString();
                if (stoneText.Length % 2 == 0)
                {
                    Add(newStones, long.Parse(stoneText[0..(stoneText.Length / 2)]), stone.Value);
                    Add(newStones, long.Parse(stoneText[(stoneText.Length / 2)..]), stone.Value);
                }
                else
                    Add(newStones, stone.Key * 2024L, stone.Value);
            }
        }

        stones = newStones;
    }
}

public class StonesTests
{
    [Fact]
    public void FirstSample()
    {
        var stones = new Stones("0 1 10 99 999");

        stones.Blink();

        Assert.Equal(7, stones.Count);
    }

    [Theory]
    [InlineData(1, 3L)]
    [InlineData(2, 4L)]
    [InlineData(3, 5L)]
    [InlineData(4, 9L)]
    [InlineData(5, 13L)]
    [InlineData(6, 22L)]
    public void SecondSample(int blinkCount, long expected)
    {
        var stones = new Stones("125 17");

        while (blinkCount > 0)
        {
            stones.Blink();
            --blinkCount;
        }

        Assert.Equal(expected, stones.Count);
    }
}

public class Tests
{
    [Theory]
    [InlineData("Data1.txt", 55_312, 25)]
    [InlineData("Data2.txt", 217_812, 25)]
    [InlineData("Data2.txt", 259_112_729_857_522, 75)]
    public void Count(string filename, long expected, int iterations)
    {
        var stones = new Stones(File.ReadAllLines(filename).First());

        for (var count = 0; count < iterations; ++count)
            stones.Blink();

        Assert.Equal(expected, stones.Count);
    }
}
