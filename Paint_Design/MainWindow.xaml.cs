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
        #region Thuoctinh
        public Status status = new Status();
        public Line line = new Line();
        public Polyline polyl = new Polyline();
        public MyEllip ellip = new MyEllip();
        public MyRectangle Myrectangle = new MyRectangle();
        public Point point_start = new Point(); 
        public Point pointdrag = new Point();
        public Boolean ClickDown =false;
        public Boolean Draging = new Boolean();
        public TextBox text;
        public BitmapSource source;
        public double dpi = 96d;
        Image img = new Image();
        private Color FillColor = Color.FromArgb(Convert.ToByte(00), Convert.ToByte(00), Convert.ToByte(00), Convert.ToByte(00));
        private Color boderColor=Color.FromArgb(Convert.ToByte(255),Convert.ToByte(00),Convert.ToByte(00),Convert.ToByte(00));
        #endregion
        #region Constructor
        public MainWindow()
        {
            InitializeComponent();
            status.setTool("Select");
            Button_Click_select(btn_Select, new RoutedEventArgs());
            MyCanvas.Width = 1100;
            MyCanvas.Height = 522;
        }
        #endregion
        
        
#region "Mouse events on MyCanvas"
        #region Mouse Down
// Xu ly su kien MouseDown tren panel Canvas
        public void Mycanvas_Mouse_Down(object sender, MouseEventArgs e)
        {
/// <ul>
/// <li>> Lấy tọa độ điểm mouse down - point_start </li>
            point_start.X = e.GetPosition(MyCanvas).X;
            point_start.Y = e.GetPosition(MyCanvas).Y;
/// <li>> Xử lý nếu công cụ đang được chọn là line - Vẽ đường thẳng </li>
            if (status.getTool() == "line")
            {
                status.setTool("lineDraw");
                line = new Line();
                line.X1 = e.GetPosition(MyCanvas).X;
                line.Y1 = e.GetPosition(MyCanvas).Y;
                line.X2 = line.X1;
                line.Y2 = line.Y1;
                line.Stroke = new SolidColorBrush(boderColor);
                line.HorizontalAlignment = HorizontalAlignment.Left;
                line.VerticalAlignment = VerticalAlignment.Center;
                line.StrokeThickness = (listSize.SelectedIndex + 1) * 2;
                MyCanvas.Children.Add(line);
            }
/// <li>> Xử lý nếu công cụ đang được chọn là ellip - Vẽ ellipse </li>
            if (status.getTool() == "ellip")
            {
                status.setTool("ellipDraw");
                ellip = new MyEllip();
                ellip.setX_Up(e.GetPosition(MyCanvas).X);
                ellip.setY_Up(e.GetPosition(MyCanvas).Y);
                ellip.ellip.Height = 1;
                ellip.ellip.Width = 1;
                ellip.ellip.Fill = new SolidColorBrush(FillColor);
                ellip.ellip.StrokeThickness = (listSize.SelectedIndex + 1) * 2;
                ellip.ellip.Stroke = new SolidColorBrush(boderColor);
                MyCanvas.Children.Add(ellip.ellip);

            }
/// <li>> Xử lý nếu công cụ đang được chọn là rectangle - Vẽ hình chữ nhật </li>
            if (status.getTool() == "rectangle")
            {
                Myrectangle = new MyRectangle();
                status.setTool("rectangleDraw");
                Myrectangle.setX_Up(e.GetPosition(MyCanvas).X);
                Myrectangle.setY_Up(e.GetPosition(MyCanvas).Y);
                Myrectangle.rectangle.Fill = new SolidColorBrush(FillColor);
                Myrectangle.rectangle.Height = 1;
                Myrectangle.rectangle.Width = 1;
                Myrectangle.rectangle.StrokeThickness = (listSize.SelectedIndex + 1) * 2;
                Myrectangle.rectangle.Stroke = new SolidColorBrush(boderColor);
                MyCanvas.Children.Add(Myrectangle.rectangle);

            }
/// <li>> Xử lý nếu công cụ đang được chọn là select </li>
            if (status.getTool() == "Select")
            {
                Canvas.SetZIndex(selectionRectangle, Canvas.GetZIndex(MyCanvas.Children[MyCanvas.Children.Count - 1]) + 1);
                ClickDown = true;
            }
/// <li>> Xử lý nếu công cụ đang được chọn là erase </li>
            if (status.getTool() == "erase")
            {
                Xoa(Canvas.GetLeft(erase_rec), Canvas.GetTop(erase_rec), erase_rec.Height, erase_rec.Width);
                status.setTool("erasing");
            }
/// <li>> Xử lý nếu công cụ đang được chọn là polyline </li>
            if (status.getTool() == "polyline")
            {
                polyl = new Polyline();
                polyl.Stroke = new SolidColorBrush(boderColor);
                polyl.StrokeThickness = (listSize.SelectedIndex + 1) * 2;
                status.setTool("polylinenext");
                polyl.Points.Add(point_start);
                MyCanvas.Children.Add(polyl);
            }
/// <li>> Xử lý nếu công cụ được chọn là text </li>
            if (status.getTool() == "text")
            {
                status.setTool("textDraw");
                text = new TextBox();
                text.BorderBrush = new SolidColorBrush(boderColor);
                text.Background = new SolidColorBrush(FillColor);
                text.Visibility = Visibility.Visible;
                MyCanvas.Children.Add(text);
            }
/// <li>> Xử lý khi trạng thái là Selected ( đã chọn) </li>
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
                img = new Image();
            }
/// <li>> Xử lý khi trạng thái là write</li>            
            if (status.getTool() == "write")
            {
                status.setTool("text");
            }

        }
/// </ul>
#endregion
        #region "Mouse Move"
        /// <summary>
        /// Xử lý sự kiện MouseMove trên MyCanvas
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Mycanvas_Mouse_Move(object sender, MouseEventArgs e)
        {
            statusbar.Text = status.getTool();
            /// - Lấy tọa độ chuột
            double x = e.GetPosition(MyCanvas).X;
            double y = e.GetPosition(MyCanvas).Y;
            /// - Xử lí
            /// <ul>
            /// <li> Vẽ đoạn thẳng</li>
            if (status.getTool()=="lineDraw")
            {
             
                line.X2 = x;
                line.Y2 = y;
            }
            /// <li>Vẽ ellip </li>
            if (status.getTool() == "ellipDraw")
            {
                ellip.ellip.Height = Math.Abs((y - ellip.getY_Up()));
                ellip.ellip.Width = Math.Abs((x - ellip.getX_Up()));
                if (e.GetPosition(MyCanvas).Y >ellip.getY_Up())
                {
                    Canvas.SetTop(ellip.ellip, ellip.getY_Up());
                }
                else Canvas.SetTop(ellip.ellip, e.GetPosition(MyCanvas).Y);
                
                if (e.GetPosition(MyCanvas).X > ellip.getX_Up())
                {
                    Canvas.SetLeft(ellip.ellip, ellip.getX_Up());
                }
                else Canvas.SetLeft(ellip.ellip, e.GetPosition(MyCanvas).X);
            }
            /// <li> Vẽ hình chữ nhật </li>
            if (status.getTool() == "rectangleDraw")
            {

                Myrectangle.rectangle.Height = Math.Abs((y - Myrectangle.getY_Up()));
                Myrectangle.rectangle.Width = Math.Abs((x - Myrectangle.getX_Up()));
                if (e.GetPosition(MyCanvas).Y > Myrectangle.getY_Up())
                {
                    Canvas.SetTop(Myrectangle.rectangle, Myrectangle.getY_Up());
                }
                else Canvas.SetTop(Myrectangle.rectangle, e.GetPosition(MyCanvas).Y);

                if (e.GetPosition(MyCanvas).X > Myrectangle.getX_Up())
                {
                    Canvas.SetLeft(Myrectangle.rectangle, Myrectangle.getX_Up());
                }
                else Canvas.SetLeft(Myrectangle.rectangle, e.GetPosition(MyCanvas).X);
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
                if (selectionRectangle.Visibility == Visibility.Collapsed) ;
                img.MouseLeftButtonDown -= selectionRectangle_MouseLeftButtonDown;
                img.Cursor = Cursors.Cross;
            }
            if (status.getTool() == "Draging")
            {
                Canvas.SetLeft(selectionRectangle, x - pointdrag.X-1);
                Canvas.SetLeft(MyCanvas.Children[MyCanvas.Children.Count-1], x-pointdrag.X);
                Canvas.SetTop(selectionRectangle, y - pointdrag.Y-1);
                Canvas.SetTop(MyCanvas.Children[MyCanvas.Children.Count - 1], y - pointdrag.Y);
                //MyCanvas.Children[MyCanvas.Children.Count - 1].Visibility = Visibility.Visible;
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
            if (status.getTool() == "polylinenext")
            {
                polyl.Points.Add(new Point(x, y));
            }
            if (status.getTool() == "textDraw")
            {
                text.SetValue(Canvas.LeftProperty, Math.Min(x, point_start.X));
                text.SetValue(Canvas.TopProperty, Math.Min(y, point_start.Y));
                text.Width = Math.Abs(x - point_start.X);
                text.Height = Math.Abs(y - point_start.Y);
            }
            MousePosition.Content = (x.ToString() + "," + y.ToString());
            /// </ul>
        }
        #endregion
        #region Mouse Up
        // Xu ly su kien MouseUp tren panel Canvas
        public void Mycanvas_Mouse_Up(object sender, MouseEventArgs e)
        {
            if (status.getTool()=="lineDraw")
            {
                status.setTool("line");
            }
            if (status.getTool()=="ellipDraw")
            {
                status.setTool("ellip");
            }
            if (status.getTool()=="rectangleDraw")
            {
                status.setTool("rectangle");
            }
             if ((ClickDown==true)&&(status.getTool() == "Select"))
             {
                 ClickDown = false;
                 if ((selectionRectangle.Width > 2) && (selectionRectangle.Height > 2))
                 {

                     status.setTool("Selected");
                     Rect bounds = new Rect();                   
                     bounds = VisualTreeHelper.GetDescendantBounds(MyCanvas);
                     RenderTargetBitmap rtb = new RenderTargetBitmap((int)bounds.Width, (int)bounds.Height, dpi, dpi, PixelFormats.Default);
                     DrawingVisual dv = new DrawingVisual();
                     using (DrawingContext dc = dv.RenderOpen())
                     {
                         VisualBrush vb = new VisualBrush(MyCanvas);
                         dc.DrawRectangle(vb, null, new Rect(new Point(), bounds.Size));
                     }

                     rtb.Render(dv);
                     CroppedBitmap cropbmt = new CroppedBitmap(rtb, new Int32Rect((int)Math.Max((Canvas.GetLeft(selectionRectangle) + 1),1), (int)Math.Max((Canvas.GetTop(selectionRectangle)+ 1),1), (int)(selectionRectangle.Width - 2), (int)(selectionRectangle.Height - 2)));
                     img = new Image { Source = cropbmt };
                     source = cropbmt;
                     Canvas.SetLeft(img, Canvas.GetLeft(selectionRectangle) + 1);
                     Canvas.SetTop(img, Canvas.GetTop(selectionRectangle) + 1);
                     double width = selectionRectangle.Width;
                     double height = selectionRectangle.Height;
                     img.MouseLeftButtonDown += selectionRectangle_MouseLeftButtonDown;
                     Xoa(Canvas.GetLeft(selectionRectangle) + 1, Canvas.GetTop(selectionRectangle) + 1, width - 2, height - 2);
                     img.Cursor = Cursors.SizeAll;
                     img.ContextMenu = imgContext;
                     MyCanvas.Children.Add(img);
                     Canvas.SetZIndex(selectionRectangle, Canvas.GetZIndex(img) + 1);
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
             if (status.getTool() == "polylinenext")
             {
                 status.setTool("polyline");
             }
             if (status.getTool() == "textDraw")
             {
                 status.setTool("write");
             }
        }
        #endregion
#endregion
        #region Xoa
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
        #region Command Undo
        private void undo_CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = MyCanvas.Children.Count > 3;
        }
        public void undo_CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
           int count = MyCanvas.Children.Count;
           if (count > 0)
               MyCanvas.Children.RemoveAt(count - 1);
             else MessageBox.Show("Can not undo");
        }
        #endregion

        #region Toolbar Click
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
            ///- Thêm vào danh sách đường thẳng ban đầu
            status.setTool("line");
            selectionRectangle.Visibility = Visibility.Collapsed;
        }
        public void Button_Click_select(object sender, RoutedEventArgs e)
        {
            status.setTool("Select");
            selectionRectangle.Fill = null;
        }
        public void Ellip_Click(object sender, RoutedEventArgs e)
        {
            status.setTool("ellip");
            selectionRectangle.Visibility = Visibility.Collapsed;
        }
        public void Rectangle_Click(object sender, RoutedEventArgs e)
        {
            status.setTool("rectangle");
            selectionRectangle.Visibility = Visibility.Collapsed;
        }
        public void erase_Click(object sender, RoutedEventArgs e)
        {
            status.setTool("erase");
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
        private void fill_Click(object sender, RoutedEventArgs e)
        {
            PickColor pickcolor = new PickColor(FillColor);
            if ((bool)pickcolor.ShowDialog())
            {
            }
            if (pickcolor.result) FillColor = pickcolor.color;
            lColor.Background = new SolidColorBrush(FillColor);
        }
        private void Boder_Click(object sender, RoutedEventArgs e)
        {
            PickColor pickcolor = new PickColor(boderColor);
            if ((bool)pickcolor.ShowDialog())
            {
            }
            if (pickcolor.result) boderColor = pickcolor.color;
            BoderColor.Background = new SolidColorBrush(boderColor);
        }

        private void PolyLine_Click(object sender, RoutedEventArgs e)
        {
            selectionRectangle.Visibility = Visibility.Collapsed;
            status.setTool("polyline");
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            Xoa(Canvas.GetLeft(img), Canvas.GetTop(img), selectionRectangle.Width - 2, selectionRectangle.Height - 2);
        }
        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetImage(source);
        }
        private void Resize_Click(object sender, RoutedEventArgs e)
        {
            Resize resize= new Resize(MyCanvas.Width,MyCanvas.Height);
            if ((bool)resize.ShowDialog())
            {
            }
            if (resize.result)
            {
                if (resize.Mode == "Pixel")
                {
                    MyCanvas.Width = resize.width;
                    MyCanvas.Height = resize.height;
                }
                if (resize.Mode == "Per")
                {
                    MyCanvas.Width = MyCanvas.Width * resize.width / 100;
                    MyCanvas.Height = MyCanvas.Height * resize.height / 100;
                }
            }
        }
        #endregion
        #region Event
        private void selectionRectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            status.setTool("Draging");
            point_start.X = e.GetPosition(MyCanvas).X;
            point_start.Y = e.GetPosition(MyCanvas).Y;
            MyCanvas.Children[MyCanvas.Children.Count - 2].Visibility = Visibility.Visible;
            pointdrag.X = point_start.X - Canvas.GetLeft(selectionRectangle);
            pointdrag.Y = point_start.Y - Canvas.GetTop(selectionRectangle);
        }
        private void main_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (main.Width < 1000) SCW.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
            if (main.Height < 500) SCW.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
        }
        public void Text_Click(object sender, RoutedEventArgs e)
        {
            status.setTool("text");
        }

        private void del_CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (status.getTool() == "Selected");
        }

        private void del_CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Xoa(Canvas.GetLeft(img), Canvas.GetTop(img), selectionRectangle.Width - 2, selectionRectangle.Height - 2);
            selectionRectangle.Visibility = Visibility.Collapsed;
        }
        private void Paste_Click(object sender, RoutedEventArgs e)
        {
            img.MouseLeftButtonDown -= selectionRectangle_MouseLeftButtonDown;
            BitmapSource bitmap = Clipboard.GetImage();
            img = new Image {Source = bitmap};
            Canvas.SetLeft(img, 0);
            Canvas.SetTop(img, 0);
            img.MouseLeftButtonDown += selectionRectangle_MouseLeftButtonDown;
            img.Cursor = Cursors.SizeAll;
            MyCanvas.Children.Add(img);
            Canvas.SetLeft(selectionRectangle, 0);
            Canvas.SetTop(selectionRectangle, 0);
            //MessageBox.Show("" + IMG.Height + IMG.Width);
            selectionRectangle.Width =img.Source.Width;
            selectionRectangle.Height =img.Source.Height;
            selectionRectangle.Visibility = Visibility.Visible;
            Canvas.SetZIndex(selectionRectangle, Canvas.GetZIndex(MyCanvas.Children[MyCanvas.Children.Count-1])+1);
            status.setTool("Selected");
        }
        #endregion
    }
}
