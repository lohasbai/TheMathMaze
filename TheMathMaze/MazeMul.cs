using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SKSpecial;

namespace TheMathMaze
{
    /// <summary>
    /// 乘法有很多复杂的情况暂且不论，比如乘0什么的……（搞事情……）
    /// </summary>
    class MazeMul
    {
        public delegate void OnProcessCall(object sender, ConsoleMazeMain.BaseEquationEventArgs e);
        public event OnProcessCall callback;

        public async Task<string> get_results(BaseEquation equation)
        {
            string ret = "answer not found\r\n";
            LinkedList<MulEquation> sorted_equation_list = new LinkedList<MulEquation>();
            List<MulEquation> calculated_equation_list = new List<MulEquation>();
            MulEquation root = new MulEquation(equation);
            sorted_equation_list = sorted_insert(sorted_equation_list, calculated_equation_list, root);
            while (true)
            {
                if (sorted_equation_list.Count == 0)
                {
                    break;
                }
                ConsoleMazeMain.BaseEquationEventArgs ee = new ConsoleMazeMain.BaseEquationEventArgs();
                ee.be = sorted_equation_list.First.Value;
                callback(this, ee);
                bool bk = false;
                await Task.Run(() =>
                {
                    if (sorted_equation_list.First.Value.ans_found)
                    {
                        if (ret == "answer not found\r\n")
                            ret = "";
                        ret += sorted_equation_list.First.Value.equation_console + "\r\n";
                        bk = true;
                        return;
                    }
                    LinkedListNode<MulEquation> now = sorted_equation_list.First;
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
                        MulEquation new_eq = new MulEquation(new_eva);
                        if (new_eq.ans_found)
                        {
                            if (ret == "answer not found\r\n")
                                ret = "";
                            ret += new_eq.equation_console + "\r\n";
                            continue;
                        }
                        sorted_equation_list = sorted_insert(sorted_equation_list, calculated_equation_list, new_eq);
                    }
                });
                if (bk)
                    break;
            }
            return ret;
        }

        private static LinkedList<MulEquation> sorted_insert(LinkedList<MulEquation> list, List<MulEquation> calculated, MulEquation to_insert)
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

                LinkedListNode<MulEquation> now = list.First;
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

    class MulEquation : BaseEquation
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
        public MulEquation(string str) : base(str)
        {
            if (method != METHOD.MUL)
            {
                clear();
            }
            init();
        }
        public MulEquation(BaseEquation _be) : base(_be.equation_console)
        {
            if (method != METHOD.MUL)
            {
                clear();
            }
            init();
        }

        private void init()
        {
            cal_node_point();
        }

        private SKSpecialDecimal cal_node_point()
        {
            if (equation_console == "")
                return new SKSpecialDecimal();
            if (NodePointStatic.compare_to(new SKSpecialDecimal(-1)) != 0)
                return NodePointStatic;

            SKSpecialDecimal ret = new SKSpecialDecimal();

            //剪枝0，行数与乘数不对应
            string[] lines = spilt_string;
            if (lines[1].Length < 2 || (lines[1].Length == 2 && lines.Length != 3) || (lines[1].Length > 2 && lines[1].Length - 1 != lines.Length - 3))
            {
                ret = new SKSpecialDecimal(-2);
                NodePointStatic = ret;
                return ret;
            }
            //剪枝0.5：不可能的乘法-结果位数太多
            if (lines[lines.Length - 1].Length > (lines[0].Length + lines[1].Length))
            {
                ret = new SKSpecialDecimal(-2);
                NodePointStatic = ret;
                return ret;
            }
            //分类-按单位数乘法和多位数乘法
            if (lines[1].Length == 2)
            {
                //剪枝1：低位乘法
                if (lines[1][1] >= '0' && lines[1][1] <= '9')
                {
                    int min_last_len = -1;
                    string lastup, lastdown;
                    lastup = get_last_num(lines[0]);
                    lastdown = get_last_num(lines[2]);
                    min_last_len = (lastup.Length >= lastdown.Length) ? lastdown.Length : lastup.Length;
                    if (min_last_len > 0)
                    {
                        SKSpecialDecimal mup = new SKSpecialDecimal(lines[0].Substring(lines[0].Length - min_last_len));
                        //SKSpecialDecimal mdown = new SKSpecialDecimal(lines[2].Substring(lines[2].Length - min_last_len));
                        string left = (mup * new SKSpecialDecimal(lines[1][1].ToString())).to_string_only_integer();
                        if (left.Length < min_last_len)
                        {
                            string tmpstring = "";
                            for (int i = 0; i < min_last_len - left.Length; i++)
                                tmpstring += "0";
                            left = tmpstring + left;
                        }
                        left = left.Substring(left.Length - min_last_len);
                        string right = lines[2].Substring(lines[2].Length - min_last_len);
                        if (left != right)
                        {
                            ret = new SKSpecialDecimal(-2);
                            NodePointStatic = ret;
                            return ret;
                        }
                    }
                }
                //剪枝2：不可能乘法（结果太大或太小）
                string[] linesa = spilt_string;//A*B最大的结果比当前C最小的数还小
                string[] linesb = spilt_string;//A*B最小的结果比当前C最大的数还大
                linesa[1] = linesa[1].Substring(1);
                linesb[1] = linesb[1].Substring(1);
                for (int j = 0; j < linesa[0].Length; j++)
                    if (linesa[0][j] >= 'A' && linesa[0][j] <= 'J')
                    {
                        linesa[0] = linesa[0].Replace(linesa[0][j], '9');
                        linesb[0] = linesb[0].Replace(linesb[0][j], '0');
                    }
                if (linesa[1][0] >= 'A' && linesa[1][0] <= 'J')
                {
                    linesa[1] = "9";
                    linesb[1] = "0";//TODO:这个有必要检查么？
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
                if ((g1 * g2).compare_to(g3) < 0 || (h1 * h2).compare_to(h3) > 0)
                {
                    ret = new SKSpecialDecimal(-2);
                    NodePointStatic = ret;
                    return ret;
                }
                //计算分值返回
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
                ret = SKSpecialDecimal.abs(a3 - (a1 * a2));
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
            else
            {
                //多位数乘法！
                //按位执行单位数剪枝
                string up = lines[0];
                ret.reset(0);
                int lines_len = lines[1].Length - 1;//乘法中间行数
                for (int i = 1; i < lines[1].Length; i++)
                {
                    string mid = "_*" + lines[1][i];
                    //down是从下往上数第i行
                    string down = "_" + lines[lines.Length - i - 1];
                    string temp_console = up + mid + down;
                    MulEquation tmp_eq = new MulEquation(temp_console);
                    if (tmp_eq.node_point.compare_to(0) < 0)
                    {
                        ret = new SKSpecialDecimal(-2);
                        NodePointStatic = ret;
                        return ret;
                    }
                    ret = ret + tmp_eq.node_point;
                }
                //加法剪枝
                //目前只写一个低位剪枝先
                int min_last_len = -1;
                string[] last = new string[lines_len + 1];
                for (int i = 0; i < lines_len + 1; i++)
                {
                    last[i] = get_last_num(lines[i + 2]);
                    //乘法中间层补零
                    if(i < lines_len)
                        for (int j = 0; j < i; j++)
                            last[i] = last[i] + '0';
                    if (last[i].Length < min_last_len || min_last_len == -1)
                        min_last_len = last[i].Length;
                }
                if (min_last_len > 0)
                {
                    SKSpecialDecimal tmp_add = new SKSpecialDecimal();
                    SKSpecialDecimal left_sum = new SKSpecialDecimal(0);
                    for (int i = 0; i < lines_len; i++)
                    {
                        tmp_add.reset(last[i].Substring(last[i].Length - min_last_len));
                        left_sum = left_sum + tmp_add;
                    }
                    string left = left_sum.to_string_only_integer();
                    left = left.Substring(left.Length - min_last_len);
                    string right = last[lines_len].Substring(last[lines_len].Length - min_last_len);
                    if (left != right)
                    {
                        ret = new SKSpecialDecimal(-2);
                        NodePointStatic = ret;
                        return ret;
                    }
                }
                //计算分值返回
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
                //只计算加法分的影响
                SKSpecialDecimal _tmp = new SKSpecialDecimal();
                SKSpecialDecimal _tmp_sum = new SKSpecialDecimal(0);
                for (int i = 2; i < _lines.Length - 1; i++)
                {
                    _tmp.reset(_lines[i]);
                    _tmp.mul_10(i - 2);
                    _tmp_sum = _tmp_sum + _tmp;
                }
                SKSpecialDecimal _tmp_right_sum = new SKSpecialDecimal(_lines[_lines.Length - 1]);
                ret = ret + SKSpecialDecimal.abs(_tmp_right_sum - _tmp_sum);
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
