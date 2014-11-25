using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Paint_Design
{
    public class Status
    {
        /// <summary>
        /// Là trạng thái của chương trình ( công cụ đang sử dụng, Fillcolor, BoderColor, kích thước )
        /// </summary>
        private String ToolBar;
        private Color FillColor;
        private Color BoderColor;
        private int Size;
        /// <summary>
        /// Constructor
        /// </summary>
        public Status()
        {
            ToolBar = "Select";
            FillColor = Color.FromArgb(Convert.ToByte(00), Convert.ToByte(00), Convert.ToByte(00), Convert.ToByte(00));
            BoderColor = Color.FromArgb(Convert.ToByte(255), Convert.ToByte(00), Convert.ToByte(00), Convert.ToByte(00));
            Size = 2; 
        }
        /// Các phương thức Setter
        /// <summary>
        /// Setter : ToolBar
        /// </summary>
        /// <param name="s_toolBar"></param>
        public void setTool(String s_toolBar)
        {
            this.ToolBar = s_toolBar;
        }
        /// <summary>
        /// Setter : FillColor
        /// </summary>
        /// <param name="cl_Color"></param>
        public void setFillColor(Color cl_Color)
        {
            this.FillColor = cl_Color;
        }
        /// <summary>
        /// Setter :  BoderColor
        /// </summary>
        /// <param name="cl_Color"></param>
        public void setBoderColor(Color cl_Color)
        {
            this.BoderColor = cl_Color;
        }
        /// <summary>
        /// Setter : Size
        /// </summary>
        /// <param name="i_Size"></param>
        public void setSize(int i_Size)
        {
            this.Size = i_Size;
        }
        /// <summary>
        /// Phương thức Getter
        /// </summary>
        /// <returns>ToolBar</returns>
        public String getTool()
        {
            return this.ToolBar;
        }
        /// <summary>
        /// Phương thức Getter
        /// </summary>
        /// <returns>FillColor</returns>
        public Color getFillColor()
        {
            return this.FillColor;
        }
        /// <summary>
        /// Phương thức Getter
        /// </summary>
        /// <returns>BoderColor</returns>
        public Color getBoderColor()
        {
            return this.BoderColor;
        }
        /// <summary>
        /// Phương thức Getter
        /// </summary>
        /// <returns>Size</returns>
        public int getSize()
        {
            return this.Size;
        }
    }
}
