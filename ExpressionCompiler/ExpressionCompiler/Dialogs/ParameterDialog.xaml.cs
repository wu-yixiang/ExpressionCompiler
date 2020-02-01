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
    /// ParameterDialog.xaml 的交互逻辑
    /// </summary>
    public partial class ParameterDialog : Window
    {
        private Parameter mParameter;
        public Parameter Parameter { get => mParameter; }

        public ParameterDialog()
        {
            InitializeComponent();
        }


        private void Sure_Button_Click(object sender, RoutedEventArgs e)
        {
            if (Validate()) {
                mParameter = new Parameter() { Name = txtName.Text.Trim(), Description = txtDescription.Text.Trim() };
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
            return true;
        }
    }
}
