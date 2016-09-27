using System.Collections.Generic;

namespace HighDecimal
{
    //高位在后，低位在前。
    public class Number
    {
        private List<sbyte> self = new List<sbyte>();
        public int length
        {
            private set{}
            get
            {
                return self.Count;
            }
        }

        private static int min(int x, int y)
        {
            return x < y ? x : y;
        }
        private static int max(int x, int y)
        {
            return x > y ? x : y;
        }
        private static Number Multiply_s(Number x, sbyte y)
        {
            Number ans = new Number();
            sbyte carry = 0;
            if (y == 1)
            {
                ans.SetVal(x);
            }
            else if (y > 1 && y < 10)
            {
                for (int i = 0; i < x.length; i++)
                {
                    ans.self.Add((sbyte)((x.self[i] * y + carry) % 10));
                    carry = (sbyte)((x.self[i] * y + carry) / 10);
                }
                if (carry > 0)
                    ans.self.Add(carry);
            }
            return ans;
        }
        private static Number Multiply_10n(Number x,int n)
        {
            Number ans = new Number();
            for (int i = 0; i < n; i++)
                ans.self.Add(0);
            for (int i = 0; i < x.length; i++)
            {
                ans.self.Add(x.self[i]);
            }
            return ans;
        }

        public static Number operator +(Number x, int y)
        {
            Number ny = new Number(y);
            return x + ny;
        }
        public static Number operator +(Number x, Number y)
        {
            Number ans = new Number();
            bool carry = false;
            for (int i = 0; i < min(x.self.Count, y.self.Count); i++)
            {
                int temp;
                if (carry)
                    temp = x.self[i] + y.self[i] + 1;
                else
                    temp = x.self[i] + y.self[i];
                if (temp > 9)
                    carry = true;
                else
                    carry = false;
                ans.self.Add((sbyte)(temp % 10));
            }
            if (x.self.Count > y.self.Count)
            {
                for (int i = y.self.Count; i < x.self.Count; i++)
                {
                    int temp;
                    if (carry)
                        temp = x.self[i] + 1;
                    else
                        temp = x.self[i];
                    if (temp > 9)
                        carry = true;
                    else
                        carry = false;
                    ans.self.Add((sbyte)(temp % 10));
                }
            }
            else
            {
                for (int i = x.self.Count; i < y.self.Count; i++)
                {
                    int temp;
                    if (carry)
                        temp = y.self[i] + 1;
                    else
                        temp = y.self[i];
                    if (temp > 9)
                        carry = true;
                    else
                        carry = false;
                    ans.self.Add((sbyte)(temp % 10));
                }
            }
            if (carry)
                ans.self.Add(1);
            return ans;
        }
        public static Number operator *(Number x, int y)
        {
            Number ny = new Number(y);
            return x * ny;
        }
        public static Number operator *(Number x, Number y)
        {
            Number ans = new Number();
            if (x.length == 0 || y.length == 0)
                return ans;
            for (int i = 0; i < y.length; i++)
            {
                Number temp = new Number();
                temp = Multiply_s(x, y.self[i]);
                temp = Multiply_10n(temp, i);
                ans = ans + temp;
            }
            return ans;
        }
        public static Number operator -(Number x, int y)
        {
            Number ny = new Number(y);
            return x - ny;
        }
        public static Number operator -(Number x, Number y)
        {
            Number ans = new Number();
            sbyte carry = 0;
            if (x > y)
            {
                for (int i = 0; i < x.length; i++)
                {
                    sbyte temp = (sbyte)(x.self[i] - carry - ((i >= y.length) ? 0 : y.self[i]));
                    if (temp < 0)
                    {
                        carry = 1;
                        temp = (sbyte)(temp + 10);
                    }
                    else
                    {
                        carry = 0;
                    }
                    ans.self.Add(temp);
                }
                sbyte temp2 = ans.self[ans.length - 1];
                while (temp2 == 0)
                {
                    ans.self.RemoveAt(ans.length - 1);
                    temp2 = ans.self[ans.length - 1];
                }
            }
            return ans;
        }//若x<y 返回0
        public static Number operator /(Number x, int y)
        {
            Number ny = new Number(y);
            return x / ny;
        }
        public static Number operator /(Number x, Number y)
        {
            Number ans = new Number();
            if (x >= y)
            {
                int max_length = x.length - y.length + 1;
                Number remain = new Number(x);
                for (int i = max_length - 1; i >= 0; i--)
                {
                    Number basenum = new Number(Multiply_10n(y, i));
                    bool clear = false;
                    for (int j = 1; j < 10; j++)
                    {
                        Number now = new Number(Multiply_s(basenum,(sbyte)j));
                        Number next = new Number(Multiply_s(basenum,(sbyte)(j+1)));
                        if (now == remain)
                        {
                            clear = true;
                            ans.self.Insert(0, (sbyte)j);
                            for (int k = i; k > 0; k--)
                                ans.self.Insert(0, 0);
                            break;
                        }
                        else if((now < remain && (j == 9 || next > remain)))
                        {
                            ans.self.Insert(0, (sbyte)j);
                            remain.SetVal(remain - now);
                        }
                    }
                    if (clear)
                        break;
                }
            }
            return ans;
        }

        public static bool operator >(Number x, Number y)
        {
            if (x.length > y.length) return true;
            else if (x.length < y.length) return false;
            else
            {
                for (int i = 0; i < x.length; i++)
                {
                    if (x.self[x.length - i - 1] > y.self[x.length - i - 1]) return true;
                    else if (x.self[x.length - i - 1] < y.self[x.length - i - 1]) return false;
                }
            }
            return false;
        }
        public static bool operator <(Number x, Number y)
        {
            if (x == y || x > y) return false;
            return true;
        }
        public static bool operator >=(Number x, Number y)
        {
            if (x < y) return false;
            return true;
        }
        public static bool operator <=(Number x, Number y)
        {
            if (x > y) return false;
            return true;
        }
        public static bool operator ==(Number x, Number y)
        {
            if (x.length != y.length) return false;
            for (int i = 0; i < x.length; i++)
                if (x.self[i] != y.self[i]) return false;
            return true;
        }
        public static bool operator !=(Number x, Number y)
        {
            if (x == y)
                return false;
            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            return (this == (obj as Number));
        }

        public Number()
        {
        }
        public Number(int num)
        {
            string s = num.ToString();
            for (int i = 0; i < s.Length; i++)
            {
                self.Add((sbyte)(s[s.Length - i - 1] - '0'));
            }
        }
        public Number(Number nums)
        {
            self = new List<sbyte>(nums.self.ToArray());
        }
        public Number(string s)
        {
            for (int i = 0; i < s.Length; i++)
            {
                if (s[s.Length - i - 1] > '9' || s[s.Length - i - 1] < '0')
                    throw (new System.Exception("Wrong String"));
                self.Add((sbyte)(s[s.Length - i - 1] - '0'));
            }
        }
        public override string ToString()
        {
            if (length == 0)
                return "0";
            string s = "";
            for (int i = 0; i < length; i++)
            {
                s += self[length - i - 1].ToString();
            }
            return s;
        }
        public string ToStringWithComma()
        {
            if (self.Count == 0)
                return "0";
            string s = "";
            for (int i = 0; i < length; i++)
            {
                s += self[self.Count - i - 1].ToString();
                if ((length - i - 1) % 3 == 0 && i != length - 1)
                    s += ',';
            }
            return s;
        }
        public string ToStringWith5()
        {
            if (length < 6)
                return ToString();
            string s = "";
            for (int i = 0; i < 5; i++)
            {
                s += self[length - i - 1].ToString();
                if (i == 0)
                    s += ".";
            }
            s += " * 10^";
            s += (length - 1).ToString();
            return s;
        }
        public string ToStringWith(int len = 5)
        {
            if (length < len + 1)
                return ToString();
            string s = "";
            for (int i = 0; i < len; i++)
            {
                s += self[length - i - 1].ToString();
                if (i == 0)
                    s += ".";
            }
            s += " * 10^";
            s += (length - 1).ToString();
            return s;
        }
        public void SetVal(int num)
        {
            self.Clear();
            string s = num.ToString();
            for (int i = 0; i < s.Length; i++)
            {
                self.Add((sbyte)(s[s.Length - i - 1] - '0'));
            }
        }
        public void SetVal(Number nums)
        {
            self = new List<sbyte>(nums.self.ToArray());
        }
        public bool SetVal(string s)
        {
            self.Clear();
            for (int i = 0; i < s.Length; i++)
            {
                if (s[s.Length - i - 1] > '9' || s[s.Length - i - 1] < '0')
                    return false;
                self.Add((sbyte)(s[s.Length - i - 1] - '0'));
            }
            return true;
        }
    }
}