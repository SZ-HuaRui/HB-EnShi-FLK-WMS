using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CWCS.Entity.Convery
{
    public class ConveryEntity : EquipmentBase
    {
        #region 站台基础信息
        private string m_StationNo;//站台号

        public string StationNo
        {
            get { return m_StationNo; }
            set { m_StationNo = value; }
        }
        private int m_StationType;//站台类型

        public int StationType
        {
            get { return m_StationType; }
            set { m_StationType = value; }
        }
        private int m_LocationX;//X 坐标

        public int LocationX
        {
            get { return m_LocationX; }
            set { m_LocationX = value; }
        }
        private int m_LocationY;//Y 坐标

        public int LocationY
        {
            get { return m_LocationY; }
            set { m_LocationY = value; }
        }
        private int m_Width;//宽度

        public int Width
        {
            get { return m_Width; }
            set { m_Width = value; }
        }
        private int m_Height;//高度

        public int Height
        {
            get { return m_Height; }
            set { m_Height = value; }
        }
        public string config;
        public string field3;
        public int value;// DB值
        public int length;//DB长度
        private string m_Key_ip;//IP地址

        public string Key_ip
        {
            get { return m_Key_ip; }
            set { m_Key_ip = value; }
        }
        private int m_DBAddress;//DB块

        public int DBAddress
        {
            get { return m_DBAddress; }
            set { m_DBAddress = value; }
        }
        private bool m_cFlag;//连接标识

        public bool CFlag
        {
            get { return m_cFlag; }
            set { m_cFlag = value; }
        }

        private string m_GroupType;//所属组

        public string GroupType
        {
            get { return m_GroupType; }
            set { m_GroupType = value; }
        }
        private string m_Belong;

        public string Belong
        {
            get { return m_Belong; }
            set { m_Belong = value; }
        }

        public string AreaCode
        {
            get { return m_AreaCode; }
            set { m_AreaCode = value; }
        }
        private string m_AreaCode;
        #endregion

        #region 站台任务信息
        ConveryTaskInfo _c_TaskInfo;

        public ConveryTaskInfo C_TaskInfo
        {
            get { return _c_TaskInfo; }
            set
            {
                if (_c_TaskInfo == null)
                {
                    _c_TaskInfo = new ConveryTaskInfo();
                }
                _c_TaskInfo = value;
            }
        }
        #endregion
    }
}
