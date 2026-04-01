using CWCS.BLL;
using CWCS.Entity;
using CWCS.Repository;
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
    public partial class FrmAddConvery : C1.Win.C1Ribbon.C1RibbonForm
    {
        public OracleRepository _repository = new OracleRepository();
        public FrmAddConvery()
        {
            InitializeComponent();
        }

        private void FrmAddConvery_Load(object sender, EventArgs e)
        {

        }

        private void btnGeneral_Click(object sender, EventArgs e)
        {
            string str_start = txtFromNum.Text.Trim();
            string str_end = txtEndNum.Text.Trim();
            string str_Db = txtDB.Text.Trim();
            string str_Length = txtLength.Text.Trim();
            string str_startValue = tb_startValue.Text.Trim();
            if (string.IsNullOrEmpty(str_start))
            {
                MessageBox.Show("起始站台为空！");
                return;
            }
            if (string.IsNullOrEmpty(str_end))
            {
                MessageBox.Show("目标站台为空！");
                return;
            }
            if (string.IsNullOrEmpty(str_Db))
            {
                MessageBox.Show("DB块为空！");
                return;
            }
            if (string.IsNullOrEmpty(str_Length))
            {
                MessageBox.Show("DB块单位长度为空！");
                return;
            }
            if (string.IsNullOrEmpty(str_startValue))
            {
                MessageBox.Show("DB块起始偏移量为空！");
                return;
            }
            List<WCS_OPCITEM> lst = new List<WCS_OPCITEM>();
            int x = 1;
            int y = 1;
            int width = 1;
            int height = 1;
            int a = 0;
            int dbOffset = Convert.ToInt32(str_startValue);
            for (int i = int.Parse(str_start); i <= int.Parse(str_end); i++)
            {
                WCS_OPCITEM entity = new WCS_OPCITEM();
                entity.Stationno = i.ToString();
                entity.Stationtype = "0";
                entity.Itemid = i.ToString();
                entity.Itemname = i.ToString();
                entity.Groupname = "Convery";
                entity.Userid = "127.0.0.1";
                entity.Objects = str_Db;
                entity.Signaltype = str_Length;
                entity.Value = dbOffset.ToString();
                entity.Status = "1";
                entity.ArrowDirection = "1";
                entity.Warehouseid = "1112599550767534080";
                entity.Locationx = x.ToString();
                entity.Locationy = (width + y).ToString();
                entity.Width = width.ToString();
                entity.Height = height.ToString();
                entity.Belong = (i / 10).ToString();
                entity.AreaCode = tb_areaCode.Text;
                entity.ZoneCode = tb_zoneCode.Text;
                if (x % 50 == 0)
                {
                    x -= 2;
                    entity.Height =width.ToString();
                    entity.Width = height.ToString();
                    y += 2;
                }
                else
                {
                    x += 2;
                }
                if (x <= 0)
                    x = 1;
                a++;
                lst.Add(entity);
                dbOffset = dbOffset + Convert.ToInt32(str_Length);
            }
            if (_repository.InsertManyStation(lst))
            {
                MessageBox.Show("批量添加成功！", "提示");
            }
            else
            {
                MessageBox.Show("批量添加失败！","提示");
            }
        }

        private void btnCancle_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
