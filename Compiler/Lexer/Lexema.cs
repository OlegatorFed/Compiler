using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    class Lexema
    {
        public readonly int Ln;
        public readonly int Ch;
        public readonly string Buff;
        public string States;
        public readonly string Value;

        public bool IsEOf() 
        {
            return States == "EOF";
            
        }
        public Lexema(int ln, int ch, string lexema, string buff,string value)
        {
           Ln = ln;   
           Ch = ch;
           States = lexema;
           Buff = buff;
           Value = value;
        }
    }
}
