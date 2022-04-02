using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfNetAssit.Pages
{
    public class TextMonitorModel : ViewModelBase
    {

        private string displayString = "";
        public string DisplayString
        {
            get { return displayString; }
            set { Set("DisplayString", ref displayString, value); }
        }

        private int displayLastLine = 0;
        public int DisplayLastLine
        {
            get { return displayLastLine; }
            set 
            {
                int v = value;
                if (v < 0)
                    v = 0;
                else if (v > displayLines)
                    v = displayLines;
                Set("DisplayLastLine", ref displayLastLine, v);
                MakeDisplayString();
            }
        }

        private int displayLines = 0;
        public int DisplayLines
        {
            get { return displayLines; }
            set { Set("DisplayLines", ref displayLines, value); }
        }


        /// <summary>
        /// 一行显示的最大字符数
        /// </summary>
        public int MaxCharNumInLine { get; set; } = 80;

        /// <summary>
        /// 最多显示行数
        /// </summary>
        public int MaxLinesDisplay { get; set; } = 30;

        //已经完成的行数据
        private List<string> stringlines = new List<string>();
        //正在添加的行数据
        private string curaddingline = "";

        /// <summary>
        /// 显示接收到的数据
        /// </summary>
        /// <param name="buf"></param>
        /// <param name="size"></param>
        public void MonitorData(string str)
        {
            AddStringToBufOrAddingline(str);

            if (stringlines.Count > 5000)
                stringlines.RemoveRange(0, 100);
            CalcLines();

            MakeDisplayString();
        }

        private void CalcLines()
        {
            DisplayLines = stringlines.Count + 1;
            DisplayLastLine = DisplayLines;
        }

        public void ClearMonitor()
        {
            DisplayString = "";
            stringlines.Clear();
            curaddingline = "";
            CalcLines();
        }

        private void MakeDisplayString()
        {
            int lines = MaxLinesDisplay;
            int listcount = DisplayLastLine - 1;
            lines = listcount > lines ? lines : listcount;
            if (lines < 0)
                lines = 0;

            string[] array = new string[lines];
            int start = listcount - lines;
            if (start < 0)
                start = 0;
            stringlines.CopyTo(start, array, 0, lines);
            StringBuilder sb = new StringBuilder();
            foreach (var item in array)
            {
                sb.Append(item);
                sb.Append("\r\n");
            }
            if(DisplayLines == DisplayLastLine)
                sb.Append(curaddingline);
            DisplayString = sb.ToString();
        }

        private void AddStringToBufOrAddingline(string str)
        {
            int count = str.Length;
            int maxsize = MaxCharNumInLine - curaddingline.Length;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < count; i++)
            {
                char cur = str[i];
                if (cur == '\r' || cur == '\n')
                {
                    if ((i + 1) < count)
                    {
                        char next = str[i + 1];
                        if ((next == '\r' || next == '\n') && (next != cur))
                            i++;
                    }
                    maxsize = MaxCharNumInLine;
                    stringlines.Add(curaddingline + sb.ToString());
                    curaddingline = "";
                    sb.Clear();
                }
                else
                {
                    if (maxsize > 0)
                    {
                        sb.Append(str[i]);
                        maxsize--;

                        if(maxsize == 0)
                        {
                            maxsize = MaxCharNumInLine;
                            stringlines.Add(curaddingline + sb.ToString());
                            curaddingline = "";
                            sb.Clear();
                        }
                    }
                }
            }
            curaddingline = curaddingline + sb.ToString();
            sb.Clear();
        }
    }
}
