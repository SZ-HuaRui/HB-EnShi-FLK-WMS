using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CWCS.Entity.SRM
{
    public enum SRMCommand:int
    {
        None=99,

        #region 清除指令
        CR_NO_FUNC = 0,
        #endregion

        PutAndGet = 1,//取放货指令

        #region 基础指令
        CR_POS_GET = 16,//移动
        CR_GET = 32,//取货指令
        CR_POS_PUT = 48,//移动
        CR_PUT = 64,//放货指令
        #endregion

        #region 半循环指令
        CR_GETC = 112,//取货指令
        CR_PUTC=128,//放货指令
        #endregion

        CR_DROPSH_GETC = 368,//0x0170
        CR_PICKSH_GETC = 624,//0x0270

        CR_DROPSH_PUTC = 384,//0x0180
        CR_PICKSH_PUTC = 640,//0x0280

        CR_DROPSH_PARK = 400,//0x0190
        CR_PICKSH_PARK = 656,//0x0290

        CR_POS_COUNT = 96//盘点指令

    }

    public enum SCTaskMode
    {
        All = 0,//出入库
        OnlyInFeed = 1,//仅入库
        OnlyOutFeed = 2,//仅出库,
        Pause = 3,//任务暂停(原因：1.发生火警；2.手动暂停)
    }

    public enum STACommand
    {
        ClearToStation = 1,
        ReWrite = 2
    }

    public enum SCTaskType
    {
        None = 0,
        Task_InBound = 1,
        Task_OutBound = 2,
        Task_Transfer = 3,
        Task_CountAuto = 4,
        Task_DropShuttle = 5,
        Task_PickShuttle = 6
    }

}
