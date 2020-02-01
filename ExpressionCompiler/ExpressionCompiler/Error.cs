using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionCompiler
{
    public class Error
    {
        private int level;
        private string message;

        public int Level { get => level; set => level = value; }
        public string Message { get => message; set => message = value; }
    }
}
