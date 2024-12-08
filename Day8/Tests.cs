using Xunit.Internal;

namespace Day8;

public readonly record struct GridLocation(int Row, int Column)
{
    public bool IsValid(Grid grid) =>
        Row >= 0 && Row < grid.RowCount && Column >= 0 && Column < grid.ColumnCount;

    public static GridLocation operator +(GridLocation location, GridOffset offset) =>
        new(location.Row + offset.RowDelta, location.Column + offset.ColumnDelta);

    public static GridLocation operator -(GridLocation location, GridOffset offset) =>
        new(location.Row - offset.RowDelta, location.Column - offset.ColumnDelta);
}

public readonly record struct GridOffset(int RowDelta, int ColumnDelta)
{
    public static GridOffset Between(GridLocation location1, GridLocation location2) =>
        new(location1.Row - location2.Row, location1.Column - location2.Column);
}

public class Grid
{
    readonly char[][] grid;
    readonly Dictionary<char, List<GridLocation>> locationsByAntenna = [];

    public Grid(string[] gridLines)
    {
        grid = gridLines.Select(line => line.ToCharArray()).ToArray();

        for (var row = 0; row < grid.Length; ++row)
            for (var column = 0; column < grid[row].Length; ++column)
            {
                var antenna = grid[row][column];
                if (antenna == '.')
                    continue;

                var locations = locationsByAntenna.AddOrGet(antenna);
                locations.Add(new(row, column));
            }
    }

    public int AntiNodeCount =>
        grid.Select(row => row.Select(node => node == '#' ? 1 : 0).Sum()).Sum();

    public int ColumnCount => grid[0].Length;

    public int RowCount => grid.Length;

    public void PerAntennaPair(Action<GridLocation, GridLocation> callback)
    {
        foreach (var locations in locationsByAntenna.Values)
            for (var location1Idx = 0; location1Idx < locations.Count - 1; ++location1Idx)
                for (var location2Idx = location1Idx + 1; location2Idx < locations.Count; ++location2Idx)
                    callback(locations[location1Idx], locations[location2Idx]);
    }

    public bool SetAntiNode(GridLocation location)
    {
        if (!location.IsValid(this))
            return false;

        grid[location.Row][location.Column] = '#';
        return true;
    }
}

public class Tests
{
    [Theory]
    [InlineData("Data1.txt", 14)]
    [InlineData("Data2.txt", 390)]
    public void Part1(string filename, long expected)
    {
        var grid = new Grid(File.ReadAllLines(filename));

        grid.PerAntennaPair((location1, location2) =>
        {
            var offset = GridOffset.Between(location1, location2);
            grid.SetAntiNode(location1 + offset);
            grid.SetAntiNode(location2 - offset);
        });

        Assert.Equal(expected, grid.AntiNodeCount);
    }

    [Theory]
    [InlineData("Data1.txt", 34)]
    [InlineData("Data2.txt", 1246)]
    public void Part2(string filename, long expected)
    {
        var grid = new Grid(File.ReadAllLines(filename));

        grid.PerAntennaPair((location1, location2) =>
        {
            var offset = GridOffset.Between(location1, location2);

            while (grid.SetAntiNode(location1))
                location1 += offset;
            while (grid.SetAntiNode(location2))
                location2 -= offset;
        });

        Assert.Equal(expected, grid.AntiNodeCount);
    }
}
