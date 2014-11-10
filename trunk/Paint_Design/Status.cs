using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paint_Design
{
    public class Status
    {
        /// <summary>
        /// Là trạng thái của chương trình ( công cụ đang sử dụng, màu sắc, kích thước
        /// </summary>
        private String ToolBar;
        private String Color;
        private int Size;

        public Status()
        {
            ToolBar = "thang";
            Color = "#FF0000";
            Size = 1; 
        }
        public void setTool(String s_toolBar)
        {
            this.ToolBar = s_toolBar;
        }
        public void setColor(String s_Color)
        {
            this.Color = s_Color;
        }
        public void setSize(int i_Size)
        {
            this.Size = i_Size;
        }
        public String getTool()
        {
            return this.ToolBar;
        }
        public String getColor()
        {
            return this.Color;
        }
        public int getSize()
        {
            return this.Size;
        }
    }
}
