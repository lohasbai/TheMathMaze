using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SKSpecial;

namespace TheMathMaze
{
    /// <summary>
    /// 加法算式搜索
    /// </summary>
    class AddMaze
    {
    }

    class AddEquation : BaseEquation
    {
        public AddEquation(string str) : base(str)
        {
            if (get_method != METHOD.ADD)
            {
                clear();
            }
            init();
        }
        public AddEquation(BaseEquation _be) : base(_be.equation_console)
        {
            if (get_method != METHOD.ADD)
            {
                clear();
            }
            init();
        }

        //越小越好
        public SKSpecialDecimal NodePoint()
        {
            if (equation_console == "")
                return new SKSpecialDecimal();
            if (NodePointStatic.compare_to(new SKSpecialDecimal(-1, 30)) != 0)
                return NodePointStatic;
            List<int> ava = available_nums();
            int avg = (int)Math.Round((double)ava.Sum() / ava.Count);
            if (avg >= 10)
                avg = 9;
            else if (avg <= 0)
                avg = 0;
            for (int i = 0; i < equation_console.Length; i++)
                if (equation_console[i] >= 'A' && equation_console[i] <= 'Z')
                    equation_console.Replace(equation_console[i], (char)(avg + '0'));
            string[] lines = spilt_to_3();
            SKSpecialDecimal a1 = new SKSpecialDecimal(lines[0]);
            SKSpecialDecimal a2 = new SKSpecialDecimal(lines[1]);
            SKSpecialDecimal b = new SKSpecialDecimal(lines[2]);
            SKSpecialDecimal ret = b - (a1 + a2);
            ret = ret * ret;
            NodePointStatic = ret;
            return ret;
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

        //利用加法特性看看最高位进位
        private void init()
        {
            string[] lines = spilt_to_3();
            if (lines[0].Length == lines[1].Length && lines[2].Length == lines[0].Length + 1 && lines[2][0] != '1')
                equation_console.Replace(lines[2][0], '1');
        }

        private string[] spilt_to_3()
        {
            string[] lines = equation_console.Split(new char[1] { '_' });
            lines[1] = lines[1].Substring(1);
            return lines;
        }

        private SKSpecialDecimal NodePointStatic = new SKSpecialDecimal(-1, 30);
    }
}
