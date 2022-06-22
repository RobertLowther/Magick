namespace Magick
{
    public class Token
    {
        public readonly TokenType type = TokenType.ERROR;
        public readonly string value = "";
        public readonly int lineNumber;

        public Token(TokenType t, string v, int line = -1)
        {
            type = t;
            value = v;
            lineNumber = line;
        }
    }

    public enum TokenType
    {
        IDENTIFIER,
        KEYWORD,
        CONSTANT,
        OPERATOR,
        SPECIAL_SYMBOL,
        STRING,
        ERROR,
    }
}