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

        /// <summary>
        /// 一行显示的最大字符数
        /// </summary>
        public int MaxCharNumInLine { get; set; } = 80;

        /// <summary>
        /// 最多显示行数
        /// </summary>
        public int MaxLinesDisplay { get; set; } = 30;

        private List<string> stringlines = new List<string>();
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

            MakeDisplayString();
        }

        public void ClearMonitor()
        {
            DisplayString = "";
            stringlines.Clear();
            curaddingline = "";
        }

        private void MakeDisplayString()
        {
            int lines = MaxLinesDisplay;
            if (curaddingline != "")
                lines--;
            lines = stringlines.Count > lines ? lines:stringlines.Count;

            string[] array = new string[lines];
            stringlines.CopyTo(stringlines.Count - lines, array, 0, lines);
            StringBuilder sb = new StringBuilder();
            foreach (var item in array)
            {
                sb.Append(item);
                sb.Append("\r\n");
            }
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
