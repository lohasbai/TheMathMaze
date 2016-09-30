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
        /// <summary>
        /// 寻找解，找不到时返回"answer not found"
        /// <para>每个解占一行</para>
        /// </summary>
        /// <param name="equation">要找的方程</param>
        /// <returns></returns>
        public static string get_results(BaseEquation equation)
        {
            string ret = "answer not found";
            LinkedList<AddEquation> sorted_equation_list = new LinkedList<AddEquation>();
            List<AddEquation> calculated_equation_list = new List<AddEquation>();
            AddEquation root = new AddEquation(equation);
            sorted_equation_list = sorted_insert(sorted_equation_list, calculated_equation_list, root);
            while (true)
            {
                if (sorted_equation_list.Count == 0)
                {
                    break;
                }
                if (sorted_equation_list.First.Value.ans_found)
                {
                    if (ret == "answer not found")
                        ret = "";
                    ret += sorted_equation_list.First.Value.equation_console + "\r\n";
                    break;
                }
                LinkedListNode<AddEquation> now = sorted_equation_list.First;
                sorted_equation_list.RemoveFirst();
                List<char> ava_l = now.Value.available_letters_from_last();
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
                    AddEquation new_eq = new AddEquation(new_eva);
                    if (new_eq.ans_found)
                    {
                        if (ret == "answer not found")
                            ret = "";
                        ret += new_eq.equation_console + "\r\n";
                        continue;
                    }
                    sorted_equation_list = sorted_insert(sorted_equation_list, calculated_equation_list, new_eq);
                }
            }
            return ret;
        }

        private static LinkedList<AddEquation> sorted_insert(LinkedList<AddEquation> list,List<AddEquation> calculated, AddEquation to_insert)
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
                ////本段用于测试深度优先搜索

                //list.AddFirst(to_insert);
                //calculated.Add(to_insert);
                //return list;


                LinkedListNode<AddEquation> now = list.First;
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

    class AddEquation : BaseEquation
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

        public AddEquation(string str) : base(str)
        {
            if (method != METHOD.ADD)
            {
                clear();
            }
            init();
        }
        public AddEquation(BaseEquation _be) : base(_be.equation_console)
        {
            if (method != METHOD.ADD)
            {
                clear();
            }
            init();
        }

        /// <summary>
        /// 计算启发式节点值，若为-1表示未计算过，若为-2表示被剪枝，其他情况（正数）越小越好
        /// </summary>
        /// <returns></returns>
        private SKSpecialDecimal cal_node_point()
        {
            SKSpecialDecimal ret;
            if (equation_console == "")
                return new SKSpecialDecimal();
            if (NodePointStatic.compare_to(new SKSpecialDecimal(-1)) != 0)
                return NodePointStatic;

            

            string tmpa = equation_console;
            string[] linesa = spilt_to_3(tmpa);
            string[] linesb = spilt_to_3(tmpa);
            //剪枝0：是否是不可能的加法
            if (Math.Max(linesa[0].Length, linesa[1].Length) != linesa[2].Length ||
                Math.Max(linesa[0].Length, linesa[1].Length) != linesa[2].Length - 1)
            {
                ret = new SKSpecialDecimal(-2);
                NodePointStatic = ret;
                return ret;
            }
            //剪枝1：低位相加是否相同
            int min_last_len = -1;
            string[] last = new string[3];
            for (int i = 0; i < 3; i++)
            {
                last[i] = get_last_num(linesa[i]);
                if (last[i].Length < min_last_len || min_last_len == -1)
                    min_last_len = last[i].Length;
            }
            if (min_last_len > 0)
            {
                SKSpecialDecimal j1 = new SKSpecialDecimal(last[0].Substring(last[0].Length - min_last_len));
                SKSpecialDecimal j2 = new SKSpecialDecimal(last[1].Substring(last[1].Length - min_last_len));
                string left = (j1 + j2).to_string_only_integer();
                left = left.Substring(left.Length - min_last_len);
                string right = last[2].Substring(last[2].Length - min_last_len);
                if (left != right)
                {
                    ret = new SKSpecialDecimal(-2);
                    NodePointStatic = ret;
                    return ret;
                }
            }
            //剪枝1.5：对应加数位是否相等或相差1（进位只可能为1）
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
                    int tmpi = (xi[0] + xi[1]) % 10;
                    if (tmpi != xi[2] && (tmpi + 1) % 10 != xi[2])
                    {
                        ret = new SKSpecialDecimal(-2);
                        NodePointStatic = ret;
                        return ret;
                    }
                }
            }
            //剪枝2：加数和是否能达到和数或是否超过和数
            for (int i = 0; i < 2; i++)
                for (int j = 0; j < linesa[i].Length; j++)
                    if (linesa[i][j] >= 'A' && linesa[i][j] <= 'J')
                    {
                        linesa[i] = linesa[i].Replace(linesa[i][j], '9');
                        linesb[i] = linesb[i].Replace(linesb[i][j], '0');
                    }
            for (int j = 0; j < linesa[2].Length; j++)
                if (linesa[2][j] >= 'A' && linesa[2][j] <= 'J')
                {
                    linesa[2] = linesa[2].Replace(linesa[2][j], '0');
                    linesb[2] = linesb[2].Replace(linesb[2][j], '9');
                }
            SKSpecialDecimal g1 = new SKSpecialDecimal(linesa[0]);
            SKSpecialDecimal g2 = new SKSpecialDecimal(linesa[1]);
            SKSpecialDecimal g3 = new SKSpecialDecimal(linesa[2]);
            SKSpecialDecimal h1 = new SKSpecialDecimal(linesb[0]);
            SKSpecialDecimal h2 = new SKSpecialDecimal(linesb[1]);
            SKSpecialDecimal h3 = new SKSpecialDecimal(linesb[2]);
            if ((g1 + g2).compare_to(g3) < 0 || (h1 + h2).compare_to(h3) > 0)
            {
                ret = new SKSpecialDecimal(-2);
                NodePointStatic = ret;
                return ret;
            }
            else
            {
                //剪枝3：判断当前算式可能正确否
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
                string[] lines = spilt_to_3(tmpb);
                SKSpecialDecimal a1 = new SKSpecialDecimal(lines[0]);
                SKSpecialDecimal a2 = new SKSpecialDecimal(lines[1]);
                SKSpecialDecimal b = new SKSpecialDecimal(lines[2]);
                ret = b - (a1 + a2);
                //SKSpecialDecimal append = new SKSpecialDecimal(available_letters().Count - 1);
                //append.mul_10(max_line_len - 1);
                ret = SKSpecialDecimal.abs(ret) + available_letters().Count;//+append ;
                if (!ret.is_zero() && available_letters().Count == 0)
                {
                    ret = new SKSpecialDecimal(-2);
                    NodePointStatic = ret;
                    return ret;
                }
                NodePointStatic = ret;
                return ret;
            }
        }


        /// <summary>
        /// 判断和数最高位是否为1，同时计算节点值
        /// </summary>
        private void init()
        {
            string[] lines = spilt_to_3(equation_console);
            if (Math.Max(lines[0].Length, lines[1].Length) == lines[2].Length - 1 && lines[2][0] != '1')
                equation_console = equation_console.Replace(lines[2][0], '1');
            cal_node_point();
        }

        private string[] spilt_to_3(string equ)
        {
            string[] lines = equ.Split(new char[1] { '_' });
            lines[1] = lines[1].Substring(1);
            return lines;
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
