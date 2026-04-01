using C1.Win.C1FlexGrid;
using C1.Win.C1Ribbon;
using CWCS.UIDesigner.SRMControl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CWCS.UIControl
{
    public partial class FrmSRMDesigner : C1RibbonForm
    {
        DataTable dtSource = new DataTable();
        BLL.SRMBLL bll = new BLL.SRMBLL();
        public FrmSRMDesigner()
        {
            InitializeComponent();
        }

        private void FrmSCDesigner_Load(object sender, EventArgs e)
        {
           
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("EquipmentNum", "设备编号");
            dic.Add("FACTORY", "厂家");
            dic.Add("equipmenttype", "设备类型");
            dic.Add("PROTOCOLTYPE", "协议类型");
            dic.Add("RIPADDR", "读取IP");
            dic.Add("RPORT", "读取端口");

            dic.Add("RDBADDR", "读取DB块");
            dic.Add("RDBLENGTH", "读取长度");

            dic.Add("WIPADDR", "写入IP");
            dic.Add("WPORT", "写入端口");


            dic.Add("WDBRDBADDR", "写入DB块");
            dic.Add("WDBLENGTH", "写入DB长度");

            dic.Add("PICKPOINT", "取货点");
            dic.Add("PUTPOINT", "放货点");
            dtSource = null;//bll.GetWCS_Equipment();
            if (dtSource.Rows.Count <= 0)
            {
                dtSource = MakeDateTable("WCS_EquipmentInfo", dic);
            }
            else
            {
                dtSource.PrimaryKey = new DataColumn[] { dtSource.Columns["EquipmentNum"] };
            }
            grdSendLog.DataSource = dtSource;

        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            if (dtSource.Select("EquipmentNum='" + txtEquipNum.Text.Trim() + "'").Count() <= 0)
            {
                DataRow dr = dtSource.NewRow();
                dr["EquipmentNum"] = txtEquipNum.Text.Trim();
                dr["FACTORY"] = "NTI";
                dr["equipmenttype"] = "SC";
                dr["PROTOCOLTYPE"] = "PLC";
                dr["RIPADDR"] = txtReadIP.Text.Trim();
                dr["RPORT"] =  txtReadPort.Text.Trim();
                dr["WIPADDR"] = txtWriteIP.Text.Trim();
                dr["WPORT"] = txtWritePort.Text.Trim();

                dr["RDBADDR"] = txtRDBAddr.Text.Trim();
                dr["RDBLENGTH"] = txtRDBLength.Text.Trim();


                dr["WDBRDBADDR"] = txtWDBAddr.Text.Trim();
                dr["WDBLENGTH"] = txtWDBLength.Text.Trim();

                dr["PICKPOINT"] = txtPickPoint.Text.Trim();
                dr["PUTPOINT"] = txtPutPoint.Text.Trim();

                dtSource.Rows.Add(dr);
                SortSerial();
                this.grdSendLog.Refresh();
            }
            else
            {
                MessageBox.Show("已经存在：" + txtEquipNum.Text.Trim());
            }
        }



        private void btnModity_Click(object sender, EventArgs e)
        {
            if (grdSendLog.Rows.Selected.Count > 0)
            {
                DataRow dr = ((DataRowView)grdSendLog.Rows[grdSendLog.Row].DataSource).Row;
                dr["EquipmentNum"] = txtEquipNum.Text.Trim();
                dr["FACTORY"] = "NTI";
                dr["PROTOCOLTYPE"] = "PLC";
                dr["equipmenttype"] = "SC";
                dr["RIPADDR"] = txtReadIP.Text.Trim();
                dr["RPORT"] = txtReadPort.Text.Trim();
                dr["WIPADDR"] = txtWriteIP.Text.Trim();
                dr["WPORT"] = txtWritePort.Text.Trim();

                dr["RDBADDR"] = txtRDBAddr.Text.Trim();
                dr["RDBLENGTH"] = txtRDBLength.Text.Trim();


                dr["WDBRDBADDR"] = txtWDBAddr.Text.Trim();
                dr["WDBLENGTH"] = txtWDBLength.Text.Trim();

                dr["PICKPOINT"] = txtPickPoint.Text.Trim();
                dr["PUTPOINT"] = txtPutPoint.Text.Trim();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (grdSendLog.Rows.Selected.Count > 0)
            {
                DataRow row = ((DataRowView)grdSendLog.Rows[grdSendLog.Row].DataSource).Row;
                dtSource.Rows.Remove(row);
                LoadData();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //if (dtSource.Rows.Count > 0)
            //{
            //    string errro="";

            //    if (bll.InsertEquipmentData(dtSource, ref errro))
            //    {
            //        MessageBox.Show("保存成功!");
            //    }
            //    else
            //    {
            //        MessageBox.Show("保存失败，原因如下："+errro);
            //    }
            //}
        }

        private DataTable MakeDateTable(string tableName, Dictionary<string, string> dic)
        {
            DataTable dataTable = new DataTable();
            dataTable.TableName = tableName;

            foreach (string key in dic.Keys)
            {
                DataColumn taskColumn2 = new DataColumn();
                taskColumn2.DataType = System.Type.GetType("System.String");
                taskColumn2.ColumnName = key;
                taskColumn2.Caption = dic[key];
                taskColumn2.MaxLength = 100;
                dataTable.Columns.Add(taskColumn2);
                if (key == "EquipmentNum")
                {
                    dataTable.PrimaryKey=new DataColumn[]{taskColumn2};
                    
                }
            }

            return dataTable;
        }

        private void LoadData()
        {
            this.SetRowData(dtSource);
            SortSerial();
            this.grdSendLog.Refresh();
        }
        private void SetRowData(DataTable dt)
        {
            foreach (DataRow dataRow in dt.Rows)
            {
                Row row = this.grdSendLog.Rows.Add();
                row.UserData = dataRow;
                if (this.grdSendLog.Cols.Contains("Serial"))
                {
                    row[row.Index] = "Serial";
                }
                foreach (DataColumn dataColumn in dt.Columns)
                {
                    if (this.grdSendLog.Cols.Contains(dataColumn.Caption.Trim()))
                    {
                        row[dataColumn.Caption] = dataRow[dataColumn.Caption];
                    }
                    else
                    {
                        row[dataColumn.ColumnName] = dataRow[dataColumn.ColumnName];
                    }
                }
            }
            if (this.grdSendLog.Rows.Count - 1 > 0)
                this.grdSendLog.Rows[this.grdSendLog.Rows.Count - 1].Selected = true;
        }

        public void SortSerial()
        {
            try
            {
                for (int i = 1; i < grdSendLog.Rows.Count; i++)
                {
                    grdSendLog.Rows[i]["Serial"] = i;
                }
            }
            catch
            {
            }
        }

        private void grdSendLog_MouseClick(object sender, MouseEventArgs e)
        {
            if (this.grdSendLog.RowSel > 0)
            {
                DataRow row = ((DataRowView)grdSendLog.Rows[grdSendLog.Row].DataSource).Row;

                txtEquipNum.Text = row["EquipmentNum"].ToString().Trim();
                cmbProrocol.Text = row["PROTOCOLTYPE"].ToString().Trim();
                txtReadIP.Text = row["RIPADDR"].ToString().Trim();

                txtReadPort.Text = row["RPORT"].ToString().Trim();

                txtWriteIP.Text = row["WIPADDR"].ToString().Trim();

                txtWritePort.Text = row["WPORT"].ToString().Trim();

                txtRDBAddr.Text = row["RDBADDR"].ToString().Trim();

                txtRDBLength.Text = row["RDBLENGTH"].ToString().Trim();

                txtWDBAddr.Text = row["WDBRDBADDR"].ToString().Trim();


                txtWDBLength.Text = row["WDBLENGTH"].ToString().Trim();

                txtPickPoint.Text = row["PICKPOINT"].ToString().Trim();

                txtPutPoint.Text = row["PUTPOINT"].ToString().Trim();
            }
        }
    }
}
