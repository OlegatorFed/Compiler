using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    class IntegerNode : Node
    {
        int Value;
        Lexema lexema;

        public IntegerNode(Lexema lexema)
        {
            this.lexema = lexema;
        }
        public override string GetValue()
        {
            return Convert.ToString(lexema.Value);
        }
        public override string Print(int priority = 0)
        {
            return GetValue();
        }
    }
}
