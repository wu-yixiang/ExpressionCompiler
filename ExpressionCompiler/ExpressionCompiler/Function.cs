using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionCompiler
{
    public class Function
    {
        private string name;
        private string description;
        private Parameters parameters;
        private string expression;

        public string Name { get => name; set => name = value; }
        public string Description { get => description; set => description = value; }
        public Parameters Parameters { get { if (parameters == null) parameters = new Parameters(); return parameters; } set => parameters = value; }
        public string Expression { get => expression; set => expression = value; }
    }
    public class Parameter {
        private string name;
        private string description;

        public string Name { get => name; set => name = value; }
        public string Description { get => description; set => description = value; }

    }
    public class Parameters : List<Parameter> {
        public int GetIndex(string name) {
            for (int i = 0; i < this.Count; i++) {
                if (this[i].Name.Equals(name)) {
                    return i;
                }
            }
            return -1;
        }
    }
}
