using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    class UnaryOpNode : Node
    {
        private Node operand;
        private Lexema lexema;

        public UnaryOpNode(Node operand, Lexema lexema)
        {
            this.operand = operand;
            this.lexema = lexema;
        }
        public override string GetValue()
        {
            return Convert.ToString(lexema.Value);
        }
        public override string Print(int prioryty = 0)
        {
            return $"{GetValue()} {operand.GetValue()}";
        }
    }
}
