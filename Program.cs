using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ascript
{
    enum Mnemonic
    {
        ADD,
        SUBTRACT,
        LOADREG,
        HCF,
        JNZ
    }

    class Program
    {
        public const int WORD_LENGTH = 32;

        static Mnemonic GetMnemonic(string line)
        {
            var op = line.Substring(0,3);
            return (Mnemonic)Convert.ToInt32(op, 2);
        }

        static int GetReg(string line, int regIndex)
        {
            string regCode;

            if (regIndex == 1)
            {
                regCode = line.Substring(3, 4);
            }
            else if (regIndex == 2)
            {
                regCode = line.Substring(7, 4);
            }
            else
            {
                throw new Exception("RegIndex is not a good value.");
            }

            return Convert.ToInt32(regCode, 2);
        }

        static int GetVal(string line, int valIndex)
        {
            if (valIndex == 1)
            {
                return Convert.ToInt32(line.Substring(7, 14), 2);
            }
            else if (valIndex == 2)
            {
                return Convert.ToInt32(line.Substring(4, 14), 2);
            }
            else if (valIndex == 3)
            {
                return Convert.ToInt32(line.Substring(18, 14), 2);
            }
            else
            {
                throw new Exception("ValIndex is not a good value.");
            }
        }

        static void WriteBytes()
        {
            var bytes = Console.ReadLine();

            while (bytes != null)
            {
                string[] words = bytes.Select(x => Convert.ToString(x, 2).PadLeft(8, '0')).ToArray();

                foreach (var w in words)
                {
                    Console.WriteLine(w.ToString());
                }

                bytes = Console.ReadLine();
            }
        }

        static void ReadBytes()
        {
            var line = Console.ReadLine();

            while (line != null)
            {
                var output = 0;
                foreach (var bit in line)
                {
                    if (bit == '0')
                    {
                        output *= 2;
                    }
                    else if (bit == '1')
                    {
                        output = output * 2 + 1;
                    }
                    else
                    {
                        continue;
                    }
                }

                Console.Write((char)output);
                line = Console.ReadLine();
            }
        }

        /* INPUT: ascii strings of 1's and 0's, separated by newline characters
         * only the first 16 bits of each line will be read.
         * If there are less than 16 bits, the line will be skipped
         * */
        static void Compile(Boolean isVerbose)
        {
            var line = Console.ReadLine();
            var machine = new Machine(WORD_LENGTH);

            while (line != null)
            {
                int reg1, reg2, val;

                if (line.Length < WORD_LENGTH)
                {
                    line = Console.ReadLine();
                    continue;
                }

                line = line.Substring(0, WORD_LENGTH);

                var mnemonic = GetMnemonic(line);

                switch (mnemonic)
                {
                    case Mnemonic.ADD:
                        reg1 = GetReg(line, 1);
                        reg2 = GetReg(line, 2);
                        machine.DoAdd(reg1, reg2);
                        break;

                    case Mnemonic.SUBTRACT:
                        reg1 = GetReg(line, 1);
                        reg2 = GetReg(line, 2);
                        machine.DoSub(reg1, reg2);
                        break;

                    case Mnemonic.LOADREG:
                        reg1 = GetReg(line, 1);
                        val = GetVal(line, 1);
                        machine.DoLoadReg(reg1, val);
                        break;

                    case Mnemonic.JNZ:
                        val = GetVal(line, 1);
                        machine.DoJnz(val);
                        break;

                    case Mnemonic.HCF:
                        machine.HCF();
                        break;

                    default:
                        throw new Exception("The command was not well formed! -- " + line);
                        break;
                }

                line = Console.ReadLine();
            }

            try
            {
                machine.Run(isVerbose);
            }
            catch (Hcf ex)
            {
                Console.WriteLine(ex.Message);

                machine.Dump();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }

        static void Main(string[] args)
        {
            if (args == null || args.Length < 1) { return; }

            /* read the actual byte stream and covert into human readable bits */
            if (args[0] == "-b")
            {
                WriteBytes();
            }
            /* read the ascii file of 1's and 0's and turn it into actual bits. */
            else if (args[0] == "-h")
            {
                ReadBytes();
            }
            /* "compile" the "code" */
            else if (args[0] == "-c")
            {
                var isVerbose = args.Length > 1 && args[1] == "-v";
                Compile(isVerbose);
            }
            /* print mnemonics */
            else if (args[0] == "-p")
            {
                foreach (var name in Enum.GetNames(typeof(Mnemonic)))
                {
                    int num = (int)(Mnemonic)Enum.Parse(typeof(Mnemonic), name);
                    string val = Convert.ToString(num, 2).PadLeft(3, '0');
                    Console.WriteLine("{0}\t\t{1}", val, name);
                }
            }
        }
    }
}
