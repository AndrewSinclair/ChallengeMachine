using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ascript
{
    public class Machine
    {
        private int _pc;
        private int[] _regStore;
        private Dictionary<int, int> _memory;
        private Action[] _commands;
        private int _wordLength;
        private bool _z;

        public Machine(int wordLength)
        {
            this._pc = 0;
            this._regStore = new int[8];
            this._commands = new Action[5000];
            this._memory = new Dictionary<int, int>();
            this._wordLength = wordLength;
            this._z = false;
        }

        public void DoAdd(int reg1, int reg2)
        {
            Action command = () =>
            {
                var regValue1 = _regStore[reg1];
                var regValue2 = _regStore[reg2];
                var val = regValue1 + regValue2;
                _regStore[reg1] = val;

                _z = val == 0;
            };
            
            _commands[_pc] = command;
            _pc++;
        }

        public void DoSub(int reg1, int reg2)
        {
            Action command = () =>
            {
                var regValue1 = _regStore[reg1];
                var regValue2 = _regStore[reg2];
                var val = regValue1 - regValue2;
                _regStore[reg1] = val;

                _z = val == 0;
            };
            
            _commands[_pc] = command;
            _pc++;
        }

        public void DoLoadReg(int reg, int val)
        {
            Action command = () =>
            {
                _regStore[reg] = val;
                _z = false;
            };

            _commands[_pc] = command;
            _pc++;
        }

        public void DoJnz(int val)
        {
            Action command = () =>
            {
                if (!_z)
                {
                    _pc -= val;
                }
                else
                {
                    _pc++;
                }
                _z = false;
            };
        }

        public void HCF()
        {
            Action command = () =>
            {
                throw new Hcf("Halt and Catch Fire Interrupt!");
            };

            _commands[_pc] = command;
        }

        public void Run(bool isVerbose)
        {
            _pc = 0;

            while(true) {
                if (isVerbose) Dump();

                Action cmd = _commands[_pc];
                cmd.Invoke();
                _pc++;
            }
        }

        private string ConvertToPrettyPrint(int num)
        {
            var shortBinary = Convert.ToString(num, 2);
            return String.Format("{0} #{1}", shortBinary.PadLeft(_wordLength, '0'), num);
        }

        public void Dump()
        {
            Console.WriteLine();
            Console.WriteLine("PC: {0}",  ConvertToPrettyPrint(_pc));

            for (var i = 0; i < 8; i++)
            {
                Console.WriteLine("R{0}: {1}", i, ConvertToPrettyPrint(_regStore[i]));
            }
            Console.WriteLine();
        }
    }
}
