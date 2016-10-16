using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheMathMazeWindow
{
    /// <summary>
    /// 用于GUI模式和Console模式的互换
    /// </summary>
    class ExpressionTranslate
    {
        public const int HEIGHT_MAX = 10;
        public const int LENTH_MAX = 40;

        public static string get_GUI(TheMathMaze.BaseEquation be)
        {
            string ret = "";
            string[] lines = be.spilt_string_without_operator;
            if (lines == null)
                return "";
            int longlen = be.max_line_len;
            if (lines.Length > HEIGHT_MAX)
                return "Expression too high";
            if (be.max_line_len > LENTH_MAX)
                return "Expression too long";
            if (be.method == TheMathMaze.BaseEquation.METHOD.ADD || be.method == TheMathMaze.BaseEquation.METHOD.SUB)
            {
                ret += lines[0];
                ret += "\r\n";
                ret += (be.method == TheMathMaze.BaseEquation.METHOD.ADD) ? "+ " : "- ";
                if (lines[1].Length < lines[0].Length)
                {
                    for (int i = 0; i < lines[0].Length - lines[1].Length; i++)
                    {
                        ret += " ";
                    }
                }
                ret += lines[1];
                ret += "\r\n";
                for (int i = 0; i < longlen + 3; i++)
                {
                    ret += "—";
                }
                ret += "\r\n";
                ret += be.spilt_string[2];
            }
            if (be.method == TheMathMaze.BaseEquation.METHOD.MUL)
            {
                ret += lines[0];
                ret += "\r\n";
                ret += "* ";
                if (lines[1].Length < lines[0].Length)
                {
                    for (int i = 0; i < lines[0].Length - lines[1].Length; i++)
                    {
                        ret += " ";
                    }
                }
                ret += lines[1];
                ret += "\r\n";
                for (int i = 0; i < longlen + 3; i++)
                {
                    ret += "—";
                }
                ret += "\r\n";
                if (lines.Length == 3)
                {
                    ret += lines[2];
                }
                else
                {
                    for (int i = 2; i < lines.Length - 1; i++)
                    {
                        ret += lines[i];
                        for (int j = 2; j < i; j++)
                            ret += " ";
                        ret += "\r\n";
                    }
                    for (int i = 0; i < longlen + 3; i++)
                    {
                        ret += "—";
                    }
                    ret += "\r\n";
                    ret += lines[lines.Length - 1];
                }
            }
            return ret;
        }
        /// <summary>
        /// 请注意可能返回null
        /// </summary>
        /// <param name="textbox_str"></param>
        /// <returns></returns>
        public static TheMathMaze.BaseEquation get_console(string textbox_str)
        {
            if (textbox_str == null)
                return null;
            //textbox_str = textbox_str.Replace('x', '*');//乘号换一下
            //处理字母 - 置换为A~J
            textbox_str = textbox_str.ToUpper();
            for (int i = 0; i < 16; i++)
            {
                if (textbox_str.IndexOf((char)('J' + i + 1)) != -1)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        if (textbox_str.IndexOf((char)('A' + j)) == -1)
                        {
                            textbox_str = textbox_str.Replace((char)('J' + i + 1), (char)('A' + j));
                            break;
                        }
                    }
                }
            }
            string ret_str = "";
            string[] lines_textbox = textbox_str.Split(new char[] { '\r'});
            if (lines_textbox.Length < 4)
                return null;
            lines_textbox[0] = lines_textbox[0].Replace(" ", "");
            lines_textbox[0] = lines_textbox[0].Replace("\r", "");
            lines_textbox[0] = lines_textbox[0].Replace("\n", "");
            lines_textbox[0] = lines_textbox[0].Replace("_", "");
            ret_str += lines_textbox[0];
            for (int i = 1; i < lines_textbox.Length; i++)
            {
                lines_textbox[i] = lines_textbox[i].Replace(" ", "");
                lines_textbox[i] = lines_textbox[i].Replace("\r", "");
                lines_textbox[i] = lines_textbox[i].Replace("\n", "");
                lines_textbox[i] = lines_textbox[i].Replace("_", "");
                if (lines_textbox[i] != "")
                {
                    char c = lines_textbox[i][0];
                    if (c != '—')//TODO:貌似没考虑除法，晚点再说
                    {
                        ret_str += "_" + lines_textbox[i];
                    }
                }
            }
            TheMathMaze.BaseEquation ret = new TheMathMaze.BaseEquation(ret_str);
            if (ret.equation_console != string.Empty)
                return ret;
            else
                return null;
        }
        /// <summary>
        /// 检查是否符合Equation,0=success,1=未知错误,2=太长,3=太高
        /// </summary>
        /// <param name="textbox_str"></param>
        /// <returns></returns>
        public static int textGUI(string textbox_str)
        {
            //textbox_str = textbox_str.Replace('x', '*');//乘号换一下
            //处理字母 - 置换为A~J
            textbox_str = textbox_str.ToUpper();
            for (int i = 0; i < 16; i++)
            {
                if (textbox_str.IndexOf((char)('J' + i + 1)) != -1)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        if (textbox_str.IndexOf((char)('A' + j)) == -1)
                        {
                            textbox_str = textbox_str.Replace((char)('J' + i + 1), (char)('A' + j));
                            break;
                        }
                    }
                }
            }
            char c_addin = 'A';
            for (int i = 0; i < 10; i++)
            {
                if (textbox_str.IndexOf((char)('0' + i)) != -1)
                {
                    c_addin = (char)('0' + i);
                    break;
                }
                if (textbox_str.IndexOf((char)('A' + i)) != -1)
                {
                    c_addin = (char)('A' + i);
                    break;
                }
            }
            string mix_str = "";
            string[] lines_textbox = textbox_str.Split(new char[] { '\r' });
            if (lines_textbox.Length > HEIGHT_MAX)
                return 3;
            if (lines_textbox.Length < 4)
                return 1;
            lines_textbox[0] = lines_textbox[0].Replace(" ", "");
            lines_textbox[0] = lines_textbox[0].Replace("\r", "");
            lines_textbox[0] = lines_textbox[0].Replace("\n", "");
            lines_textbox[0] = lines_textbox[0].Replace("_", "");
            if (lines_textbox[0] == "")
                lines_textbox[0] = c_addin.ToString();
            mix_str += lines_textbox[0];
            bool is_mul = false;
            for (int i = 1; i < lines_textbox.Length; i++)
            {
                if (lines_textbox[i].Length > LENTH_MAX)
                    return 2;
                if(i < 4)
                    lines_textbox[i] = lines_textbox[i].Replace(" ", "");
                lines_textbox[i] = lines_textbox[i].Replace("\r", "");
                lines_textbox[i] = lines_textbox[i].Replace("\n", "");
                lines_textbox[i] = lines_textbox[i].Replace("_", "");
                if (lines_textbox[i] == "")
                    lines_textbox[i] = c_addin.ToString();
                char c = lines_textbox[i][0];
                if (c != '—')//TODO:貌似没考虑除法2，晚点再说
                {
                    mix_str += "_" + lines_textbox[i] + c_addin;
                    //TODO: 检查符号替换问题
                    //TODO: 除法
                    //特别：检查乘除法行数 
                    if (c == '*')
                    {
                        is_mul = true;
                        if ((lines_textbox[i].Length == 2 && lines_textbox.Length != 4) ||
                            (lines_textbox[i].Length > 2 && lines_textbox.Length != lines_textbox[i].Length + 4))
                        {
                            return 1;
                        }
                    }
                    //检查最右空格
                    if (is_mul && i >= 4 && i <= lines_textbox.Length - 3)
                    {
                        //右边应当有(i-3)个空格
                        for (int j = 0; j < i - 3; j++)
                        {
                            if (lines_textbox[i][lines_textbox[i].Length - j - 1] != ' ')
                                return 1;
                        }
                    }
                }
                else
                {
                    //检查这一行是不是都是这个破符号
                    for (int j = 0; j < lines_textbox[i].Length; j++)
                    {
                        if (lines_textbox[i][j] != '—')
                            return 1;
                    }
                }

            }
            TheMathMaze.BaseEquation be = new TheMathMaze.BaseEquation(mix_str);
            return (be.equation_console != string.Empty) ? 0 : 1;
        }
    }
}
