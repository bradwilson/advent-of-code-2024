namespace Day10;

public class HikingMap
{
    readonly List<(int dRow, int dCol)> directions = [(-1, 0), (1, 0), (0, -1), (0, 1)];
    readonly int[][] grid;
    readonly List<(int row, int col)> trailheads = [];

    public HikingMap(string map)
    {
        grid =
            map
                .Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries)
                .Select(row => row.Select(node => node - '0').ToArray())
                .ToArray();

        for (var row = 0; row < grid.Length; ++row)
            for (var col = 0; col < grid[row].Length; ++col)
                if (GetGridValue(row, col) == 0)
                    trailheads.Add((row, col));
    }

    public IReadOnlyList<(int row, int col)> Trailheads =>
        trailheads;

    public int? GetGridValue(int row, int col) =>
        row < 0 || row >= grid.Length || col < 0 || col >= grid[row].Length ? null : grid[row][col];

    public List<(int row, int col)> GetReachableEndPoints(int row, int col)
    {
        var current = grid[row][col];
        if (current == 9)
            return [(row, col)];

        var result = new List<(int row, int col)>();

        foreach (var (dRow, dCol) in directions)
        {
            var nextRow = row + dRow;
            var nextCol = col + dCol;
            var next = GetGridValue(nextRow, nextCol);

            if (next == current + 1)
                result.AddRange(GetReachableEndPoints(nextRow, nextCol));
        }

        return result;
    }
}

public class HikingMapTests
{
    [Theory]
    [InlineData("""
        0123
        1234
        8765
        9876
        """, 1)]
    [InlineData("""
        ...0...
        ...1...
        ...2...
        6543456
        7.....7
        8.....8
        9.....9
        """, 2)]
    [InlineData("""
        ..90..9
        ...1.98
        ...2..7
        6543456
        765.987
        876....
        987....
        """, 4)]
    [InlineData("""
        10..9..
        2...8..
        3...7..
        4567654
        ...8..3
        ...9..2
        .....01
        """, 3)]
    public void CalculateTrailheadScores(string map, int expected)
    {
        var hikingMap = new HikingMap(map);
        var result = 0;

        foreach (var (row, col) in hikingMap.Trailheads)
            result += hikingMap.GetReachableEndPoints(row, col).ToHashSet().Count;

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("""
        .....0.
        ..4321.
        ..5..2.
        ..6543.
        ..7..4.
        ..8765.
        ..9....
        """, 3)]
    public void CalculateTrailheadRatings(string map, int expected)
    {
        var hikingMap = new HikingMap(map);
        var result = 0;

        foreach (var (row, col) in hikingMap.Trailheads)
            result += hikingMap.GetReachableEndPoints(row, col).Count;

        Assert.Equal(expected, result);
    }
}

public class Tests
{
    [Theory]
    [InlineData("Data1.txt", 36)]
    [InlineData("Data2.txt", 538)]
    public void Part1(string filename, int expected)
    {
        var hikingMap = new HikingMap(File.ReadAllText(filename));
        var result = 0;

        foreach (var (row, col) in hikingMap.Trailheads)
            result += hikingMap.GetReachableEndPoints(row, col).ToHashSet().Count;

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("Data1.txt", 81)]
    [InlineData("Data2.txt", 1110)]
    public void Part2(string filename, int expected)
    {
        var hikingMap = new HikingMap(File.ReadAllText(filename));
        var result = 0;

        foreach (var (row, col) in hikingMap.Trailheads)
            result += hikingMap.GetReachableEndPoints(row, col).Count;

        Assert.Equal(expected, result);
    }
}
