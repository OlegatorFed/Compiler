using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Compiler
{
    class Lexer
    {
        public static int FileCounter;

        Program program = new Program();
        char[] sm = new char[1];
        string buf = "";
        
        private string[] ReserveWords = { "program", "var", "integer", "real", "bool", "begin",
            "end", "if", "then", "else", "while", "do", "read", "write", "true", "false" };
        private string[] Operators = {
            "*", "/", "div", "mod", "and", "or", "+", "-", "=", "<", 
            ">", "<>", "<=", ">=", "in", "not"};
        private string[] Separators = { ";", ".", ":",",","..","[","]","(",")"};
        private string[] Assigments = { ":=", "/=", "*=", "+=","-="};
        private string[] SpaceSymbols = { " ", "\r", "\n", "\0", "\t", ",",":",";"};

        IFormatProvider formatter = new NumberFormatInfo { NumberDecimalSeparator = "." };

        string Value = "";

        public enum State { Start, Number, Identifier, ChoiceLex, ArOperator,
            ReserveWords, Separator, Integer, Real,RealExp,String,Assignment,
            BlockComment,StringComment,CtrlString, Error}

        State state;
        BinaryReader Reader;
        int Ln = 0;
        int Ch = 0;
        int ChCounter = 0;
        string LexName;
        char SaveSymbol = '\0';

        public bool NotEof = true;
        public bool NotFoundLexem = true;

        private Lexema _lexema;

        public Lexer(BinaryReader reader)
        {
            Reader = reader;
        }

        public void SetLexema(Lexema lexema)
        {
            _lexema = lexema;
        }

        public Lexema GetCurrentLexema()
        {
            return _lexema;
        }

         public Lexema AddLex(int Ln, int Ch, string LexName, string Buff,string Value)
         {
            return new Lexema(Ln, Ch, LexName, Buff, Value);
         }

        public void Errors(string error)
        {
            NotEof = false;
            NotFoundLexem = false;
            string errors = $"{Ln}:{Ch} {error}";
            throw new MyExeption(error);
        }

        private void GetNext()
        {
            if (Reader.PeekChar() != -1)
            {
                sm[0] = Reader.ReadChar();
                ChCounter++;
            }
            else
            {
                sm[0] = ' ';
                NotEof = false;
            }
        }

        private void AddLexName()
        {
            LexName = state.ToString();
        }

        private void ClearBuf()
        {
            buf = "";
        }

        private void AddValue()
        {
            Value = buf;
        }

        private void AddBuf(char symb)
        {
            buf += symb;
        }

         public Lexema GetLexem()
         {
            ClearBuf();
            NotEof = true;
            while (NotFoundLexem || NotEof)
            {
                NotFoundLexem = true;
                switch (state)
                {
                    case State.Start:
                        Ch = ChCounter;
                        if (sm[0] == ' ' || sm[0] == '\t'  || sm[0] == '\r')
                        {
                            if (NotEof)
                            {
                               GetNext();
                            }
                            else
                            {
                                NotEof = false;
                                NotFoundLexem = false;
                            }
                        }
                        else if(sm[0] == '\n' || sm[0] == '\0')
                        {
                            GetNext();
                            ChCounter = 1;
                            Ln++;
                        }
                        else if (Char.IsLetter(sm[0]) || sm[0] == '_')
                        {
                            ClearBuf();
                            AddBuf(sm[0]);
                            state = State.ChoiceLex;
                            GetNext();
                        }
                        else if (Char.IsDigit(sm[0]))
                        {
                            ClearBuf();
                            AddBuf(sm[0]);
                            state = State.Number;
                            GetNext();
                        }
                        else if (Separators.Contains(sm[0].ToString()))
                        {
                            ClearBuf();
                            if (SaveSymbol != '\0')
                            {
                                buf += SaveSymbol;
                                SaveSymbol = '\0';
                            }
                            AddBuf(sm[0]);
                            state = State.Separator;
                            GetNext();
                        }
                        else if (Operators.Contains(sm[0].ToString()))
                        {
                            ClearBuf();
                            AddBuf(sm[0]);
                            state = State.ArOperator;
                            GetNext();
                        }
                        else if (sm[0] == '\'')
                        {
                            ClearBuf();
                            AddBuf(sm[0]);
                            state = State.String;
                            GetNext();
                        }
                        else if (sm[0] == '{')
                        {
                            ClearBuf();
                            AddBuf(sm[0]);
                            state = State.BlockComment;
                            GetNext();
                        }
                        else if (sm[0] == '#')
                        {
                            ClearBuf();
                            AddBuf(sm[0]);
                            state = State.CtrlString;
                            GetNext();
                        }
                        else
                        {
                            state = State.Error;
                        }
                        break;

                    case State.ChoiceLex:
                        if (!ReserveWords.Contains(buf)
                            && !Operators.Contains(buf) 
                            && (Separators.Contains(sm[0].ToString()) 
                            || SpaceSymbols.Contains(sm[0].ToString())
                            || Operators.Contains(sm[0].ToString())))
                        {
                            state = State.Identifier;
                        }
                        else if (ReserveWords.Contains(buf) 
                            && (SpaceSymbols.Contains(sm[0].ToString()) || sm[0] == ';'||sm[0] == '.' || sm[0] == '('))
                        {
                            state = State.ReserveWords;
                        }
                        else if (Operators.Contains(buf)
                            && (SpaceSymbols.Contains(sm[0].ToString()) 
                            || Separators.Contains(sm[0].ToString())
                            || Char.IsDigit(sm[0])))
                        {
                            state = State.ArOperator;
                        }
                        else if (Char.IsLetter(sm[0]) || Char.IsDigit(sm[0]))
                        {
                            AddBuf(sm[0]);
                            GetNext();
                        }
                        else
                        {
                            state = State.Error;
                        }
                        break;

                    case State.Number:
                        if (Int32.TryParse(buf, out int x) && sm[0] !='.'
                            && (SpaceSymbols.Contains(sm[0].ToString())
                            || Separators.Contains(sm[0].ToString()) || Operators.Contains(sm[0].ToString())))
                        {
                            state = State.Integer;
                        }
                        else if (sm[0] == '.' )
                        {
                            AddBuf(sm[0]);
                            GetNext();
                            state = State.Real;
                        }
                        else if (Char.ToLower(sm[0]) == 'e')
                        {
                            AddBuf(sm[0]);
                            GetNext();
                            state = State.RealExp;
                        }
                        else if (Char.IsDigit(sm[0]) )
                        {
                            AddBuf(sm[0]);
                            GetNext();
                        }
                        else
                        {
                            state = State.Error;
                        }
                        break;

                    case State.Separator:
                        if (Separators.Contains(buf)
                            && (!Separators.Contains(sm[0].ToString()) || sm[0] == ';' || sm[0] == '.' || sm[0] == '\'')
                            && ((SpaceSymbols.Contains(sm[0].ToString()) || sm[0] == '\'')
                            || Char.IsLetterOrDigit(sm[0])
                            ||( Operators.Contains(sm[0].ToString()) && sm[0] != '=')))
                        {
                            AddValue();
                            AddLexName();
                            state = State.Start;
                            NotFoundLexem = false;
                            SetLexema(new Lexema(Ln, Ch, LexName, buf, Value));
                            return GetCurrentLexema(); 
                        }
                        else if (Assigments.Contains((buf).ToString())
                            && (SpaceSymbols.Contains(sm[0].ToString()) || Char.IsLetterOrDigit(sm[0])))
                        {
                            state = State.Assignment;
                        }
                        else if (Separators.Contains((buf).ToString()))
                        {
                            AddBuf(sm[0]);
                            GetNext();
                        }
                        else
                        {
                            state = State.Error;
                        }
                        break;

                    case State.Real:
                        if (SpaceSymbols.Contains(sm[0].ToString()) && buf[buf.Length - 1] != '.')
                        {
                            AddValue();
                            AddLexName();
                            state = State.Start ;
                            NotFoundLexem = false;
                            SetLexema(new Lexema(Ln, Ch, LexName, buf, Value));
                            return GetCurrentLexema();
                        }
                        else if (Char.IsDigit(sm[0]))
                        {
                            AddBuf(sm[0]);
                            GetNext();
                        }
                        else if (buf[buf.Length-1] != '.' && sm[0] == 'e')
                        {
                            AddBuf(sm[0]);
                            GetNext();
                            state = State.RealExp;
                        }
                        else if (sm[0] == '.' && buf[buf.Length - 1] == '.')
                        {
                            SaveSymbol = '.';
                            buf = buf.Substring(0, buf.Length - 1);
                            if (Int32.TryParse(buf, out int z))
                            {
                                AddValue();
                                state = State.Start;
                                SetLexema(new Lexema(Ln, Ch, Convert.ToString(State.Integer), buf, Value));
                                return GetCurrentLexema();
                            }
                        }
                        else
                        {
                            state = State.Error;
                        }
                        break;

                    case State.RealExp:
                        if ((sm[0] == '+' || sm[0] == '-') && buf[buf.Length -1] == 'e')
                        {
                            AddBuf(sm[0]);
                            GetNext();
                        }
                        else if (Char.IsDigit(sm[0]))
                        {
                            AddBuf(sm[0]);
                            GetNext();
                        }
                        else if (float.TryParse(buf, NumberStyles.Float, formatter, out float y) )
                        {
                            Value = Convert.ToString(float.Parse(buf, formatter));
                            AddLexName();
                            state = State.Start;
                            NotFoundLexem = false;
                            SetLexema(new Lexema(Ln, Ch, State.Real.ToString(), buf, Value));
                            return GetCurrentLexema();
                        }
                        else
                        {
                            state = State.Error;
                        }
                        break;

                    case State.Integer:
                        AddValue();
                        AddLexName();
                        state = State.Start;
                        NotFoundLexem = false;
                        SetLexema(new Lexema(Ln, Ch, LexName, buf, Value));
                        return GetCurrentLexema();

                    case State.Identifier:
                        AddValue();
                        AddLexName();
                        state = State.Start;
                        NotFoundLexem = false;
                        SetLexema(new Lexema(Ln, Ch, LexName, buf, Value));
                        return GetCurrentLexema();

                    case State.ReserveWords:
                        AddValue();
                        AddLexName();
                        state = State.Start;
                        NotFoundLexem = false;
                        SetLexema(new Lexema(Ln, Ch, LexName, buf, Value));
                        return GetCurrentLexema();

                    case State.Assignment:
                        AddValue();
                        AddLexName();
                        state = State.Start;
                        NotFoundLexem = false;
                        SetLexema(new Lexema(Ln, Ch, LexName, buf, Value));
                        return GetCurrentLexema();

                    case State.ArOperator:
                        if (Operators.Contains(buf)
                            && (SpaceSymbols.Contains(sm[0].ToString()) 
                            || Char.IsLetterOrDigit(sm[0]))|| Separators.Contains(sm[0].ToString()))
                        {
                            AddValue();
                            AddLexName();
                            state = State.Start;
                            NotFoundLexem = false;
                            SetLexema(new Lexema(Ln, Ch, LexName, buf, Value));
                            return GetCurrentLexema();
                        }
                        else if (Assigments.Contains((buf).ToString())
                          && (SpaceSymbols.Contains(sm[0].ToString())))
                        {
                            state = State.Assignment;
                        }
                        else if (buf == "//")
                        {
                            state = State.StringComment;
                        }
                        else if (Char.IsLetter(sm[0]) 
                            || Operators.Contains((buf).ToString()) 
                            || Assigments.Contains((buf).ToString()))
                        {
                            AddBuf(sm[0]);
                            GetNext();
                        }
                        else
                        {
                            state = State.Error;
                        }
                        break;

                    case State.String:
                        if (sm[0] == '\'')
                        {
                            string value = null;
                            AddBuf(sm[0]);
                            //Value = buf.Trim(new Char[] { '\'' });
                            AddValue();
                            AddLexName();
                            state = State.Start;
                            NotFoundLexem = false;
                            GetNext();
                            /*if (sm[0] != '#' && buf[buf.Length -1] == '\'')
                            {
                                NotFoundLexem = true;
                                Regex regex = new Regex(@"#\d+");

                                 value = regex.Replace(buf, delegate (Match m){
                                    int res = Int32.Parse(m.Value.Substring(1));
                                    return ((char)res).ToString();

                                });
                                */
                            value = Value.Replace("'", "");
                               // value = $"'{value}'";

                                SetLexema(new Lexema(Ln, Ch, LexName, buf, value));
                                return GetCurrentLexema();
                               
                              

                            /*}
                            else if( buf[buf.Length - 1] == '\'')
                            {
                                SetLexema(new Lexema(Ln, Ch, LexName, buf, value));
                                return GetCurrentLexema();

                            }
                            else
                            {
                                AddBuf(sm[0]);
                                GetNext();
                                state = State.CtrlString;
                            }*/
                        }
                        else if (Reader.PeekChar() != -1)
                        {
                            AddBuf(sm[0]);
                            GetNext();
                        }
                        else
                        {
                            AddBuf(sm[0]);
                            state = State.Error;
                        }
                        break;

                    case State.BlockComment:
                        if (sm[0] == '}')
                        {
                            GetNext();
                            AddBuf(sm[0]);
                            ClearBuf();
                            state = State.Start;
                        }
                        else if (Reader.PeekChar() != -1)
                        {
                            AddBuf(sm[0]);
                            GetNext();
                        }
                        else
                        {
                            state = State.Start;
                        }
                        break;

                    case State.StringComment:
                        if (sm[0] == '\r')
                        {
                            ClearBuf();
                            GetNext();
                            state = State.Start;
                        }
                        else if (Reader.PeekChar() != -1)
                        {
                            AddBuf(sm[0]);
                            GetNext();
                        }
                        else
                        {
                            state = State.Start;
                        }
                        break;

                    case State.CtrlString:
                        if (Char.IsDigit(sm[0]))
                        {
                            AddBuf(sm[0]);
                            GetNext();
                            state = State.CtrlString;
                        }
                        else if (sm[0] == '\'' && buf[0] == '\'')
                        {
                            AddBuf(sm[0]);
                            GetNext();
                            state = State.String;
                        }
                        else if (Char.IsLetter(sm[0]))
                        {
                            AddBuf(sm[0]);
                            GetNext();
                            state = State.Error;
                        }
                        else
                        {
                            if (!Char.IsDigit(buf[buf.Length - 1]))
                            {
                                Errors("syntax error: " + buf);
                            }
                            else
                            {
                                Regex regex = new Regex(@"#\d+");
                                string value = regex.Replace(buf, (Match m) => {
                                    int res = Int32.Parse(m.Value.Substring(1));
                                    return ((char)res).ToString();
                                });

                                value = Regex.Replace(value, "'", "");
                                state = State.Start;
                                //Console.WriteLine(sm[0]);
                                if (sm[0] == '#')
                                {
                                    AddBuf(sm[0]);
                                    GetNext();
                                    state = State.CtrlString ;
                                }
                                else
                                {
                                    SetLexema(new Lexema(Ln, Ch, LexName, buf, value));
                                    return GetCurrentLexema();
                                }
                            }
                        }
                        break;

                    case State.Error:
                        AddValue();
                        AddLexName();
                        state = State.Start;
                        GetNext();
                        Errors("syntax error: "+ buf);
                        break;
                }
            }
            SetLexema(new Lexema(0, 0, "EOf", "", ""));
            return GetCurrentLexema();
        }
    }
}