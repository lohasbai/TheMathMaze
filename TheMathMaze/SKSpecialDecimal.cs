//SKSpecialDecimal
//Author: ShadowK
//E-mail: zhu.shadowk@gmail.com

using System;
using System.Collections.Generic;

namespace SKSpecial
{
    /// <summary>
    /// 有符号高精度，特殊地，零只有一位精度
    /// </summary>
    public class SKSpecialDecimal
    {
        /// <summary>
        /// 指示是否在进行高精度四则运算时进行精度外的消减
        /// </summary>
        public const bool CUT_IN_OPERATOR = false;

        /// <summary>
        /// 默认的构造函数，此时不表示任何数，有效位数为零
        /// </summary>
        public SKSpecialDecimal() { }
        /// <summary>
        /// 用指定的double数初始化高精度小数，默认精度15位
        /// <para>超过15位的部分将自动补零</para>
        /// </summary>
        /// <param name="x"></param>
        public SKSpecialDecimal(double x, int precision = 15)
        {
            reset(x,precision);
        }
        /// <summary>
        /// 用指定的int数初始化高精度小数，默认精度为9位
        /// <para>超过9位的部分将自动补零</para>
        /// </summary>
        /// <param name="x"></param>
        public SKSpecialDecimal(int x, int precision = 9)
        {
            reset(x,precision);
        }
        /// <summary>
        /// 用指定的string初始化高精度小数
        /// </summary>
        /// <param name="x"></param>
        public SKSpecialDecimal(string x)
        {
            reset(x);
        }
        /// <summary>
        /// 用指定的高精度数初始化自己
        /// </summary>
        /// <param name="origin"></param>
        public SKSpecialDecimal(SKSpecialDecimal origin)
        {
            reset(origin);
        }
        /// <summary>
        /// 返回位数上的值，高位在前，若超过索引将返回0
        /// <para>请注意，单独设定某一位的值可能会无意提高精度</para>
        /// </summary>
        /// <param name="index">该位的索引</param>
        /// <returns></returns>
        public byte this[int index]
        {
            get
            {
                if (index < 0 || index >= get_digit())
                    return 0;
                return data[index];
            }
            set
            {
                if (value > 9)
                    throw new Exception("某一位的值不应该超过9");
                if (index < 0)
                    throw new Exception("不允许设置索引小于零的某一位的值");
                else if (index >= get_digit())
                {
                    int tmp = get_digit();
                    for (int i = 0; i < index - tmp; i++)
                        data.Add(0);
                    data.Add(value);
                }
                else
                    data[index] = value;
            }
        }
        /// <summary>
        /// 清除data、positive与exp_10
        /// </summary>
        public void clear()
        {
            data.Clear();
            positive = true;
            exp_10 = 0;
        }
        /// <summary>
        /// 用指定的double数初始化高精度小数，默认精度15位
        /// <para>超过15位的部分将自动补零</para>
        /// </summary>
        /// <param name="x"></param>
        public void reset(double x, int precision = 15)
        {
            private_reset(x, precision, 15);
        }
        /// <summary>
        /// 用指定的int数初始化高精度小数，默认精度为9位
        /// <para>超过9位的部分将自动补零</para>
        /// </summary>
        /// <param name="x"></param>
        public void reset(int x, int precision = 9)
        {
            private_reset((double)x, precision, 9);
        }
        /// <summary>
        /// 用指定的string初始化高精度小数
        /// </summary>
        /// <param name="x"></param>
        public void reset(string _x)
        {
            clear();
            if (_x == string.Empty)
                return;
            string x = _x.Replace("\r", "");
            x = x.Replace("\n", "");
            x = x.Replace(" ", "");
            if (x[0] == '-')
            {
                positive = false;
                x = x.Substring(1);
            }
            int find_comma = x.IndexOf('.');
            if (find_comma == -1)
                exp_10 = x.Length - 1;
            else
            {
                exp_10 = find_comma - 1;
                x = x.Replace(".", "");
            }
            for (int i = 0; i < x.Length; i++)
            {
                byte tmp = 0;
                if (!byte.TryParse(x[i].ToString(), out tmp))
                {
                    clear();
                    return;
                }
                data.Add(tmp);
            }
            fix();
        }
        /// <summary>
        /// 用指定的高精度数初始化自己
        /// </summary>
        /// <param name="another"></param>
        public void reset(SKSpecialDecimal another)
        {
            clear();
            foreach (byte tmp in another.data)
                data.Add(tmp);
            positive = another.positive;
            exp_10 = another.exp_10;
        }
        /// <summary>
        /// 裁剪有效位数，四舍六入五取偶
        /// </summary>
        /// <param name="x">剩余的有效位数</param>
        public void cut(int x)
        {
            if (x < 0)
                x = 0;
            if (x > data.Count)
                return;// throw (new Exception("Cut To Long Exception"));
            if (x < data.Count)
            {
                if (x == 0)
                    clear();
                else if (data[x] < 5)
                    data.RemoveRange(x, data.Count - x);
                else if (data[x] > 5)
                {
                    //进位了，悲剧
                    upgrade(x);
                }
                else
                {
                    //5“取偶”
                    bool all_zero = true;
                    for(int i = x + 1;i < get_digit();i++)
                        if (this[i] != 0)
                        {
                            all_zero = false;
                            break;
                        }
                    if (all_zero)
                    {
                        if (data[x - 1] % 2 == 0)//前边是偶数
                            data.RemoveRange(x, data.Count - x);
                        else//前边是奇数
                        {
                            upgrade(x);
                        }
                    }
                    else
                    {
                        upgrade(x);
                    }
                }
            }
        }
        /// <summary>
        /// 取反
        /// </summary>
        public void inverse()
        {
            positive = !positive;
        }
        /// <summary>
        /// 去除前缀零
        /// </summary>
        public void fix()
        {
            if (get_digit() == 1 && this[0] == 0 && get_exp() != 0)//零
            {
                exp_10 = 0;
                positive = true;
                return;
            }
            else if (get_digit() < 2)
                return;
            int zero_count = 0;
            foreach (byte c in data)
            {
                if (c == 0)
                    zero_count++;
                else
                    break;
            }
            if (zero_count == get_digit())//这个数是零
            {
                data.RemoveRange(1, data.Count - 1);
                exp_10 = 0;
                positive = true;
            }
            else
            {
                data.RemoveRange(0, zero_count);
                exp_10 = exp_10 - zero_count;
            }
        }
        /// <summary>
        /// 返回这个数是否是零
        /// </summary>
        /// <param name="auto_fix">是否自动fix</param>
        /// <returns></returns>
        public bool is_zero(bool auto_fix = true)
        {
            if (auto_fix)
                fix();
            return (get_digit() == 1 && this[0] == (byte)0);
        }
        /// <summary>
        /// 乘上10的次幂
        /// </summary>
        /// <param name="times">10的幂次</param>
        public void mul_10(int times)
        {
            if (get_digit() == 0 || is_zero())
                return;
            exp_10 += times;
        }
        /// <summary>
        /// 获得有效位数
        /// </summary>
        /// <returns></returns>
        public int get_digit()
        {
            return data.Count;
        }
        /// <summary>
        /// 获得科学计数法中10的幂次
        /// </summary>
        /// <returns></returns>
        public int get_exp()
        {
            return exp_10;
        }
        /// <summary>
        /// 返回是否是正数
        /// </summary>
        /// <returns></returns>
        public bool get_positive()
        {
            return positive;
        }
        /// <summary>
        /// 返回与另外某数的比较（不考虑精度）
        /// <para>若大于该数，返回1，相等则返回0，否则返回-1</para>
        /// <para>若某数不存在位数（有效位数为0），返回0</para>
        /// <para>当且仅当有效位数、各位数及符号位全部一致时，才判定相等</para>
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public int compare_to(SKSpecialDecimal x)
        {
            if (get_digit() == 0 || x.get_digit() == 0)
                return 0;
            if (positive && (!x.positive))
                return 1;
            else if ((!positive) && x.positive)
                return -1;
            if (get_exp() > x.get_exp())
                return (positive) ? 1 : -1;
            else if (get_exp() < x.get_exp())
                return (positive) ? -1 : 1;
            for (int i = 0; i < Math.Min(get_digit(), x.get_digit()); i++)
            {
                if(this[i] > x[i])
                    return (positive) ? 1 : -1;
                else if(this[i] < x[i])
                    return (positive) ? -1 : 1;
            }
            if (get_digit() > x.get_digit())//有效位数较多且不为零
            {
                for (int i = x.get_digit(); i < get_digit(); i++)
                    if (this[i] != (byte)0)
                        return (positive) ? 1 : -1;
            }
            else if (get_digit() < x.get_digit())
            {
                for (int i = get_digit(); i < x.get_digit(); i++)
                    if (x[i] != (byte)0)
                        return (positive) ? -1 : 1;
            }
            return 0;
        }
        /// <summary>
        /// 返回与另外某数的比较（不考虑精度）
        /// <para>若大于该数，返回1，相等则返回0，否则返回-1</para>
        /// <para>当且仅当有效位数、各位数及符号位全部一致时，才判定相等</para>
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public int compare_to(int x)
        {
            return this.compare_to(new SKSpecialDecimal(x));
        }
        /// <summary>
        /// 返回与另外某数的比较（不考虑精度）
        /// <para>若大于该数，返回1，相等则返回0，否则返回-1</para>
        /// <para>当且仅当有效位数、各位数及符号位全部一致时，才判定相等</para>
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public int compare_to(double x)
        {
            return this.compare_to(new SKSpecialDecimal(x));
        }
        /// <summary>
        /// 返回当前数对应的double值，注意精度有误差，若本数太大，可能甚至无法表示
        /// <para>出错时返回零</para>
        /// </summary>
        /// <returns></returns>
        public double to_double()
        {
            try
            {
                double ret = 0;
                for (int i = 0; i < get_digit(); i++)
                {
                    ret += this[i] * Math.Pow(10, get_exp() - i);
                }
                return ret;
            }
            catch (Exception)
            {
                return .0D;
            }
        }
        /// <summary>
        /// 按科学计数法的方式输出字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string ans = string.Empty;
            if (data.Count == 0)
                return ans;
            if (!positive)
                ans += "-";
            ans += data[0].ToString();
            if (data.Count > 1)
            {
                ans += ".";
                for (int i = 1; i < data.Count; i++)
                    ans += data[i].ToString();
            }
            if (exp_10 != 0)
            {
                ans += " * 10^";
                if (exp_10 < 0)
                    ans += "(" + exp_10.ToString() + ")";
                else
                    ans += exp_10.ToString();
            }
            return ans;
        }

        /// <summary>
        /// 返回绝对值
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static SKSpecialDecimal abs(SKSpecialDecimal a)
        {
            SKSpecialDecimal a_abs = new SKSpecialDecimal(a);
            a_abs.positive = true;
            return a_abs;
        }
        /// <summary>
        /// 返回幂次值，仅支持整数次幂
        /// </summary>
        /// <param name="x"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        public static SKSpecialDecimal pow(SKSpecialDecimal x, int a)
        {
            if (a == 0)
                return new SKSpecialDecimal(1, x.get_digit());
            else if (a == 1)
                return new SKSpecialDecimal(x);
            else if (a == 2)
                return x * x;
            else if (a < 0)
                return (new SKSpecialDecimal(1, x.get_digit())) / pow(x, -1 * a);
            else
            {
                if (a % 2 == 0)
                    return pow(x, a / 2) * pow(x, a / 2);
                else
                    return pow(x, a / 2) * pow(x, a / 2) * x;
            }
        }
        /// <summary>
        /// 取不超过x的最大整数
        /// <para>若x的绝对值大于int的最大表示范围，返回0</para>
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static int floor(SKSpecialDecimal x)
        {
            if (abs(x).compare_to(SKSpecialDecimal.INT_MAX) > 0)
                return 0;
            int ret = 0;
            if (x.get_exp() > -1)
            {
                for (int i = x.get_exp(); i > -1; i--)
                    ret += (int)Math.Round(x[x.get_exp() - i] * Math.Pow(10,i));
                if (!x.get_positive())
                    ret = ret * -1 - 1;
            }
            else if (!x.get_positive())
            {
                ret = -1;
            }
            return ret;
        }

        /// <summary>
        /// 加法，有效位数取决于绝对误差最大的数
        /// <para>例：1.001+100.1=101.1</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static SKSpecialDecimal operator +(SKSpecialDecimal a, SKSpecialDecimal b)
        {
            if (a.is_zero())
                return new SKSpecialDecimal(b);
            else if (b.is_zero())
                return new SKSpecialDecimal(a);
            SKSpecialDecimal a_copy = new SKSpecialDecimal(a);
            SKSpecialDecimal b_copy = new SKSpecialDecimal(b);
            //越小越精确，表示末位所在的位置
            SKSpecialDecimal ret = add(a_copy, b_copy);
            if (CUT_IN_OPERATOR)
            {
                int a_min_bit = a_copy.exp_10 - a_copy.get_digit() + 1;
                int b_min_bit = b_copy.exp_10 - b_copy.get_digit() + 1;
                int ret_min_bit = ret.exp_10 - ret.get_digit() + 1;
                if (a_min_bit > b_min_bit)//以a的精度为准
                    ret.cut(ret.get_digit() - a_min_bit + ret_min_bit);
                else//以b的精度为准
                    ret.cut(ret.get_digit() - b_min_bit + ret_min_bit);
            }
            return ret;
        }
        /// <summary>
        /// 加法，有效位数取决于绝对误差最大的数
        /// <para>例：1.001+100.1=101.1</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static SKSpecialDecimal operator +(SKSpecialDecimal a, double b)
        {
            SKSpecialDecimal bb = new SKSpecialDecimal(b);
            return a + bb;
        }
        /// <summary>
        /// 加法，有效位数取决于绝对误差最大的数
        /// <para>例：1.001+100.1=101.1</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static SKSpecialDecimal operator +(double a, SKSpecialDecimal b)
        {
            SKSpecialDecimal aa = new SKSpecialDecimal(a);
            return aa + b;
        }
        /// <summary>
        /// 加法，有效位数取决于绝对误差最大的数
        /// <para>例：1.001+100.1=101.1</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static SKSpecialDecimal operator +(SKSpecialDecimal a, int b)
        {
            SKSpecialDecimal bb = new SKSpecialDecimal(b);
            return a + bb;
        }
        /// <summary>
        /// 加法，有效位数取决于绝对误差最大的数
        /// <para>例：1.001+100.1=101.1</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static SKSpecialDecimal operator +(int a, SKSpecialDecimal b)
        {
            SKSpecialDecimal aa = new SKSpecialDecimal(a);
            return aa + b;
        }

        /// <summary>
        /// 减法，有效位数与加法类似，取决于绝对误差最大的数
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static SKSpecialDecimal operator -(SKSpecialDecimal a, SKSpecialDecimal b)
        {
            SKSpecialDecimal b_copy = new SKSpecialDecimal(b);
            b_copy.inverse();
            return (a + b_copy);
        }
        /// <summary>
        /// 减法，有效位数与加法类似，取决于绝对误差最大的数
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static SKSpecialDecimal operator -(SKSpecialDecimal a, double b)
        {
            SKSpecialDecimal bb = new SKSpecialDecimal(b);
            return a - bb;
        }
        /// <summary>
        /// 减法，有效位数与加法类似，取决于绝对误差最大的数
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static SKSpecialDecimal operator -(double a, SKSpecialDecimal b)
        {
            SKSpecialDecimal aa = new SKSpecialDecimal(a);
            return aa - b;
        }
        /// <summary>
        /// 减法，有效位数与加法类似，取决于绝对误差最大的数
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static SKSpecialDecimal operator -(SKSpecialDecimal a, int b)
        {
            SKSpecialDecimal bb = new SKSpecialDecimal(b);
            return a - bb;
        }
        /// <summary>
        /// 减法，有效位数与加法类似，取决于绝对误差最大的数
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static SKSpecialDecimal operator -(int a, SKSpecialDecimal b)
        {
            SKSpecialDecimal aa = new SKSpecialDecimal(a);
            return aa - b;
        }

        /// <summary>
        /// 乘法，结果的有效位数与两数有效位数较少的数相同
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static SKSpecialDecimal operator *(SKSpecialDecimal a, SKSpecialDecimal b)
        {
            if (a.get_digit() == 0 || b.get_digit() == 0)
                return new SKSpecialDecimal();
            int less = Math.Min(a.get_digit(), b.get_digit());
            SKSpecialDecimal ret = mul(a, b);
            if(CUT_IN_OPERATOR)
                ret.cut(less);
            return ret;
        }
        /// <summary>
        /// 乘法，结果的有效位数与两数有效位数较少的数相同
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static SKSpecialDecimal operator *(SKSpecialDecimal a, double b)
        {
            SKSpecialDecimal bb = new SKSpecialDecimal(b);
            return a * bb;
        }
        /// <summary>
        /// 乘法，结果的有效位数与两数有效位数较少的数相同
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static SKSpecialDecimal operator *(double a, SKSpecialDecimal b)
        {
            SKSpecialDecimal aa = new SKSpecialDecimal(a);
            return aa * b;
        }
        /// <summary>
        /// 乘法，结果的有效位数与两数有效位数较少的数相同
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static SKSpecialDecimal operator *(SKSpecialDecimal a, int b)
        {
            SKSpecialDecimal bb = new SKSpecialDecimal(b);
            return a * bb;
        }
        /// <summary>
        /// 乘法，结果的有效位数与两数有效位数较少的数相同
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static SKSpecialDecimal operator *(int a, SKSpecialDecimal b)
        {
            SKSpecialDecimal aa = new SKSpecialDecimal(a);
            return aa * b;
        }

        /// <summary>
        /// 除法，结果的有效位数与两数有效位数较少的数相同
        /// </summary>
        /// <param name="a">除数</param>
        /// <param name="b">被除数</param>
        /// <returns></returns>
        public static SKSpecialDecimal operator /(SKSpecialDecimal a, SKSpecialDecimal b)
        {
            if (a.get_digit() == 0 || b.get_digit() == 0)
                return new SKSpecialDecimal();
            int less = Math.Min(a.get_digit(), b.get_digit());
            SKSpecialDecimal ret = div(a, b);
            if(CUT_IN_OPERATOR)
                ret.cut(less);
            return ret;
        }
        /// <summary>
        /// 除法，结果的有效位数与两数有效位数较少的数相同
        /// </summary>
        /// <param name="a">除数</param>
        /// <param name="b">被除数</param>
        /// <returns></returns>
        public static SKSpecialDecimal operator /(SKSpecialDecimal a, double b)
        {
            SKSpecialDecimal bb = new SKSpecialDecimal(b);
            return a / bb;
        }
        /// <summary>
        /// 除法，结果的有效位数与两数有效位数较少的数相同
        /// </summary>
        /// <param name="a">除数</param>
        /// <param name="b">被除数</param>
        /// <returns></returns>
        public static SKSpecialDecimal operator /(double a, SKSpecialDecimal b)
        {
            SKSpecialDecimal aa = new SKSpecialDecimal(a);
            return aa / b;
        }
        /// <summary>
        /// 除法，结果的有效位数与两数有效位数较少的数相同
        /// </summary>
        /// <param name="a">除数</param>
        /// <param name="b">被除数</param>
        /// <returns></returns>
        public static SKSpecialDecimal operator /(SKSpecialDecimal a, int b)
        {
            SKSpecialDecimal bb = new SKSpecialDecimal(b);
            return a / bb;
        }
        /// <summary>
        /// 除法，结果的有效位数与两数有效位数较少的数相同
        /// </summary>
        /// <param name="a">除数</param>
        /// <param name="b">被除数</param>
        /// <returns></returns>
        public static SKSpecialDecimal operator /(int a, SKSpecialDecimal b)
        {
            SKSpecialDecimal aa = new SKSpecialDecimal(a);
            return aa / b;
        }

        /// <summary>
        /// 不考虑精度、误差情况下的加法
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private static SKSpecialDecimal add(SKSpecialDecimal _a, SKSpecialDecimal _b)
        {
            SKSpecialDecimal a = new SKSpecialDecimal(_a);
            SKSpecialDecimal b = new SKSpecialDecimal(_b);
            if (a.get_digit() == 0 || a.is_zero())
                return new SKSpecialDecimal(b);
            else if (b.get_digit() == 0 || b.is_zero())
                return new SKSpecialDecimal(a);
            SKSpecialDecimal ans = new SKSpecialDecimal();
            int compare_abs = abs(a).compare_to(abs(b));

            ans.exp_10 = Math.Max(a.get_exp(), b.get_exp()) + 1;
            int up_range = ans.exp_10;
            int down_range = Math.Min(a.get_exp() - a.get_digit(), b.get_exp() - b.get_digit()) + 1;
            int range = up_range - down_range + 1;
            int carry = 0;
            if (a.positive == b.positive)
            {
                ans.positive = a.positive;
                for (int i = range - 1; i > -1; i--)
                {
                    byte tmp = (byte)((a[i - ans.get_exp() + a.get_exp()] + b[i - ans.get_exp() + b.get_exp()] + carry));
                    ans[i] = (byte)(tmp % 10);
                    if (tmp > 9)
                        carry = 1;
                    else
                        carry = 0;
                }
            }
            else if (compare_abs == 0)
                ans[0] = 0;
            else
            {
                SKSpecialDecimal bigger, smaller;
                if (compare_abs > 0)
                {
                    ans.positive = a.positive;
                    bigger = a;
                    smaller = b;
                }
                else
                {
                    ans.positive = b.positive;
                    bigger = b;
                    smaller = a;
                }
                for (int i = range - 1; i > -1; i--)
                {
                    sbyte tmp = (sbyte)(((sbyte)bigger[i - ans.get_exp() + bigger.get_exp()] - (sbyte)smaller[i - ans.get_exp() + smaller.get_exp()] - carry));
                    ans[i] = (byte)((tmp + 10) % 10);
                    if (tmp < 0)
                        carry = 1;
                    else
                        carry = 0;
                }
            }
            ans.fix();
            return ans;
        }
        /// <summary>
        /// 乘上一个一位数，未考虑精度取舍问题
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private static SKSpecialDecimal mul_single(SKSpecialDecimal _a, byte b)
        {
            SKSpecialDecimal a = new SKSpecialDecimal(_a);
            if (b > 9)
                throw new Exception("必须是一位数啊！");
            if (a.get_digit() == 0 || a.is_zero() || b == 1)
                return new SKSpecialDecimal(a);
            else if (b == 0)
                return new SKSpecialDecimal(0);
            SKSpecialDecimal ret = new SKSpecialDecimal();
            byte carry = 0;
            for (int i = a.get_digit() - 1; i > -1; i--)
            {
                byte tmp = (byte)(a[i] * b+carry);
                ret[i + 1] = (byte)(tmp % 10);
                carry = (byte)(tmp / 10);
            }
            ret[0] = carry;
            ret.fix();
            if(!ret.is_zero())
                ret.exp_10 = (carry == 0) ? a.get_exp() : (a.get_exp() + 1);
            return ret;
        }
        /// <summary>
        /// 不考虑精度、误差情况下的乘法
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private static SKSpecialDecimal mul(SKSpecialDecimal _a, SKSpecialDecimal _b)
        {
            SKSpecialDecimal a = new SKSpecialDecimal(_a);
            SKSpecialDecimal b = new SKSpecialDecimal(_b);
            if (a.get_digit() == 0)
                return new SKSpecialDecimal(b);
            else if (b.get_digit() == 0)
                return new SKSpecialDecimal(a);
            else if (a.is_zero() || b.is_zero())
                return new SKSpecialDecimal(0);
            SKSpecialDecimal ret = new SKSpecialDecimal(0);
            SKSpecialDecimal longer, shorter;
            if (a.get_digit() > b.get_digit())
            {
                longer = a;
                shorter = b;
            }
            else
            {
                longer = b;
                shorter = a;
            }
            for (int i = 0; i < shorter.get_digit(); i++)
            {
                SKSpecialDecimal tmp = mul_single(longer, shorter[i]);
                tmp.mul_10(shorter.get_exp() - i);
                ret = add(ret, tmp);
            }
            ret.fix();
            ret.positive = (a.get_positive() == b.get_positive());
            return ret;
        }
        /// <summary>
        /// 不考虑精度、误差情况下的除法，会一直除到两者的有效位数之和
        /// <para>a除以b</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private static SKSpecialDecimal div(SKSpecialDecimal _a, SKSpecialDecimal _b)
        {
            SKSpecialDecimal a = new SKSpecialDecimal(_a);
            SKSpecialDecimal b = new SKSpecialDecimal(_b);
            if (a.get_digit() == 0 || b.get_digit() == 0 || b.is_zero())
                return new SKSpecialDecimal();
            else if (a.is_zero())
                return new SKSpecialDecimal(0);
            SKSpecialDecimal ret = new SKSpecialDecimal();

            SKSpecialDecimal tmp = new SKSpecialDecimal(b);
            tmp.exp_10 = a.get_exp();
            bool first_zero = (abs(tmp).compare_to(abs(a)) > 0);

            ret.positive = (a.get_positive() == b.get_positive()) ? true : false;
            ret.exp_10 = a.get_exp() - b.get_exp();
            bool stop = false;
            SKSpecialDecimal accumulate = new SKSpecialDecimal(0);
            a = abs(a);
            b = abs(b);
            for (int i = (first_zero) ? 1 : 0; i < a.get_digit() + b.get_digit(); i++)
            {
                for (int j = 1; j < 10; j++)
                {
                    SKSpecialDecimal mul_tmp2 = mul_single(b, (byte)j);
                    mul_tmp2.mul_10(ret.get_exp() - i);
                    SKSpecialDecimal now_acc = add(accumulate, mul_tmp2);
                    //ret[i] = (byte)j;
                    int compare_ans = now_acc.compare_to(a);
                    if (compare_ans == 0)
                    {
                        ret[i] = (byte)j;
                        stop = true;
                        break;
                    }
                    else if (compare_ans > 0)
                    {
                        ret[i] = (byte)(j - 1);
                        mul_tmp2 = mul_single(b, (byte)(j - 1));
                        mul_tmp2.mul_10(ret.get_exp() - i);
                        accumulate = add(accumulate, mul_tmp2);
                        break;
                    }
                    else if (j == 9)
                    {
                        ret[i] = (byte)j;
                        accumulate = add(accumulate, mul_tmp2);
                    }
                }
                if (stop)
                {
                    for (int j = i + 1; j < a.get_digit() + b.get_digit(); j++)
                        ret[j] = 0;
                    break;
                }
            }
            ret.fix();
            return ret;
        }

        /// <summary>
        /// reset(double or int)的内部实现
        /// </summary>
        /// <param name="x">要设定的数</param>
        /// <param name="precision">精度</param>
        /// <param name="threshold">超过threshold赋值为零</param>
        private void private_reset(double x, int precision, int threshold)
        {
            clear();
            if (x < 0) positive = false;
            else if (x == 0)
            {
                exp_10 = 0;
                data.Add(0);
                return;
            }
            x = Math.Abs(x);
            exp_10 = Convert.ToInt32(Math.Floor(Math.Log10(x)));
            double tmp = (x * Math.Pow(10, -Math.Floor(Math.Log10(x))));
            for (int i = 0; i < precision; i++)
            {
                if (i >= threshold)
                    data.Add(0);
                else
                    data.Add(Convert.ToByte(Math.Floor(tmp * Math.Pow(10, i)) % 10));
            }
        }
        /// <summary>
        /// 在(x-1)位处发生进位，同时截取x位有效数字
        /// </summary>
        /// <param name="x"></param>
        private void upgrade(int x)
        {
            if (x < 2)
                return;
            data.RemoveRange(x, data.Count - x);
            SKSpecialDecimal _tmp = new SKSpecialDecimal(this);
            SKSpecialDecimal _add = new SKSpecialDecimal();
            _add[x - 1] = 1;
            _add.exp_10 = _tmp.get_exp();
            _tmp = add(_tmp, _add);
            if (_tmp.get_digit() > get_digit())//进位导致位数增加了
                _tmp.cut(get_digit());//本次cut一定不会导致进位了
            reset(_tmp);
        }

        /// <summary>
        /// 存储数据，其长度表示有效位数，高位在前
        /// <para>科学计数法，十进制</para>
        /// </summary>
        private List<byte> data = new List<byte>();
        /// <summary>
        /// 表示10的幂次
        /// </summary>
        private int exp_10 = 0;
        /// <summary>
        /// 是否为正数
        /// </summary>
        private bool positive = true;

        private static SKSpecialDecimal INT_MAX = new SKSpecialDecimal(int.MaxValue);
    }
}
