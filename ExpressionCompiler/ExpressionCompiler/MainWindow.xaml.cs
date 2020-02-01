using ExpressionCompiler.Dialogs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ExpressionCompiler
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private Compiler mCompiler = new Compiler();
        private Functions mFunctions;
        private int cursorIndex;
        public MainWindow()
        {
            InitializeComponent();
            mCompiler = new Compiler();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            mFunctions = Functions.GetFunctions();
            lstFunctions.ItemsSource = mFunctions;
            if (mFunctions.Count > 0)
                lstFunctions.SelectedIndex = 0;
        }
        private void Calculate_Button_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            object result = mCompiler.LexicalAnalysis(txtExpression.Text.Trim());
            if (result is Tokens)
            {
                Tokens tokens = result as Tokens;
                txtResult.Text = Calculate(tokens) + "";
            }
            else if (result is Error)
            {
                MessageBox.Show((result as Error).Message, "计算表达式", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
            else {
                MessageBox.Show("未知错误", "计算表达式", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
            Cursor = Cursors.Arrow;
        }

        private double? GetFunctionResult(Function func, List<double> paras) {
            double? result = null;
            object obj = mCompiler.LexicalAnalysis(func.Expression);
            if (obj is Tokens)
            {
                Tokens tokens = obj as Tokens;
                for (int i = 0; i < tokens.Count; i++) {
                    if (tokens[i].Type == TokenType.Identifier) {
                        int index = func.Parameters.GetIndex(tokens[i].Value);
                        if (index > -1 && index < paras.Count) {
                            tokens[i].Type = TokenType.Number;
                            tokens[i].Value = paras[index] + "";
                        }
                    }
                }
                result = Calculate(tokens);
            }
            else if (obj is Error)
            {
                MessageBox.Show((obj as Error).Message, "处理函数" + func.Name, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
            else
            {
                MessageBox.Show("未知错误", "处理函数" + func.Name, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
            return result;
        }
        private double Calculate(Tokens tokens) {
            int curPriority = 0;
            Stack<double> opNumbers = new Stack<double>();
            Stack<PriorityOp> opOperators = new Stack<PriorityOp>();
            for (int i = 0; i < tokens.Count; i++)
            {
                switch (tokens[i].Type)
                {
                    case TokenType.Number:
                        opNumbers.Push(double.Parse(tokens[i].Value));//TODO 检测正确性
                        break;
                    case TokenType.Operator:
                        int priority = curPriority;
                        if ("*".Equals(tokens[i].Value) || "//".Equals(tokens[i].Value))
                            priority++;
                        while (opOperators.Count > 0 && opOperators.Peek().priority >= priority)
                        {
                            opNumbers.Push(Calculate(opNumbers.Pop(), opNumbers.Pop(), opOperators.Pop().op));
                        }
                        opOperators.Push(new PriorityOp(priority, tokens[i].Value.ToCharArray()[0]));
                        break;
                    case TokenType.End:
                        while (opOperators.Count > 0)
                        {
                            opNumbers.Push(Calculate(opNumbers.Pop(), opNumbers.Pop(), opOperators.Pop().op));
                        }
                        break;
                    case TokenType.Identifier:
                        Function function = mFunctions.GetFunction(tokens[i].Value);
                        if (function != null)
                        {
                            int paraCount = function.Parameters.Count;
                            if (tokens[i + 1].Type == TokenType.ParenthesesLeft)
                            {
                                //List<double> paras = new List<double>();
                                //for (int j = i + 2; j < i + 1 + paraCount * 2; j+=2)
                                //{
                                //    paras.Add(double.Parse(tokens[j].Value));
                                //}
                                //double? funcResult = GetFunctionResult(function, paras);
                                //if (funcResult.HasValue) {
                                //    opNumbers.Push(funcResult.Value);
                                //    i += function.Parameters.Count * 2 + 1;
                                //}
                                //else
                                //    break;
                                List<double> paras = new List<double>();
                                int lPriority = 0;
                                int paraStart = i + 2;
                                Tokens paraTokens = new Tokens();
                                int j = paraStart;
                                for (; j < tokens.Count; j++)
                                {
                                    if (tokens[j].Type == TokenType.Comma && lPriority == 0)
                                    {
                                        paraTokens.Add(new Token() { Type = TokenType.End });
                                        paras.Add(Calculate(paraTokens));
                                        paraTokens.Clear();
                                        paraStart = j + 1;
                                        continue;
                                    }
                                    else if (tokens[j].Type == TokenType.ParenthesesLeft)
                                    {
                                        lPriority += 10;
                                    }
                                    else if (tokens[j].Type == TokenType.ParenthesesRight)
                                    {
                                        if (lPriority == 0)
                                        {
                                            if (paraTokens.Count > 0) {
                                                paraTokens.Add(new Token() { Type = TokenType.End });
                                                paras.Add(Calculate(paraTokens));
                                            }
                                            break;
                                        }
                                        lPriority -= 10;
                                    }
                                    paraTokens.Add(tokens[j]);
                                }
                                double? funcResult = GetFunctionResult(function, paras);
                                if (funcResult.HasValue)
                                {
                                    opNumbers.Push(funcResult.Value);
                                    i = j;
                                }
                                else
                                    break;
                            }
                            else
                            {
                                MessageBox.Show("函数 " + tokens[i].Value + " 参数个数不合适。", "计算表达式", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                                break;
                            }
                        }
                        else
                        {
                            MessageBox.Show("无法识别函数 " + tokens[i].Value + " 。", "计算表达式", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                            break;
                        }
                        break;
                    case TokenType.ParenthesesLeft:
                        curPriority += 10;
                        break;
                    case TokenType.ParenthesesRight:
                        curPriority -= 10;
                        break;

                }
            }
            return opNumbers.Pop();
        }
        private double Calculate(double arg1, double arg2, char opOperator) {
            double result = 0;
            switch (opOperator)
            {
                case '+':
                    result = arg2 + arg1;
                    break;
                case '-':
                    result = arg2 - arg1;
                    break;
                case '*':
                    result = arg2 * arg1;
                    break;
                case '/':
                    result = arg2 / arg1;
                    break;
                default:
                    break;
            }

            return result;
        }

        private void lstFunctions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstFunctions.SelectedItem == null) return;
            //显示说明
            Function function = lstFunctions.SelectedItem as Function;
            StringBuilder stringBuilder = new StringBuilder(512);
            stringBuilder.AppendLine("函数名称：" + function.Name);
            stringBuilder.AppendLine("函数体：" + function.Expression);
            stringBuilder.AppendLine("描述：" + function.Description);
            for (int i = 0; i < function.Parameters.Count; i++) {
                stringBuilder.AppendLine("参数" + (i + 1) + "：" + function.Parameters[i].Name + "，" + function.Parameters[i].Description);
            }
            txtDescription.Text = stringBuilder.ToString();
        }

        private void lstFunctions_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!(lstFunctions.SelectedItem is Function)) return;
            Function function = lstFunctions.SelectedItem as Function;
            int parametersCount = function.Parameters.Count;
            StringBuilder sbInsertString = new StringBuilder();
            sbInsertString.Append(function.Name + "(");
            if (parametersCount > 1)
                sbInsertString.Append(',', parametersCount - 1);
            sbInsertString.Append(")");

            txtExpression.Text = txtExpression.Text.Insert(cursorIndex, sbInsertString.ToString());
            txtExpression.SelectionStart = cursorIndex + sbInsertString.Length - parametersCount;
            txtExpression.Focus();
        }
        private void txtExpression_LostFocus(object sender, RoutedEventArgs e)
        {
            cursorIndex = txtExpression.SelectionStart;
        }
        

        private void AddFunction_Button_Click(object sender, RoutedEventArgs e)
        {
            FunctionDialog functionDialog = new FunctionDialog() { Owner = this };
            if (functionDialog.ShowDialog() == true) {
                mFunctions.Add(functionDialog.Function);
                System.IO.File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "Functions.txt", JsonConvert.SerializeObject(mFunctions, Formatting.None));
            }
        }

        struct PriorityOp
        {
            //操作符的优先级
            public int priority;
            //操作符
            public char op;
            public PriorityOp(int priority, char op)
            {
                this.priority = priority;
                this.op = op;
            }
        }

    }
}
