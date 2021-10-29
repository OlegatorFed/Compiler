using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Compiler
{
    class Program
    {

        static void Main(string[] args)
        {
            string debugAppDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string lexerTestPath = System.IO.Path.Combine(debugAppDirectory, @"..\..\Tests");
            string parserTestPath = System.IO.Path.Combine(debugAppDirectory, @"..\..\TestPs");

            if (args[0] == "-l")
            {

                string DirNameCode = lexerTestPath;
                string result = "";
                string TestResult = "";
                string LineResult = "";

                int CountW = 0;
                int CountF = 0;
                Lexema Lex;
                IEnumerable<string> CodeFile = Directory.EnumerateFiles(DirNameCode, "*code*");
                foreach (string s in CodeFile)
                {
                    string PathResultFile = s.Remove(s.LastIndexOf('(')) + "(result).txt";
                    var ResultFile = new StreamReader(PathResultFile);

                    Lexer.FileCounter++;
                    var StRead = new BinaryReader(File.OpenRead(s));
                    Lexer lexer = new Lexer(StRead);

                    try
                    {
                        while ((lexer.NotEof || lexer.NotFoundLexem))
                        {
                            Lex = lexer.GetLexem();
                            LineResult = ResultFile.ReadLine();
                            Console.WriteLine(LineResult);
                            result = Lex.Ln + ":"
                           + Lex.Ch + "\t" + Lex.States
                           + "\t" + "\"" + Lex.Buff + "\"" + "\t" + Lex.Value + "\n";
                            Console.WriteLine(result);
                            result = result.Substring(0, result.LastIndexOf('\n'));
                            if (result == LineResult)
                            {
                                TestResult = "Тест пройден";
                            }
                            else
                            {
                                TestResult = "Тест не пройден";
                                //break;
                            }

                        }
                    }
                    catch (MyExeption ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    if (TestResult == "Тест пройден")
                    {
                        CountW++;
                    }
                    else
                    {
                        CountF++;
                    }
                    Console.WriteLine(TestResult);
                    Console.WriteLine("---------------------------------------------------------------");

                }
                Console.WriteLine($"Пройдено тестов - {CountW}, Не пройдено тест - {CountF} ");
                Console.ReadKey();
            }
            if ( args[0] == "-p")
            {
                int CountW = 0;
                int CountF = 0;

                string DirNameCode = parserTestPath;
                string LineResult = "";
                string TestResult = "";
                IEnumerable<string> CodeFile = Directory.EnumerateFiles(DirNameCode, "*code*");
                foreach (string s in CodeFile)
                {
                    var StRead = new BinaryReader(File.OpenRead(s));
                    Lexer lexer = new Lexer(StRead);
                    string PathResultFile = s.Remove(s.LastIndexOf('(')) + "(result).txt";
                    var ResultFile = new StreamReader(PathResultFile);
                    while ((lexer.NotEof || lexer.NotFoundLexem))
                    {
                        try
                        {
                            LineResult = ResultFile.ReadToEnd();
                            lexer.GetLexem();
                            
                            Node node = new Parser(lexer).ParserExpr();
                            string res = node.Print(1);
                            Console.WriteLine(res);
                            Console.WriteLine("---------------------------------------");
                            Console.WriteLine(LineResult);
                            if (res == LineResult)
                            {
                                TestResult = "Тест пройден";
                            }
                            else
                            {
                                TestResult = "Тест не пройден";
                                //break;
                            }
                        }
                        catch (MyExeption ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        if (TestResult == "Тест пройден")
                        {
                            CountW++;
                        }
                        else
                        {
                            CountF++;
                        }
                        Console.WriteLine(TestResult);
                    }
                }
                Console.WriteLine($"Пройдено тестов - {CountW}, Не пройдено тест - {CountF} ");
                Console.ReadKey();

            }
        }
    }
}
