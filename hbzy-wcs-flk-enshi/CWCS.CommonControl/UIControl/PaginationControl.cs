using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace CWCS.CommonControl.UIControl
{
    /// <summary>
    /// Ahthor:zpw
    /// Description:Custom Paging Control
    /// </summary>
    public partial class PaginationControl : UserControl
    {
        private int currentPageIndex = 1;
        private int totalCount = 0;
        private int pageSize = 10;
        private int totalPageCount;
        
        [Description("响应页面点击")]
        [Browsable(true)]
        public event EventHandler OnPageChanged;
      
        public int PageSize
        {
            get
            {
                return pageSize;
            }
            private set
            {

            }
        }

        public int CurrentPageIndex
        {
            get
            {
                return currentPageIndex;
            }
            set
            {
                currentPageIndex = value;
                this.lblPageIndex.Text = currentPageIndex.ToString();
                ChangeButtonEnabled();
            }
        }

        public int TotalCount
        {
            get
            {
                return totalCount;
            }
            set
            {
                totalCount = value;
                totalPageCount = totalCount % pageSize == 0 ? totalCount / pageSize : totalCount / pageSize + 1;
                this.lblTotalCount.Text = this.totalCount.ToString();
                this.lblTotalPageCount.Text = totalPageCount.ToString();
            }
        }

        public int TotalPageCount
        {
            get
            {
                totalPageCount = totalCount % pageSize == 0 ? totalCount / pageSize : totalCount / pageSize + 1;
                return totalPageCount;
            }
            private set
            {
                totalPageCount = value;
                this.lblTotalPageCount.Text = totalPageCount.ToString();
            }
        }
        public PaginationControl()
        {
            
            InitializeComponent();
            InitPageSize();
        }


        public void InitPageSize()
        {
            this.cmbPageSize.DataSource = new string[] { "10", "20", "30", "40", "50" };
            this.cmbPageSize.SelectedIndex = 2;
            this.PageSize = Convert.ToInt32(this.cmbPageSize.Text);
        }
        

        private void ChangeButtonEnabled()
        {
            lblTextInit();
            if (CurrentPageIndex == TotalPageCount)
            {
                this.lblLast.Enabled = false;
                this.lblNext.Enabled = false;
            }
            if (CurrentPageIndex == 1)
            {
                this.lblFirst.Enabled = false;
                this.lblPrevious.Enabled = false;
            }
            if (TotalPageCount == 0)
            {
                this.lblFirst.Enabled = false;
                this.lblPrevious.Enabled = false;
                this.lblLast.Enabled = false;
                this.lblNext.Enabled = false;
            }
        }

        private void lblFirst_Click(object sender, EventArgs e)
        {
            this.CurrentPageIndex = 1;
            ChangeButtonEnabled();
            if (OnPageChanged != null)
            {
                OnPageChanged(sender,e);
            }
        }

        private void lblPrevious_Click(object sender, EventArgs e)
        {
            if (this.CurrentPageIndex == 1)
            {
                return;
            }
            this.CurrentPageIndex = --this.CurrentPageIndex;
            ChangeButtonEnabled();
            if (OnPageChanged != null)
            {
                OnPageChanged(sender, e);
            }

        }

        private void lblNext_Click(object sender, EventArgs e)
        {
            if (this.CurrentPageIndex == this.TotalPageCount)
            {
                return;
            }
            this.CurrentPageIndex = ++this.CurrentPageIndex;
            ChangeButtonEnabled();
            if (OnPageChanged != null)
            {
                OnPageChanged(sender, e);
            }
        }

        private void lblLast_Click(object sender, EventArgs e)
        {
          
            this.CurrentPageIndex = TotalPageCount;
            ChangeButtonEnabled();
            if (OnPageChanged != null)
            {
                OnPageChanged(sender, e);
            }
        }

       

        public void lblTextInit()
        {
            this.lblPrevious.Enabled = true;
            this.lblNext.Enabled = true;
            this.lblFirst.Enabled = true;
            this.lblLast.Enabled = true;
        }

        private void lblGO_Click(object sender, EventArgs e)
        {
            try
            {

                if (!Regex.IsMatch(this.txtSkip.Text.Trim(), "^[0-9]*$"))
                {
                    MessageBox.Show("请输入合法的页码！","提示");
                    return;
                }
                if (this.txtSkip.Text.Trim() == "0" || Convert.ToInt32(this.txtSkip.Text.Trim()) > TotalPageCount)
                {
                    MessageBox.Show("输入的页码不在范围内！", "提示");
                    return;
                }
                this.CurrentPageIndex = Convert.ToInt32(this.txtSkip.Text.Trim());
                ChangeButtonEnabled();
                if (OnPageChanged != null)
                {
                    OnPageChanged(sender, e);
                }
            }
            catch
            {
                return;
            }
            
        }
        public void lbl_CursorEvent(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }
       

        private void cmbPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.pageSize = Convert.ToInt32(this.cmbPageSize.Text);
            this.CurrentPageIndex = 1;
            this.TotalPageCount = this.TotalPageCount;
            ChangeButtonEnabled();
            if (OnPageChanged != null)
            {
                OnPageChanged(sender, e);
            }
        }
    }
}
