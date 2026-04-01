using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DataGridViewAutoFilter;
using System.Drawing.Drawing2D;
using System.Web.UI.WebControls;
using CWCS.Repository;
using CWCS.Common.Uitl;

namespace CWCS.UI
{
    public partial class FrmGLBeventlog : Form
    {
        private int pageSize = 38;     //每页显示行数
        private int nMax = 0;         //总记录数
        private int pageCount = 1;    //页数＝总记录数/每页显示行数
        private int pageCurrent = 1;   //当前页号
        private int nCurrent = 0;      //当前记录行
        public BLL.ConveryBLL bll = new BLL.ConveryBLL();
        DataSet ds = new DataSet();
        DataTable dtInfo = new DataTable();
        private DateTime dts, dte;
        public FrmGLBeventlog()
        {
            InitializeComponent();
            C1.Win.C1FlexGrid.CellStyle s1 = gv_logmanager.Styles.Add("Red");
            s1.BackColor = Color.LightGreen;
            C1.Win.C1FlexGrid.CellStyle s2 = gv_logmanager.Styles.Add("Yellow");
            s2.BackColor = Color.Yellow;
            gv_logmanager.AfterDataRefresh += gv_logmanager_AfterDataRefresh;
        }

        void gv_logmanager_AfterDataRefresh(object sender, ListChangedEventArgs e)
        {
            for (int i = 1; i < gv_logmanager.Rows.Count; i++)
            {
                gv_logmanager.Rows[i]["Serial"] = i;
                if (gv_logmanager != null && gv_logmanager.Rows[i]["logtext"] != null && gv_logmanager.Rows[i]["logtext"].ToString().Equals("失败"))
                {
                    gv_logmanager.Rows[i].Style = gv_logmanager.Styles["Red"];
                }
            }
        }

        private void BindData(int load)
        {
            if (load == 1)
            {
                bool IsException = false;
                if (rb_error.Checked) IsException = true;
                dtInfo = null;
                OracleRepository repository = new OracleRepository();
                dtInfo = repository.GetEventLog(dtp_updatestart.Value.ToString(), dtp_updateend.Value.ToString(), txt_key.Text, txt_barcode.Text, txt_taskno.Text, IsException);

                if (dtInfo != null)
                {
                    nMax = dtInfo.Rows.Count;
                    pageCount = (nMax / pageSize);
                    if ((nMax % pageSize) > 0)
                    { pageCount++; }
                    pageCurrent = 1;
                    nCurrent = 0;
                }

            }
            if (dtInfo != null)
            {
                int nStartPos = 1;
                int nEndPos = 0;
                DataTable dtTemp = dtInfo.Clone();
                if (nMax > 0)
                {
                    if (pageCurrent == pageCount)
                    {
                        nEndPos = nMax;
                    }
                    else
                    {
                        nEndPos = pageSize * pageCurrent;
                    }
                    txtCurrentPage.Text = Convert.ToString(pageCurrent);
                }
                else
                {
                    nEndPos = 0;
                    txtCurrentPage.Text = "0";
                }
                nCurrent = pageSize * (pageCurrent - 1);
                nStartPos = nCurrent;
                lblPageCount.Text = "/ " + pageCount.ToString();

                //从元数据源复制记录行
                for (int i = nStartPos; i < nEndPos; i++)
                {
                    dtTemp.ImportRow(dtInfo.Rows[i]);
                    nCurrent++;
                }
                bindingNavigatorCountItem.Text = "/ " + nCurrent;

                bdsInfo.DataSource = dtTemp;
                gv_logmanager.AutoGenerateColumns = false;
                //bdnInfo.BindingSource = bdsInfo;
                gv_logmanager.DataSource = bdsInfo;
                gv_logmanager.Refresh();
            }
            else
            {
                gv_logmanager.DataSource = dtInfo;
            }
        }

        private void btn_search_Click(object sender, EventArgs e)
        {
            if (!DataCheckUtil.IsNumber(txt_barcode.Text.Trim())) { MessageBox.Show("输入的烟箱号不合法，请重新输入！"); return; }
            if (!DataCheckUtil.IsNumber(txt_taskno.Text.Trim())) { MessageBox.Show("输入的任务号不合法，请重新输入！"); return; }
            BindData(1);
        }

        private void formGLBeventlog_Load(object sender, EventArgs e)
        {
            dtp_updatestart.Value = DateTime.ParseExact("00:00:00", "HH:mm:ss", null);
            dtp_updateend.Value = DateTime.ParseExact("23:59:59", "HH:mm:ss", null);
        }



        private void dtp_updatestart_ValueChanged(object sender, EventArgs e)
        {
            if (Convert.ToDateTime(dtp_updatestart.Value.ToString()) > Convert.ToDateTime(dtp_updateend.Value.ToString()))
            {
                dtp_updatestart.Value = dts;
            }
            else
            {
                dts = dtp_updatestart.Value;
            }
        }

        private void dtp_updateend_ValueChanged(object sender, EventArgs e)
        {
            if (Convert.ToDateTime(dtp_updateend.Value.ToString()) < Convert.ToDateTime(dtp_updatestart.Value.ToString()))
            {
                dtp_updateend.Value = dte;
            }
            else
            {
                dte = dtp_updateend.Value;
            }
        }

        private void bdnInfo_ItemClicked_1(object sender, ToolStripItemClickedEventArgs e)
        {
            try
            {
                if (pageCount > 1)
                {
                    if (e.ClickedItem.Text == "上一页")
                    {
                        pageCurrent--;
                        if (pageCurrent <= 0)
                        {
                            pageCurrent++;
                            return;
                        }
                        else
                        {

                            BindData(0);
                        }
                    }
                    if (e.ClickedItem.Text == "下一页")
                    {
                        pageCurrent++;
                        if (pageCurrent > pageCount)
                        {
                            pageCurrent--;
                            return;
                        }
                        else
                        {
                            BindData(0);
                        }
                    }
                    if (e.ClickedItem.Text == "首页")
                    {
                        pageCurrent = 1;

                        BindData(0);
                    }
                    if (e.ClickedItem.Text == "末页")
                    {
                        pageCurrent = pageCount;
                        BindData(0);
                    }
                }
                if (e.ClickedItem.Text == "刷新")
                {
                    BindData(1);
                }
            }
            catch { }
        }


        private void gv_logmanager_MouseClick(object sender, MouseEventArgs e)
        {
            if (gv_logmanager.Rows.Selected.Count > 0 && gv_logmanager.DataSource != null)
            {
                DataRow row = ((DataRowView)gv_logmanager.Rows[gv_logmanager.Row].DataSource).Row;
                this.rtb_msg.Text = string.Format("{0}  {1}", row["createtime"].ToString(), row["logtext"].ToString());
            }
        }
    }
}