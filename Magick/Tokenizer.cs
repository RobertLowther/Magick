using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Magick
{
    public static class Tokenizer
    {
        const string CONTROL_CHARS = "\r";
        const string WHITESPACE = " \t";
        const char NEWLINE = '\n';
        const string NUMBER_CHARS = "0123456789.";
        const string ALPHA_CHARS = "ABCDEFGHIJKLMNOPQRSTUVWKYZabcdefghijklmnopqrstuvwxyz_";
        const string IDENTIFIER_CHARS = "ABCDEFGHIJKLMNOPQRSTUVWKYZabcdefghijklmnopqrstuvwxyz_0123456789";
        const string SPECTIAL_CHARS = "{}()[],;";
        static List<string> OPERATORS = new List<string>() {
            "+", "-", "*", "/", "%", "!", "~", "<", ">", "&", "|",
            "?", ":", "^", "=", "||", "<<", ">>", "<=", ">=", "!=",
            "&&", "++", "--", "+=", "-=", "/=", "*=", "//"
        };

        public static Queue<Token> Run(string path)
        {
            int lineNum = 1;

            Queue<Token> tokens = new Queue<Token>();

            StreamReader sr = new StreamReader(path);
            
            while (sr.Peek() != -1)
            {
                Char c = (char)sr.Read();
                // we don't want to process these characters at this time
                if (WHITESPACE.Contains(c) || CONTROL_CHARS.Contains(c))
                    continue;

                else if (c == NEWLINE)
                    lineNum++;

                else if (SPECTIAL_CHARS.Contains(c))
                    tokens.Enqueue(new Token(TokenType.SPECIAL_SYMBOL, c.ToString(), lineNum));

                else if (OPERATORS.Contains(c.ToString()))
                {
                    string value = c.ToString();
                    if (OPERATORS.Contains(value + (char)sr.Peek()))
                        value += (char)sr.Read();
                    if (value == @"//")
                    {
                        sr.ReadLine();
                        lineNum++;
                    }
                    else
                        tokens.Enqueue(new Token(TokenType.OPERATOR, value, lineNum));
                }

                else if (c == '\'')
                {
                    if (sr.Peek() == -1)
                        tokens.Enqueue(new Token(TokenType.ERROR, "Unexpected EOF", lineNum));

                    c = (char)sr.Read();

                    if (sr.Peek() == -1)
                        tokens.Enqueue(new Token(TokenType.ERROR, "Unexpected EOF", lineNum));

                    if (sr.Peek() != (int)'\'')
                        tokens.Enqueue(new Token(TokenType.CONSTANT, c.ToString(), lineNum));
                    sr.ReadLine();
                }

                else if (c == '"')
                    tokens.Enqueue(new Token(TokenType.STRING, ScanString(c, sr), lineNum));

                else if (NUMBER_CHARS.Contains(c))
                {
                    if (c == '.') // this is either the start of a float/double or a namespace classifier
                    {
                        if (ALPHA_CHARS.Contains((char)sr.Peek())) // this is a namespace classifier
                        {
                            tokens.Enqueue(new Token(TokenType.SPECIAL_SYMBOL, ".", lineNum));
                            continue;
                        }
                    }

                    // this is the start of an int, float, or double
                    String number = Scan(c, sr, NUMBER_CHARS);
                    int dotCount = Regex.Matches(number, "[.]").Count;

                    if (sr.Peek() == 'f')
                        tokens.Enqueue(new Token(TokenType.CONSTANT, number + (char)sr.Read(), lineNum));
                    else if (dotCount == 0)
                        tokens.Enqueue(new Token(TokenType.CONSTANT, number, lineNum));
                    else if (dotCount == 1)
                        tokens.Enqueue(new Token(TokenType.CONSTANT, number, lineNum));
                }

                else if (ALPHA_CHARS.Contains(c))
                {
                    string str = Scan(c, sr, IDENTIFIER_CHARS);
                    tokens.Enqueue(new Token(Program.KEYWORDS.Contains(str) ? TokenType.KEYWORD : TokenType.IDENTIFIER, str, lineNum));
                }

                else
                    tokens.Enqueue(new Token(TokenType.ERROR, $"Unexpected token - '{c.ToString()}'", lineNum));
            }

            return tokens;
        }

        private static string Scan(char first, StreamReader sr, String allowed)
        {
            int dotCount = 0;
            string res = first.ToString();

            int next = sr.Peek();
            char nextChar = (char)next;

            while (next != -1)
            {
                char c;
                if (allowed.Contains(nextChar) && !(dotCount > 0 && nextChar == '.'))
                    c = (char)sr.Read();
                else
                    break;

                if (c == '.') dotCount++;

                res += c;
                next = sr.Peek();
                nextChar = (char)next;
            }

            return res;
        }

        private static string ScanString(Char delim, StreamReader sr)
        {
            string res = "";

            while (sr.Peek() != delim)
            {
                int next = sr.Read();

                if (next == -1)
                {
                    throw new IOException("Error - A string ran off the end of the file");
                }

                char c = (char)next;
                if (next == '~')
                    Console.WriteLine("");

                if (c == '\\')
                {
                    // check for escape characters
                    next = (char)sr.Peek();

                    if (next == 'n')
                        c = '\n';
                    else if (next == 'r')
                        c = '\r';
                    else if (next == 't')
                        c = '\t';
                    else if (next == '"')
                        c = '\"';
                    else if (next == '\'')
                        c = '\'';
                    else
                    {
                        res += c;
                        continue;
                    }

                    sr.Read();
                }

                res += c;
            }

            sr.Read();
            return res;
        }
    }
}