namespace Day12;

public class Grid
{
    static readonly List<(int dRow, int dCol)> directions = [(-1, 0), (1, 0), (0, -1), (0, 1)];
    readonly List<(char Crop, List<(int row, int col)> Locations)> plots = [];

    readonly char[][] grid;

    public Grid(string gridText)
    {
        grid =
            gridText
                .Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries)
                .Select(row => row.ToCharArray())
                .ToArray();

        HashSet<(int row, int col)> visited = [];

        for (var row = 0; row < grid.Length; ++row)
            for (var col = 0; col < grid[row].Length; ++col)
                if (!visited.Contains((row, col)))
                {
                    var list = new List<(int row, int col)>();
                    var identifier = grid[row][col];

                    visit(row, col, identifier, list);
                    plots.Add((identifier, list));
                }

        void visit(int row, int col, char identifier, List<(int row, int col)> list)
        {
            var crop = GetCrop(row, col);
            if (visited.Contains((row, col)) || crop is null || crop != identifier)
                return;

            list.Add((row, col));
            visited.Add((row, col));

            foreach (var (dRow, dCol) in directions)
                visit(row + dRow, col + dCol, identifier, list);
        }
    }

    public List<(char Crop, List<(int row, int col)> Locations)> Plots => plots;

    public char? GetCrop(int row, int col) =>
        row < 0 || row >= grid.Length || col < 0 || col >= grid[row].Length ? null : grid[row][col];

    public IEnumerable<(int row, int col, char direction)> GetPerimeters((char Crop, List<(int row, int col)> Locations) plot)
    {
        foreach (var (row, col) in plot.Locations)
            foreach (var (dRow, dCol, direction) in new[] { (-1, 0, 'U'), (1, 0, 'D'), (0, -1, 'L'), (0, 1, 'R') })
                if (GetCrop(row + dRow, col + dCol) != plot.Crop)
                    yield return (row, col, direction);
    }
}

public class UnitTest1
{
    [Theory]
    [InlineData("Data1.txt", 1930)]
    [InlineData("Data2.txt", 1477762)]
    public void Part1(string filename, long expected)
    {
        var grid = new Grid(File.ReadAllText(filename));

        var result = grid.Plots.Sum(plot => plot.Locations.Count * grid.GetPerimeters(plot).Count());

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("Data1.txt", 1206)]
    [InlineData("Data2.txt", 923480)]
    public void Part2(string filename, long expected)
    {
        var grid = new Grid(File.ReadAllText(filename));

        var result = grid.Plots.Sum(plot =>
        {
            var perimeters = grid.GetPerimeters(plot).ToArray();
            var horizontals =
                perimeters
                    .Where(p => p.direction is 'U' or 'D')
                    .GroupBy(p => (p.row, p.direction), p => p.col)
                    .Select(group => group.OrderBy(x => x).ToArray())
                    .ToArray();
            var verticals =
                perimeters
                    .Where(p => p.direction is 'L' or 'R')
                    .GroupBy(p => (p.col, p.direction), p => p.row)
                    .Select(group => group.OrderBy(x => x).ToArray())
                    .ToArray();

            return plot.Locations.Count * (countSides(horizontals) + countSides(verticals));
        });

        Assert.Equal(expected, result);

        static long countSides(int[][] locationsByOrigin)
        {
            var result = 0L;

            foreach (var locations in locationsByOrigin)
            {
                result++;

                for (int col = 1; col < locations.Length; ++col)
                    if (locations[col - 1] + 1 != locations[col])
                        result++;
            }

            return result;
        }
    }
}
