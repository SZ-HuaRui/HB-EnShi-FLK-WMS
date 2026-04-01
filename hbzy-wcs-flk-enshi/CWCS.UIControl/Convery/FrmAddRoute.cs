using CWCS.BLL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CWCS.UIControl.Convery
{
    public partial class FrmAddRoute : C1.Win.C1Ribbon.C1RibbonForm
    {
        ConveryBLL bll = new ConveryBLL();
        public FrmAddRoute()
        {
            InitializeComponent();
        }

        private void btnGeneral_Click(object sender, EventArgs e)
        {
            string fromstation = txtFromNum.Text.Trim();
            string tostation = txtEndNum.Text.Trim();
            string type = cmbType.SelectedValue.ToString();
            string equipment = cmbEquipment.SelectedValue.ToString();

            if (string.IsNullOrEmpty(fromstation))
            {
                MessageBox.Show("请输入起始站台");
                return;
            }
            if (string.IsNullOrEmpty(tostation))
            {
                MessageBox.Show("请输入目标站台");
                return;
            }
            if (string.IsNullOrEmpty(type))
            {
                MessageBox.Show("请选择类型");
                return;
            }
            if (string.IsNullOrEmpty(equipment))
            {
                MessageBox.Show("请选择巷道");
                return;
            }
        }
        private void FrmAddRoute_Load(object sender, EventArgs e)
        {
            InitData();
        }

        private void InitData()
        {
            //DataTable dt =  bll.GetEquipment("Equipment_AS", "to_number(substr(equipmentnumber,instr(equipmentnumber,'SC',1)+2,2))");
            //cmbEquipment.DataSource = dt;
            //cmbEquipment.DisplayMember = "equipmentnumber";
            //cmbEquipment.ValueMember = "equipmentnumber";

            //DataTable dtType = new DataTable();
            //dtType.Columns.Add("TypeCode");
            //dtType.Columns.Add("TypeDesc");

            //for (int i = 0; i < 2; i++)
            //{
            //    DataRow dr = dtType.NewRow();

            //    if (i == 0)
            //    {
            //        dr["TypeCode"] = "Task_InBound";
            //        dr["TypeDesc"] = "入库";

            //    }
            //    else
            //    {
            //        dr["TypeCode"] = "Task_OutBound";
            //        dr["TypeDesc"] = "出库";
            //    }
            //    dtType.Rows.Add(dr);
            //}

            //cmbType.DataSource = dtType;
            //cmbType.DisplayMember = "TypeDesc";
            //cmbType.ValueMember = "TypeCode";
        }
    }
}
