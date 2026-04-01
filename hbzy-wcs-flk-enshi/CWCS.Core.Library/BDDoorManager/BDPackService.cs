using System;
using System.Text;

namespace CWCS.Core.Library.BDManager
{
    public class BDPackServic
    {
        private int g_nDatagramCounter;//发送报文统计

        private readonly object obj_Lock = new object();

        public int DatagramCounter { get => g_nDatagramCounter; set => g_nDatagramCounter = value; }

        public static string ToExceptionMessageString(Exception exception)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("【异常消息】：" + exception.Message);
            stringBuilder.AppendLine("【内部异常】：" + exception.InnerException?.Message);
            stringBuilder.AppendLine("【堆栈消息】：" + exception.StackTrace);
            return stringBuilder.ToString();
        }
        /// <summary>
        /// 生成消息号
        /// </summary>
        public void AddHeartSequenceNumber()
        {
            lock (obj_Lock)
            {
                g_nDatagramCounter++;
                if (g_nDatagramCounter == 10000)
                    g_nDatagramCounter = 1;
            }
        }
        /// <summary>
        /// 组装发送报文
        /// </summary>
        /// <param name="Funcode">功能码</param>
        /// <param name="Text">命令内容</param>
        /// <param name="BDCount">计数器</param>
        /// <returns></returns>
        public OperateResult<byte[]> PackSendData(byte Funcode, byte cmdText)
        {
            if (Funcode == 0) return OperateResult.CreateFailResult<byte[]>("不需要反馈");
            var header = GetHeaderSenderData(Funcode);
            var operateResult = new OperateResult<byte[]>();
            try
            {
                header[8] = cmdText;
                header[9] = 0;

                AddHeartSequenceNumber();

                header[10] = (byte)(g_nDatagramCounter % 256);
                header[11] = (byte)(g_nDatagramCounter / 256);
                operateResult.IsSuccess = true;
                operateResult.Content = header;
            }
            catch (Exception ex)
            {
                operateResult.IsSuccess = false;
                operateResult.Description = $"WCS->BD:AK:异常，异常信息：{ToExceptionMessageString(ex)}";
            }
            return operateResult;
        }
        private byte[] GetHeaderSenderData(byte Funcode)
        {
            byte bufferLength = 12;
            byte[] bytes = new byte[bufferLength];
            bytes[0] = bufferLength;
            bytes[1] = 0;
            bytes[2] = 0x53;

            bytes[3] = Funcode;
            bytes[4] = 0;
            bytes[5] = 3;
            bytes[6] = 2;

            bytes[7] = 0x05;//0x05代表双字

            return bytes;
        }
        public OperateResult<byte[]> PackSendData(byte Funcode, int? BDCount = null)
        {
            var header = GetHeaderSenderData(Funcode);
            var operateResult = new OperateResult<byte[]>();
            try
            {
                if (BDCount is null)
                {
                    AddHeartSequenceNumber();
                    BDCount = g_nDatagramCounter;
                }
                var countByte = BitConverter.GetBytes(BDCount.Value);
                header[8] = countByte[0];
                header[9] = countByte[1];
                header[10] = countByte[2];
                header[11] = countByte[3];
                operateResult.IsSuccess = true;
                operateResult.Content = header;
            }
            catch (Exception ex)
            {
                operateResult.IsSuccess = false;
                operateResult.Description = $"WCS->BD:AK:异常，异常信息：{ToExceptionMessageString(ex)}";
            }
            return operateResult;
        }

        /// <summary>
        /// 读取BD材料数量
        /// </summary>
        /// <param name="StationNo"></param>
        /// <returns></returns>

        public OperateResult<byte[]> SendBDbufferInfo(BDDoorType StationNo)
        {
            OperateResult<byte[]> result = new OperateResult<byte[]>();
            byte[] bytes = new byte[73];
            bytes[0] = 0x49;
            bytes[1] = 0;
            bytes[2] = 0x11;//Frame type ‘Read Data’
            bytes[3] = 2;
            bytes[4] = 3;
            bytes[5] = (byte)'B';
            bytes[6] = (byte)'D';
            bytes[7] = (byte)'t';
            bytes[8] = (byte)'o';
            bytes[9] = (byte)'W';
            bytes[10] = (byte)'M';
            bytes[11] = (byte)'S';
            bytes[12] = (byte)'D';
            bytes[13] = (byte)'a';
            bytes[14] = (byte)'t';
            bytes[15] = (byte)'a';
            bytes[16] = (byte)'b';
            bytes[17] = (byte)'u';
            bytes[18] = (byte)'f';
            bytes[19] = (byte)'f';
            bytes[20] = (byte)'e';
            bytes[21] = (byte)'r';
            bytes[22] = (byte)StationNo;
            for (int i = 23; i < 69; i++)
            {
                bytes[i] = 0x20;
            }
            bytes[69] = 0;
            bytes[70] = 0;
            bytes[71] = 83;
            bytes[72] = 0;
            result.IsSuccess = true;
            result.Content = bytes;
            return result;
        }
        /// <summary>
        /// 解析BD材料数量
        /// </summary>
        /// <param name="readedBytes"></param>
        /// <returns></returns>
        public OperateResult<Tuple<BDDoorType, float, float>> GetReadResult(byte[] readedBytes)
        {
            if (readedBytes.Length > 84)
            {
                var door = (BDDoorType)readedBytes[22];//几号门
                var bufferNo = BitConverter.ToSingle(readedBytes, 77);//缓存区盘数zhf
                var itemNum = BitConverter.ToSingle(readedBytes, 81);//盘数zhf
                var paper = readedBytes[85];//1卷纸， 2盘纸
                return OperateResult.SuccessResult(Tuple.Create(door, bufferNo, itemNum));

            }
            else
            {
                return new OperateResult<Tuple<BDDoorType, float, float>> { IsSuccess = false };
            }
        }
    }
}
