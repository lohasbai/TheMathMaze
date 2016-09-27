using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheMathMaze
{
    class MazeConsoleMain
    {

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
        /// 初始化，若字符串不符合规范，将会赋值空字符串
        /// </summary>
        /// <param name="str">初始化字符串</param>
        public BaseEquation(string str)
        {
            equation_console = str;
            if (!basic_check())
                equation_console = string.Empty;
        }
        /// <summary>
        /// 获得本算式的法则
        /// </summary>
        public METHOD get_method
        {
            get
            {
                if (basic_check())
                {
                    int t = equation_console.IndexOf('_');
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
                return METHOD.UNDEFINE;
            }
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
                    equation_console = string.Empty;
                return false;
            }
            if (auto_up)
                equation_console = equation_console.ToUpper();
            //foreach (char c in equation_console)
            //{
            //    if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || c == '_' || c == '+' || c == '-' || c == '*' || c == '/')
            //        continue;
            //    else
            //    {
            //        if (auto_clear)
            //            equation_console = string.Empty;
            //        return false;
            //    }
            //}
            string[] lines = equation_console.Split(new char[1] { '_' });
            if (lines.Length < 3 || lines[1].Length < 2)
            {
                if (auto_clear)
                    equation_console = string.Empty;
                return false;
            }
            char method = lines[1].ToArray()[0];
            if (method != '+' && method != '-' && method != '*' && method != '/')
            {
                if (auto_clear)
                    equation_console = string.Empty;
                return false;
            }
            lines[1] = lines[1].Substring(1);
            foreach (string line in lines)
            {
                //此处可能有争议，目前暂不允许空行
                // TODO:证实是否需要空行
                if (line == string.Empty)
                {
                    if (auto_clear)
                        equation_console = string.Empty;
                    return false;
                }
                foreach (char c in line)
                {
                    if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z'))
                        continue;
                    else
                    {
                        if (auto_clear)
                            equation_console = string.Empty;
                        return false;
                    }
                }
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

        private string _equation_console = string.Empty;
    }
}
