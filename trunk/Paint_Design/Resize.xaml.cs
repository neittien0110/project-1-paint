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
    /// Interaction logic for Resize.xaml
    /// </summary>
    public partial class Resize : Window
    {
        public double width;
        public double height;
        public String Mode="Pixel";
        public Boolean result = false;
        public Resize(double x,double y)
        {
            InitializeComponent();
            width = x;
            height = y;
            Hoz.Text = width.ToString();
            Ver.Text = height.ToString();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            result = true;
            if ((double.TryParse(Hoz.Text, out width))&&(double.TryParse(Ver.Text, out height))){
                this.Close();
            } else
                MessageBox.Show("Vui lòng nhập số!");
        }    
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Per_checked(object sender, RoutedEventArgs e)
        {
            Mode = "Per";
            Hoz.Text = "100";
            Ver.Text = "100";
        }
        private void Per_Unchecked(object sender, RoutedEventArgs e)
        {
            Mode = "Pixel";
            Hoz.Text = width.ToString();
            Ver.Text = height.ToString();
        }
    }
}
