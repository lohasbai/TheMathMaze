using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SKSpecial;

namespace TheMathMaze
{
    class MazeMul
    {
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
