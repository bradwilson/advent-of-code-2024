using System.Text.RegularExpressions;

namespace Day14;

public class Robot(int x, int y, int dx, int dy)
{
    public int X { get; set; } = x;
    public int Y { get; set; } = y;
    public int DX { get; set; } = dx;
    public int DY { get; set; } = dy;
}

public class Tests
{
    static readonly Regex robotRegex = new(@"p=(\d+),(\d+) v=(-?\d+),(-?\d+)");

    [Theory]
    [InlineData("Data1.txt", 12)]
    [InlineData("Data2.txt", 218619120)]
    public void Part1(string filename, long expected)
    {
        var robots =
            (from line in File.ReadAllLines(filename)
             let match = robotRegex.Match(line)
             where match.Success
             select new Robot(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value), int.Parse(match.Groups[3].Value), int.Parse(match.Groups[4].Value))).ToArray();

        var maxX = robots.Max(r => r.X);
        var maxY = robots.Max(r => r.Y);

        for (var second = 0; second < 100; ++second)
            foreach (var robot in robots)
            {
                var newX = wrap(robot.X + robot.DX, maxX + 1);
                var newY = wrap(robot.Y + robot.DY, maxY + 1);

                robot.X = newX;
                robot.Y = newY;
            }

        long q1 = 0, q2 = 0, q3 = 0, q4 = 0;
        var midX = maxX / 2;
        var midY = maxY / 2;

        foreach (var robot in robots)
        {
            if (robot.X < midX && robot.Y < midY)
                q1++;
            else if (robot.X > midX && robot.Y < midY)
                q2++;
            else if (robot.X < midX && robot.Y > midY)
                q3++;
            else if (robot.X > midX && robot.Y > midY)
                q4++;
        }

        Assert.Equal(expected, q1 * q2 * q3 * q4);

        int wrap(int value, int maxValue)
        {
            while (value >= maxValue)
                value -= maxValue;
            while (value < 0)
                value += maxValue;

            return value;
        }
    }

    [Theory]
    [InlineData("Data2.txt", 7055)]
    public void Part2(string filename, long expected)
    {
        var robots =
            (from line in File.ReadAllLines(filename)
             let match = robotRegex.Match(line)
             where match.Success
             select new Robot(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value), int.Parse(match.Groups[3].Value), int.Parse(match.Groups[4].Value))).ToArray();

        var maxX = robots.Max(r => r.X);
        var maxY = robots.Max(r => r.Y);
        var seconds = 0;

        while (true)
        {
            ++seconds;

            var image = new char[maxY + 1][];
            for (var y = 0; y <= maxY; ++y)
                image[y] = new char[maxX + 1];

            foreach (var robot in robots)
            {
                var newX = wrap(robot.X + robot.DX, maxX + 1);
                var newY = wrap(robot.Y + robot.DY, maxY + 1);

                robot.X = newX;
                robot.Y = newY;

                image[newY][newX] = '*';
            }

            foreach (var line in image)
                if (string.Join("", line.Select(c => c == '*' ? '*' : '.')).Contains("*******************************"))
                {
                    foreach (var line2 in image)
                        Console.WriteLine(string.Join("", line2.Select(c => c == '*' ? '*' : '.')));

                    Assert.Equal(seconds, expected);
                    return;
                }
        }

        int wrap(int value, int maxValue)
        {
            while (value >= maxValue)
                value -= maxValue;
            while (value < 0)
                value += maxValue;

            return value;
        }
    }
}
