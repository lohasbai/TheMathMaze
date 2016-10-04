using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheMathMaze
{
    class ConsoleMazeMain
    {
        public static string get_result(string console)
        {
            console = console.Replace(" ", "");
            console = console.Replace("\r", "");
            console = console.Replace("\n", "");
            BaseEquation be = new BaseEquation(console);
            if (be.method == BaseEquation.METHOD.ADD)
                return MazeAdd.get_results(be);
            else if (be.method == BaseEquation.METHOD.SUB)
                return MazeSub.get_results(be);
            return "Wrong input";
        }
    }
    /// <summary>
    /// 一个基本方程
    /// </summary>
    class BaseEquation
    {
        /// <summary>
        /// 算式的运算法则
        /// </summary>
        public enum METHOD {ADD, SUB, MUL, DIV, UNDEFINE};

        public const string LETTER_TABLE = "ABCDEFGHIJ";

        /// <summary>
        /// 初始化，若字符串不符合规范，将会赋值空字符串
        /// </summary>
        /// <param name="str">初始化字符串</param>
        public BaseEquation(string str)
        {
            equation_console = str;
            if (!basic_check())
                clear();
        }

        /// <summary>
        /// <para>记录这个方程的字符串序列，一个合理的序列如下（引号不算）</para>
        /// <para>"5BC_+DDE_FEG"</para>
        /// </summary>
        public string equation_console
        {
            get
            {
                return _equation_console;
            }
            protected set
            {
                _equation_console = value;
            }
        }
        /// <summary>
        /// 获得拆分的各行，包含运算符
        /// </summary>
        public string[] spilt_string
        {
            get
            {
                if (equation_console == "")
                    return null;
                return equation_console.Split(new char[1] { '_' });
            }
        }
        /// <summary>
        /// 拆分各行不含运算符
        /// </summary>
        public string[] spilt_string_without_operator
        {
            get
            {
                if (equation_console == "")
                    return null;
                string[] tmpstring;
                tmpstring = equation_console.Split(new char[1] { '_' });
                for (int i = 0; i < tmpstring.Length; i++)
                {
                    if (tmpstring[i].Length > 0)
                    {
                        char c = tmpstring[i][0];
                        if ((c < '0' || c > '9') && (c < 'A' || c > 'Z'))
                            tmpstring[i] = tmpstring[i].Substring(1);
                    }
                }
                return tmpstring;
            }
        }
        /// <summary>
        /// 最长的一行数（不含运算符）
        /// </summary>
        public int max_line_len
        {
            get
            {
                int ret = 0;
                if (equation_console == "")
                    return 0;
                string[] lines = equation_console.Split(new char[] { '_' });
                lines[1] = lines[1].Substring(1);
                if (method == METHOD.SUB && lines[2][0] == '-')
                    lines[2] = lines[2].Substring(1);
                for (int i = 0; i < lines.Length; i++)
                    if (lines[i].Length > ret)
                        ret = lines[i].Length;
                return ret;
            }
        }

        /// <summary>
        /// 获得本算式的法则
        /// </summary>
        public METHOD method
        {
            get
            {
                if (equation_console == "")
                    return METHOD.UNDEFINE;
                int t = equation_console.IndexOf('_');
                if (t < 0 || t >= equation_console.Length)
                    return METHOD.UNDEFINE;
                if (equation_console[t + 1] == '+')
                    return METHOD.ADD;
                else if (equation_console[t + 1] == '-')
                    return METHOD.SUB;
                else if (equation_console[t + 1] == '*')
                    return METHOD.MUL;
                else if (equation_console[t + 1] == '/')
                    return METHOD.DIV;
                else
                    return METHOD.UNDEFINE;
            }
        }
        /// <summary>
        /// 获得每行首字母（不含运算符）
        /// </summary>
        /// <returns></returns>
        public List<char> get_first_in_each_line()
        {
            if (equation_console == "")
                return null;
            string[] lines = equation_console.Split(new char[1] { '_' });
            lines[1] = lines[1].Substring(1);
            if (method == METHOD.SUB && lines[2][0] == '-')
                lines[2] = lines[2].Substring(1);
            List<char> ret = new List<char>();
            for (int i = 0; i < lines.Length; i++)
            {
                    ret.Add(lines[i][0]);
            }
            return ret;
        }
        /// <summary>
        /// 字符串基本检查
        /// </summary>
        /// <param name="auto_up">自动将小写改为大写，本程序目前仅处理大写字母和数字</param>
        /// <param name="auto_clear">若检查出错，自动清空字符串</param>
        /// <returns></returns>
        public bool basic_check(bool auto_up = true, bool auto_clear = true)
        {
            if (equation_console == string.Empty)
            {
                if (auto_clear)
                    clear();
                return false;
            }
            if (auto_up)
                equation_console = equation_console.ToUpper();
            string[] lines = equation_console.Split(new char[1] { '_' });
            if (lines.Length < 3 || lines[1].Length < 2)
            {
                if (auto_clear)
                    clear();
                return false;
            }
            char method_c = lines[1].ToArray()[0];
            if (method_c != '+' && method_c != '-' && method_c != '*' && method_c != '/')
            {
                if (auto_clear)
                    clear();
                return false;
            }
            lines[1] = lines[1].Substring(1);
            if (method == METHOD.SUB && lines[2][0] == '-')
                lines[2] = lines[2].Substring(1);
            List<char> category_already = new List<char>();
            foreach (string line in lines)
            {
                //此处可能有争议，目前暂不允许空行
                // TODO:证实是否需要空行
                if (line == string.Empty)
                {
                    if (auto_clear)
                        clear();
                    return false;
                }
                foreach (char c in line)
                {
                    if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'J'))
                    {
                        if (category_already.IndexOf(c) == -1)
                        {
                            category_already.Add(c);
                            if (category_already.Count > 10)
                            {
                                if (auto_clear)
                                    clear();
                                return false;
                            }
                        }
                    }
                    else
                    {
                        if (auto_clear)
                            clear();
                        return false;
                    }
                }
            }
            if ((method == METHOD.ADD || method == METHOD.SUB) && lines.Length != 3)
            {
                if (auto_clear)
                    clear();
                return false;
            }
            return true;
        }
        /// <summary>
        /// 清空
        /// </summary>
        public void clear()
        {
            equation_console = "";
        }
        /// <summary>
        /// 可用的数字
        /// </summary>
        /// <returns></returns>
        public List<int> available_nums()
        {
            List<int> ret = new List<int>();
            for (int i = 0; i < 10; i++)
            {
                ret.Add(i);
            }
            foreach (char c in equation_console)
            {
                if (c >= '0' && c <= '9')
                {
                    int a = c - '0';
                    ret.Remove(a);
                }
            }
            return ret;
        }
        /// <summary>
        /// 可用的字母，先高位
        /// </summary>
        /// <returns></returns>
        public List<char> available_letters_from_begin(int count = -1)
        {
            List<char> ret = new List<char>();
            string[] lines = equation_console.Split(new char[] { '_' });
            lines[1] = lines[1].Substring(1);
            if (method == METHOD.SUB && lines[2][0] == '-')
                lines[2] = lines[2].Substring(1);
            int _max_line_len = max_line_len;
            for (int i = 0; i < _max_line_len; i++)
            {
                for (int j = 0; j < lines.Length; j++)
                {
                    if(lines[j].Length > i)
                        if (lines[j][i] >= 'A' && lines[j][i] <= 'J' && (ret.IndexOf(lines[j][i]) == -1))
                        {
                            ret.Add(lines[j][i]);
                        }
                    if (ret.Count == 10 || (count > 0 && ret.Count == count))
                        break;
                }
                if (ret.Count == 10 || (count > 0 && ret.Count == count))
                    break;
            }
            return ret;
        }
        /// <summary>
        /// 可用的字母，先低位
        /// </summary>
        /// <returns></returns>
        public List<char> available_letters_from_last(int count = -1)
        {
            List<char> ret = new List<char>();
            string[] lines = equation_console.Split(new char[] { '_' });
            lines[1] = lines[1].Substring(1);
            if (method == METHOD.SUB && lines[2][0] == '-')
                lines[2] = lines[2].Substring(1);
            int _max_line_len = max_line_len;
            for (int i = 0 ; i < _max_line_len; i++)
            {
                for (int j = 0; j < lines.Length; j++)
                {
                    if (lines[j].Length > i)
                    {
                        char c = lines[j][lines[j].Length - i - 1];
                        if (c >= 'A' && c <= 'J' && (ret.IndexOf(c) == -1))
                            ret.Add(c);
                    }
                    if (ret.Count == 10 || (count > 0 && ret.Count == count))
                        break;
                }
                if (ret.Count == 10 || (count > 0 && ret.Count == count))
                    break;
            }
            return ret;
        }
        /// <summary>
        /// 可用的字母，顺序扫描
        /// </summary>
        /// <returns></returns>
        public List<char> available_letters(int count = -1)
        {
            List<char> ret = new List<char>();
            foreach (char c in equation_console)
            {
                if (c >= 'A' && c <= 'J' && ret.IndexOf(c) == -1)
                    ret.Add(c);
                if (ret.Count == 10 || (count > 0 && ret.Count == count))
                    break;
            }
            return ret;
        }
        /// <summary>
        /// 获得某一行从低位起数的位置的值，超过索引返回0
        /// </summary>
        /// <param name="line_num">行号</param>
        /// <param name="index">第几位</param>
        /// <returns></returns>
        public char get_from_line_end(int line_num, int index)
        {
            string[] lines = spilt_string;
            if (line_num == 1)
            {
                if (index >= lines[line_num].Length - 1)
                    return '0';
                return lines[line_num][lines[line_num].Length - 1 - index];
            }
            else
            {
                if (index >= lines[line_num].Length)
                    return '0';
                //仅有的减法负号
                if (lines[line_num][lines[line_num].Length - 1 - index] == '-')
                    return '0';
                return lines[line_num][lines[line_num].Length - 1 - index];
            }
        }

        /// <summary>
        /// 将某个字母置换为数字
        /// </summary>
        /// <param name="c">要被置换的字母</param>
        /// <param name="k">要被置换成的数字</param>
        public string replace(char c, int k)
        {
            string ret = "";
            if (c >= 'A' && c <= 'J' && k >= 0 && k <= 9)
            {
                ret = equation_console.Replace(c, (char)(k + '0'));
            }
            return ret;
        }
        /// <summary>
        /// 判断对方算式是否与我相同
        /// </summary>
        /// <param name="base_equation">被判断的算式</param>
        /// <returns></returns>
        public bool same_as(BaseEquation base_equation)
        {
            string a = equation_console;
            string b = base_equation.equation_console;
            if (a.Length != b.Length)
                return false;
            if (this.method != base_equation.method)
                return false;
            foreach (char c in LETTER_TABLE)
            {
                a = a.Replace(c, 'Z');
                b = b.Replace(c, 'Z');
            }
            return (a == b);
        }
        public override string ToString()
        {
            return equation_console;
        }


        private string _equation_console = string.Empty;
    }
}
