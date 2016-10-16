using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SKSpecial;

namespace TheMathMaze
{
    class MazeSub
    {
        /// <summary>
        /// 寻找解，找不到时返回"answer not found\r\n"
        /// <para>每个解占一行</para>
        /// </summary>
        /// <param name="equation">要找的方程</param>
        /// <returns></returns>
        public static string get_results(BaseEquation equation)
        {
            string ret = "answer not found\r\n";
            LinkedList<SubEquation> sorted_equation_list = new LinkedList<SubEquation>();
            List<SubEquation> calculated_equation_list = new List<SubEquation>();
            SubEquation root = new SubEquation(equation);
            sorted_equation_list = sorted_insert(sorted_equation_list, calculated_equation_list, root);
            while (true)
            {
                if (sorted_equation_list.Count == 0)
                {
                    break;
                }
                if (sorted_equation_list.First.Value.ans_found)
                {
                    if (ret == "answer not found\r\n")
                        ret = "";
                    ret += sorted_equation_list.First.Value.equation_console + "\r\n";
                    break;
                }
                LinkedListNode<SubEquation> now = sorted_equation_list.First;
                sorted_equation_list.RemoveFirst();
                List<char> ava_l = now.Value.available_letters_from_last(1);
                List<int> ava_n = now.Value.available_nums();
                for (int i = 0; i < ava_n.Count; i++)
                {
                    if (ava_n[i] == 0)
                    {
                        int first_l = now.Value.get_first_in_each_line().IndexOf(ava_l[0]);
                        if (first_l != -1 && now.Value.spilt_string_without_operator[first_l].Length > 1)
                            continue;
                    }
                    string new_eva = now.Value.replace(ava_l[0], ava_n[i]);
                    SubEquation new_eq = new SubEquation(new_eva);
                    if (new_eq.ans_found)
                    {
                        if (ret == "answer not found\r\n")
                            ret = "";
                        ret += new_eq.equation_console + "\r\n";
                        continue;
                    }
                    sorted_equation_list = sorted_insert(sorted_equation_list, calculated_equation_list, new_eq);
                }
            }
            return ret;
        }

        private static LinkedList<SubEquation> sorted_insert(LinkedList<SubEquation> list, List<SubEquation> calculated, SubEquation to_insert)
        {
            if (to_insert.node_point.compare_to(0) < 0)
                return list;
            if (list.Count == 0)
            {
                list.AddFirst(to_insert);
                calculated.Add(to_insert);
            }
            else
            {
                for (int i = 0; i < calculated.Count; i++)
                {
                    if (to_insert.same_as(calculated[i]))
                        return list;
                }

                LinkedListNode<SubEquation> now = list.First;
                bool inserted = false;
                for (int i = 0; i < list.Count; i++)
                {
                    if (to_insert.node_point.compare_to(now.Value.node_point) < 0)
                    {
                        list.AddBefore(now, to_insert);
                        calculated.Add(to_insert);
                        inserted = true;
                        break;
                    }
                    now = now.Next;
                }
                if (!inserted)
                {
                    list.AddLast(to_insert);
                    calculated.Add(to_insert);
                }
            }
            return list;
        }
    }

    class SubEquation : BaseEquation
    {
        public SKSpecialDecimal node_point
        {
            get
            {
                return cal_node_point();
            }
        }
        /// <summary>
        /// 找到结果了？
        /// </summary>
        public bool ans_found
        {
            get
            {
                return (node_point.is_zero());
            }
        }

        public SubEquation(string str) : base(str)
        {
            init();
        }
        public SubEquation(BaseEquation _be) : base(_be.equation_console)
        {
            init();
        }
        public void init()
        {
            if (method != METHOD.SUB)
            {
                clear();
                return;
            }
            cal_node_point();
        }

        private SKSpecialDecimal cal_node_point()
        {
            if (equation_console == "")
                return new SKSpecialDecimal();
            if (NodePointStatic.compare_to(new SKSpecialDecimal(-1)) != 0)
                return NodePointStatic;

            SKSpecialDecimal ret = new SKSpecialDecimal();

            string[] lines = spilt_string;
            //剪枝0：是否是不可能的减法
            if (Math.Max(lines[0].Length, lines[1].Length) < lines[2].Length - 1)
            {
                ret = new SKSpecialDecimal(-2);
                NodePointStatic = ret;
                return ret;
            }
            //剪枝1：对应被减数和减数位相减后的情况剪枝
            int max_line_LEN = max_line_len;
            for (int i = 0; i < max_line_LEN; i++)
            {
                char[] xc = new char[3];
                int[] xi = new int[3];
                bool do_this = true;
                for (int j = 0; j < 3; j++)
                {
                    xc[j] = get_from_line_end(j, i);
                    if (xc[j] < '0' || xc[j] > '9')
                    {
                        do_this = false;
                        break;
                    }
                    xi[j] = xc[j] - '0';
                }
                if (do_this)
                {
                    if (lines[2][0] != '-')
                    {
                        int tmpi = (xi[0] + 10 - xi[1]) % 10;
                        if (tmpi != xi[2] && tmpi - 1 != xi[2])
                        {
                            ret = new SKSpecialDecimal(-2);
                            NodePointStatic = ret;
                            return ret;
                        }
                    }
                    else
                    {
                        //A-B=-C  =>  A+C=B
                        int tmpi = (xi[0] + xi[2]) % 10;
                        if (tmpi != xi[1] && (tmpi + 1) % 10 != xi[1])
                        {
                            ret = new SKSpecialDecimal(-2);
                            NodePointStatic = ret;
                            return ret;
                        }
                    }
                }
            }
            //剪枝2：低位相减是否相同
            int min_last_len = -1;
            string[] last = new string[3];
            for (int i = 0; i < 3; i++)
            {
                last[i] = get_last_num(lines[i]);
                if (last[i].Length < min_last_len || min_last_len == -1)
                    min_last_len = last[i].Length;
            }
            if (min_last_len > 0)
            {
                SKSpecialDecimal j1 = new SKSpecialDecimal(last[0].Substring(last[0].Length - min_last_len));
                SKSpecialDecimal j2 = new SKSpecialDecimal(last[1].Substring(last[1].Length - min_last_len));
                SKSpecialDecimal j3 = new SKSpecialDecimal(last[2].Substring(last[2].Length - min_last_len));
                string left = "", right = "";
                if (lines[2][0] != '-')
                {
                    left = SKSpecialDecimal.abs(j2 + j3).to_string_only_integer();
                    right = last[0].Substring(last[0].Length - min_last_len);
                }
                else
                {
                    left = SKSpecialDecimal.abs(j1 + j3).to_string_only_integer();
                    right = last[1].Substring(last[1].Length - min_last_len);
                }
                if (left.Length < min_last_len)
                {
                    string tmpstring = "";
                    for (int i = 0; i < min_last_len - left.Length; i++)
                        tmpstring += "0";
                    left = tmpstring + left;
                }
                left = left.Substring(left.Length - min_last_len);
                
                if (left != right)
                {
                    ret = new SKSpecialDecimal(-2);
                    NodePointStatic = ret;
                    return ret;
                }
            }
            //TODO:剪枝3，不可能的算式
            string[] linesa = spilt_string;//A-B最大的结果比当前C最小的数还小
            string[] linesb = spilt_string;//A-B最小的结果比当前C最大的数还大
            linesa[1] = linesa[1].Substring(1);
            linesb[1] = linesb[1].Substring(1);
            for (int j = 0; j < linesa[0].Length; j++)
                if (linesa[0][j] >= 'A' && linesa[0][j] <= 'J')
                {
                    linesa[0] = linesa[0].Replace(linesa[0][j], '9');
                    linesb[0] = linesb[0].Replace(linesb[0][j], '0');
                }
            for (int j = 0; j < linesa[1].Length; j++)
                if (linesa[1][j] >= 'A' && linesa[1][j] <= 'J')
                {
                    linesa[1] = linesa[1].Replace(linesa[1][j], '0');
                    linesb[1] = linesb[1].Replace(linesb[1][j], '9');
                }
            for (int j = 0; j < linesa[2].Length; j++)
                if (linesa[2][j] >= 'A' && linesa[2][j] <= 'J')
                {
                    if (lines[2][0] != '-')
                    {
                        linesa[2] = linesa[2].Replace(linesa[2][j], '0');
                        linesb[2] = linesb[2].Replace(linesb[2][j], '9');
                    }
                    else
                    {
                        linesa[2] = linesa[2].Replace(linesa[2][j], '9');
                        linesb[2] = linesb[2].Replace(linesb[2][j], '0');
                    }
                }
            SKSpecialDecimal g1 = new SKSpecialDecimal(linesa[0]);
            SKSpecialDecimal g2 = new SKSpecialDecimal(linesa[1]);
            SKSpecialDecimal g3 = new SKSpecialDecimal(linesa[2]);
            SKSpecialDecimal h1 = new SKSpecialDecimal(linesb[0]);
            SKSpecialDecimal h2 = new SKSpecialDecimal(linesb[1]);
            SKSpecialDecimal h3 = new SKSpecialDecimal(linesb[2]);
            if ((g1 - g2).compare_to(g3) < 0 || (h1 - h2).compare_to(h3) > 0)
            {
                ret = new SKSpecialDecimal(-2);
                NodePointStatic = ret;
                return ret;
            }
            //计算分值判断结果
            List<int> ava = available_nums();
            if (ava.Count == 0)
                return new SKSpecialDecimal(0);
            int avg = (int)Math.Round((double)ava.Sum() / ava.Count);
            if (avg >= 10)
                avg = 9;
            else if (avg <= 0)
                avg = 0;
            string tmpb = equation_console;
            for (int i = 0; i < tmpb.Length; i++)
                if (tmpb[i] >= 'A' && tmpb[i] <= 'J')
                    tmpb = tmpb.Replace(tmpb[i], (char)(avg + '0'));
            string[] _lines = tmpb.Split(new char[1] { '_' });
            _lines[1] = _lines[1].Substring(1);
            SKSpecialDecimal a1 = new SKSpecialDecimal(_lines[0]);
            SKSpecialDecimal a2 = new SKSpecialDecimal(_lines[1]);
            SKSpecialDecimal a3 = new SKSpecialDecimal(_lines[2]);
            ret = SKSpecialDecimal.abs(a3 - (a1 - a2));
            ret = SKSpecialDecimal.abs(ret) + available_letters().Count;
            if (!ret.is_zero() && available_letters().Count == 0)
            {
                ret = new SKSpecialDecimal(-2);
                NodePointStatic = ret;
                return ret;
            }
            NodePointStatic = ret;
            return ret;
        }
        private SKSpecialDecimal NodePointStatic = new SKSpecialDecimal(-1);

        private string get_last_num(string line)
        {
            string ret = "";
            for (int i = 0; i < line.Length; i++)
            {
                char c = line[line.Length - i - 1];
                if (c >= '0' && c <= '9')
                {
                    ret = ret.Insert(0, c.ToString());
                }
                else
                {
                    break;
                }
            }
            return ret;
        }
    }
}
