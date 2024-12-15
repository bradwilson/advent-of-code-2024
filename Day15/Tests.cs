namespace Day15;

public class StorageRoom
{
    Dictionary<char, (int dRow, int dCol)> directions = new() { { '<', (0, -1) }, { '>', (0, 1) }, { '^', (-1, 0) }, { 'v', (1, 0) } };
    char[][] grid;
    char[] instructions;
    (int row, int col) robot;

    public StorageRoom(string input, bool doubleWide = false)
    {
        var lines = input.Split(Environment.NewLine).ToList();
        var splitIdx = lines.IndexOf(string.Empty);

        grid =
            lines[0..splitIdx]
                .Select(row => row.ToCharArray())
                .ToArray();

        if (doubleWide)
            grid =
                grid.Select(row => string.Join("", row.Select(node => node switch
                {
                    '.' => "..",
                    '@' => "@.",
                    '#' => "##",
                    'O' => "[]",
                    _ => throw new InvalidOperationException(),
                })).ToCharArray()).ToArray();

        instructions =
            string.Join("", lines[(splitIdx + 1)..]).ToCharArray();

        for (var row = 0; row < grid.Length; ++row)
            for (var col = 0; col < grid[row].Length; ++col)
                if (GetNode(row, col) == '@')
                    robot = (row, col);
    }

    public int GetGPSValue()
    {
        var result = 0;

        for (var row = 0; row < grid.Length; ++row)
            for (var col = 0; col < grid[row].Length; ++col)
                if (GetNode(row, col) is 'O' or '[')
                    result += row * 100 + col;

        return result;
    }

    char GetNode(int row, int col) =>
        row < 0 || row >= grid.Length || col < 0 || col >= grid[row].Length ? '#' : grid[row][col];

    public void PerformMovements()
    {
        foreach (var instruction in instructions)
        {
            var (dRow, dCol) = directions[instruction];
            if (TryMove(robot.row, robot.col, dRow, dCol))
                robot = (robot.row + dRow, robot.col + dCol);
        }
    }

    public override string ToString() =>
        string.Join(Environment.NewLine, grid.Select(row => string.Join("", row)));

    bool TryMove(int row, int col, int dRow, int dCol)
    {
        var current = GetNode(row, col);
        var adjacent = GetNode(row + dRow, col + dCol);
        if (adjacent is '#')
            return false;

        if (current is ']' && dCol is 0)
            return TryMove(row, col - 1, dRow, dCol);

        if (current is '[' && dCol is 0)
        {
            var adjacentRight = GetNode(row + dRow, col + 1);
            var gridCopy = grid.Select(row => row.ToArray()).ToArray();

            var success = (adjacent, adjacentRight) switch
            {
                ('.', '.') => true,
                ('.', '[') => TryMove(row + dRow, col + 1, dRow, dCol),
                (']', '.') => TryMove(row + dRow, col - 1, dRow, dCol),
                (']', '[') => TryMove(row + dRow, col - 1, dRow, dCol) && TryMove(row + dRow, col + 1, dRow, dCol),
                ('[', ']') => TryMove(row + dRow, col, dRow, dCol),
                _ => false,
            };

            if (success)
            {
                grid[row + dRow][col] = current;
                grid[row + dRow][col + 1] = GetNode(row, col + 1);
                grid[row][col] = '.';
                grid[row][col + 1] = '.';
                return true;
            }

            grid = gridCopy;
            return false;
        }

        if (adjacent is '.' || TryMove(row + dRow, col + dCol, dRow, dCol))
        {
            grid[row][col] = '.';
            grid[row + dRow][col + dCol] = current;
            return true;
        }

        return false;
    }
}

public class StorageRoomTests
{
    public class SingleWide
    {
        [Theory]
        [InlineData("""
        ########
        #..O.O.#
        ##@.O..#
        #...O..#
        #.#.O..#
        #...O..#
        #......#
        ########

        <
        """, """
        ########
        #..O.O.#
        ##@.O..#
        #...O..#
        #.#.O..#
        #...O..#
        #......#
        ########
        """)]
        [InlineData("""
        ########
        #..O.O.#
        ##@.O..#
        #...O..#
        #.#.O..#
        #...O..#
        #......#
        ########

        <^
        """, """
        ########
        #.@O.O.#
        ##..O..#
        #...O..#
        #.#.O..#
        #...O..#
        #......#
        ########
        """)]
        [InlineData("""
        ########
        #..O.O.#
        ##@.O..#
        #...O..#
        #.#.O..#
        #...O..#
        #......#
        ########

        <^^>
        """, """
        ########
        #..@OO.#
        ##..O..#
        #...O..#
        #.#.O..#
        #...O..#
        #......#
        ########
        """)]
        [InlineData("""
        ########
        #..O.O.#
        ##@.O..#
        #...O..#
        #.#.O..#
        #...O..#
        #......#
        ########

        <^^>>>
        """, """
        ########
        #...@OO#
        ##..O..#
        #...O..#
        #.#.O..#
        #...O..#
        #......#
        ########
        """)]
        [InlineData("""
        ########
        #..O.O.#
        ##@.O..#
        #...O..#
        #.#.O..#
        #...O..#
        #......#
        ########

        <^^>>>vv
        """, """
        ########
        #....OO#
        ##..@..#
        #...O..#
        #.#.O..#
        #...O..#
        #...O..#
        ########
        """)]
        [InlineData("""
        ########
        #..O.O.#
        ##@.O..#
        #...O..#
        #.#.O..#
        #...O..#
        #......#
        ########

        <^^>>>vv<v>>v<<
        """, """
        ########
        #....OO#
        ##.....#
        #.....O#
        #.#O@..#
        #...O..#
        #...O..#
        ########
        """)]
        [InlineData("""
        ##########
        #..O..O.O#
        #......O.#
        #.OO..O.O#
        #..O@..O.#
        #O#..O...#
        #O..O..O.#
        #.OO.O.OO#
        #....O...#
        ##########

        <vv>^<v^>v>^vv^v>v<>v^v<v<^vv<<<^><<><>>v<vvv<>^v^>^<<<><<v<<<v^vv^v>^
        vvv<<^>^v^^><<>>><>^<<><^vv^^<>vvv<>><^^v>^>vv<>v<<<<v<^v>^<^^>>>^<v<v
        ><>vv>v^v^<>><>>>><^^>vv>v<^^^>>v^v^<^^>v^^>v^<^v>v<>>v^v^<v>v^^<^^vv<
        <<v<^>>^^^^>>>v^<>vvv^><v<<<>^^^vv^<vvv>^>v<^^^^v<>^>vvvv><>>v^<<^^^^^
        ^><^><>>><>^^<<^^v>>><^<v>^<vv>>v>>>^v><>^v><<<<v>>v<v<v>vvv>^<><<>^><
        ^>><>^v<><^vvv<^^<><v<<<<<><^v<<<><<<^^<v<^^^><^>>^<v^><<<^>>^v<v^v<v^
        >^>>^v>vv>^<<^v<>><<><<v<<v><>v<^vv<<<>^^v^>^^>>><<^v>>v^v><^^>>^<>vv^
        <><^^>^^^<><vvvvv^v<v<<>^v<v>v<<^><<><<><<<^^<<<^<<>><<><^^^>^^<>^>v<>
        ^^>vv<^v^v<vv>^<><v<^v>^^^>>>^^vvv^>vvv<>>>^<^>>>>>^<<^v>^vvv<>^<><<v>
        v^^>>><<^^<>>^v^<v^vv<>v^<<>^<^v^v><^<<<><<^<v><v<>vv>>v><v^<vv<>v^<<^
        """, """
        ##########
        #.O.O.OOO#
        #........#
        #OO......#
        #OO@.....#
        #O#.....O#
        #O.....OO#
        #O.....OO#
        #OO....OO#
        ##########
        """)]
        public void MovementsShowNewBoard(string input, string expectedRoom)
        {
            var storageRoom = new StorageRoom(input);

            storageRoom.PerformMovements();

            Assert.Equal<object>(expectedRoom, storageRoom.ToString());
        }
    }

    public class DoubleWide
    {
        [Theory]
        [InlineData("""
            #######
            #...#.#
            #.....#
            #..OO@#
            #..O..#
            #.....#
            #######


            """,
            """
            ##############
            ##......##..##
            ##..........##
            ##....[][]@.##
            ##....[]....##
            ##..........##
            ##############
            """)]
        [InlineData("""
            #######
            #...#.#
            #.....#
            #..OO@#
            #..O..#
            #.....#
            #######

            <
            """, """
            ##############
            ##......##..##
            ##..........##
            ##...[][]@..##
            ##....[]....##
            ##..........##
            ##############
            """)]
        [InlineData("""
            #######
            #...#.#
            #.....#
            #..OO@#
            #..O..#
            #.....#
            #######

            <vv<<^
            """, """
            ##############
            ##......##..##
            ##...[][]...##
            ##....[]....##
            ##.....@....##
            ##..........##
            ##############
            """)]
        [InlineData("""
            #######
            #...#.#
            #.....#
            #..OO@#
            #..O..#
            #.....#
            #######

            <vv<<^^<<^^
            """, """
            ##############
            ##...[].##..##
            ##...@.[]...##
            ##....[]....##
            ##..........##
            ##..........##
            ##############
            """)]
        [InlineData("""
            ##########
            #..O..O.O#
            #......O.#
            #.OO..O.O#
            #..O@..O.#
            #O#..O...#
            #O..O..O.#
            #.OO.O.OO#
            #....O...#
            ##########

            <vv>^<v^>v>^vv^v>v<>v^v<v<^vv<<<^><<><>>v<vvv<>^v^>^<<<><<v<<<v^vv^v>^
            vvv<<^>^v^^><<>>><>^<<><^vv^^<>vvv<>><^^v>^>vv<>v<<<<v<^v>^<^^>>>^<v<v
            ><>vv>v^v^<>><>>>><^^>vv>v<^^^>>v^v^<^^>v^^>v^<^v>v<>>v^v^<v>v^^<^^vv<
            <<v<^>>^^^^>>>v^<>vvv^><v<<<>^^^vv^<vvv>^>v<^^^^v<>^>vvvv><>>v^<<^^^^^
            ^><^><>>><>^^<<^^v>>><^<v>^<vv>>v>>>^v><>^v><<<<v>>v<v<v>vvv>^<><<>^><
            ^>><>^v<><^vvv<^^<><v<<<<<><^v<<<><<<^^<v<^^^><^>>^<v^><<<^>>^v<v^v<v^
            >^>>^v>vv>^<<^v<>><<><<v<<v><>v<^vv<<<>^^v^>^^>>><<^v>>v^v><^^>>^<>vv^
            <><^^>^^^<><vvvvv^v<v<<>^v<v>v<<^><<><<><<<^^<<<^<<>><<><^^^>^^<>^>v<>
            ^^>vv<^v^v<vv>^<><v<^v>^^^>>>^^vvv^>vvv<>>>^<^>>>>>^<<^v>^vvv<>^<><<v>
            v^^>>><<^^<>>^v^<v^vv<>v^<<>^<^v^v><^<<<><<^<v><v<>vv>>v><v^<vv<>v^<<^
            """, """
            ####################
            ##[].......[].[][]##
            ##[]...........[].##
            ##[]........[][][]##
            ##[]......[]....[]##
            ##..##......[]....##
            ##..[]............##
            ##..@......[].[][]##
            ##......[][]..[]..##
            ####################
            """)]
        public void CanMoveInDoubleWideSpace(string input, string expectedGrid)
        {
            var room = new StorageRoom(input, doubleWide: true);

            room.PerformMovements();

            Assert.Equal<object>(expectedGrid, room.ToString());
        }
    }
}

public class Tests
{
    [Theory]
    [InlineData("Data1.txt", 10092)]
    [InlineData("Data2.txt", 1360570)]
    public void Part1(string filename, int expected)
    {
        var storageRoom = new StorageRoom(File.ReadAllText(filename));

        storageRoom.PerformMovements();

        Assert.Equal(expected, storageRoom.GetGPSValue());
    }

    [Theory]
    [InlineData("Data1.txt", 9021)]
    [InlineData("Data2.txt", 1381446)]
    public void Part2(string filename, int expected)
    {
        var storageRoom = new StorageRoom(File.ReadAllText(filename), doubleWide: true);

        storageRoom.PerformMovements();

        Assert.Equal(expected, storageRoom.GetGPSValue());
    }
}
