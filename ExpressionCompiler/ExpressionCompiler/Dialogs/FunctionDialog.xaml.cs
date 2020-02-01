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
using System.Windows.Shapes;

namespace ExpressionCompiler.Dialogs
{
    /// <summary>
    /// FunctionDialog.xaml 的交互逻辑
    /// </summary>
    public partial class FunctionDialog : Window
    {
        private Function mFunction;
        public Function Function { get => mFunction; }
        public FunctionDialog()
        {
            InitializeComponent();
            txtParameters.Tag = new Parameters();
            mFunction = new Function();
        }

        private void Add_Button_Click(object sender, RoutedEventArgs e)
        {
            ParameterDialog parameterDialog = new ParameterDialog() { Owner = this };
            if (parameterDialog.ShowDialog() == true) {
                (txtParameters.Tag as Parameters).Add(parameterDialog.Parameter);
                

                StringBuilder stringBuilder = new StringBuilder(64);
                stringBuilder.Append("(");
                foreach (Parameter para in txtParameters.Tag as Parameters) {
                    stringBuilder.Append(para.Name + ", ");
                }
                stringBuilder.Remove(stringBuilder.Length - 2, 2);
                stringBuilder.Append(")");
                txtParameters.Text = stringBuilder.ToString();
            }
        }

        private void Sure_Button_Click(object sender, RoutedEventArgs e)
        {
            if(Validate()){
                FillProperties();
                DialogResult = true;
            }
        }
        private bool Validate() {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                //TODO 名称不能以数字开头，不能用特殊字符
                MessageBox.Show("请输入参数名称。", this.Title, MessageBoxButton.OK, MessageBoxImage.Stop, MessageBoxResult.OK);
                txtName.Focus();
            }
            if (string.IsNullOrWhiteSpace(txtBody.Text))
            {
                MessageBox.Show("请输入函数体。", this.Title, MessageBoxButton.OK, MessageBoxImage.Stop, MessageBoxResult.OK);
                txtName.Focus();
            }
            return true;
        }
        private void FillProperties() {
            mFunction.Name = txtName.Text.Trim();
            mFunction.Description = txtDescription.Text.Trim();
            mFunction.Parameters = txtParameters.Tag as Parameters;
            mFunction.Expression = txtBody.Text.Trim();
        }
    }
}
