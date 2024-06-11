using System;
using System.Collections.Generic;
using System.IO;
namespace LoadBinary
{
        class Program
{
    // Loadbin sig
    // #[LOADBIN]
    static void Main(string[] args)
    {
        //--version, made by AzureianGH, 2024, GNU GPL v3
        if (args.Length == 1 && args[0] == "--version")
        {
            Console.WriteLine("loadbinary v2.0.0");
            Console.WriteLine("Made by AzureianGH, 2024");
            Console.WriteLine("Licensed under GNU GPL v3");

            return;
        }
        else if (args.Length == 1 && args[0] == "--help")
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Usage: loadbinary <from-file> <to-file>");
            Console.WriteLine("Usage: loadbinary -p <from-file>");
            Console.WriteLine("Usage: loadbinary -o <from-file> <to-file>");
            Console.WriteLine("Usage: loadbinary -p --no-db0 <from-file>");
            Console.WriteLine("Usage: loadbinary -p --no-db <from-file>");
            Console.WriteLine("Usage: loadbinary -o --no-db0 <from-file> <to-file>");
            Console.WriteLine("Usage: loadbinary -o --no-db <from-file> <to-file>"); // Remove all DB instructions no matter what
            Console.WriteLine("Usage: loadbinary -p -a <address> <from-file>");
            Console.WriteLine("Usage: loadbinary -o -a <address> <from-file> <to-file>");
            Console.ResetColor();
            return;
        }

        //if the user wants to print the disassembled output
        if (args[0] == "-p")
        {
            if (args.Length < 2)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Usage: loadbinary -p <from-file>");
                Console.ResetColor();
                return;
            }

            // Handle --no-db and --no-db0 options
            bool removeDbInstructions = false;
            int fileIndex = 1;
            if (args.Length > 2 && (args[1] == "--no-db" || args[1] == "--no-db0"))
            {
                removeDbInstructions = true;
                fileIndex++;
            }

            byte[] fromFile = File.ReadAllBytes(args[fileIndex]);
            Decompiler decompiler = new Decompiler();
            string disassembled = decompiler.Disassemble(fromFile);
            
            if (removeDbInstructions)
            {
                disassembled = RemoveDbInstructions(disassembled, args[1] == "--no-db0");
            }

            Console.WriteLine(disassembled);
            return;
        }

        // -o if the user wants to output the disassembled output to a file
        if (args[0] == "-o")
        {
            if (args.Length < 3 || (args.Length > 3 && (args[2] == "--no-db" || args[2] == "--no-db0")))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Usage: loadbinary -o <from-file> <to-file>");
                Console.WriteLine("Usage: loadbinary -o --no-db0 <from-file> <to-file>");
                Console.WriteLine("Usage: loadbinary -o --no-db <from-file> <to-file>");
                Console.ResetColor();
                return;
            }

            // Handle --no-db and --no-db0 options
            bool removeDbInstructions = false;
            int fileIndex = 1;
            if (args.Length > 3 && (args[2] == "--no-db" || args[2] == "--no-db0"))
            {
                removeDbInstructions = true;
                fileIndex++;
            }

            byte[] fromFile = File.ReadAllBytes(args[fileIndex]);
            Decompiler decompiler = new Decompiler();
            string disassembled = decompiler.Disassemble(fromFile);
            
            if (removeDbInstructions)
            {
                disassembled = RemoveDbInstructions(disassembled, args[2] == "--no-db0");
            }

            File.WriteAllText(args[fileIndex + 1], disassembled);
            return;
        }

        //open the second file and look for LOADBIN signature and replace it with the contents of the first file
        if (args.Length == 2)
        {
            byte[] fromFile = File.ReadAllBytes(args[0]);
            string toFile = File.ReadAllText(args[1]);
            string newContent = toFile.Replace("#[LOADBIN] " + Path.GetFileName(args[0]), System.Text.Encoding.UTF8.GetString(fromFile));
            
            if (newContent != toFile)
            {
                File.WriteAllText(args[1], newContent);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("File loaded successfully");
                Console.ResetColor();
                return;
            }
        }

        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Error: Invalid arguments or LOADBIN signature not found in file");
        Console.ResetColor();
    }

    // Function to remove DB instructions based on option
    static string RemoveDbInstructions(string disassembled, bool removeZeroBytes)
    {
        string[] lines = disassembled.Split('\n');
        List<string> newLines = new List<string>();

        foreach (string line in lines)
        {
            if (!line.Contains("DB") || (removeZeroBytes && line.Contains("DB 0x00")))
            {
                newLines.Add(line);
            }
        }

        return string.Join('\n', newLines);
    }
}
    public class Decompiler
{
    // Intel x64 instructions
    public enum ByteToInstruction
    {
        AAA = 0x37,
        AAD = 0xD5,
        AAM = 0xD4,
        AAS = 0x3F,
        ADC = 0x14,
        ADD = 0x04,
        AND = 0x24,
        CALL = 0xE8,
        CBW = 0x98,
        CLC = 0xF8,
        CLD = 0xFC,
        CLI = 0xFA,
        CMC = 0xF5,
        CMP = 0x3C,
        CMPSB = 0xA6,
        CMPSW = 0xA7,
        CWD = 0x99,
        DAA = 0x27,
        DAS = 0x2F,
        DEC = 0x2B,
        DIV = 0x6E,
        HLT = 0xF4,
        IDIV = 0x6F,
        IMUL = 0x69,
        IN = 0xE4,
        INC = 0x2C,
        INT = 0xCD,
        INTO = 0xCE,
        IRET = 0xCF,
        JA = 0x77,
        JAE = 0x73,
        JB = 0x72,
        JBE = 0x76,
        JC = 0x72,
        JCXZ = 0xE3,
        JE = 0x74,
        JG = 0x7F,
        JGE = 0x7D,
        JL = 0x7C,
        JLE = 0x7E,
        JMP = 0xEB,
        JNA = 0x76,
        JNAE = 0x72,
        JNB = 0x73,
        JNBE = 0x77,
        JNC = 0x73,
        JNE = 0x75,
        JNG = 0x7E,
        JNGE = 0x7C,
        JNL = 0x7D,
        JNLE = 0x7F,
        JNO = 0x71,
        JNP = 0x7B,
        JNS = 0x79,
        JNZ = 0x75,
        JO = 0x70,
        JP = 0x7A,
        JPE = 0x7A,
        JPO = 0x7B,
        JS = 0x78,
        JZ = 0x74,
        LAHF = 0x9F,
        LDS = 0xC5,
        LEA = 0x8D,
        LES = 0xC4,
        LOCK = 0xF0,
        LODSB = 0xAC,
        LODSW = 0xAD,
        LOOP = 0xE2,
        LOOPE = 0xE1,
        LOOPNE = 0xE0,
        LOOPNZ = 0xE0,
        LOOPZ = 0xE1,
        MOV = 0x88,
        MOVSB = 0xA4,
        MOVSW = 0xA5,
        MUL = 0x6A,
        NEG = 0xF6,
        NOP = 0x90,
        NOT = 0xF6,
        OR = 0x0C,
        OUT = 0xE6,
        POP = 0x58,
        POPA = 0x61,
        POPF = 0x9D,
        PUSH = 0x50,
        PUSHA = 0x60,
        PUSHF = 0x9C,
        RCL = 0xD0,
        RCR = 0xD1,
        REP = 0xF3,
        REPE = 0xF3,
        REPNE = 0xF2,
        REPNZ = 0xF2,
        REPZ = 0xF3,
        RET = 0xC3,
        RETN = 0xC2,
        ROL = 0xD0,
        ROR = 0xD1,
        SAHF = 0x9E,
        SAL = 0xD0,
        SAR = 0xD0,
        SBB = 0x1C,
        SCASB = 0xAE,
        SCASW = 0xAF,
        SHL = 0xD0,
        SHR = 0xD0,
        STC = 0xF9,
        STD = 0xFD,
        STI = 0xFB,
        STOSB = 0xAA,
        STOSW = 0xAB,
        SUB = 0x2C,
        TEST = 0xA8,
        WAIT = 0x9B,
        XCHG = 0x86,
        XLAT = 0xD7,
        XOR = 0x34,
        SYSCALL = 0x05,
    }

    private static readonly Dictionary<byte, List<string>> instructionMap = new Dictionary<byte, List<string>>();
    private static readonly Dictionary<string, Func<byte[], int, int, string>> operandDecoders = new Dictionary<string, Func<byte[], int, int, string>>();

    static Decompiler()
    {
        // Populate instruction map
        foreach (ByteToInstruction instruction in Enum.GetValues(typeof(ByteToInstruction)))
        {
            byte opcode = (byte)instruction;
            string instructionName = instruction.ToString();

            if (!instructionMap.ContainsKey(opcode))
            {
                instructionMap[opcode] = new List<string>();
            }

            instructionMap[opcode].Add(instructionName);
        }

        // Populate operand decoders
        operandDecoders["AAA"] = (code, index, length) => "";
        operandDecoders["AAD"] = (code, index, length) => "";
        operandDecoders["AAM"] = (code, index, length) => "";
        operandDecoders["AAS"] = (code, index, length) => "";
        operandDecoders["ADC"] = DecodeBinaryOperands;
        operandDecoders["ADD"] = DecodeBinaryOperands;
        operandDecoders["AND"] = DecodeBinaryOperands;
        operandDecoders["CALL"] = DecodeCallOperands;
        operandDecoders["CBW"] = (code, index, length) => "";
        operandDecoders["CLC"] = (code, index, length) => "";
        operandDecoders["CLD"] = (code, index, length) => "";
        operandDecoders["CLI"] = (code, index, length) => "";
        operandDecoders["CMC"] = (code, index, length) => "";
        operandDecoders["CMP"] = DecodeBinaryOperands;
        operandDecoders["CMPSB"] = (code, index, length) => "";
        operandDecoders["CMPSW"] = (code, index, length) => "";
        operandDecoders["CWD"] = (code, index, length) => "";
        operandDecoders["DAA"] = (code, index, length) => "";
        operandDecoders["DAS"] = (code, index, length) => "";
        operandDecoders["DEC"] = DecodeUnaryOperands;
        operandDecoders["DIV"] = DecodeUnaryOperands;
        operandDecoders["HLT"] = (code, index, length) => "";
        operandDecoders["IDIV"] = DecodeUnaryOperands;
        operandDecoders["IMUL"] = DecodeUnaryOperands;
        operandDecoders["IN"] = DecodeInOutOperands;
        operandDecoders["INC"] = DecodeUnaryOperands;
        operandDecoders["INT"] = (code, index, length) => $"0x{code[index + 1]:X2}";
        operandDecoders["INTO"] = (code, index, length) => "";
        operandDecoders["IRET"] = (code, index, length) => "";
        operandDecoders["JA"] = DecodeJumpOperands;
        operandDecoders["JAE"] = DecodeJumpOperands;
        operandDecoders["JB"] = DecodeJumpOperands;
        operandDecoders["JBE"] = DecodeJumpOperands;
        operandDecoders["JC"] = DecodeJumpOperands;
        operandDecoders["JCXZ"] = DecodeJumpOperands;
        operandDecoders["JE"] = DecodeJumpOperands;
        operandDecoders["JG"] = DecodeJumpOperands;
        operandDecoders["JGE"] = DecodeJumpOperands;
        operandDecoders["JL"] = DecodeJumpOperands;
        operandDecoders["JLE"] = DecodeJumpOperands;
        operandDecoders["JMP"] = DecodeJumpOperands;
        operandDecoders["JNA"] = DecodeJumpOperands;
        operandDecoders["JNAE"] = DecodeJumpOperands;
        operandDecoders["JNB"] = DecodeJumpOperands;
        operandDecoders["JNBE"] = DecodeJumpOperands;
        operandDecoders["JNC"] = DecodeJumpOperands;
        operandDecoders["JNE"] = DecodeJumpOperands;
        operandDecoders["JNG"] = DecodeJumpOperands;
        operandDecoders["JNGE"] = DecodeJumpOperands;
        operandDecoders["JNL"] = DecodeJumpOperands;
        operandDecoders["JNLE"] = DecodeJumpOperands;
        operandDecoders["JNO"] = DecodeJumpOperands;
        operandDecoders["JNP"] = DecodeJumpOperands;
        operandDecoders["JNS"] = DecodeJumpOperands;
        operandDecoders["JNZ"] = DecodeJumpOperands;
        operandDecoders["JO"] = DecodeJumpOperands;
        operandDecoders["JP"] = DecodeJumpOperands;
        operandDecoders["JPE"] = DecodeJumpOperands;
        operandDecoders["JPO"] = DecodeJumpOperands;
        operandDecoders["JS"] = DecodeJumpOperands;
        operandDecoders["JZ"] = DecodeJumpOperands;
        operandDecoders["LAHF"] = (code, index, length) => "";
        operandDecoders["LDS"] = DecodeLoadOperands;
        operandDecoders["LEA"] = DecodeLoadOperands;
        operandDecoders["LES"] = DecodeLoadOperands;
        operandDecoders["LOCK"] = (code, index, length) => "";
        operandDecoders["LODSB"] = (code, index, length) => "";
        operandDecoders["LODSW"] = (code, index, length) => "";
        operandDecoders["LOOP"] = DecodeJumpOperands;
        operandDecoders["LOOPE"] = DecodeJumpOperands;
        operandDecoders["LOOPNE"] = DecodeJumpOperands;
        operandDecoders["LOOPNZ"] = DecodeJumpOperands;
        operandDecoders["LOOPZ"] = DecodeJumpOperands;
        operandDecoders["MOV"] = DecodeMovOperands;
        operandDecoders["MOVSB"] = (code, index, length) => "";
        operandDecoders["MOVSW"] = (code, index, length) => "";
        operandDecoders["MUL"] = DecodeUnaryOperands;
        operandDecoders["NEG"] = DecodeUnaryOperands;
        operandDecoders["NOP"] = (code, index, length) => "";
        operandDecoders["NOT"] = DecodeUnaryOperands;
        operandDecoders["OR"] = DecodeBinaryOperands;
        operandDecoders["OUT"] = DecodeInOutOperands;
        operandDecoders["POP"] = DecodeUnaryOperands;
        operandDecoders["POPA"] = (code, index, length) => "";
        operandDecoders["POPF"] = (code, index, length) => "";
        operandDecoders["PUSH"] = DecodeUnaryOperands;
        operandDecoders["PUSHA"] = (code, index, length) => "";
        operandDecoders["PUSHF"] = (code, index, length) => "";
        operandDecoders["RCL"] = DecodeShiftOperands;
        operandDecoders["RCR"] = DecodeShiftOperands;
        operandDecoders["REP"] = (code, index, length) => "";
        operandDecoders["REPE"] = (code, index, length) => "";
        operandDecoders["REPNE"] = (code, index, length) => "";
        operandDecoders["REPNZ"] = (code, index, length) => "";
        operandDecoders["REPZ"] = (code, index, length) => "";
        operandDecoders["RET"] = (code, index, length) => "";
        operandDecoders["RETN"] = (code, index, length) => "";
        operandDecoders["ROL"] = DecodeShiftOperands;
        operandDecoders["ROR"] = DecodeShiftOperands;
        operandDecoders["SAHF"] = (code, index, length) => "";
        operandDecoders["SAL"] = DecodeShiftOperands;
        operandDecoders["SAR"] = DecodeShiftOperands;
        operandDecoders["SBB"] = DecodeBinaryOperands;
        operandDecoders["SCASB"] = (code, index, length) => "";
        operandDecoders["SCASW"] = (code, index, length) => "";
        operandDecoders["SHL"] = DecodeShiftOperands;
        operandDecoders["SHR"] = DecodeShiftOperands;
        operandDecoders["STC"] = (code, index, length) => "";
        operandDecoders["STD"] = (code, index, length) => "";
        operandDecoders["STI"] = (code, index, length) => "";
        operandDecoders["STOSB"] = (code, index, length) => "";
        operandDecoders["STOSW"] = (code, index, length) => "";
        operandDecoders["SUB"] = DecodeBinaryOperands;
        operandDecoders["TEST"] = DecodeBinaryOperands;
        operandDecoders["WAIT"] = (code, index, length) => "";
        operandDecoders["XCHG"] = DecodeBinaryOperands;
        operandDecoders["XLAT"] = (code, index, length) => "";
        operandDecoders["XOR"] = DecodeBinaryOperands;
        operandDecoders["SYSCALL"] = (code, index, length) => "";
    }

    private static string DecodeMovOperands(byte[] code, int index, int length)
    {
        // Example for MOV decoding, simplified
        return $"r{code[index + 1]}, r{code[index + 2]}";
    }

    private static string DecodeAddOperands(byte[] code, int index, int length)
    {
        // Example for ADD decoding, simplified
        return $"r{code[index + 1]}, r{code[index + 2]}";
    }

    private static string DecodeSubOperands(byte[] code, int index, int length)
    {
        // Example for SUB decoding, simplified
        return $"r{code[index + 1]}, r{code[index + 2]}";
    }

    private static string DecodeCmpOperands(byte[] code, int index, int length)
    {
        // Example for CMP decoding, simplified
        return $"r{code[index + 1]}, r{code[index + 2]}";
    }

    private static string DecodeJumpOperands(byte[] code, int index, int length)
    {
        int offset = code[index + 1];
        //remove the address from the jump
        
        return $"0x{offset:X2}";
    }

    private static string DecodeCallOperands(byte[] code, int index, int length)
    {
        int offset = BitConverter.ToInt32(code, index + 1);
        return $"0x{offset:X8}";
    }

    private static string DecodeImmediate(byte[] code, int index)
    {
        return $"0x{code[index + 1]:X2}";
    }
    private static string DecodeBinaryOperands(byte[] code, int index, int length)
    {
        // Example for decoding binary operations (e.g., ADD, SUB)
        return $"r{code[index + 1]}, r{code[index + 2]}";
    }

    private static string DecodeUnaryOperands(byte[] code, int index, int length)
    {
        // Example for decoding unary operations (e.g., INC, DEC)
        return $"r{code[index + 1]}";
    }

    private static string DecodeInOutOperands(byte[] code, int index, int length)
    {
        // Example for decoding IN and OUT instruction operands
        return $"0x{code[index + 1]:X2}, 0x{code[index + 2]:X2}";
    }

    private static string DecodeLoadOperands(byte[] code, int index, int length)
    {
        // Example for decoding LDS, LES, and LEA instruction operands
        return $"r{code[index + 1]}, [0x{BitConverter.ToInt16(code, index + 2):X4}]";
    }

    private static string DecodeShiftOperands(byte[] code, int index, int length)
    {
        // Example for decoding shift operations (e.g., ROL, ROR)
        return $"r{code[index + 1]}, {code[index + 2]}";
    }


    public string Disassemble(byte[] code)
    {
        List<string> disassembledInstructions = new List<string>();
        Dictionary<int, string> labels = new Dictionary<int, string>();

        for (int i = 0; i < code.Length; i++)
        {
            byte instructionByte = code[i];

            if (instructionMap.TryGetValue(instructionByte, out List<string> instructions))
            {
                string instruction = instructions[0]; // Take the first match for simplicity
                string operands = "";

                if (operandDecoders.TryGetValue(instruction, out Func<byte[], int, int, string> decodeOperands))
                {
                    operands = decodeOperands(code, i, code.Length - i);
                }

                disassembledInstructions.Add($"{instruction} {operands}");

                // Handle branching instructions to generate labels
                if (instruction.StartsWith("J") || instruction == "CALL")
                {
                    int offset = (instruction == "CALL") ? BitConverter.ToInt32(code, i + 1) : code[i + 1];
                    int targetAddress = i + 2 + offset;

                    if (!labels.ContainsKey(targetAddress))
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.WriteLine($"Found branching jump at {targetAddress:X2}");
                        Console.WriteLine("Adding label...");
                        Console.ResetColor();
                        labels[targetAddress] = $"label_{labels.Count}";
                        
                    }
                    disassembledInstructions[^1] += $" {labels[targetAddress]}";
                    i += (instruction == "CALL") ? 4 : 1; // Move index forward for the operand
                    //remove the hex number before the label and then the number
                    disassembledInstructions[^1] = disassembledInstructions[^1].Replace("0x", "");
                    disassembledInstructions[^1] = disassembledInstructions[^1].Replace(" ", "");
                    //add one more space to the label
                    disassembledInstructions[^1] = disassembledInstructions[^1].Insert(disassembledInstructions[^1].IndexOf("label"), " ");
                    disassembledInstructions[^1] = disassembledInstructions[^1].Replace($"{offset:X2}", "");
                }
            }
            else
            {
                disassembledInstructions.Add($"DB 0x{instructionByte:X2}"); // Unrecognized byte
            }
        }

        // Add labels to the disassembled instructions
        for (int i = 0; i < disassembledInstructions.Count; i++)
        {
            if (labels.ContainsKey(i))
            {
                disassembledInstructions.Insert(i, $"{labels[i]}:");
            }
        }
        //find the start of the code in text and the first label replace it with _start
        for (int i = 0; i < disassembledInstructions.Count; i++)
        {
            if (disassembledInstructions[i].Contains("label_0:"))
            {
                disassembledInstructions[i] = disassembledInstructions[i].Replace("label_0:", "_start:");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Assuming Start at address: " + i);
                Console.ResetColor();
            }
        }
        return string.Join("\n", disassembledInstructions);
    }
}
}