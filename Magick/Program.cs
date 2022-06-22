using System;
using System.Collections.Generic;
using System.IO;

namespace Magick
{
    class Program
    {
        public static List<string> KEYWORDS = new List<string> {
            "double", "int", "float", "char", "null", "if", "else", "do", "while", "for", "break", "switch",
            "case", "return", "const", "continue", "for", "static", "void", "class", "namespace", "throw", "new"
        };

        static void Main(string[] args)
        {
            string inPath = "";

            if (args.Length > 0)
            {
                inPath = args[0];
            }
            if (args.Length > 1)
            {
                Debug.outPath = args[1];
            }

            if (inPath == "")
                return;
            if (Debug.outPath == "")
                Debug.outPath = ".\\Result.txt";

            Debug.ClearLog();

            if (!File.Exists(inPath))
            {
                throw new FileNotFoundException(inPath);
            }

            StreamReader sr = new StreamReader(inPath);
            Debug.Log(sr.ReadToEnd());
            Debug.Log("");
            sr.Close();

            // Run the lexer to get token queue
            Queue<Token> tokens = Tokenizer.Run(inPath);

            while (tokens.Count > 0)
            {
                Token token = tokens.Dequeue();
                Debug.Log($"({token.lineNumber}, {token.type}, \"{token.value.Replace("\n", "\\n")}\")");
                Console.WriteLine($"({token.lineNumber}, {token.type}, \"{token.value.Replace("\n", "\\n")}\")");
            }
            Parser.Parse(tokens);
        }
    }
    public enum BUILTIN_TYPE
    {
        VOID,
        INT8,
        UINT8,
        INT32,
        UINT32,
        DOUBLE,
        FLOAT,
        STRUCT
    }

    public class Type
    {
        public string name;
        public BUILTIN_TYPE type;
        public List<Type> field;
    }

    public static class Debug
    {
        public static string outPath;
        public static void Log(string log)
        {
            if (outPath == "")
            {
                Console.WriteLine(log);
            }
            
            StreamWriter sw = File.AppendText(outPath);
            sw.WriteLine(log);
            sw.Close();
        }

        public static void ClearLog()
        {
            if (!File.Exists(outPath))
                File.Create(outPath);
            else
            {
                StreamWriter sw = new StreamWriter(outPath);
                sw.Write("");
                sw.Close();
            }
        }
    }
}