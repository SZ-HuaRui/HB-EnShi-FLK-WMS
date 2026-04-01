
using CWCS.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace CWCS.UI
{
    public partial class FrmOtherSetting : Form
    {
        public OracleRepository _repository = new OracleRepository();
        public FrmOtherSetting()
        {
            InitializeComponent();
            this.btnReflash.Click += new System.EventHandler(this.btnReflash_Click);
            this.btnSet.Click += new System.EventHandler(this.btnSet_Click);
            this.btnSmallReflash.Click += new System.EventHandler(this.btnReflash_Click);
            this.btnSmallSet.Click += new System.EventHandler(this.btnSet_Click);
        }

        private void btnReflash_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;
            DataTable dt = new DataTable();
            if (button.Name == "btnReflash")
            {
                dt = _repository.GetSTVCacheInfo(OracleRepository.STVCacheType.Cache_Large);
                this.textBoxTotal.Text = dt.Rows[0][0].ToString();
                this.textBoxLeft.Text = dt.Rows[0][1].ToString();
                return;
            }
            if (button.Name == "btnSmallReflash")
            {
                dt = _repository.GetSTVCacheInfo(OracleRepository.STVCacheType.Cache_Small);
                this.textBoxSmallTotal.Text = dt.Rows[0][0].ToString();
                this.textBoxSmallLeft.Text = dt.Rows[0][1].ToString();
                return;
            }
        
          
        }

        private void btnSet_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;
            DataSet ds = new DataSet();
            string total = string.Empty;
            string left = string.Empty;
            OracleRepository.STVCacheType cacheType = OracleRepository.STVCacheType.Cache_Large;
            if (button.Name == "btnSet")
            {
                total = this.textBoxTotal.Text;
                left = this.textBoxLeft.Text;
                cacheType = OracleRepository.STVCacheType.Cache_Large;
            }

            if (button.Name == "btnSmallSet")
            {
                total = this.textBoxSmallTotal.Text;
                left = this.textBoxSmallLeft.Text;
                cacheType = OracleRepository.STVCacheType.Cache_Small;
            }

            if (string.IsNullOrEmpty(total) || string.IsNullOrEmpty(left))
            {
                MessageBox.Show("填写的信息不完整", "提示",MessageBoxButtons.OK);
                return;
            }
            if (!Regex.IsMatch(total, @"^[0-9]+$"))
            {
                MessageBox.Show("缓存总量初始值不是数字！", "提示",  MessageBoxButtons.OK);
                return;
            }
            if (!Regex.IsMatch(left, @"^[0-9]+$"))
            {
                MessageBox.Show("剩余缓存不是数字！", "提示", MessageBoxButtons.OK);
                return;
            }
            if (Convert.ToInt32(total) < Convert.ToInt32(left))
            {
                MessageBox.Show("剩余缓存数不能大于缓存总量初始值！", "提示",  MessageBoxButtons.OK);
                return;
            }
            if (_repository.UpdateSTVCacheInit(cacheType, Convert.ToInt32(total), Convert.ToInt32(left)))
            {
                MessageBox.Show( "修改成功！\n修改缓存总量初始值为：" + total+"\n修改剩余缓存数为："+left, "提示", MessageBoxButtons.OK);
                return;
            }
            else
            {
                MessageBox.Show("修改失败！\n修改缓存总量初始值为：" + total + "\n修改剩余缓存数为：" + left, "提示",  MessageBoxButtons.OK);
                return;
            }
        }

    }
}
