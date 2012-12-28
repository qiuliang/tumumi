using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace TMM.Core.Utils
{
    public class MyRandom
    {
        /// <summary>
        /// 返回大于或等于零且小于 System.Int32.MaxValue 的 32 位带符号整数。
        /// </summary>
        /// <returns></returns>
        public static int Next()
        {
            Random rand = new Random();
            return rand.Next();
        }
        /// <summary>
        /// 返回大于或等于零且小于 maxValue 的 32 位带符号整数
        /// </summary>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        public static int Next(int maxValue)
        {
            Random rand = new Random();
            return rand.Next(maxValue);
        }
        /// <summary>
        /// 返回一个大于或等于 minValue 且小于 maxValue 的 32 位带符号整数。
        /// </summary>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        public static int Next(int minValue, int maxValue)
        {
            Random rand = new Random();
            return rand.Next(minValue, maxValue);
        }

        /// <summary>
        /// C#中随机生成指定长度的密码
        /// </summary>
        private static string MakePassword(int pwdLength)
        {
            //声明要返回的字符串
            string tmpstr = "";
            //密码中包含的字符数组
            string pwdchars = "abcdefghijklmnopqrstuvwxyz0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            //数组索引随机数
            int iRandNum;
            //随机数生成器
            Random rnd = new Random();
            for (int i = 0; i < pwdLength; i++)
            {
                //Random类的Next方法生成一个指定范围的随机数
                iRandNum = rnd.Next(pwdchars.Length);
                //tmpstr随机添加一个字符
                tmpstr += pwdchars[iRandNum];
            }
            return tmpstr;
        }

        /// <summary>
        /// 在区间[minValue,maxValue]取出num个互不相同的随机数，返回数组。 
        /// </summary>
        /// <param name="num"></param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        public int[] DifferSamenessRandomNum(int num, int minValue, int maxValue)
        {

            Random rnd = new Random(unchecked((int)DateTime.Now.Ticks));//保证产生的数字的随机性 

            int[] intArr = new int[num];

            ArrayList myList = new ArrayList();

            while (myList.Count < num)
            {
                int value = rnd.Next(minValue, maxValue);
                if (!myList.Contains(value))    // 这句是关键
                    myList.Add(value);
            }

            //  转换为整形数组
            for (int i = 0; i < num; i++)
                intArr[i] = (int)myList[i];

            ////  排序
            //for (int i = 0; i < intArr.Length-1; i++)
            //{
            //    for (int j = 0; j < intArr.Length-1; j++)
            //    {
            //        if(intArr[j] > intArr[j+1])
            //        {
            //            temp = intArr[j + 1];
            //            intArr[j + 1] = intArr[j];
            //            intArr[j] = temp;
            //        }
            //    }
            //}


            return intArr;

        }

    }

}
