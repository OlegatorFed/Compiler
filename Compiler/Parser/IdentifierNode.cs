using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    class IdentifierNode: Node
    {
        string Name;
        private Lexema lexema;

        public IdentifierNode(Lexema lexema)
        {
            this.lexema = lexema;
        }
        public override string GetValue()
        {
            return lexema.Value;
        }
        public override string Print(int priority = 0)
        {
            return GetValue();
        }
    }
}
