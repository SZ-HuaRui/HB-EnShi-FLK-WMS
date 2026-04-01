using System.Collections.Generic;
using CWCS.Core.Library.AGVManager;

public enum BDDoorType : byte
{
    /// <summary>
    /// 盘纸门 1 TIPPaper
    /// </summary>
    TP = 0x31,

    /// <summary>
    /// 卷纸门 2 CigarettePaper
    /// </summary>
    CP = 0x32
}

/// <summary>
/// 反馈过来的当前门状态
/// </summary>
public struct BDDoorStatus
{
    public static Dictionary<byte, string> WhichDoorDesc = new Dictionary<byte, string>
    {
        //门的状态 20 送货时，BD门已经打开，AGV可以放托盘  21取货时，BD门已经打开，AGV可以取走托盘
        { BDRecCMD.RecDoorJZ,"卷纸门"},
        { BDRecCMD.RecDoorPZ,"盘纸门"},
    };
    public static Dictionary<byte, string> DoorStatusDesc = new Dictionary<byte, string>
    {
        //门的状态 20 送货时，BD门已经打开，AGV可以放托盘  21取货时，BD门已经打开，AGV可以取走托盘
        { DoorOpenWhenPut,"送托盘时，BD门已经打开，AGV可以放托盘"},
        { DoorOpenWhenGet,"退托盘时，BD门已经打开，AGV可以取走托盘"},
    };

    /// <summary>
    /// 重置通讯状态
    /// </summary>
    public const byte InterfaceReset = 99;
    /// <summary>
    /// 放货时开门状态
    /// </summary>
    public const byte DoorOpenWhenPut = 20;
    /// <summary>
    /// 取货时开门状态
    /// </summary>
    public const byte DoorOpenWhenGet = 21;
}
/// <summary>
/// BD门命令编号：发送
/// </summary>
public struct BDSendCMD
{
    /// <summary>
    /// 卷纸门命令编号 30
    /// </summary>
    public const byte DoorJZ = 0x1E;
    /// <summary>
    /// 盘纸门命令编号 31
    /// </summary>
    public const byte DoorPZ = 0x1F;
    /// <summary>
    /// 主动发送时针对门的命令
    /// </summary>
    public static Dictionary<byte, byte> SendCMD = new Dictionary<byte, byte>
    {
        { BDRecCMD.Confirm,0},//确认
        { BDRecCMD.RecDoorJZ,DoorJZ},//左门
        { BDRecCMD.RecDoorPZ,DoorPZ}//右门
    };
}
/// <summary>
/// BD门命令编号：接收
/// </summary>
public struct BDRecCMD
{
    /// <summary>
    /// 确认命令编号
    /// </summary>
    public const byte Confirm = 0x55;
    /// <summary>
    /// 卷纸门命令编号
    /// </summary>
    public const byte RecDoorJZ = 0xF5;
    /// <summary>
    /// 盘纸门命令编号 32
    /// </summary>
    public const byte RecDoorPZ = 0xF7;

    private const byte ReplyDoorJZ = 0x20;
    /// <summary>
    /// 盘纸门命令编号 33
    /// </summary>
    private const byte ReplyDoorPZ = 0x21;
    /// <summary>
    /// 回复映射
    /// </summary>
    public static Dictionary<byte, byte> ReplyCMD = new Dictionary<byte, byte>
    {
        { Confirm,0},//确认
        { RecDoorJZ,ReplyDoorJZ},//左门
        { RecDoorPZ,ReplyDoorPZ}//右门
    };

}
public class WorkStation
{
    public string DeviceCode { get; set; }
    public byte WorkStationCode { get; set; }
    public byte DoorStatus { get; set; }
    public BDControl Context { get; set; }
}
/// <summary>
/// 发给BD门的
/// </summary>

public struct BDOPerationType
{
    #region AGV放货到BD通过WMS开关门
   
    /// <summary>
    /// AGV放货完成之后WCS通知BD关门
    /// </summary>
    //public const int WMSCloseDoorAfterPut = 40;

    ///// <summary>
    ///// AGV放货前到达门口请求开门
    ///// </summary>
    //public const byte OpenDoorBeforPut = 10;
    ///// <summary>
    ///// AGV取托盘前到达门口请求开门
    ///// </summary>
    //public const byte OpenDoorBeforGet = 11;
    ///// <summary>
    ///// AGV取托盘前到达门口请求关门
    ///// </summary>
    //public const int WMSCloseDoorAfterGet = 41;
    #endregion


    #region WCS-BD
    /// <summary>
    /// AGV取托盘前到达门口请求开门
    /// </summary>
    public const int WMSOpenDoorBeforGet = 12;
    /// <summary>
    /// AGV取托盘前到达门口请求关门
    /// </summary>
    public const byte CloseDoorAfterGet = 25;

    /// <summary>
    /// AGV放货前到达门口请求开门
    /// </summary>
    public const int WMSOpenDoorBeforPut = 13;

    /// <summary>
    /// AGV放货完成之后WCS通知BD关门
    /// </summary>
    public const byte CloseDoorAfterPut = 24;


    /// <summary>
    /// 1.初始化连接
    /// </summary>
    public const byte InitConnect = 27;

    /// <summary>
    /// 2.启用WMS控制
    /// </summary>
    public const byte WMSToBDOpera = 29;

    /// <summary>
    /// 3.心跳
    /// </summary>
    public const byte HeartBeat = 28;

    /// <summary>
    /// 复位卷纸
    /// </summary>
    public const byte ResetInterface1 = 30;
    /// <summary>
    /// 复位盘纸
    /// </summary>
    public const byte ResetInterface2 = 31;
    #endregion
}