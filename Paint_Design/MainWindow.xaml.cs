using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
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
        /// <summary>
        /// Trạng thái của phần mềm
        /// </summary>
        public Status status = new Status();
        public Line line = new Line();
        public Polyline polyl = new Polyline();
        public Ellipse ellip = new Ellipse();
        public Rectangle rectangle = new Rectangle();
        public Point point_start = new Point();
        public Point pointdrag = new Point();
        public Boolean ClickDown = false;
        public Boolean Dragging = new Boolean();
        public TextBox text;
        public BitmapSource source;
        public Canvas savedCanvas = null;
        public Image img = new Image();
        #endregion
        #region Constructor
        public MainWindow()
        {
            InitializeComponent();
            Button_Click_select(btn_Select, new RoutedEventArgs());
            MyCanvas.Width = 1100;
            MyCanvas.Height = 522;
        }
        #endregion


        #region "Mouse events on MyCanvas"
        #region Mouse Down
        // Xu ly su kien MouseDown tren panel Canvas
        /// <summary>
        ///  Xu ly su kien MouseDown tren panel Canvas
        /// </summary>
        /// <param name="sender"></param>
        /// <param name=""></param>
        public void Mycanvas_Mouse_Down(object sender, MouseEventArgs e)
        {
            /// <ul>
            /// <li>> Lấy tọa độ điểm mouse down - point_start </li>
            ClickDown = true;
            point_start.X = e.GetPosition(MyCanvas).X;
            point_start.Y = e.GetPosition(MyCanvas).Y;
            /// <li>> Xử lý nếu công cụ đang được chọn là line - Vẽ đường thẳng
            if (status.getTool() == "line")
            {
                /// <ul> 
                /// <li> setTool "lineDraw" và khởi tạo đối tượng line </li>
                status.setTool("lineDraw");
                line = new Line();
                /// <li> Set các thuộc tính cho line </li>
                line.X1 = e.GetPosition(MyCanvas).X;
                line.Y1 = e.GetPosition(MyCanvas).Y;
                line.X2 = line.X1;
                line.Y2 = line.Y1;
                line.Stroke = new SolidColorBrush(status.getBoderColor());
                line.HorizontalAlignment = HorizontalAlignment.Left;
                line.VerticalAlignment = VerticalAlignment.Center;
                line.StrokeThickness = status.getSize();
                /// <li> add lỉne vào Pannel Canvas
                MyCanvas.Children.Add(line);
                /// </ul>
            }
            /// </li>
            /// <li>> Xử lý nếu công cụ đang được chọn là ellip - Vẽ ellipse
            if (status.getTool() == "ellip")
            {
                /// <ul>
                ///  <li> setTool "ellipDraw" và khởi tạo </li>
                status.setTool("ellipDraw");
                ellip = new Ellipse();
                /// <li> Set các thuộc tính cho ellip </li>
                ellip.Height = 1;
                ellip.Width = 1;
                ellip.Fill = new SolidColorBrush(status.getFillColor());
                ellip.StrokeThickness = status.getSize();
                ellip.Stroke = new SolidColorBrush(status.getBoderColor());
                /// <li> add ellip vào Pannel Canvas
                MyCanvas.Children.Add(ellip);
                /// </ul>
                /// </li>

            }
            /// <li>> Xử lý nếu công cụ đang được chọn là rectangle - Vẽ hình chữ nhật
            if (status.getTool() == "rectangle")
            {
                /// <ul>
                ///  <li> setTool "rectangleDraw" và khởi tạo </li>
                rectangle = new Rectangle();
                status.setTool("rectangleDraw");
                /// <li> Set các thuộc tính cho rectangle </li>
                rectangle.Fill = new SolidColorBrush(status.getFillColor());
                rectangle.Height = 1;
                rectangle.Width = 1;
                rectangle.StrokeThickness = status.getSize();
                rectangle.Stroke = new SolidColorBrush(status.getBoderColor());
                /// <li> add ellip vào Pannel Canvas
                MyCanvas.Children.Add(rectangle);
                /// </ul>
            }
            ///  </li>
            /// <li>> Xử lý khi trạng thái là Selected ( đã chọn) </li>
            if (status.getTool() == "Selected")
            {
                selectionRectangle.Height = 0;
                selectionRectangle.Width = 0;
                selectionRectangle.Visibility = Visibility.Collapsed;
                img.MouseLeftButtonDown -= selectionRectangle_MouseLeftButtonDown;
                img.Cursor = null;
                img.ContextMenu = null;
                status.setTool("Select");
                point_start.X = e.GetPosition(MyCanvas).X;
                point_start.Y = e.GetPosition(MyCanvas).Y;
                ClickDown = true;
                img = new Image();
            }
            /// <li>> Xử lý nếu công cụ đang được chọn là select </li>
            if (status.getTool() == "Select")
            {
                /// -       SetZIndex
                status.setTool("Selecting");
                Canvas.SetZIndex(selectionRectangle, Canvas.GetZIndex(MyCanvas.Children[MyCanvas.Children.Count - 1]) + 1);
            }
            /// <li>> Xử lý nếu công cụ đang được chọn là erase </li>
            if (status.getTool() == "erase")
            {
                /// -       Gọi hàm Xoa(Canvas.GetLeft(erase_rec), Canvas.GetTop(erase_rec), erase_rec.Height, erase_rec.Width)
                Xoa(Canvas.GetLeft(erase_rec), Canvas.GetTop(erase_rec), erase_rec.Height, erase_rec.Width);
                polyl = new Polyline();
                /// -       Set thuộc tính
                polyl.Stroke = Brushes.White;
                polyl.StrokeThickness = status.getSize() * 2;
                polyl.Points.Add(point_start);
                status.setTool("erasing");
                MyCanvas.Children.Add(polyl);
            }
            /// <li>> Xử lý nếu công cụ đang được chọn là polyline </li>
            if (status.getTool() == "polyline")
            {
                /// -       SetTool "polylinenext" và khởi tạo đối tượng kiểu Polyline
                polyl = new Polyline();
                status.setTool("polylinenext");
                /// -       Set thuộc tính
                polyl.Stroke = new SolidColorBrush(status.getBoderColor());
                polyl.StrokeThickness = status.getSize();
                polyl.Points.Add(point_start);
                /// -       Thêm vào panel Canvas
                MyCanvas.Children.Add(polyl);
            }
            /// <li>> Xử lý nếu công cụ được chọn là text </li>
            if (status.getTool() == "text")
            {
                /// -       setTool "textDraw" và khởi tạo đối tượng kiểu TextBox
                status.setTool("textDraw");
                text = new TextBox();
                /// -       set thuộc tính
                text.BorderBrush = new SolidColorBrush(status.getBoderColor());
                text.Background = new SolidColorBrush(status.getFillColor());
                text.Visibility = Visibility.Visible;
                /// -       Thêm vào panel Canvas
                MyCanvas.Children.Add(text);
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
            /// <li> Nếu trạng thái là "erase" hoặc "erasing" </li>
            if ((status.getTool() == "erase") || ((status.getTool() == "erasing")))
                if ((x > 0) && (y > 0))
                {
                    MyCanvas.Cursor = Cursors.None;
                    erase_rec.Width = status.getSize() * 2;
                    erase_rec.Height = status.getSize() * 2;
                    Canvas.SetZIndex(erase_rec, Canvas.GetZIndex(MyCanvas.Children[MyCanvas.Children.Count - 1]) + 1);
                    Canvas.SetLeft(erase_rec, x - erase_rec.Width / 2);
                    Canvas.SetTop(erase_rec, y - erase_rec.Width / 2);
                    erase_rec.Visibility = Visibility.Visible;
                }
                else erase_rec.Visibility = Visibility.Collapsed;
            else MyCanvas.Cursor = Cursors.Pen;
            /// <li> Nếu ClickDown==true 
            /// <ul>
            if (ClickDown)
            {
                /// <li> Nếu trạng thái là "lineDraw" => vẽ đoạn thẳng từ </li>
                if (status.getTool() == "lineDraw")
                {
                    line.X2 = x;
                    line.Y2 = y;
                }
                /// <li>Vẽ ellip </li>
                if (status.getTool() == "ellipDraw")
                {
                    ellip.Height = Math.Abs(y - point_start.Y);
                    ellip.Width = Math.Abs(x - point_start.X);
                    ellip.SetValue(Canvas.LeftProperty, Math.Min(x, point_start.X));
                    ellip.SetValue(Canvas.TopProperty, Math.Min(y, point_start.Y));
                }
                /// <li> Vẽ hình chữ nhật </li>
                if (status.getTool() == "rectangleDraw")
                {

                    rectangle.Height = Math.Abs(y - point_start.Y);
                    rectangle.Width = Math.Abs(x - point_start.X);
                    rectangle.SetValue(Canvas.LeftProperty, Math.Min(x, point_start.X));
                    rectangle.SetValue(Canvas.TopProperty, Math.Min(y, point_start.Y));
                }
                /// <li> Nếu trạng thái là "Select" </li>
                if (status.getTool() == "Selecting")
                {
                    {
                        selectionRectangle.SetValue(Canvas.LeftProperty, Math.Min(x, point_start.X));
                        selectionRectangle.SetValue(Canvas.TopProperty, Math.Min(y, point_start.Y));
                        selectionRectangle.Width = Math.Abs(x - point_start.X);
                        selectionRectangle.Height = Math.Abs(y - point_start.Y);
                        if (selectionRectangle.Visibility != Visibility.Visible)
                            selectionRectangle.Visibility = Visibility.Visible;
                    }
                }
                /// <li> Nếu trạng thái là "Selected" </li>
                if (status.getTool() != "Selected")
                {
                    if (selectionRectangle.Visibility == Visibility.Collapsed)
                    {
                        img.MouseLeftButtonDown -= selectionRectangle_MouseLeftButtonDown;
                        img.Cursor = null;
                        img.ContextMenu = null;
                    }
                }
                /// <li> Nếu trạng thái là "Dragging" </li>
                if (status.getTool() == "Dragging")
                {
                    Canvas.SetLeft(selectionRectangle, x - pointdrag.X - 1);
                    Canvas.SetLeft(MyCanvas.Children[MyCanvas.Children.Count - 1], x - pointdrag.X);
                    Canvas.SetTop(selectionRectangle, y - pointdrag.Y - 1);
                    Canvas.SetTop(MyCanvas.Children[MyCanvas.Children.Count - 1], y - pointdrag.Y);
                }
                /// <li> Nếu trạng thái chỉ là "erasing" </li>
                if (status.getTool() == "erasing")
                    if ((x > 0) && (y > 0))
                    {
                        polyl.Points.Add(new Point(x, y));
                    }
                /// <li> Nếu trạng thái là "polylinenext" </li>
                if (status.getTool() == "polylinenext")
                {
                    polyl.Points.Add(new Point(x, y));
                }
                /// <li> Nếu trạng thái là "textDraw" </li>
                if (status.getTool() == "textDraw")
                {
                    text.SetValue(Canvas.LeftProperty, Math.Min(x, point_start.X));
                    text.SetValue(Canvas.TopProperty, Math.Min(y, point_start.Y));
                    text.Width = Math.Abs(x - point_start.X);
                    text.Height = Math.Abs(y - point_start.Y);
                }
            }
            /// </ul>
            /// </li>
            /// <li> Hiển thị tọa độ lên statusBar </li>
            MousePosition.Content = (x.ToString() + "," + y.ToString());
            /// </ul>
        }
        #endregion
        #region Mouse Up
        // Xu ly su kien MouseUp tren panel Canvas
        /// <summary>
        ///  Xu ly su kien MouseUp tren panel Canvas
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Mycanvas_Mouse_Up(object sender, MouseEventArgs e)
        {
            /// <ul> 
            /// <li> Vẽ xong line - trả về trạng thái "line" </li>
            if (status.getTool() == "lineDraw")
            {
                status.setTool("line");
            }
            /// <li> Vẽ xong ellipse - trả về trạng thái "ellip" </li>
            if (status.getTool() == "ellipDraw")
            {
                status.setTool("ellip");
            }
            /// <li> Vẽ xong rectangle - trả về trạng thái "rectangle" </li>
            if (status.getTool() == "rectangleDraw")
            {
                status.setTool("rectangle");
            }
            /// <li> Xử lí sau khi Select - Chuyển sang "Selected" </li>
            if ((ClickDown == true) && (status.getTool() == "Selecting"))
            {
                if ((selectionRectangle.Width > 0) && (selectionRectangle.Height > 0))
                {
                    if (Canvas.GetLeft(selectionRectangle) < 0)
                    {
                        selectionRectangle.Width = selectionRectangle.Width + Canvas.GetLeft(selectionRectangle);
                        Canvas.SetLeft(selectionRectangle, 0);
                    }
                    if (Canvas.GetTop(selectionRectangle) < 0)
                    {
                        selectionRectangle.Height = selectionRectangle.Height + Canvas.GetTop(selectionRectangle);
                        Canvas.SetTop(selectionRectangle, 0);
                    }
                    status.setTool("Selected");
                    selectionRectangle.Visibility = Visibility.Hidden;
                    /// -       Xử lí để có Bitmapsource từ panel Mycanvas
                    Rect bounds = new Rect(0, 0, MyCanvas.Width, MyCanvas.Height);
                    double dpi = 96d;
                    RenderTargetBitmap rtb = new RenderTargetBitmap((int)bounds.Width, (int)bounds.Height, dpi, dpi, PixelFormats.Default);
                    DrawingVisual dv = new DrawingVisual();
                    using (DrawingContext dc = dv.RenderOpen())
                    {
                        VisualBrush vb = new VisualBrush(MyCanvas);
                        dc.DrawRectangle(vb, null, new Rect(new Point(), bounds.Size));
                    }
                    rtb.Render(dv);
                    /// -       Cắt lấy phần trong selectionRectangle
                    CroppedBitmap cropbmt = new CroppedBitmap(rtb, new Int32Rect((int)Math.Max((Canvas.GetLeft(selectionRectangle)), 0), (int)Math.Max((Canvas.GetTop(selectionRectangle)), 0), (int)(selectionRectangle.Width), (int)(selectionRectangle.Height)));
                    img = new Image { Source = cropbmt };
                    source = cropbmt;
                    Canvas.SetLeft(img, Canvas.GetLeft(selectionRectangle));
                    Canvas.SetTop(img, Canvas.GetTop(selectionRectangle));
                    double width = selectionRectangle.Width;
                    double height = selectionRectangle.Height;
                    selectionRectangle.Visibility = Visibility.Visible;
                    img.MouseLeftButtonDown += selectionRectangle_MouseLeftButtonDown;
                    Xoa(Canvas.GetLeft(selectionRectangle), Canvas.GetTop(selectionRectangle), width, height);
                    img.Cursor = Cursors.SizeAll;
                    img.ContextMenu = imgContext;
                    MyCanvas.Children.Add(img);
                    Canvas.SetZIndex(selectionRectangle, Canvas.GetZIndex(img) + 1);
                    selectionRectangle.Fill = null;
                }
            }
            /// <li> Nếu trạng thái đang là "Dragging" - Chuyển sang "Selected" </li>
            if (status.getTool() == "Dragging")
            {
                status.setTool("Selected");
            }
            /// <li> Nếu trạng thái đang là "erasing" - Chuyển sang "erase" </li>
            if (status.getTool() == "erasing")
            {
                status.setTool("erase");
            }
            /// <li> Nếu trạng thái đang là "polylinenext" - Chuyển sang "polyline" </li>
            if (status.getTool() == "polylinenext")
            {
                status.setTool("polyline");
            }
            /// <li> Nếu trạng thái đang là "TextDraw" - Chuyển sang "write" </li>
            if (status.getTool() == "textDraw")
            {
                status.setTool("write");
            }
            ClickDown = false;
        }
        #endregion
        #endregion
        #region Xoa
        public void Xoa(double p1, double p2, double width, double height)
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
        #region Menu Click
        /// <summary>
        /// Menu Resize
        /// </summary>
        /// <param name="menuResize"></param>
        /// <param name="e"></param>
        public void Resize_Click(object menuResize, RoutedEventArgs e)
        {
            Resize resize = new Resize(MyCanvas.Width, MyCanvas.Height);
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
        /// <summary>
        /// Xử lí khi click vào chọn FillColor
        /// </summary>
        /// <param name="btnFill"></param>
        /// <param name="e"></param>
        public void fill_Click(object btnFill, RoutedEventArgs e)
        {
            PickColor pickcolor = new PickColor(status.getFillColor());
            pickcolor.Title = "Edit Fill Color";
            if ((bool)pickcolor.ShowDialog())
            { }
            if (pickcolor.result) status.setFillColor(pickcolor.color);
            lColor.Background = new SolidColorBrush(status.getFillColor());
        }
        /// <summary>
        /// Xử lí khi click vào chọn BoderColor
        /// </summary>
        /// <param name="btnBoder"></param>
        /// <param name="e"></param>
        public void Boder_Click(object btnBoder, RoutedEventArgs e)
        {
            PickColor pickcolor = new PickColor(status.getBoderColor());
            pickcolor.Title = "Edit Boder Color";
            if ((bool)pickcolor.ShowDialog())
            { }
            if (pickcolor.result) status.setBoderColor(pickcolor.color);
            BoderColor.Background = new SolidColorBrush(status.getBoderColor());
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
        /// <param name="menuSave"></param>
        /// <param name="e"></param>
        public void Save_Click(object menuSave, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "JPEG Image|*.jpg|PNG Image|*.png| Bitmap Image|*.bmp";
            dlg.FileName = "Image";
            if ((bool)dlg.ShowDialog())
                try
                {
                    selectionRectangle.Visibility = Visibility.Hidden;
                    string fileName = dlg.FileName;
                    Rect bounds = new Rect(0, 0, MyCanvas.Width, MyCanvas.Height);
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
        public void Copy_Click(object menuDel, RoutedEventArgs e)
        {
            Clipboard.SetImage(source);
        }
        /// <summary>
        /// Lưu panel Canvas dưới dạng file XML
        /// </summary>
        /// <param name="menuPaste"></param>
        /// <param name="e"></param>

        public void Save_XML_Click(object menuPaste, RoutedEventArgs e)
        {
            /// Tạo SaveFileDialog
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "XML File|*.XAML";
            dlg.FileName = "MyCanvas";
            if ((bool)dlg.ShowDialog())
                try
                {
                    /// Lưu MyCanvas dưới dạng XML tới FileName trong SaveFileDialog
                    string mystrXAML = XamlWriter.Save(MyCanvas);
                    FileStream filestream = File.Create(dlg.FileName);
                    StreamWriter streamwriter = new StreamWriter(filestream);
                    streamwriter.Write(mystrXAML);
                    streamwriter.Close();
                    filestream.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Can not save File" + ex.Message);
                }
        }
        /// <summary>
        ///  Mở file dạng XML
        /// </summary>
        /// <param name="menuPaste"></param>
        /// <param name="e"></param>
        public void Open_XML_Click(object menuPaste, RoutedEventArgs e)
        {
            /// <ul>
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "XML File | *.XAML";
            dlg.Multiselect = false;
            /// <li> Hiển thị dialog </li>
            try
            {
                if ((bool)dlg.ShowDialog())
                {
                    /// <li> Load file XML</li>
                    FileStream fs = new FileStream(@dlg.FileName, FileMode.Open);
                    savedCanvas = System.Windows.Markup.XamlReader.Load(fs) as Canvas;
                    fs.Close();
                    /// <li> add các UIElement của Canvas trong file XML vào Mycanvas</li>

                    New_Click(new object(), new RoutedEventArgs());
                    while (savedCanvas.Children.Count > 2)
                    {
                        UIElement uie = savedCanvas.Children[2];
                        savedCanvas.Children.Remove(uie);
                        MyCanvas.Children.Add(uie);
                    }


                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Can not open File" + ex.Message);
            }
            /// </ul>
        }
        /// <summary>
        /// More size
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void More_Size_Click(object sender, RoutedEventArgs e)
        {
            MoreSize moresize = new MoreSize();
            if ((bool)moresize.ShowDialog())
            {
            }
            if (moresize.result)
            {
                listSize.SelectedItem = null;
                status.setSize(moresize.int_size);
            }
        }
        private void info_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Name: My Paint \nAuthor : Hoàng Minh Tuấn \nTrường Đại học Bách Khoa Hà Nội\nLớp : CNTT2.01 - K57  ");
        }
        #endregion
        #region Toolbar Click
        
        //Xu ly su kien cho Button line
        /// <summary>
        ///  Khi click vào Tool vẽ đường thằng . . .
        /// </summary>
        /// <param name="btnLine"></param>
        /// <param name="e"></param>
        public void Button_Click_line(object btnLine, RoutedEventArgs e)
        {
            ///- Thêm 1 đối tượng thuộc lớp System.Windows.Shapes.Line
            Line _line = new Line();
            ///- Thêm vào danh sách đường thẳng ban đầu
            status.setTool("line");
            selectionRectangle.Visibility = Visibility.Collapsed;
        }
        /// <summary>
        /// Khi click vào Tool vẽ Select . . .
        /// </summary>
        /// <param name="btnSelect"></param>
        /// <param name="e"></param>
        public void Button_Click_select(object btnSelect, RoutedEventArgs e)
        {
            status.setTool("Select");
            btn_Select.IsChecked = false;
            btn_Select.IsChecked = true;
            selectionRectangle.Fill = null;
            MyCanvas.Cursor = Cursors.Cross;
        }
        /// <summary>
        /// Khi click vào Tool vẽ ellip . . .
        /// </summary>
        /// <param name="btnSelect"></param>
        /// <param name="e"></param>
        public void Ellip_Click(object btnSelect, RoutedEventArgs e)
        {
            /// - setTool
            status.setTool("ellip");
            /// - Ẩn selectionRectangle
            selectionRectangle.Visibility = Visibility.Collapsed;
        }
        /// <summary>
        /// Khi click vào Tool vẽ rectangle . . .
        /// </summary>
        /// <param name="btnRectangle"></param>
        /// <param name="e"></param>
        public void Rectangle_Click(object btnRectangle, RoutedEventArgs e)
        {
            status.setTool("rectangle");
            selectionRectangle.Visibility = Visibility.Collapsed;
        }
        /// <summary>
        /// Khi click vào Tool Tẩy (erase)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void erase_Click(object sender, RoutedEventArgs e)
        {
            status.setTool("erase");
            erase_rec.Cursor = Cursors.None;
            erase_rec.StrokeThickness = 1;
            erase_rec.Stroke = Brushes.Black;
            erase_rec.Fill = Brushes.White;
        }
        /// <summary>
        /// Khi click vào Tool vẽ bằng bút
        /// </summary>
        /// <param name="btnPolyLine"></param>
        /// <param name="e"></param>
        public void PolyLine_Click(object btnPolyLine, RoutedEventArgs e)
        {
            selectionRectangle.Visibility = Visibility.Collapsed;
            status.setTool("polyline");
        }
        public void Text_Click(object sender, RoutedEventArgs e)
        {
            status.setTool("text");
        }

        private void btn_Select_Unchecked(object btnSelect, RoutedEventArgs e)
        {
            selectionRectangle.Visibility = Visibility.Collapsed;
            img.MouseLeftButtonDown -= selectionRectangle_MouseLeftButtonDown;
            img.ContextMenu = null;
            img.Cursor = null;
            ClickDown = false;
            img = new Image();
            MyCanvas.Cursor = Cursors.Pen;
        }

        private void RadioButton_Unchecked(object sender, RoutedEventArgs e)
        {
            erase_rec.Visibility = Visibility.Collapsed;
            erase_rec.Height = 0;
            erase_rec.Width = 0;
        }
        
        /// <summary>
        /// Menu Copy
        /// </summary>
        /// <param name="menuDel"></param>
        /// <param name="e"></param>
        private void NotInVersion(object button, RoutedEventArgs e)
        {
            MessageBox.Show("Sorry! Công cụ này chưa được xây dựng trong phiên bản hiện tại!");
            btn_Select.IsChecked = true;
        }
        
        #endregion
        #region Event
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
        public void selectionRectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            status.setTool("Dragging");
            point_start.X = e.GetPosition(MyCanvas).X;
            point_start.Y = e.GetPosition(MyCanvas).Y;
            MyCanvas.Children[MyCanvas.Children.Count - 2].Visibility = Visibility.Visible;
            pointdrag.X = point_start.X - Canvas.GetLeft(selectionRectangle);
            pointdrag.Y = point_start.Y - Canvas.GetTop(selectionRectangle);
        }
        /// <summary>
        /// Xử lí khi thay đổi kích thước window
        /// </summary>
        /// <param name="window"></param>
        /// <param name="e"></param>
        public void main_SizeChanged(object window, SizeChangedEventArgs e)
        {
            if (main.Width < 1000) SCW.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
            if (main.Height < 500) SCW.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
        }
        

        private void del_CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (status.getTool() == "Selected");
        }

        private void del_CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MyCanvas.Children.Remove(img);
            selectionRectangle.Visibility = Visibility.Collapsed;
        }
        /// <summary>
        /// Menu Paste
        /// </summary>
        /// <param name="menuPaste"></param>
        /// <param name="e"></param>
        public void Paste_Click(object menuPaste, RoutedEventArgs e)
        {
            BitmapSource bitmap = Clipboard.GetImage();
            if (bitmap != null)
            {
                img.MouseLeftButtonDown -= selectionRectangle_MouseLeftButtonDown;
                img.ContextMenu = null;
                img = new Image { Source = bitmap };
                Canvas.SetLeft(img, 0);
                Canvas.SetTop(img, 0);
                img.MouseLeftButtonDown += selectionRectangle_MouseLeftButtonDown;
                img.Cursor = Cursors.SizeAll;
                img.ContextMenu = imgContext;
                MyCanvas.Children.Add(img);
                Canvas.SetLeft(selectionRectangle, 0);
                Canvas.SetTop(selectionRectangle, 0);
                MyCanvas.Width = MyCanvas.ActualWidth < img.Source.Width ? img.Source.Width : MyCanvas.ActualWidth;
                MyCanvas.Height = MyCanvas.ActualHeight < img.Source.Height ? img.Source.Height : MyCanvas.ActualHeight;
                selectionRectangle.Width = img.Source.Width;
                selectionRectangle.Height = img.Source.Height;
                selectionRectangle.Visibility = Visibility.Visible;
                Canvas.SetZIndex(selectionRectangle, Canvas.GetZIndex(MyCanvas.Children[MyCanvas.Children.Count - 1]) + 1);
                status.setTool("Selected");
            }
        }
        /// <summary>
        /// Xử lí tạo ConfirmDialog khi click vào "X" - thoát
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void main_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Bạn có muốn lưu lại trước khi thoát không", "Warning", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            if (result == MessageBoxResult.Cancel)
            {
                e.Cancel = true;
            }
            if (result == MessageBoxResult.Yes)
            {
                Save_Click(new Object(), new RoutedEventArgs());
            }
        }
        /// <summary>
        ///  Khi thay đổi kích thước
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void listSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            status.setSize((listSize.SelectedIndex + 1) * 2);
        }
        #endregion
    }
}