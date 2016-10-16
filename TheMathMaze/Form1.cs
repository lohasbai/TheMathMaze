using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TheMathMaze
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            //textBox1.BackColor = System.Drawing.Color.Transparent;
            //richTextBox1.BackColor = Color.Transparent;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox2.Text = "";
            DateTime t1 = DateTime.Now;
            string ans = ConsoleMazeMain.get_result(textBox1.Text);
            int milisec = (int)DateTime.Now.Subtract(t1).TotalMilliseconds;
            int ans_num = ans.Split(new char[1] { '\r' }).Length - 1;

            textBox2.Text = "找到" + ans_num.ToString() + "个解\r\n" + "消耗时间：" + milisec.ToString() + "毫秒\r\n" + ans;
        }

        //public class RichEdit50 : RichTextBox
        //{
        //    [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        //    static extern IntPtr LoadLibrary(string lpFileName);

        //    protected override CreateParams CreateParams
        //    {
        //        get
        //        {
        //            CreateParams prams = base.CreateParams;
        //            if (LoadLibrary("msftedit.dll") != IntPtr.Zero)
        //            {
        //                prams.ExStyle |= 0x020; // transparent 
        //                prams.ClassName = "RICHEDIT50W";
        //            }
        //            return prams;
        //        }
        //    }
        //}

        private void Form1_Load(object sender, EventArgs e)
        {
            //RichEdit50 re = new RichEdit50();
            //re.Text = "123";

            //this.Controls.Add(re);
            //re.BorderStyle = BorderStyle.FixedSingle;
            //re.BackColor = Color.Black;
            //re.ForeColor = Color.White;
            //re.BringToFront();
            //this.Show();
        }
    }
}



/*
 * 废弃代码块


                
 */
