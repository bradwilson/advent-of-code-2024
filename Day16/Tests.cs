namespace Day16;

public class Maze
{
    static readonly (int dRow, int dCol)[] directions = [(-1, 0), (0, 1), (1, 0), (0, -1)]; // 0 == north, 1 == east, 2 == south, 3 == west
    readonly (int row, int col) end;
    readonly char[][] grid;
    readonly (int row, int col) start;

    public Maze(string input)
    {
        grid =
            input
                .Split(Environment.NewLine)
                .Select(row => row.ToCharArray())
                .ToArray();

        for (var row = 0; row < grid.Length; ++row)
            for (var col = 0; col < grid[row].Length; ++col)
            {
                if (grid[row][col] == 'S')
                    start = (row, col);
                else if (grid[row][col] == 'E')
                    end = (row, col);
            }
    }

    public (int cheapest, int nodeCountOnCheapestPaths) Solve()
    {
        // Using a priority queue ensures that we always compute the best path values by keeping us on
        // the optimal path.
        var queue = new PriorityQueue<(int row, int col, int direction), int>();

        // Start assuming the worst case scenario
        var cheapest = int.MaxValue;

        // First, we walk the path forward from the start, headed East, and find not only the cheapest route,
        // but the lowest cost from the starting point to every other valid point. This also acts as a protection
        // against looping by bailing out if we've already seen the given row & column in the given direction.
        var forwardCosts = new Dictionary<(int row, int col, int direction), int>();
        queue.Enqueue((start.row, start.col, 1), 0);

        while (queue.TryDequeue(out var current, out var distance))
        {
            // We know this is the lowest cost to get here because we used a priority queue
            if (current.row == end.row && current.col == end.col)
                cheapest = Math.Min(cheapest, distance);

            // Prevent loops
            if (forwardCosts.ContainsKey((current.row, current.col, current.direction)))
                continue;

            // Record the cost
            forwardCosts[(current.row, current.col, current.direction)] = distance;

            // Try to walk forward if we're not pointing at a wall
            var next = directions[current.direction];
            if (grid[current.row + next.dRow][current.col + next.dCol] != '#')
                queue.Enqueue((current.row + next.dRow, current.col + next.dCol, current.direction), distance + 1);

            // Also try the clockwise/counterclockwise directions with the higher cost
            queue.Enqueue((current.row, current.col, (current.direction + 1) % 4), distance + 1000);
            queue.Enqueue((current.row, current.col, (current.direction + 3) % 4), distance + 1000);
        }

        // Now, we want to compute the cost from the end, to find the lowest costs from every valid point
        // to the end point. Since we don't know which direction we'll arrive at for the end point, this time
        // we assume any incoming direction is valid.
        var backwardCosts = new Dictionary<(int row, int col, int direction), int>();
        foreach (var direction in new[] { 0, 1, 2, 3 })
            queue.Enqueue((end.row, end.col, direction), 0);

        while (queue.TryDequeue(out var current, out var distance))
        {
            // Prevent loops
            if (backwardCosts.ContainsKey((current.row, current.col, current.direction)))
                continue;

            // Record the cost
            backwardCosts[(current.row, current.col, current.direction)] = distance;

            // Try to walk backward if we're not backed up against a wall
            var previous = directions[(current.direction + 2) % 4];
            if (grid[current.row + previous.dRow][current.col + previous.dCol] != '#')
                queue.Enqueue((current.row + previous.dRow, current.col + previous.dCol, current.direction), distance + 1);

            // Also try the clockwise/counterclockwise directions with the higher cost
            queue.Enqueue((current.row, current.col, (current.direction + 1) % 4), distance + 1000);
            queue.Enqueue((current.row, current.col, (current.direction + 3) % 4), distance + 1000);
        }

        // Now that we have all the costs from start to (r,c), and from end to (r,c), we just walk every valid
        // space (that is, every space that has a cost, which should be every non-wall piece), and if that space
        // is on the optimal path (meaning, start distance + end distance == cheapest), then that node is part
        // of at least one optimal path. Put it in the list so we can count it.
        var optimalNodes = new HashSet<(int row, int col)>();

        for (var row = 0; row < grid.Length; ++row)
            for (var col = 0; col < grid[row].Length; ++col)
                for (var direction = 0; direction < 4; ++direction)
                    if (forwardCosts.TryGetValue((row, col, direction), out var forwardDistance) &&
                            backwardCosts.TryGetValue((row, col, direction), out var backwardDistance) &&
                            forwardDistance + backwardDistance == cheapest)
                        optimalNodes.Add((row, col));

        return (cheapest, optimalNodes.Count);
    }
}

public class Tests
{
    [Theory]
    [InlineData("Data0.txt", 7036, 45)]
    [InlineData("Data1.txt", 11048, 64)]
    [InlineData("Data2.txt", 85480, 518)]
    public void PathTests(string filename, int expectedCheapest, int expectedNodeCount)
    {
        var maze = new Maze(File.ReadAllText(filename));

        var solution = maze.Solve();

        Assert.Equal(expectedCheapest, solution.cheapest);
        Assert.Equal(expectedNodeCount, solution.nodeCountOnCheapestPaths);
    }
}
