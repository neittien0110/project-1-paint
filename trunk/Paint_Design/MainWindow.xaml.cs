﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Paint_Design
{
    /// <summary>
    /// Chương trình chính 
    /// </summary>
    public partial class MainWindow : Window
    {
        public Status status = new Status(); 
        public List<Line> line = new List<Line>(); 
        public List<MyEllip> ellip = new List<MyEllip>();
        public List<Polygon> polygon = new List<Polygon>();
        public List<MyRectangle> rectangle = new List<MyRectangle>();
        public MainWindow()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Xóa hết các hình trên panel Canvas
        /// </summary>
        public void New_Click(object sender, RoutedEventArgs e)
        {
            /// Xử lý sự kiện khi click vào MenuItem New
            /// Sử dụng hàm Clear() được dựng sẵn để xóa các đối tượng trong MyCanvas
            MyCanvas.Children.Clear();
        }
        /// <summary>
        ///  Xử lý sự kiện khi click vào MenuItem Exit
        /// </summary>
        public void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        /*!
         * @brief Hàm mở OpenFileDialog và chọn file Image
         */
        public void Open_Click(object sender, RoutedEventArgs e)
        {
            ///- Thực hiện : File image được chọn được load trên MyCanvas
            ///- Quá trình:
            /// <ul>
            /// <li> Tạo dialog</li>          
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "All Image File | *.*";
            dlg.Multiselect = false;
            /// <li> Hiển thị dialog </li>
            if ((bool)dlg.ShowDialog())
                try
                {
                    /// <li> Load file image</li>
                    ImageSource img = new BitmapImage(new Uri(dlg.FileName));
                    Image bitmap = new Image { Source = img };
                    Canvas.SetLeft(bitmap, 0);
                    Canvas.SetTop(bitmap, 0);
                    MyCanvas.Children.Clear();
                    /// <li> add image vào MyCanvas</li>
                    /// </ul>
                    MyCanvas.Children.Add(bitmap);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
        }
        //Xy ly su kien chi MenuItem Save
        /// <summary>
        ///  Lưu file image lên ổ cứng
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Save_Click(object sender, RoutedEventArgs e)
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
        /// <summary>
        ///  Khi click vào Tool vẽ đường thằng . . .
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Button_Click_line(object sender, RoutedEventArgs e)
        {
            ///- Thêm 1 đối tượng thuộc lớp System.Windows.Shapes.Line
            Line _line = new Line();
            line.Add(_line);
            ///- Thêm vào danh sách đường thẳng ban đầu
            status.setTool("line");
            
        }
        public void Button_Click_select(object sender, RoutedEventArgs e)
        {
            status.setTool("Select");
        }
        // Xu ly su kien MouseMove tren panel Canvas
        /// <summary>
        /// Xử lý sự kiện MouseMove trên MyCanvas
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Mycanvas_Mouse_Move(object sender, MouseEventArgs e)
        {   
            /// - Lấy tọa độ chuột
            double x = e.GetPosition(MyCanvas).X;
            double y = e.GetPosition(MyCanvas).Y;
            /// - Xử lí
            /// <ul>
            /// <li> Vẽ đoạn thẳng</li>
            if (status.getTool()=="line")
            {
             
                line[line.Count-1].X2 = x;
                line[line.Count-1].Y2 = y;
            }
            /// <li>Vẽ ellip </li>
            if (status.getTool() == "ellip")
            {
                MyEllip myellip= ellip[ellip.Count-1];
                Ellipse ell;
                ell = ellip[ellip.Count - 1].ellip;
                ell.Height = Math.Abs((y - myellip.getY_Up()));
                ell.Width = Math.Abs((x - myellip.getX_Up()));
                if (e.GetPosition(MyCanvas).Y > myellip.getY_Up())
                {
                    Canvas.SetTop(myellip.ellip, myellip.getY_Up());
                }
                else Canvas.SetTop(myellip.ellip, e.GetPosition(MyCanvas).Y);
                
                if (e.GetPosition(MyCanvas).X > myellip.getX_Up())
                {
                    Canvas.SetLeft(myellip.ellip, myellip.getX_Up());
                }
                else Canvas.SetLeft(myellip.ellip, e.GetPosition(MyCanvas).X);
            }
            /// <li> Vẽ hình chữ nhật </li>
            if (status.getTool() == "rectangle")
            {
                MyRectangle myrec = rectangle[rectangle.Count - 1];
                Rectangle rec;
                rec = rectangle[rectangle.Count - 1].rectangle;
                rec.Height = Math.Abs((y - myrec.getY_Up()));
                rec.Width = Math.Abs((x - myrec.getX_Up()));
                if (e.GetPosition(MyCanvas).Y > myrec.getY_Up())
                {
                    Canvas.SetTop(myrec.rectangle, myrec.getY_Up());
                }
                else Canvas.SetTop(myrec.rectangle, e.GetPosition(MyCanvas).Y );

                if (e.GetPosition(MyCanvas).X > myrec.getX_Up())
                {
                    Canvas.SetLeft(myrec.rectangle, myrec.getX_Up());
                }
                else Canvas.SetLeft(myrec.rectangle, e.GetPosition(MyCanvas).X);
            }
            MousePosition.Content = (x.ToString() + "," + y.ToString());
            /// </ul>
        }
        // Xu ly su kien MouseUp tren panel Canvas
        public void Mycanvas_Mouse_Down(object sender, MouseEventArgs e)
        {
            if (status.getTool()=="line")
            {
                line[line.Count - 1].X1 = e.GetPosition(MyCanvas).X ;
                line[line.Count - 1].Y1 = e.GetPosition(MyCanvas).Y ;
                line[line.Count - 1].X2 = line[line.Count-1].X1;
                line[line.Count - 1].Y2 = line[line.Count-1].Y1;
                line[line.Count - 1].Stroke = System.Windows.Media.Brushes.Black;
                line[line.Count - 1].HorizontalAlignment = HorizontalAlignment.Left;
                line[line.Count - 1].VerticalAlignment = VerticalAlignment.Center;
                line[line.Count - 1].StrokeThickness = (listSize.SelectedIndex + 1)*2;
                MyCanvas.Children.Add(line[line.Count-1]);
            }
            if (status.getTool()=="ellip")
            {

                ellip[ellip.Count - 1].setX_Up(e.GetPosition(MyCanvas).X );
                ellip[ellip.Count - 1].setY_Up(e.GetPosition(MyCanvas).Y );
                ellip[ellip.Count - 1].ellip.Height = 1;
                ellip[ellip.Count - 1].ellip.Width = 1;
                ellip[ellip.Count - 1].ellip.StrokeThickness = (listSize.SelectedIndex + 1) * 2;
                ellip[ellip.Count - 1].ellip.Stroke = System.Windows.Media.Brushes.Black;
                MyCanvas.Children.Add(ellip[ellip.Count - 1].ellip);

            }
            if (status.getTool() == "rectangle")
            {

                rectangle[rectangle.Count - 1].setX_Up(e.GetPosition(MyCanvas).X);
                rectangle[rectangle.Count - 1].setY_Up(e.GetPosition(MyCanvas).Y);
                rectangle[rectangle.Count - 1].rectangle.Height = 1;
                rectangle[rectangle.Count - 1].rectangle.Width = 1;
                rectangle[rectangle.Count - 1].rectangle.StrokeThickness = (listSize.SelectedIndex + 1) * 2;
                rectangle[rectangle.Count - 1].rectangle.Stroke = System.Windows.Media.Brushes.Black;
                MyCanvas.Children.Add(rectangle[rectangle.Count - 1].rectangle);

            }
        }
        // Xu ly su kien MouseUp tren panel Canvas
        public void Mycanvas_Mouse_Up(object sender, MouseEventArgs e)
        {
            if (status.getTool()=="line")
            {
                line[line.Count-1].X2 = e.GetPosition(MyCanvas).X;
                line[line.Count-1].Y2 = e.GetPosition(MyCanvas).Y;
                Line _line = new Line();
                line.Add(_line);
            }
            if (status.getTool()=="ellip")
            {
                MyEllip _ell = new MyEllip();
                ellip.Add(_ell);
            }
             if (status.getTool()=="rectangle")
             {
                 MyRectangle _rec = new MyRectangle();
                 rectangle.Add(_rec);
             }
        }
       
        public void Undo_Click(object sender, RoutedEventArgs e)
        {
           int count = MyCanvas.Children.Count;
           if (count > 0)
               MyCanvas.Children.RemoveAt(count - 1);
             else MessageBox.Show("Can not undo");
        }

        public void Ellip_Click(object sender, RoutedEventArgs e)
        {
            MyEllip ell = new MyEllip();
            ellip.Add(ell);
            status.setTool("ellip");
        }
        public void Rectangle_Click(object sender, RoutedEventArgs e)
        {
            MyRectangle rec = new MyRectangle();
            rectangle.Add(rec);
            status.setTool("rectangle");
        }

       
    }
}