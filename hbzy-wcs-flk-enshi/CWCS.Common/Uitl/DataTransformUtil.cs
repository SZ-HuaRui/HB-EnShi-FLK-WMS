using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CWCS.Common.Uitl
{
    /// <summary>
    /// Auhtor:zpw
    /// Description:General DataType Transform Utils
    /// </summary>
    public class DataTransformUtil
    {
        /// <summary>
        /// 两字节数组合并成一个字节数组
        /// </summary>
        /// <param name="byte1"></param>
        /// <param name="byte2"></param>
        /// <returns></returns>
        public static byte[] CombineBytes(byte[] byte1, byte[] byte2)
        {
            byte[] result = new byte[byte1.Length + byte2.Length];
            byte1.CopyTo(result, 0);
            byte2.CopyTo(result, byte1.Length);
            return result;
        }

        /// <summary>
        /// 两字节转Int
        /// </summary>
        /// <param name="byteHigh">高字节</param>
        /// <param name="byteLow">低字节</param>
        /// <returns></returns>
        public static int TwoByteToInt(byte byteHigh, byte byteLow)
        {
            return byteHigh * 256 + byteLow;
        }

        /// <summary>
        /// 四字节转Int
        /// </summary>
        /// <param name="byte1">第一高字节</param>
        /// <param name="byte2">第二高字节</param>
        /// <param name="byte3">第三低字节</param>
        /// <param name="byte4">第四低字节</param>
        /// <returns></returns>
        public static int FourByteToInt(byte byte1, byte byte2, byte byte3, byte byte4)
        {
            return BitConverter.ToInt32(new byte[4] { byte4, byte3, byte2, byte1 }, 0);
        }

        /// <summary>
        /// int转为转化为2字节数组
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static byte[] IntToTwoByte(int param)
        {
            byte[] byteArray = new byte[2];
            string binaryString = Convert.ToString(param, 2).PadLeft(16, '0');
            byteArray[0] = Convert.ToByte(binaryString.Substring(0, 8), 2);
            byteArray[1] = Convert.ToByte(binaryString.Substring(8, 8), 2);
            return byteArray;
        }

        /// <summary>
        /// string类型数值转为转化为2字节数组
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static byte[] StringToTwoByte(string param)
        {
            try
            {
                byte[] Buffer = new byte[2];
                int i = int.Parse(param);
                Buffer[0] = (byte)(i / 256);
                Buffer[1] = (byte)(i % 256);
                return Buffer;
            }
            catch (Exception)
            { 
                return null; 
            }
        }


        /// <summary>
        /// int转化为4字节数组
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static byte[] IntToFourByte(int param)
        {
            byte[] byteArray = new byte[4];
            string binaryString = Convert.ToString(param, 2).PadLeft(32, '0');
            byteArray[0] = Convert.ToByte(binaryString.Substring(0, 8), 2);
            byteArray[1] = Convert.ToByte(binaryString.Substring(8, 8), 2);
            byteArray[2] = Convert.ToByte(binaryString.Substring(16, 8), 2);
            byteArray[3] = Convert.ToByte(binaryString.Substring(24, 8), 2);
            return byteArray;
        }



        /// <summary>
        /// 去掉字节数组尾部的无效字节，返回有效字节数组
        /// </summary>
        /// <param name="byteArray"></param>
        /// <returns></returns>
        public static byte[] ByteArrayTrimNull(byte[] byteArray)
        {
            if (byteArray == null) return null;
            int i = byteArray.Length - 1;
            int copyLength = byteArray.Length;
            while (byteArray[i] == 0 || byteArray[i] == Convert.ToByte('0'))
            {
                --copyLength;
                if (i == 0) break;
                --i;
            }
            if (copyLength <= 0) return null;
            byte[] newArray = new byte[copyLength];
            Array.Copy(byteArray,newArray,copyLength);
            return newArray;
        }
    }
}
