using CWCS.Common.Uitl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWCS.NetWork
{
    public abstract class NetFixedCommand
    {
        public static byte[] InSortingHeartBeat
        {
            get
            {
                byte[] heartByte = new byte[80];
                heartByte[0] = 0xFC;
                heartByte[1] = 0xFC;
                heartByte[2] = Convert.ToByte('W');
                heartByte[3] = Convert.ToByte('1');
                heartByte[4] = Convert.ToByte('H');
                heartByte[5] = Convert.ToByte('B');
                heartByte[6] = DataTransformUtil.IntToTwoByte(80)[0];
                heartByte[7] = DataTransformUtil.IntToTwoByte(80)[1];
                heartByte[78] = 0xFD;
                heartByte[79] = 0xFD;
                return heartByte;
            }
            private set { }
        }


        public byte[] PackHeartBeat(byte[] recieveArray)
        {
            byte[] bigPacket = new byte[1];
            byte[] start = new byte[36];
            start[0] = 0xFE;
            start[1] = 0xFE;
            start[2] = 0xFE;
            start[3] = 0xFE;
            Random random = new Random();
            int packetSequnceNO = random.Next(1, 60000);
            //数据包序号
            Array.Copy(DataTransformUtil.IntToFourByte(packetSequnceNO), 0, start, 4, 4);
            //和校验值
            start[10] = 0;
            start[11] = 0;
            //ACK确认数据包序号
            start[12] = 0;
            start[13] = 0;
            start[14] = 0;
            start[15] = 0;
            //ACK状态
            start[16] = 0;
            start[17] = 0;
            //发送设备编号
            start[18] = 0;
            start[19] = 0;
            //接受设备编号
            start[20] = 0;
            start[21] = 0;
            //备用
            for (int i = 22; i <= 35; i++)
            {
                start[i] = 0;
            }
            int totalPacketLength = 840;
            Array.Copy(DataTransformUtil.IntToTwoByte(totalPacketLength), 0, start, 8, 2);
            bigPacket = new byte[totalPacketLength];
            Array.Copy(start, 0, bigPacket, 0, start.Length);
            Array.Copy(recieveArray, 0, bigPacket, start.Length, recieveArray.Length);
            for (int i = 1; i <= 4; i++)
            {
                bigPacket[bigPacket.Length - i] = 0xFF;
            }
            return bigPacket;
        }



    }
}
