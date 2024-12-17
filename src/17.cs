using System.Diagnostics;
using System.Numerics;

namespace aoc2024;

internal class Day17 : Day
{
    [DebuggerDisplay("A={A}, B={B}, C={C}")]
    class Registers
    {
        public long A = 0;
        public long B = 0;
        public long C = 0;

        public Registers()
        {

        }

        public Registers(long a, long b, long c)
        {
            A = a;
            B = b;
            C = c;
        }

        public Registers(Registers other)
        {
            A = other.A;
            B = other.B;
            C = other.C;
        }
    }

    class ComputeResult
    {
        public int? InstructionPointer;
        public int? Output;
    }

    private readonly Registers InitRegisters = new();
    private int[] Program = [];
    private int[] outputScratch = [];

    internal override void Parse()
    {
        var lines = Util.Parsing.ReadAllLines($"{GetDay()}").ToList();
        InitRegisters.A = int.Parse(lines[0].Split(' ')[2]);
        InitRegisters.B = int.Parse(lines[1].Split(' ')[2]);
        InitRegisters.C = int.Parse(lines[2].Split(' ')[2]);

        Program = lines[4].Split(' ')[1].Split(',').Select(int.Parse).ToArray();
        outputScratch = new int[Program.Length];

        RunTests();
    }

    private void RunTests()
    {
        {
            var r = new Registers{C = 9};
            int[] program = [2, 6];
            RunProgram(program, r);
            Debug.Assert(r.B == 1);

            r = new Registers{C = 9};
            RunProgramOptimized(program, ref r.A, ref r.B, ref r.C);
            Debug.Assert(r.B == 1);
        }

        {
            var r = new Registers{A = 10};
            int[] program = [5, 0, 5, 1, 5, 4];
            var output = RunProgram(program, r);
            Debug.Assert(output.SequenceEqual([0, 1, 2]));

            r = new Registers{A = 10};
            var o2 = RunProgramOptimized(program, ref r.A, ref r.B, ref r.C);
            Debug.Assert(o2.SequenceEqual([0,1,2]));
        }

        {
            var r = new Registers{A = 2024};
            int[] program = [0,1,5,4,3,0];
            var output = RunProgram(program, r);
            Debug.Assert(output.SequenceEqual([4,2,5,6,7,7,7,7,3,1,0]));
            Debug.Assert(r.A == 0);

            r = new Registers{A = 2024};
            var o2 = RunProgramOptimized(program, ref r.A, ref r.B, ref r.C);
            Debug.Assert(o2.SequenceEqual([4,2,5,6,7,7,7,7,3,1,0]));
            Debug.Assert(r.A == 0);
        }

        {
            var r = new Registers{B = 29};
            int[] program = [1,7];
            RunProgram(program, r);
            Debug.Assert(r.B == 26);

            r = new Registers{B = 29};
            RunProgramOptimized(program, ref r.A, ref r.B, ref r.C);
            Debug.Assert(r.B == 26);
        }

        {
            var r = new Registers{B = 2024, C = 43690};
            int[] program = [4,0];
            RunProgram(program, r);
            Debug.Assert(r.B == 44354);

            r = new Registers{B = 2024, C = 43690};
            RunProgramOptimized(program, ref r.A, ref r.B, ref r.C);
            Debug.Assert(r.B == 44354);
        }

        {
            var r = new Registers{A = 117440};
            int[] program = [0,3,5,4,3,0];
            var output = RunProgram(program, r);
            Debug.Assert(output.SequenceEqual([0,3,5,4,3,0]));

            r = new Registers{A = 117440};
            var o2 = RunProgramOptimized(program, ref r.A, ref r.B, ref r.C);
            Debug.Assert(o2.SequenceEqual([0,3,5,4,3,0]));
        }
    }

    private static long GetCombo(int operand, Registers registers)
    {
        return operand switch
        {
            0 or 1 or 2 or 3 => operand,
            4 => registers.A,
            5 => registers.B,
            6 => registers.C,
            _ => throw new Exception("invalid operand value for combo")
        };
    }

    private static ComputeResult? Compute(int instruction, int operand, Registers registers)
    {
        switch (instruction)
        {
            // adv
            case 0:
            {
                div(operand, registers, out registers.A);
                break;
            }

            // bdv
            case 6:
            {
                div(operand, registers, out registers.B);
                break;
            }

            // cdv
            case 7:
            {
                div(operand, registers, out registers.C);
                break;
            }

            // bxl
            case 1:
            {
                var v1 = registers.B;
                var v2 = operand;
                var result = v1 ^ v2;
                registers.B = result;
                break;
            }

            // bst
            case 2:
            {
                var v1 = GetCombo(operand, registers);
                var result = v1 % 8;
                registers.B = result;
                break;
            }

            // jnz
            case 3:
            {
                if (registers.A == 0)
                {
                    break;
                }

                return new ComputeResult {InstructionPointer = operand};
            }

            // bxc
            case 4:
            {
                var v1 = registers.B;
                var v2 = registers.C;
                var result = v1 ^ v2;
                registers.B = result;
                break;
            }

            // out
            case 5:
            {
                var v1 = GetCombo(operand, registers);
                var result = v1 % 8;
                var truncated = (int) result;
                return new ComputeResult {Output = truncated};
            }
        }

        return null;

        static void div(int op, Registers regs, out long store)
        {
            var numer = regs.A;
            var denom = BigInteger.Pow(2, (int)GetCombo(op, regs));

            var result = numer / denom;
            int truncated = (int)(result % int.MaxValue);
            store = truncated;
        }
    }

    private static List<int> RunProgram(int[] program, Registers registers)
    {
        List<int> output = [];

        int instructionPointer = 0;
        while (instructionPointer >= 0 && instructionPointer < program.Length - 1)
        {
            int instruction = program[instructionPointer];
            int operand = program[instructionPointer + 1];
            var result = Compute(instruction, operand, registers);
            if (result?.InstructionPointer.HasValue == true)
            {
                instructionPointer = result.InstructionPointer.Value;
            }
            else
            {
                instructionPointer += 2;
            }

            if (result?.Output != null)
            {
                output.Add(result.Output.Value);
            }
        }

        return output;
    }

    internal override string Part1()
    {
        var registers = new Registers(InitRegisters);
        var output = RunProgram(Program, registers);

        return $"Output: <+white>{string.Join(',', output)}";
    }

    private ReadOnlySpan<int> RunProgramOptimized(ReadOnlySpan<int> program, ref long a, ref long b, ref long c)
    {
        int instructionPointer = 0;
        int lengthCurr = 0;
        var val = new[] { 0, 1, 2, 3, a, b, c, 0 };
        while (instructionPointer < program.Length)
        {
            (a, b, c) = (val[4], val[5], val[6]);
            var instruction = program[instructionPointer];
            var operand = program[instructionPointer + 1];
            (instructionPointer, var result) = (instruction != 3 || a == 0 ? instructionPointer + 2 : operand, (int)val[operand]);

            (val[4], val[5], val[6]) = instruction switch
            {
                0 => (a >> result, b, c),
                1 => (a, b ^ operand, c),
                2 => (a, val[operand] & 7, c),
                4 => (a, b ^ c, c),
                6 => (a, a >> result, c),
                7 => (a, b, a >> result),
                _ => (a, b, c)
            };

            if (instruction == 5)
            {
                outputScratch[lengthCurr++] = result & 7;
            }
        }

        (a, b, c) = (val[4], val[5], val[6]);
        return outputScratch.AsSpan()[..lengthCurr];
    }

    internal override string Part2()
    {
        long a = 0;
        for (int i = 0; i < Program.Length; i++)
        {
            for (a <<= 3;; ++a)
            {
                long regA = a;
                long regB = 0;
                long regC = 0;
                var outputSpan = RunProgramOptimized(Program, ref regA, ref regB, ref regC);
                if (outputSpan[..(i + 1)].SequenceEqual(Program.AsSpan()[^(i + 1)..]))
                {
                    break;
                }
            }
        }

        return $"Value of A to repeat the program: <+white>{a}";
    }
}
