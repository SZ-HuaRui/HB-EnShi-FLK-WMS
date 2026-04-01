using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CWCS.Entity;
using C1.Win.C1FlexGrid;
using CWCS.Entity.Robot;

namespace CWCS.UIDesigner.RobotControl
{
    public partial class BatteryControl :Form
    {
        private delegate void DataGridViewCallBack(C1FlexGrid grd, List<BatteryInfo> lst);
        public BatteryControl()
        {
            InitializeComponent();
         
        }
        List<BatteryInfo> lst;
        string rbnumber;
        string traycode;
        int ContainerCount=32;
        public BatteryControl(List<BatteryInfo> lst,string rbnumber,string traycode,int ContainerCount=32)
        {
            InitializeComponent();
            this.lst = lst;
            this.rbnumber = rbnumber;
            this.traycode = traycode;
            this.ContainerCount = ContainerCount;

        }

        private void GenerateDataTable(C1FlexGrid grd, List<BatteryInfo> lst)
        {
            if (grd.InvokeRequired)
            {
                DataGridViewCallBack grdcall = new DataGridViewCallBack(GenerateDataTable);
                this.Invoke(grdcall, new object[] { grd, lst });
            }
            else
            {
                try
                {
                    DataTable dtData = new DataTable();
                    dtData = MakeDateTable("dtBattery", "SerialNum", "序号", "BatteryCode", "电芯码", "BatteryResult", "结果");
                    int i = 1;
                    int count = ContainerCount / 2;
                    foreach (var item in lst)
                    {
                        if (i <= count)
                        {
                            if (dtData.Select("BatteryCode1='" + item.BatteryCode + "' and  SerialNum1='" + item.Position + "'").Count() <= 0)
                            {
                                DataRow dr = dtData.NewRow();
                                dr["SerialNum1"] = item.Position.ToString().Trim();
                                dr["BatteryCode1"] = item.BatteryCode.ToString().Trim();
                                dr["BatteryResult1"] = item.BatteryResult.ToString().Trim();
                                dtData.Rows.Add(dr);
                            }
                        }
                        else
                        {
                            dtData.Rows[i - count-1]["SerialNum2"] = item.Position;
                            dtData.Rows[i - count-1]["BatteryCode2"] = item.BatteryCode;
                            dtData.Rows[i - count-1]["BatteryResult2"] = item.BatteryResult;
                        }
                        i++;
                    }
                    grd.Rows.Count = grd.Rows.Fixed;
                    if (dtData.Rows.Count > 0)
                        SetRowData(grd, dtData);
                }
                catch
                {
                    throw;
                }
            }
        }

        private void SetRowData(C1FlexGrid grd, DataTable dt)
        {
            // int i = 0;
            try
            {

                foreach (DataRow dataRow in dt.Rows)
                {
                    Row row = grd.Rows.Add();
                    row.UserData = dataRow;

                    foreach (DataColumn dataColumn in dt.Columns)
                    {
                        if (grd.Cols.Contains(dataColumn.Caption.Trim()))
                        {
                            row[dataColumn.Caption] = dataRow[dataColumn.Caption];
                        }
                        else
                        {
                            if (grd.Cols.Contains(dataColumn.ColumnName))
                                row[dataColumn.ColumnName] = dataRow[dataColumn.ColumnName];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private DataTable MakeDateTable(string tableName, string colName1, string colCaption1, string colName2, string colCaption2, string colName3, string colCaption3)
        {
            DataTable dataTable = new DataTable();
            dataTable.TableName = tableName;

            DataColumn taskColumn1 = new DataColumn();
            taskColumn1.DataType = System.Type.GetType("System.String");
            taskColumn1.ColumnName = colName1 + "1";
            taskColumn1.Caption = colCaption1;
            taskColumn1.MaxLength = 5;

            dataTable.Columns.Add(taskColumn1);

            DataColumn taskColumn2 = new DataColumn();
            taskColumn2.DataType = System.Type.GetType("System.String");
            taskColumn2.ColumnName = colName2 + "1";
            taskColumn2.Caption = colCaption2;
            taskColumn2.MaxLength = 30;
            dataTable.Columns.Add(taskColumn2);



            DataColumn taskColumn3 = new DataColumn();
            taskColumn3.DataType = System.Type.GetType("System.String");
            taskColumn3.ColumnName = colName3 + "1";
            taskColumn3.Caption = colCaption3;
            dataTable.Columns.Add(taskColumn3);

            DataColumn taskColumn4 = new DataColumn();
            taskColumn4.DataType = System.Type.GetType("System.String");
            taskColumn4.ColumnName = colName1 + "2";
            taskColumn4.Caption = colCaption1;
            taskColumn4.MaxLength = 5;

            dataTable.Columns.Add(taskColumn4);

            DataColumn taskColumn5 = new DataColumn();
            taskColumn5.DataType = System.Type.GetType("System.String");
            taskColumn5.ColumnName = colName2 + "2";
            taskColumn5.Caption = colCaption2;
            taskColumn5.MaxLength = 30;
            dataTable.Columns.Add(taskColumn5);



            DataColumn taskColumn6 = new DataColumn();
            taskColumn6.DataType = System.Type.GetType("System.String");
            taskColumn6.ColumnName = colName3 + "2";
            taskColumn6.Caption = colCaption3;
            taskColumn6.MaxLength = 5;

            dataTable.Columns.Add(taskColumn6);

            return dataTable;
        }

        public void ShowData()
        {
            this.Text = "机械手："+rbnumber+"信息";
            if (lst != null)
            {
                if (lst.Count() > 0)
                {
                    GenerateDataTable(c1FlexGrid3, lst);
                }
                else
                {
                    MessageBox.Show("电芯数据为空");
                }
            }
            else
            {
                MessageBox.Show("电芯数据为NULL");
            }
        }

    }
}
