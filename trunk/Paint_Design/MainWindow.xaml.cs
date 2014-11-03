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
        List<Ellipse> ellip = new List<Ellipse>();
        List<Polygon> polygon = new List<Polygon>();
        const int x0 = 103;
        const int y0 = 97;
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
        //xu ly su kien cho MenuItem Open
        private void Open_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "All Image File | *.*";
            dlg.Multiselect = false;
            if ((bool)dlg.ShowDialog())
                try
                {
                    ImageSource img = new BitmapImage(new Uri(dlg.FileName));
                    Image bitmap = new Image { Source = img };
                    Canvas.SetLeft(bitmap, 0);
                    Canvas.SetTop(bitmap, 0);
                    MyCanvas.Children.Clear();
                    MyCanvas.Children.Add(bitmap);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
        }
        //Xy ly su kien chi MenuItem Save
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "JPEG Image|*.jpg|PNG Image|*.png| Bitmap Image|*.bmp";
            dlg.FileName = "Image";
            if ((bool)dlg.ShowDialog())
                try
                {
                    string fileName = dlg.FileName;
                    int width = (int)MyCanvas.ActualWidth;
                    int height = (int)MyCanvas.ActualHeight;
                    RenderTargetBitmap rtb = new RenderTargetBitmap(width, height, 96d, 96d, PixelFormats.Default);
                    rtb.Render(MyCanvas);
                    using (FileStream fs = new FileStream(fileName, FileMode.Create))
                    {
                        BitmapEncoder encoder = new BmpBitmapEncoder();
                        encoder.Frames.Add(BitmapFrame.Create(rtb));
                        encoder.Save(fs);
                        fs.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Can not save File" + ex.Message);
                }

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
                line[line.Count-1].X2 = x-x0;
                line[line.Count-1].Y2 = y-y0;
            }
            if (status.getTool() == "ellip")
            {
                Ellipse ell;
                ell = ellip[ellip.Count - 1];
                   ell.Height = Math.Abs((e.GetPosition(this).Y - Canvas.GetTop(ell) - y0));
                   ell.Width = Math.Abs((e.GetPosition(this).X - Canvas.GetLeft(ell) - x0));
            }
            MousePosition.Content = (x.ToString() + "," + y.ToString());
        }
        // Xu ly su kien MouseUp tren panel Canvas
        private void Mycanvas_Mouse_Down(object sender, MouseEventArgs e)
        {
            if (status.getTool()=="line")
            {
                line[line.Count - 1].X1 = e.GetPosition(this).X - x0;
                line[line.Count - 1].Y1 = e.GetPosition(this).Y - y0;
                line[line.Count - 1].X2 = line[line.Count-1].X1;
                line[line.Count - 1].Y2 = line[line.Count-1].Y1;
                line[line.Count - 1].Stroke = System.Windows.Media.Brushes.Black;
                line[line.Count - 1].HorizontalAlignment = HorizontalAlignment.Left;
                line[line.Count - 1].VerticalAlignment = VerticalAlignment.Center;
                line[line.Count - 1].StrokeThickness = 2;
                MyCanvas.Children.Add(line[line.Count-1]);
            }
            if (status.getTool()=="ellip")
            {
                ellip[ellip.Count - 1].Height = 1;
                ellip[ellip.Count - 1].Width = 1;
                ellip[ellip.Count - 1].StrokeThickness = 1;
                ellip[ellip.Count - 1].Stroke = System.Windows.Media.Brushes.Black;
                Canvas.SetTop(ellip[ellip.Count - 1], e.GetPosition(this).Y - y0);
                Canvas.SetLeft(ellip[ellip.Count - 1], e.GetPosition(this).X - x0);
                MyCanvas.Children.Add(ellip[ellip.Count - 1]);
            }
        }
        // Xu ly su kien MouseUp tren panel Canvas
        private void Mycanvas_Mouse_Up(object sender, MouseEventArgs e)
        {
            if (status.getTool()=="line")
            {
                line[line.Count-1].X2 = e.GetPosition(this).X - x0;
                line[line.Count-1].Y2 = e.GetPosition(this).Y - y0;
                Line _line = new Line();
                line.Add(_line);
            }
            if (status.getTool()=="ellip")
            {
                Ellipse _ell = new Ellipse();
                ellip.Add(_ell);
            }
        }
        private void Undo_Click(object sender, RoutedEventArgs e)
        {
           int count = MyCanvas.Children.Count;
           if (count > 0)
               MyCanvas.Children.RemoveAt(count - 1);
             else MessageBox.Show("Can not undo");
        }

        private void Ellip_Click(object sender, RoutedEventArgs e)
        {
            Ellipse elip = new Ellipse();
            ellip.Add(elip);
            status.setTool("ellip");
        }
        
    }
}
