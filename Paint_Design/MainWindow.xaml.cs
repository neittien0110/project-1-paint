using Microsoft.Win32;
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
        public Point point_start = new Point(); 
        public Point pointdrag = new Point();
        public Boolean ClickDown = new Boolean();
        public Boolean Draging = new Boolean();
        public double dpi = 96d;
        Image img = new Image();
        public MainWindow()
        {
            InitializeComponent();
            ClickDown = false;
            status.setTool("Select");
            Button_Click_select(btn_Select, new RoutedEventArgs());
        }
        /// <summary>
        /// Xóa hết các hình trên panel Canvas
        /// </summary>
        public void New_Click(object sender, RoutedEventArgs e)
        {
            /// Xử lý sự kiện khi click vào MenuItem New
            /// Sử dụng hàm Clear() được dựng sẵn để xóa các đối tượng trong MyCanvas
            MyCanvas.Children.Clear();
            selectionRectangle.Visibility = Visibility.Collapsed;
            MyCanvas.Children.Add(selectionRectangle);
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
                    System.Console.WriteLine(bitmap.Height);
                    MyCanvas.Height = img.Height;
                    MyCanvas.Width = img.Width;
                    Canvas.SetLeft(bitmap, 0);
                    Canvas.SetTop(bitmap, 0);
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
                    Rect bounds = VisualTreeHelper.GetDescendantBounds(MyCanvas);
                    double dpi = 96d;
                    RenderTargetBitmap rtb = new RenderTargetBitmap((int)bounds.Width, (int)bounds.Height, dpi, dpi, PixelFormats.Default);
                    DrawingVisual dv = new DrawingVisual();
                    using (DrawingContext dc = dv.RenderOpen())
                    {
                        VisualBrush vb = new VisualBrush(MyCanvas);
                        dc.DrawRectangle(vb, null, new Rect(new Point(), bounds.Size));
                    }

                    rtb.Render(dv);
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
            selectionRectangle.Visibility = Visibility.Collapsed;
        }
        public void Button_Click_select(object sender, RoutedEventArgs e)
        {
            status.setTool("Select");
            Canvas.SetZIndex(selectionRectangle, Canvas.GetZIndex(MyCanvas.Children[MyCanvas.Children.Count - 1]) + 1);
            selectionRectangle.Fill = null;
        }
        // Xu ly su kien MouseMove tren panel Canvas
        /// <summary>
        /// Xử lý sự kiện MouseMove trên MyCanvas
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
#region "Mouse events on MyCanvas"
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
            if (status.getTool() == "Select")
            {
                if (ClickDown)
                {
                    selectionRectangle.SetValue(Canvas.LeftProperty, Math.Min(x, point_start.X));
                    selectionRectangle.SetValue(Canvas.TopProperty, Math.Min(y, point_start.Y));
                    selectionRectangle.Width = Math.Abs(x - point_start.X);
                    selectionRectangle.Height = Math.Abs(y - point_start.Y);
                    if (selectionRectangle.Visibility != Visibility.Visible)
                        selectionRectangle.Visibility = Visibility.Visible;
                }
            }
            if (status.getTool() != "Selected")
            {
                img.MouseLeftButtonDown -= selectionRectangle_MouseLeftButtonDown;
                img.Cursor = Cursors.Cross;
            }
            if (status.getTool() == "Draging")
            {
                Canvas.SetLeft(selectionRectangle, x - pointdrag.X-1);
                Canvas.SetLeft(img, x-pointdrag.X);
                Canvas.SetTop(selectionRectangle, y - pointdrag.Y-1);
                Canvas.SetTop(img, y-pointdrag.Y);
                img.Visibility = Visibility.Visible;
            }
            if ((status.getTool() == "erase")||((status.getTool() == "erasing")))
                if ((x>0)&&(y>0))
            {
                Canvas.SetZIndex(erase_rec, Canvas.GetZIndex(MyCanvas.Children[MyCanvas.Children.Count-1])+1);
                erase_rec.Width = (listSize.SelectedIndex + 1) * 4;
                erase_rec.Height = (listSize.SelectedIndex + 1) * 4;
                Canvas.SetLeft(erase_rec, x - erase_rec.Width/2);
                Canvas.SetTop(erase_rec, y - erase_rec.Width/2);
                erase_rec.Visibility = Visibility.Visible;
            }
            if (status.getTool() == "erasing")
                if ((x > 0) && (y > 0))
            {
                Rectangle xoa = new Rectangle();
                Canvas.SetLeft(xoa, Canvas.GetLeft(erase_rec));
                Canvas.SetTop(xoa, Canvas.GetTop(erase_rec));
                xoa.Width = erase_rec.Width;
                xoa.Height = erase_rec.Height;
                xoa.Fill = Brushes.White;
                MyCanvas.Children.Add(xoa);
            }
            MousePosition.Content = (x.ToString() + "," + y.ToString());
            /// </ul>
        }
        // Xu ly su kien MouseDown tren panel Canvas
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
            if (status.getTool() == "Select")
            {
                point_start.X = e.GetPosition(MyCanvas).X;
                point_start.Y = e.GetPosition(MyCanvas).Y;
                ClickDown = true;
            }
            if (status.getTool() == "Selected")
            {
                selectionRectangle.Height = 0;
                selectionRectangle.Width = 0;
                selectionRectangle.Visibility = Visibility.Collapsed;
                img.MouseLeftButtonDown -= selectionRectangle_MouseLeftButtonDown;
                img.Cursor = Cursors.Cross;
                status.setTool("Select");
                point_start.X = e.GetPosition(MyCanvas).X;
                point_start.Y = e.GetPosition(MyCanvas).Y;
                ClickDown = true;
                img=new Image();
            }
            if (status.getTool()=="erase")
            {
                Xoa(Canvas.GetLeft(erase_rec), Canvas.GetTop(erase_rec), erase_rec.Height, erase_rec.Width);
                status.setTool("erasing");
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
             if ((ClickDown==true)&&(status.getTool() == "Select"))
             {
                 ClickDown = false;
                 if ((selectionRectangle.Width > 2) && (selectionRectangle.Height > 2))
                 {

                     status.setTool("Selected");
              
                     Rect bounds = VisualTreeHelper.GetDescendantBounds(MyCanvas);
                     RenderTargetBitmap rtb = new RenderTargetBitmap((int)bounds.Width, (int)bounds.Height, dpi, dpi, PixelFormats.Default);
                     DrawingVisual dv = new DrawingVisual();
                     using (DrawingContext dc = dv.RenderOpen())
                     {
                         VisualBrush vb = new VisualBrush(MyCanvas);
                         dc.DrawRectangle(vb, null, new Rect(new Point(), bounds.Size));
                     }

                     rtb.Render(dv);
                     CroppedBitmap cropbmt = new CroppedBitmap(rtb, new Int32Rect((int)Canvas.GetLeft(selectionRectangle) + 1, (int)Canvas.GetTop(selectionRectangle) + 1, (int)selectionRectangle.Width - 2, (int)selectionRectangle.Height - 2));
                     img = new Image { Source = cropbmt };
                     Canvas.SetLeft(img, Canvas.GetLeft(selectionRectangle) + 1);
                     Canvas.SetTop(img, Canvas.GetTop(selectionRectangle) + 1);
                     double width = selectionRectangle.Width;
                     double height = selectionRectangle.Height;
                     img.MouseLeftButtonDown += selectionRectangle_MouseLeftButtonDown;
                     Xoa(Canvas.GetLeft(selectionRectangle) + 1, Canvas.GetTop(selectionRectangle) + 1, width - 2, height - 2);
                     img.Cursor = Cursors.SizeAll;
                     MyCanvas.Children.Add(img);
                     Canvas.SetZIndex(selectionRectangle, Canvas.GetZIndex(img) + 2);
                     selectionRectangle.Fill = null;
                 }
             }
             if (status.getTool() == "Draging")
             {
                 status.setTool("Selected");
             }
             if (status.getTool() == "erasing")
             {
                  status.setTool("erase");
             }
        }

        private void Xoa(double p1, double p2, double width, double height)
        {
            Rectangle xoa = new Rectangle();
            Canvas.SetLeft(xoa, p1);
            Canvas.SetTop(xoa, p2);
            xoa.Width = width;
            xoa.Height = height;
            xoa.Fill = Brushes.White;
            MyCanvas.Children.Add(xoa);
        }
#endregion    
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
            selectionRectangle.Visibility = Visibility.Collapsed;
        }
        public void Rectangle_Click(object sender, RoutedEventArgs e)
        {
            MyRectangle rec = new MyRectangle();
            rectangle.Add(rec);
            status.setTool("rectangle");
            selectionRectangle.Visibility = Visibility.Collapsed;
        }
        public void erase_Click(object sender, RoutedEventArgs e)
        {
            status.setTool("erase");
        }
        private void selectionRectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            
            status.setTool("Draging");
            point_start.X = e.GetPosition(MyCanvas).X;
            point_start.Y = e.GetPosition(MyCanvas).Y;
            MyCanvas.Children[MyCanvas.Children.Count - 2].Visibility = Visibility.Visible;
            pointdrag.X = point_start.X - Canvas.GetLeft(selectionRectangle);
            pointdrag.Y = point_start.Y - Canvas.GetTop(selectionRectangle);
        }

        private void btn_Select_Unchecked(object sender, RoutedEventArgs e)
        {
            selectionRectangle.Visibility = Visibility.Collapsed;
            img.MouseLeftButtonDown -= selectionRectangle_MouseLeftButtonDown;
            img.Cursor = Cursors.Cross;
            ClickDown = false;
            img = new Image();
        }

        private void RadioButton_Unchecked(object sender, RoutedEventArgs e)
        {
            erase_rec.Visibility = Visibility.Collapsed;
            erase_rec.Height = 0;
            erase_rec.Width = 0;
        }
               
    }
}
