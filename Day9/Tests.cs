namespace Day9;

public class Tests
{
    [Theory]
    [InlineData("Data1.txt", 1928L)]
    [InlineData("Data2.txt", 6283404590840L)]
    public void Part1(string filename, long expected)
    {
        var lengths = File.ReadAllLines(filename).First().Select(x => x - '0').ToArray();
        var layout = new List<long?>(lengths.Sum());
        var isFile = true;
        var fileID = 0;

        foreach (var length in lengths)
        {
            foreach (var _ in Enumerable.Range(0, length))
                layout.Add(isFile ? fileID : null);

            isFile = !isFile;
            if (isFile)
                ++fileID;
        }

        var result = 0L;
        var frontIdx = 0;
        var backIdx = layout.Count - 1;

        while (frontIdx < layout.Count && frontIdx < backIdx)
        {
            if (layout[frontIdx] is null)
            {
                while (layout[backIdx] is null)
                    --backIdx;

                if (backIdx > frontIdx)
                {
                    layout[frontIdx] = layout[backIdx];
                    layout[backIdx] = null;
                }
            }

            if (frontIdx >= backIdx)
                break;

            result += layout[frontIdx]!.Value * frontIdx;
            ++frontIdx;
        }

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("Data1.txt", 2858L)]
    [InlineData("Data2.txt", 6304576012713L)]
    public void Part2(string filename, long expected)
    {
        var lengths = File.ReadAllLines(filename).First().Select(length => length - '0').ToArray();
        var layout = new bool[lengths.Sum()];
        var files = new List<(int StartIndex, int Length)>();

        var isFile = true;
        var nextIdx = 0;

        foreach (var length in lengths)
        {
            var startIdx = nextIdx;

            foreach (var _ in Enumerable.Range(0, length))
                layout[nextIdx++] = isFile;

            if (isFile)
                files.Add((startIdx, length));

            isFile = !isFile;
        }

        var result = 0L;

        for (var fileToMoveIdx = files.Count - 1; fileToMoveIdx >= 0; --fileToMoveIdx)
        {
            var fileToMove = files[fileToMoveIdx];
            var startIdx = 0;
            var moved = false;

            while (true)
            {
                while (startIdx < fileToMove.StartIndex && layout[startIdx])
                    startIdx++;

                if (startIdx >= fileToMove.StartIndex)
                    break;

                var endIdx = startIdx;

                while (endIdx < layout.Length && !layout[endIdx])
                    endIdx++;

                if (endIdx - startIdx >= fileToMove.Length)
                {
                    for (var count = 0; count < fileToMove.Length; ++count)
                    {
                        layout[startIdx + count] = true;
                        layout[fileToMove.StartIndex + count] = false;

                        result += (startIdx + count) * fileToMoveIdx;
                    }

                    moved = true;
                    break;
                }

                startIdx = endIdx + 1;
            }

            if (!moved)
                for (var count = 0; count < fileToMove.Length; ++count)
                    result += (fileToMove.StartIndex + count) * fileToMoveIdx;
        }

        Assert.Equal(expected, result);
    }
}
