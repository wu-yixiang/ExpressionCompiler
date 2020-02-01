using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ExpressionCompiler
{
    public class Compiler
    {
        private const int STATE_INIT = 0;
        private const int STATE_IDENTIFYER = 1;
        //private const int STATE_OPERATOR = 2;
        private const int STATE_NUMBER = 2;
        /// <summary>
        /// 输入:原始的字符串
        /// 输出:Token串
        /// </summary>
        /// <returns></returns>
        public object LexicalAnalysis(string expression) {
            int row = 1;
            int col = 0;
            StringBuilder sb = new StringBuilder(16);
            int state = STATE_INIT;
            int priority = 0;//(:+10    ):-10    [:+1    ]:-1
            Tokens tokens = new Tokens();

            string newExpression = expression + "#";
            foreach (char ch in newExpression) {
                col++;
                switch (ch) {
                    case ' ':
                        if (state == STATE_INIT) continue;
                        //else if (state == STATE_OPERATOR) continue;
                        else if (state == STATE_NUMBER)
                        {
                            tokens.Add(new Token() { Type = TokenType.Number, Value = sb.ToString() });
                            sb.Clear();
                        }
                        else if (state == STATE_IDENTIFYER)
                        {
                            tokens.Add(new Token() { Type = TokenType.Identifier, Value = sb.ToString() });
                            sb.Clear();
                        }
                        else {
                            //MessageBox.Show("无法识别字符" + ch + "。" + "行:" + row + "，列:" + col + "。");
                            return new Error() { Level = 1, Message = "无法识别字符" + ch + "。" + "行:" + row + "，列:" + col + "。" };
                        }
                        break;
                    case '#':
                        if (state == STATE_INIT) {

                        }
                        else if (state == STATE_NUMBER)
                        {
                            tokens.Add(new Token() { Type = TokenType.Number, Value = sb.ToString() });
                            sb.Clear();
                        }
                        else if (state == STATE_IDENTIFYER)
                        {
                            tokens.Add(new Token() { Type = TokenType.Identifier, Value = sb.ToString() });
                            sb.Clear();
                        }
                        else
                        {
                            return new Error() { Level = 1, Message = "无法识别字符" + ch + "。" + "行:" + row + "，列:" + col + "。" };
                        }
                        tokens.Add(new Token() { Type = TokenType.End, Value = ch + "" });
                        break;
                    case (char)13://\r 回车
                        continue;
                    case (char)10://换行符
                        row++;
                        col = 0;
                        continue;
                    case '0': case '1': case '2': case '3': case '4': case '5': case '6': case '7': case '8': case '9':
                        if (state == STATE_INIT)
                        {
                            state = STATE_NUMBER;
                            sb.Append(ch);
                        }
                        else if (state == STATE_NUMBER)
                        {
                            sb.Append(ch);
                        }
                        else if (state == STATE_IDENTIFYER) {
                            sb.Append(ch);
                        }
                        else
                        {
                            //MessageBox.Show("无法识别字符" + ch + "。" + "行:" + row + "，列:" + col + "。");
                            return new Error() { Level = 1, Message = "无法识别字符" + ch + "。" + "行:" + row + "，列:" + col + "。" };
                        }
                        break;
                    case 'a': case 'b':case 'c':case 'd':case 'e':case 'f':case 'g':case 'h':case 'i':case 'j':case 'k':case 'l':case 'm':case 'n':case 'o':case 'p':case 'q':case 'r':case 's':case 't':case 'u':case 'v':case 'w':case 'x':case 'y':case 'z':
                    case 'A': case 'B':case 'C':case 'D':case 'E':case 'F':case 'G':case 'H':case 'I':case 'J':case 'K':case 'L':case 'M':case 'N':case 'O':case 'P':case 'Q':case 'R':case 'S':case 'T':case 'U':case 'V':case 'W':case 'X':case 'Y':case 'Z':
                    case '_':
                        if (state == STATE_INIT)
                        {
                            state = STATE_IDENTIFYER;
                            sb.Append(ch);
                        }
                        else if (state == STATE_IDENTIFYER)
                        {
                            sb.Append(ch);
                        }
                        else {
                            //MessageBox.Show("无法识别字符"+ch+"。"+"行:"+ row+"，列:"+col+"。");
                            return new Error() { Level = 1, Message = "无法识别字符" + ch + "。" + "行:" + row + "，列:" + col + "。" };
                        }
                        break;
                    case ',':
                        if (state == STATE_INIT)
                        {
                            state = STATE_IDENTIFYER;
                        }
                        else if (state == STATE_NUMBER)
                        {
                            tokens.Add(new Token() { Type = TokenType.Number, Value = sb.ToString() });
                            sb.Clear();
                        }
                        else if (state == STATE_IDENTIFYER)
                        {
                            tokens.Add(new Token() { Type = TokenType.Identifier, Value = sb.ToString() });
                            sb.Clear();
                        }
                        else
                        {
                            return new Error() { Level = 1, Message = "无法识别字符" + ch + "。" + "行:" + row + "，列:" + col + "。" };
                        }
                        state = STATE_INIT;
                        tokens.Add(new Token() { Type = TokenType.Comma, Value = ch + "" });
                        break;
                    case '+': case '-': case '*':case '/':
                        if (state == STATE_INIT)
                        {
                            tokens.Add(new Token() { Type = TokenType.Operator, Value = ch+"" });
                        }
                        else if (state == STATE_NUMBER)
                        {
                            tokens.Add(new Token() { Type = TokenType.Number, Value = sb.ToString() });
                            sb.Clear();
                            tokens.Add(new Token() { Type = TokenType.Operator, Value = ch + "" });
                        }
                        else if (state == STATE_IDENTIFYER)
                        {
                            tokens.Add(new Token() { Type = TokenType.Identifier, Value = sb.ToString() });
                            sb.Clear();
                            tokens.Add(new Token() { Type = TokenType.Operator, Value = ch + "" });
                        }
                        else
                        {
                            return new Error() { Level = 1, Message = "无法识别字符" + ch + "。" + "行:" + row + "，列:" + col + "。" };
                        }
                        state = STATE_INIT;
                        break;
                    case '(':
                        if (state == STATE_INIT)
                        {
                        }
                        else if (state == STATE_NUMBER)
                        {
                            tokens.Add(new Token() { Type = TokenType.Number, Value = sb.ToString() });
                            sb.Clear();
                        }
                        else if (state == STATE_IDENTIFYER)
                        {
                            tokens.Add(new Token() { Type = TokenType.Identifier, Value = sb.ToString() });
                            sb.Clear();
                        }
                        else
                        {
                            return new Error() { Level = 1, Message = "无法识别字符" + ch + "。" + "行:" + row + "，列:" + col + "。" };
                        }
                        state = STATE_INIT;
                        priority += 10;
                        tokens.Add(new Token() { Type = TokenType.ParenthesesLeft });
                        break;
                    case ')':
                        if (state == STATE_INIT)
                        {
                        }
                        else if (state == STATE_NUMBER)
                        {
                            tokens.Add(new Token() { Type = TokenType.Number, Value = sb.ToString() });
                            sb.Clear();
                        }
                        else if (state == STATE_IDENTIFYER)
                        {
                            tokens.Add(new Token() { Type = TokenType.Identifier, Value = sb.ToString() });
                            sb.Clear();
                        }
                        else
                        {
                            return new Error() { Level = 1, Message = "无法识别字符" + ch + "。" + "行:" + row + "，列:" + col + "。" };
                        }
                        state = STATE_INIT;
                        priority -= 10;
                        tokens.Add(new Token() { Type = TokenType.ParenthesesRight});
                        break;
                    case '.':
                        if (state == STATE_NUMBER) {
                            sb.Append(ch);
                        }
                        else
                        {
                            return new Error() { Level = 1, Message = "无法识别字符" + ch + "。" + "行:" + row + "，列:" + col + "。" };
                        }
                        break;
                    default:
                        if (ch > 127)
                        {
                            if (state == STATE_INIT)
                            {
                                state = STATE_IDENTIFYER;
                                sb.Append(ch);
                            }
                            else if (state == STATE_IDENTIFYER)
                            {
                                sb.Append(ch);
                            }
                            else
                            {
                                return new Error() { Level = 1, Message = "无法识别字符" + ch + "。" + "行:" + row + "，列:" + col + "。" };
                            }
                        }
                        else {
                            return new Error() { Level = 1, Message = "无法识别字符" + ch + "。" + "行:" + row + "，列:" + col + "。" };
                        }
                        break;
                }
            }
            if (priority != 0) {
                return new Error() { Level = 1, Message = "扩号混乱。" };
            }

            return tokens;
        }
    }

    
}
