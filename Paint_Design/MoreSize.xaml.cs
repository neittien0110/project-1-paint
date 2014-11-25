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

namespace Paint_Design
{
    /// <summary>
    /// Interaction logic for MoreSize.xaml
    /// </summary>
    public partial class MoreSize : Window
    {
        public bool result = false;
        public bool change = false;
        public int int_size=2;

        public MoreSize()
        {
            InitializeComponent();
            change = true;
        }
        private void size_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (change==true)
            if (int.TryParse(size.Text, out int_size))
                line_example.StrokeThickness = int_size;
            else
                MessageBox.Show("Vui lòng nhập số");
        }
        private void Button_OK_Click(object sender, RoutedEventArgs e)
        {
            result = true;
            this.Close();
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
