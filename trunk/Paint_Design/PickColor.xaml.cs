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

namespace Paint_Design
{
    /// <summary>
    /// Interaction logic for PickColor.xaml
    /// </summary>
    public partial class PickColor : Window
    {
        public Boolean result=false;
        public Color color = new Color();
        public PickColor(Color start)
        {
            InitializeComponent();
            A_Slider.Value = start.A;
            R_Slider.Value = start.R;
            G_Slider.Value = start.G;
            B_Slider.Value = start.B;
        }
        public Color colorReturn()
        {
            return color;
        }
        public void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            color = Color.FromArgb(Convert.ToByte(A_Slider.Value), Convert.ToByte(R_Slider.Value), Convert.ToByte(G_Slider.Value), Convert.ToByte(B_Slider.Value));
            tempColor.Fill =new SolidColorBrush(color);
            A_Value.Content = ((int)(A_Slider.Value*100/255)) +" %";
            R_Value.Content = (int)R_Slider.Value;
            G_Value.Content = (int)G_Slider.Value;
            B_Value.Content = (int)B_Slider.Value;
        }

        public void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            pickcl.Close();
        }
        public void Btn_Ok_Click(object sender, RoutedEventArgs e)
        {
            result = true;
            this.Close();
        }
    }
}
