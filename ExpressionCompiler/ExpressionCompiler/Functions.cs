using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionCompiler
{
    public class Functions:ObservableCollection<Function>
    {
        private static  Functions instance;
        /// <summary>
        /// 不可调用，仅用于Json转换
        /// </summary>
        public Functions() { }
        

        public static  Functions GetFunctions() {
            if (instance == null)
            {
                string strFunctions = System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "Functions.txt");
                instance = (Functions)JsonConvert.DeserializeObject(strFunctions, typeof(Functions));
            }
            return instance;
        }
        public Function GetFunction(string funcName)
        {
            foreach (Function func in instance) {
                if (funcName.Equals(func.Name)) {
                    return func;
                }
            }
            return null;
        }
    }
}
