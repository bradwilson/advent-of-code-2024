namespace Day4;

public class Grid(string lines)
{
    readonly char[][] grid =
        lines
            .Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries)
            .Select(line => line.ToCharArray())
            .ToArray();

    char? CharAt(int row, int col)
    {
        if (row < 0 || row >= grid.Length)
            return null;
        if (col < 0 || col >= grid[row].Length)
            return null;

        return grid[row][col];
    }

    public int CountCrossMas()
    {
        return FindLetter('A', (row, col) =>
        {
            if (IsMandS(CharAt(row + 1, col + 1), CharAt(row - 1, col - 1))
                    && IsMandS(CharAt(row + 1, col - 1), CharAt(row - 1, col + 1)))
                return 1;

            return 0;
        });

        static bool IsMandS(char? v1, char? v2) =>
            (v1 == 'M' && v2 == 'S') || (v1 == 'S' && v2 == 'M');
    }

    public int CountXmas()
    {
        List<(int dRow, int dCol)> directions =
        [
            (-1, -1), (-1, 0), (-1, 1),
            (0, -1), (0, 1),
            (1, -1), (1, 0), (1, 1),
        ];

        return FindLetter('X', (row, col) =>
        {
            var result = 0;

            foreach (var (dRow, dCol) in directions)
                if (CharAt(row + dRow, col + dCol) == 'M'
                        && CharAt(row + 2 * dRow, col + 2 * dCol) == 'A'
                        && CharAt(row + 3 * dRow, col + 3 * dCol) == 'S')
                    ++result;

            return result;
        });
    }

    int FindLetter(char letter, Func<int, int, int> callback)
    {
        var result = 0;

        for (var row = 0; row < grid.Length; ++row)
            for (var col = 0; col < grid[row].Length; ++col)
                if (CharAt(row, col) == letter)
                    result += callback(row, col);

        return result;
    }
}

public class GridTests
{
    public class CountXmas
    {
        [Theory]
        [InlineData("")]
        [InlineData("XMA")]
        [InlineData("AMX")]
        public void NoMatches(string lines)
        {
            var grid = new Grid(lines);

            var result = grid.CountXmas();

            Assert.Equal(0, result);
        }

        [Theory]
        [InlineData("XMAS")]
        [InlineData("SAMX")]
        [InlineData("""
        X
        M
        A
        S
        """)]
        [InlineData("""
        X
         M
          A
           S
        """)]
        [InlineData("""
        S
         A
          M
           X
        """)]
        public void OneMatch(string lines)
        {
            var grid = new Grid(lines);

            var result = grid.CountXmas();

            Assert.Equal(1, result);
        }

        [Theory]
        [InlineData("SAMXMAS")]
        [InlineData("""
        X
         M S
          A
         M S
        X
        """)]
        public void TwoMatches(string lines)
        {
            var grid = new Grid(lines);

            var result = grid.CountXmas();

            Assert.Equal(2, result);
        }
    }

    public class CountCrossMas
    {
        [Theory]
        [InlineData("", 0)]
        [InlineData("MAS", 0)]
        [InlineData("""
            M.S
            .A.
            M.S
            """, 1)]
        [InlineData("""
            S.S
            .A.
            M.M
            """, 1)]
        [InlineData("""
            M.M
            .A.
            S.S
            """, 1)]
        [InlineData("""
            S.M
            .A.
            S.M
            """, 1)]
        [InlineData("""
            S.M
            .A.
            S.M
            .A.
            S.M
            """, 2)]
        public void CountMatches(string lines, int expectedMatches)
        {
            var grid = new Grid(lines);

            var result = grid.CountCrossMas();

            Assert.Equal(expectedMatches, result);
        }
    }
}

public class Tests
{
    [Theory]
    [InlineData("Data1.txt", 18)]
    [InlineData("Data2.txt", 2567)]
    public void Part1(string filename, int expected)
    {
        var grid = new Grid(File.ReadAllText(filename));

        var result = grid.CountXmas();

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("Data1.txt", 9)]
    [InlineData("Data2.txt", 2029)]
    public void Part2(string filename, int expected)
    {
        var grid = new Grid(File.ReadAllText(filename));

        var result = grid.CountCrossMas();

        Assert.Equal(expected, result);
    }
}
