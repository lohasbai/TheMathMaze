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
using TheMathMaze;

namespace TheMathMazeWindow
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        bool _console_mode = true;
        bool console_mode
        {
            get
            {
                return _console_mode;
            }
            set
            {
                if (value == true)
                {
                    textBox1.IsReadOnly = false;
                    textBox1.Background = new SolidColorBrush(Color.FromArgb(128, 0, 0, 0));
                    console_output("Switched to Console Mode\r\n");
                    buttonadd.IsEnabled = false;
                    buttonsub.IsEnabled = false;
                    buttonmul.IsEnabled = false;
                    buttondiv.IsEnabled = false;
                    //buttonaddline.IsEnabled = false;
                    buttonsubline.IsEnabled = false;

                    textBoxInput.IsEnabled = false;
                    textBoxInput.Background = new SolidColorBrush(Color.FromArgb(128, 128, 128, 128));
                }
                else
                {
                    //TODO:别忘了clear
                    //textBox1.Clear();
                    textBox1.IsReadOnly = true;
                    textBox1.Background = new SolidColorBrush(Color.FromArgb(128, 128, 128, 128));
                    console_output("Switched to GUI Mode\r\n");
                    buttonadd.IsEnabled = true;
                    buttonsub.IsEnabled = true;
                    buttonmul.IsEnabled = true;
                    buttondiv.IsEnabled = true;

                    textBoxInput.IsEnabled = true;
                    textBoxInput.Background = new SolidColorBrush(Color.FromArgb(128, 255, 255, 255));
                }
                _console_mode = value;
            }
        }

        string basic_add = "A_+B_C";
        string basic_sub = "A_-B_-C";
        string basic_mul = "A_*B_C";
        string basic_div = "";
        BaseEquation.METHOD _GUIMethod = BaseEquation.METHOD.ADD;
        BaseEquation.METHOD GUIMethod
        {
            get
            {
                return _GUIMethod;
            }
            set
            {
                if (!console_mode)
                {
                    human_changing = false;
                    textBoxInput.Clear();
                    human_changing = true;
                    switch (value)
                    {
                        case BaseEquation.METHOD.ADD:
                            textBoxInput.Text = ExpressionTranslate.get_GUI(new BaseEquation(basic_add));
                            break;
                        case BaseEquation.METHOD.SUB:
                            textBoxInput.Text = ExpressionTranslate.get_GUI(new BaseEquation(basic_sub));
                            break;
                        case BaseEquation.METHOD.MUL:
                            textBoxInput.Text = ExpressionTranslate.get_GUI(new BaseEquation(basic_mul));
                            break;
                        case BaseEquation.METHOD.DIV:
                            textBoxInput.Text = ExpressionTranslate.get_GUI(new BaseEquation(basic_div));
                            break;
                    }
                }
                _GUIMethod = value;
            }
        }

        public MainWindow()
        {
            InitializeComponent();


            console_mode = true;
            textBox1.Text = "FGH_*EDCB_ACEF_BJCD_BGAB_CCIJ_CFHBHIF";
            console_output("Initialize finished\r\n");
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void BtnClose_OnClick(object sender, EventArgs e)
        {
            Close();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            //textBox2.Text = "";
            //textBox2.AppendText(textBox1.Text + "\r\n");
            DateTime t1 = DateTime.Now;
            string ans = ConsoleMazeMain.get_result(textBox1.Text);
            int milisec = (int)DateTime.Now.Subtract(t1).TotalMilliseconds;
            string[] anss = ans.Split(new char[1] { '\r' });
            int ans_num = (ans == "answer not found\r\n") ? 0 : (anss.Length - 1);
            console_output(ans + "Quest:\r\n" + textBox1.Text + "\r\n" + ans_num.ToString() + " answer(s) was(were) found.\r\n" + "Elapsed Time：" + milisec.ToString() + "ms\r\n");
            textBoxOutput.Text = "First answer:\r\n" + ExpressionTranslate.get_GUI(new BaseEquation(anss[0]));
        }

        private void button_switch_Click(object sender, RoutedEventArgs e)
        {
            console_mode = !console_mode;
        }

        private void console_output(string str)
        {
            textBox2.AppendText(str + "\r\n");
            textBox2.ScrollToEnd();
        }

        private void button_add(object sender, RoutedEventArgs e)
        {
            GUIMethod = BaseEquation.METHOD.ADD;
        }

        private void button_sub(object sender, RoutedEventArgs e)
        {
            GUIMethod = BaseEquation.METHOD.SUB;
        }

        private void button_mul(object sender, RoutedEventArgs e)
        {
            GUIMethod = BaseEquation.METHOD.MUL;
        }

        private void button_div(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("除法的GUI模式未实现，请使用控制台模式");
            //GUIMethod = BaseEquation.METHOD.DIV;
        }

        private void button_sub_line(object sender, RoutedEventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (console_mode)
            {
                BaseEquation be = new BaseEquation(textBox1.Text);
                GUIMethod = be.method;
                textBoxInput.Text = ExpressionTranslate.get_GUI(be);
            }
        }

        string textBoxInputOld = "";
        /// <summary>
        /// 指示当前的change操作是后台还是人为操作
        /// </summary>
        bool human_changing = true;
        private void textBoxInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            //TODO:检查输入太长或太高
            if (!console_mode)
            {
                //为乘法匹配行数
                if (GUIMethod == BaseEquation.METHOD.MUL)
                {
                    mul_special();
                }
                if (human_changing)
                {
                    int checkans = ExpressionTranslate.textGUI(textBoxInput.Text);
                    if (checkans != 0)
                    {
                        if (checkans == 2)
                            MessageBox.Show("GUI输入不合规范，已退回上一步\r\n请检查：\r\n某行太长，最长仅限" + ExpressionTranslate.LENTH_MAX + "字符，超长请使用控制台输入方式");
                        else if (checkans == 3)
                            MessageBox.Show("GUI输入不合规范，已退回上一步\r\n请检查：\r\n行数太多，最长仅限" + ExpressionTranslate.HEIGHT_MAX + "行，超高请使用控制台输入方式");
                        else
                            MessageBox.Show("GUI输入不合规范，已退回上一步\r\n请检查：\r\n数字和字母超过了10个\r\n插入了非法符\r\n删除了重要字符\r\n算式行数不对");
                        human_changing = false;
                        textBoxInput.Text = textBoxInputOld;
                        human_changing = true;
                    }
                }
                BaseEquation be = ExpressionTranslate.get_console(textBoxInput.Text);
                if (be != null)
                    textBox1.Text = be.equation_console;
                else
                    textBox1.Text = "";
                textBoxInputOld = textBoxInput.Text;
            }

        }

        /// <summary>
        /// 乘法行数匹配函数
        /// </summary>
        private void mul_special()
        {
            string[] textlines = textBoxInput.Text.Split(new char[] { '\r'});
            for (int i = 0; i < textlines.Length; i++)
            {
                textlines[i] = textlines[i].Replace("\n", "");
            }
            if (textlines.Length >= 4)
            {
                int elements = 0;
                for (int i = 0; i < textlines[1].Length; i++)
                {
                    char c = textlines[1][i];
                    if ((c >= '0' && c <= '9') || (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z'))
                    {
                        elements++;
                    }
                }
                if (elements == 1 && textlines.Length != 4)
                {
                    //需要删除行
                    string new_textboxInput = "";
                    new_textboxInput += textlines[0] + "\r\n";
                    new_textboxInput += textlines[1] + "\r\n";
                    new_textboxInput += textlines[2] + "\r\n";
                    new_textboxInput += textlines[textlines.Length - 1];
                    //human_changing = false;
                    textBoxInput.Text = new_textboxInput;
                    //human_changing = true;

                }
                else if (elements > 1 && textlines.Length != elements + 5)
                {
                    //需要插入或者删除行
                    if (textlines.Length > elements + 5)
                    {
                        string new_textboxInput = "";
                        new_textboxInput += textlines[0] + "\r\n";
                        new_textboxInput += textlines[1] + "\r\n";
                        new_textboxInput += textlines[2] + "\r\n";
                        for (int i = 0; i < elements; i++)
                        {
                            new_textboxInput += textlines[3 + i] + "\r\n";
                        }
                        new_textboxInput += textlines[textlines.Length - 2] + "\r\n";
                        new_textboxInput += textlines[textlines.Length - 1];
                        //human_changing = false;
                        textBoxInput.Text = new_textboxInput;
                        //human_changing = true;
                    }
                    else
                    {
                        string new_textboxInput = "";
                        new_textboxInput += textlines[0] + "\r\n";
                        new_textboxInput += textlines[1] + "\r\n";
                        new_textboxInput += textlines[2] + "\r\n";
                        int nowlines = textlines.Length;
                        int addspace = 0;
                        if (textlines.Length == 4)
                        {
                            new_textboxInput += "\r\n";
                            //new_textboxInput += "————\r\n";
                            nowlines += 2;
                            addspace++;
                        }
                        else
                        {
                            for (int i = 0; i < nowlines - 5; i++)
                            {
                                new_textboxInput += textlines[i + 3] + "\r\n";
                                addspace++;
                            }
                        }
                        for (int i = 0; i < elements + 5 - nowlines; i++)
                        {
                            string addspacestr = "";
                            for (int j = 0; j < addspace; j++)
                                addspacestr += " ";
                            addspace++;
                            new_textboxInput += addspacestr + "\r\n";
                        }
                        new_textboxInput += textlines[2] + "\r\n";
                        new_textboxInput += textlines[textlines.Length - 1];
                        //human_changing = false;
                        textBoxInput.Text = new_textboxInput;
                        //human_changing = true;
                    }
                }
            }
        }

        string[] samples = { "ABCD_+ABED_EDCAD", "20CDE_-FGHEI_-H2C0F", "FGH_*EDCB_ACEF_BJCD_BGAB_CCIJ_CFHBHIF", "" };
        int now_sample = 3;
        private void buttonsample_Click(object sender, RoutedEventArgs e)
        {
            console_mode = true;
            now_sample = (now_sample + 1) % 4;
            textBox1.Text = samples[now_sample];
        }
    }
}
