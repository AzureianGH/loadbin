using System;
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
                Console.WriteLine("loadbinary v1.0.0");
                Console.WriteLine("Made by AzureianGH, 2024");
                Console.WriteLine("Licensed under GNU GPL v3");

                return;
            }
            else if (args.Length == 1 && args[0] == "--help")
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Usage: loadbinary <from-file> <to-file>");
                Console.WriteLine("");
                //Explain the required signature
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("The file to load into must contain the following signature:");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("#[LOADBIN] <filename>\n");
                //now explain the --version flag
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("Options:");
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("  --version: Display version information");
                Console.WriteLine("  --help: Display this help message");
                Console.ResetColor();
                return;
            }
            if (args.Length != 2)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Usage: loadbinary <from-file> <to-file>");
                Console.ResetColor();
                return;
            }

            //open the second file and look for LOADBIN signature and replace it with the contents of the first file
            byte[] from = System.IO.File.ReadAllBytes(args[0]);
            string to = System.IO.File.ReadAllText(args[1]);
            string newte = to.Replace("#[LOADBIN] " + Path.GetFileName(args[0]), System.Text.Encoding.UTF8.GetString(from));
            if (newte != to)
            {
                System.IO.File.WriteAllText(args[1], newte);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("File loaded successfully");
                Console.ResetColor();
                return;
            }
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Error: LOADBIN signature not found in file");
            Console.ResetColor();
        }
    }
}