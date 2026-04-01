using CWCS.Common.Log;
using System;
using System.Windows.Forms;

namespace CWCS.UIControl
{
    public partial class FrmWCSDesigner : Form
    {
        public FrmWCSDesigner()
        {
            InitializeComponent();
        }
        private void FrmWCSDesigner_Load(object sender, EventArgs e)
        {
            LogHelper sclog = new LogHelper("SC");
            sclog.Info("开始测试...");

            sclog.Info("测试结束...");

            LogHelper converylog = new LogHelper("Convery");
            converylog.Info("堆垛机测试开始...");
            for (int i = 0; i < 1000; i++)
            {
                converylog.Info("输送机站台：{0},{1},{2}",new object[]{1000+i,"申请入库：","目标地址：1001"});
            }
            converylog.Info("堆垛机测试结束...");
           
        }

    }
}
