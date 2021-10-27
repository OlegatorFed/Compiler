using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    class BinaryOpNode : Node
    {
        private Lexema operation;
        private Node left;
        private Node right;

        public BinaryOpNode(Lexema operation, Node left, Node right)
        {
            this.operation = operation;
            this.left = left;
            this.right = right;
        }
        public override string GetValue()
        {
            return operation.Value;
        }

        override public string Print(int priority = 1)
        {
            string val = operation.Value;
            string rightOp = right.Print(priority + 1);
            string leftOp = left.Print(priority + 1);

            return $"{val}\n{new string(' ', priority * 6)}{leftOp}\n{new string(' ', priority * 6)}{rightOp}";
        }
    }
}

