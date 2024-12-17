namespace Day17;

public class Computer
{
    public long A { get; set; }
    public long B { get; set; }
    public long C { get; set; }

    public static (Computer computer, long[] program) Parse(string setup)
    {
        var lines = setup.Split(Environment.NewLine);
        return (new() { A = long.Parse(lines[0][12..]), B = long.Parse(lines[1][12..]), C = long.Parse(lines[2][12..]) }, lines[4][9..].Split(',').Select(long.Parse).ToArray());
    }

    public static long Pow(long @base, long exponent)
    {
        var result = 1L;

        while (exponent-- > 0)
            result *= @base;

        return result;
    }

    public List<long> Run(long[] program, bool outputMatchesProgram = false)
    {
        var ptr = 0L;
        var output = new List<long>();

        while (ptr < program.Length)
        {
            var instruction = program[ptr];
            var operand = program[ptr + 1];

            switch (instruction)
            {
                case 0:
                    A = A / Pow(2, combo(operand));
                    ptr += 2;
                    break;

                case 1:
                    B = B ^ operand;
                    ptr += 2;
                    break;

                case 2:
                    B = combo(operand) % 8;
                    ptr += 2;
                    break;

                case 3:
                    if (A is not 0)
                        ptr = operand;
                    else
                        ptr += 2;
                    break;

                case 4:
                    B = B ^ C;
                    ptr += 2;
                    break;

                case 5:
                    var val = combo(operand) % 8;
                    if (outputMatchesProgram && val != program[output.Count])
                        return output;

                    output.Add(val);
                    ptr += 2;
                    break;

                case 6:
                    B = A / Pow(2, combo(operand));
                    ptr += 2;
                    break;

                case 7:
                    C = A / Pow(2, combo(operand));
                    ptr += 2;
                    break;

                default:
                    throw new InvalidOperationException($"Unknown instruction {instruction}");
            }
        }

        return output;

        long combo(long operand) =>
            operand switch
            {
                0 => 0,
                1 => 1,
                2 => 2,
                3 => 3,
                4 => A,
                5 => B,
                6 => C,
                _ => throw new InvalidOperationException($"Unknown combo operand {operand}"),
            };
    }
}

public class Tests
{
    [Theory]
    [InlineData("Data1.txt", "4,6,3,5,6,3,5,2,1,0")]
    [InlineData("Data2.txt", "7,1,3,7,5,1,0,3,4")]
    public void Part1(string filename, string expected)
    {
        var (computer, program) = Computer.Parse(File.ReadAllText(filename));

        var output = computer.Run(program);

        Assert.Equal(expected, string.Join(",", output));
    }

    [Theory]
    [InlineData("Data2.txt", -1)]
    public void Part2(string filename, long expected)
    {
        var (computer, program) = Computer.Parse(File.ReadAllText(filename));
        var programText = string.Join(",", program);

        var iteration = 1;
        var longest = 0;

        while (true)
        {
            // This non-generic answer was arrived at through hacking and evil magicks 😠 so instead I'll explain
            // how I arrived here.
            //
            // The number is not solvable directly through brute force, but an analysis of the program shows
            // that the (octal) input value of A will show stability in its suffix as you correctly solve more
            // digits. Octal values are much easier to see the patterns in, because so much of this program uses
            // values that are modulus 8.
            //
            // Below we start with an empty octal suffix (so, a linear search). Just keep adding digits to the octal
            // suffix once the pattern emerges, then restart the search; the provided octal suffix will constrain
            // the space that you're searching through. Keep adding a few digits each iteration and you should be
            // able to eventually get a solvable problem in small enough time. For me, once I got 9 octal digits
            // in my suffix, the search space could finish in ~ 2 seconds on a 5900X.
            //
            // Good luck with the evil magicks.

            var octalSuffix = "";
            var replacement = octalSuffix == "" ? iteration : iteration * Computer.Pow(8, octalSuffix.Length) + Convert.ToInt64(octalSuffix, 8);

            computer.A = replacement;
            computer.B = 0;
            computer.C = 0;

            var output = computer.Run(program, outputMatchesProgram: true);
            var outputText = string.Join(",", output);

            if (outputText.Length > longest)
            {
                longest = outputText.Length;
                Console.WriteLine($"Octal '{Convert.ToString(replacement, 8)}' =>  {outputText}");
            }

            if (outputText == programText)
            {
                Assert.Equal(expected, replacement);
                return;
            }

            iteration++;
        }
    }
}
