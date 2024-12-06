using Xunit.Internal;

namespace Day6;

public class Board
{
    static readonly Dictionary<char, (int dRow, int dCol, char nextDirection)> Directions = new() {
        { '^', (-1, 0, '>') },
        { '>', (0,  1, 'V') },
        { 'V', (1,  0, '<') },
        { '<', (0, -1, '^') }
    };

    readonly char[][] board;
    (int Row, int Column, char Direction) guardLocation;
    readonly Dictionary<(int Row, int Column), HashSet<char>> visitedSquares = [];

    public Board(string boardText)
    {
        board =
            boardText
                .Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries)
                .Select(line => line.ToCharArray())
                .ToArray();

        for (var row = 0; row < board.Length; ++row)
            for (var col = 0; col < board[row].Length; ++col)
                if (Directions.ContainsKey(board[row][col]))
                {
                    guardLocation = (row, col, board[row][col]);
                    break;
                }

    }

    Board(char[][] board, (int Row, int Column, char Direction) guardLocation)
    {
        this.board = board;
        this.guardLocation = guardLocation;
    }

    public bool CaughtInLoop => guardLocation.Row == -2 && guardLocation.Column == -2;

    public int Columns => board[0].Length;

    public bool Finished => guardLocation.Row < 0 && guardLocation.Column < 0;

    public (int Row, int Column, char Direction) GuardLocation =>
        guardLocation;

    public int Rows => board.Length;

    public (int Row, int Column)[] VisitedSquares =>
        [.. visitedSquares.Keys];

    public int VisitCount =>
        visitedSquares.Count;

    public void AddObstruction(int row, int col) =>
        board[row][col] = 'O';

    public void TakeAnAction()
    {
        if (Finished)
            throw new InvalidOperationException("Guard is already gone");

        var visits = visitedSquares.AddOrGet((guardLocation.Row, guardLocation.Column));
        if (visits.Contains(guardLocation.Direction))
        {
            guardLocation.Row = -2;
            guardLocation.Column = -2;
            return;
        }

        visits.Add(guardLocation.Direction);

        var (dRow, dCol, nextDirection) = Directions[guardLocation.Direction];
        var nextRow = guardLocation.Row + dRow;
        var nextCol = guardLocation.Column + dCol;

        board[guardLocation.Row][guardLocation.Column] = 'X';

        if (nextRow < 0 || nextRow >= board.Length || nextCol < 0 || nextCol >= board[nextRow].Length)
        {
            guardLocation.Row = -1;
            guardLocation.Column = -1;
            return;
        }

        if (board[nextRow][nextCol] is '#' or 'O')
        {
            guardLocation.Direction = nextDirection;
            return;
        }

        guardLocation.Row = nextRow;
        guardLocation.Column = nextCol;
    }

    public Board Clone()
    {
        var newBoard = board.Select(row => row.ToArray()).ToArray();
        return new Board(newBoard, (guardLocation.Row, guardLocation.Column, guardLocation.Direction));
    }
}

public class BoardTests
{
    [Fact]
    public void WhereIsTheGuard()
    {
        var board = new Board("""
            ....
            ..^.
            ....
            """);

        var guardLocation = board.GuardLocation;

        Assert.Equal(1, guardLocation.Row);
        Assert.Equal(2, guardLocation.Column);
        Assert.Equal('^', guardLocation.Direction);
        Assert.False(board.Finished);
    }

    [Theory]
    [InlineData('^', 0, 2)]
    [InlineData('>', 1, 3)]
    [InlineData('V', 2, 2)]
    [InlineData('<', 1, 1)]
    public void TakeAnSingleAction(char direction, int expectedRow, int expectedColumn)
    {
        var board = new Board($"""
            ....
            ..{direction}.
            ....
            """);

        board.TakeAnAction();
        var guardLocation = board.GuardLocation;

        Assert.Equal(expectedRow, guardLocation.Row);
        Assert.Equal(expectedColumn, guardLocation.Column);
        Assert.Equal(direction, guardLocation.Direction);
        Assert.False(board.Finished);
    }

    [Fact]
    public void WhatHappensWhenTheGuardLeaves()
    {
        var board = new Board("""
            ....
            ..^.
            ....
            """);

        board.TakeAnAction();
        board.TakeAnAction();
        var guardLocation = board.GuardLocation;

        Assert.Equal(-1, guardLocation.Row);
        Assert.Equal(-1, guardLocation.Column);
        Assert.True(board.Finished);
        Assert.False(board.CaughtInLoop);
    }

    [Fact]
    public void TakeAnActionAfterWereDone()
    {
        var board = new Board("""
            ....
            ..^.
            ....
            """);

        board.TakeAnAction();
        board.TakeAnAction();
        var ex = Record.Exception(board.TakeAnAction);

        Assert.NotNull(ex);
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal("Guard is already gone", ex.Message);
    }

    [Fact]
    public void WhenHittingObstruction_GuardTurns()
    {
        var board = new Board("""
            ..#.
            ..^.
            ....
            """);

        board.TakeAnAction();

        var guardLocation = board.GuardLocation;

        Assert.Equal(1, guardLocation.Row);
        Assert.Equal(2, guardLocation.Column);
        Assert.Equal('>', guardLocation.Direction);
    }

    [Fact]
    public void RecordsMovementSquares_SingleMove()
    {
        var board = new Board("""
            ....
            ..^.
            ....
            """);

        board.TakeAnAction();
        var visitCount = board.VisitCount;

        Assert.Equal(1, visitCount);
    }

    [Fact]
    public void CountAllSteps()
    {
        var board = new Board("""
            ....
            ..^.
            ....
            """);

        while (!board.Finished)
            board.TakeAnAction();

        var visitCount = board.VisitCount;
        Assert.Equal(2, visitCount);
    }

    [Fact]
    public void CanDetectInfiniteLoop()
    {
        var board = new Board("""
            ....#.....
            .........#
            ..........
            ..#.......
            .......#..
            ..........
            .#.O^.....
            ........#.
            #.........
            ......#...
            """);

        while (!board.Finished)
            board.TakeAnAction();

        Assert.True(board.CaughtInLoop);
    }
}

public class Tests
{
    [Theory]
    [InlineData("Data1.txt", 41)]
    [InlineData("Data2.txt", 5534)]
    public void Part1(string filename, int expected)
    {
        var board = new Board(File.ReadAllText(filename));

        while (!board.Finished)
            board.TakeAnAction();

        var result = board.VisitCount;

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("Data1.txt", 6)]
    [InlineData("Data2.txt", 2262)]
    public void Part2(string filename, int expected)
    {
        var text = File.ReadAllText(filename);
        var cleanBoard = new Board(text);
        var result = 0;

#if false  // This is the (somewhat) optimized version
        var firstRunBoard = cleanBoard.Clone();
        while (!firstRunBoard.Finished)
            firstRunBoard.TakeAnAction();

        foreach (var visitedSquare in firstRunBoard.VisitedSquares)
        {
            var row = visitedSquare.Row;
            var col = visitedSquare.Column;

#else  // This is the (mostly) unoptimized version)
        for (var row = 0; row < cleanBoard.Rows; ++row)
            for (var col = 0; col < cleanBoard.Columns; ++col)
            {
#endif
                if (row == cleanBoard.GuardLocation.Row && col == cleanBoard.GuardLocation.Column)
                    continue;

                var board = cleanBoard.Clone();
                board.AddObstruction(row, col);

                while (!board.Finished)
                    board.TakeAnAction();

                if (board.CaughtInLoop)
                    result++;
            }

        Assert.Equal(expected, result);
    }
}
