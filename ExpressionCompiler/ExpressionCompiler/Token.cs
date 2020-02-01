using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionCompiler
{
    public class Token
    {
        private TokenType type;
        private string value;

        public TokenType Type { get => type; set => type = value; }
        public string Value { get => value; set => this.value = value; }
    }
    public class Tokens : List<Token>
    {
    }
    public enum TokenType {
        Identifier = 0,
        Number = 1,
        Operator = 2,
        ParenthesesLeft = 3,
        ParenthesesRight = 4,
        End = 5,
        Comma = 6
    }

}
