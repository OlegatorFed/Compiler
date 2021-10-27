using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    class Parser
    {
        private Lexer _lex;
        private TokenType _tokenType = new TokenType();

        public Parser(Lexer lex)
        {
            _lex = lex;
        }
        public Node ParserExpr()
        {
            Lexema lexema = _lex.GetCurrentLexema();
            if (lexema.IsEOf())
            {
                throw new MyExeption($"{lexema.Ch}: {lexema.Ln} expected expression");
            }
            else
            {
                Node Left = ParseTerm();
                Lexema Operation = _lex.GetCurrentLexema();
                while(Operation.Value == "+"
                    || Operation.Value == "-")
                {
             
                    _lex.GetLexem();
                    Node Right = ParseTerm();
                    Left = new BinaryOpNode(Operation, Left, Right);
                    Operation = _lex.GetCurrentLexema();
                }
                return Left;
            }

        }  
        public Node ParseTerm()
        {
            Node Left = ParserFactor();
            Lexema Operation = _lex.GetCurrentLexema();
            while (Operation.Value == "*" 
                || Operation.Value =="/")
            {
                _lex.GetLexem();
                Node right = ParserFactor();
                Left = new BinaryOpNode(Operation, Left, right);
                Operation = _lex.GetCurrentLexema();
            }
            return Left;
        }
        public Node ParserFactor()
        {
            Lexema lexema = _lex.GetCurrentLexema();
            _lex.GetLexem();
            if (lexema.States == _tokenType.identifier)
                return new IdentifierNode(lexema);
            if (lexema.States== _tokenType.integer)
                return new IntegerNode(lexema);
            if (lexema.States== _tokenType.real)
                return new RealNode(lexema);
            if(lexema.Value == "-" || lexema.Value == "+")
            {
                Node Operand = ParserFactor();
                return new UnaryOpNode(Operand, lexema);
            }
            if (lexema.Value == "(")
            {
                Node Left = ParserExpr();
                lexema = _lex.GetCurrentLexema();

                if (lexema.Value != ")")
                    throw new MyExeption($"{lexema.Ln}: {lexema.Ch} ')' expected");
                
                _lex.GetLexem();
                return Left;

            }
            throw new MyExeption($"{lexema.Ln}: {lexema.Ch} Unexpected token");

        }

    }
}
