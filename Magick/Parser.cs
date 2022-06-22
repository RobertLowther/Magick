using System;
using System.Collections.Generic;

namespace Magick
{
    public static class Parser
    {

        public static void Parse(Queue<Token> tokens)
        {
            if (tokens.Count == 0)
                return;

            Token currentToken = tokens.Dequeue();

            while(tokens.Count > 0)
            {



                currentToken = tokens.Dequeue();
            }
        }

        static Token expectedIdentifier(string name)
        {
            return new Token(TokenType.ERROR, "");
        }
    }
}