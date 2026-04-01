using CWCS.Common.Log;
using CWCS.Common.Uitl;
using CWCS.Entity;
using CWCS.Interface.WMSMicroService;
using CWCS.Repository;
using DevExpress.XtraEditors.Filtering.Templates;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CWCS.UI.FL
{
    public partial class FormTest : Form
    {
        private OracleRepository _repository = new OracleRepository();
        public FormTest()
        {
            InitializeComponent();
        }

        private void btnCreateEntity_Click(object sender, EventArgs e)
        {
            try
            {
                SqlSugarClient dbContext = new SqlSugarClient(
                   new ConnectionConfig()
                   {
                       ConnectionString = ConfigurationManager.AppSettings["DBContext"].ToString(),
                       DbType = SqlSugar.DbType.Oracle,
                       IsAutoCloseConnection = true,
                       InitKeyType = InitKeyType.Attribute
                   }
               );
                dbContext.DbFirst.Where(x => x.ToLower().Equals("wcs_location") || x.ToLower().Equals("wcs_zone")).CreateClassFile(@"D:\GitWcsCode\src\CWCS.Entity\WCS", "CWCS.Entity");

                //指定表
                //dbContext.DbFirst.Where("wcs_station_status").CreateClassFile(@"D:\TCZ\所有项目\【Project】EnShiTobacco\FL\【Code】WCS\ES_WCS_FL_New\CWCS.Entity", "CWCS.Entity");

                //bool isEm =  DataCheckUtil.IsInteger("a012301");

                //_repository.DealTaskStatus(
                //    new WCS_SUBTASKINFO()
                //    {
                //        TASKNO = Convert.ToInt32(10750),
                //        EQUIPMENTTYPE = "AGV",
                //    },
                //    new WCS_SUBTASKINFO()
                //    {
                //        TASKSTATUS = "1"
                //    });

                // WCS_SUBTASKINFO t = new WCS_SUBTASKINFO()
                // {
                //     AGVINDEX = "21",
                //     AGVCARNO = "3",
                //     TASKSTATUS = "2",
                //     RUNSTATUS = "1",
                //     EQUIPMENTTYPE = "AGV"
                // };
                //WCS_SUBTASKINFO item = ObjectTransformUtil.Clone(t);
                //bool flag = _repository.IsExistSmallerTask("11517011");
                //string message = string.Empty;
                //for (int i=0;i< 10;i++)
                //{
                //    message = $"测试-{DateTime.Now}";
                //    LogUtil.Write(LogFileName.AgvLog, LogFileName.AgvLog, message);
                //}
               
            }
            catch (Exception ex)
            {

            }
           
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            string item = Encryptcs.EncryptByAES("Nti56.com");
           //DataTable  subTaskInfo = _repository.QueryTableData(new WCS_SUBTASKINFO()
           // {
           //     EQUIPMENTTYPE = "AGV"
           // });
           //subTaskInfo = subTaskInfo.AsEnumerable().OrderByDescending(x => x["create_date"]).CopyToDataTable();
        }
    }
}
