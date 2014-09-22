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
using System.Drawing;
using Microsoft.Win32;
using System.IO;
using System.Windows.Ink;
using System.Collections;

namespace Paint_Design
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Status status = new Status();
        List<Line> line = new List<Line>();
        public MainWindow()
        {
            InitializeComponent();
        }
        //Xu ly su kien cho MenuItem New
        private void New_Click(object sender, RoutedEventArgs e)
        {
            MyCanvas.Children.Clear();
        }
        //Xu ly su kien cho MenuItem Exit
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        //Xu ly su kien cho Button line
        private void Button_Click_line(object sender, RoutedEventArgs e)
        {
            Line _line = new Line();
            line.Add(_line);
            status.setTool("line");
            
        }
        private void Button_Click_select(object sender, RoutedEventArgs e)
        {
            status.setTool("Select");
        }
        // Xu ly su kien MouseMove tren panel Canvas
        public void Mycanvas_Mouse_Move(object sender, MouseEventArgs e)
        {
            double x = e.GetPosition(this).X;
            double y = e.GetPosition(this).Y;
            if (status.getTool()=="line")
            {
                line[line.Count-1].X2 = x-103;
                line[line.Count-1].Y2 = y-79;
            }
            MousePosition.Content = (x.ToString() + "," + y.ToString());
        }
        // Xu ly su kien MouseUp tren panel Canvas
        private void Mycanvas_Mouse_Down(object sender, MouseEventArgs e)
        {
            if (status.getTool()=="line")
            {
                line[line.Count - 1].X1 = e.GetPosition(this).X - 103;
                line[line.Count - 1].Y1 = e.GetPosition(this).Y - 79;
                line[line.Count - 1].X2 = line[line.Count-1].X1;
                line[line.Count - 1].Y2 = line[line.Count-1].Y1;
                line[line.Count - 1].Stroke = System.Windows.Media.Brushes.LightSteelBlue;
                line[line.Count - 1].HorizontalAlignment = HorizontalAlignment.Left;
                line[line.Count - 1].VerticalAlignment = VerticalAlignment.Center;
                line[line.Count - 1].StrokeThickness = 2;
                MyCanvas.Children.Add(line[line.Count-1]);
            }
        }
        // Xu ly su kien MouseUp tren panel Canvas
        private void Mycanvas_Mouse_Up(object sender, MouseEventArgs e)
        {
            if (status.getTool()=="line")
            {
                line[line.Count-1].X2 = e.GetPosition(this).X - 103;
                line[line.Count-1].Y2 = e.GetPosition(this).Y - 79;
                Line _line = new Line();
                line.Add(_line);
            }
        }
        
    }
}
