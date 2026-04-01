using Microsoft.AspNetCore.Mvc;

namespace NtiCloud.Web.Areas.RFS.Controllers
{
    [Area("RFS")]
    [Route("RFS/[controller]/[action]")]

    public class MainController : Controller
    {
        public ActionResult Login()
        {
            return View();
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Receive()
        {
            return View();
        }
        public ActionResult ReceiptFL()
        {
            return View();
        }
        public ActionResult ReceiptZP()
        {
            return View();
        }
        public ActionResult PositionAdjust()
        {
            return View();
        }
        public ActionResult PositionAdjust2()
        {
            return View();
        }
        public ActionResult Shelve()
        {
            return View();
        }
        public ActionResult OutShelve()
        {
            return View();
        }
        public ActionResult StorageOutShelve()
        {
            return View();
        }
        public ActionResult CodePlate()
        {
            return View();
        }
        public ActionResult UncodePlate()
        {
            return View();
        }
        public ActionResult LendOut()
        {
            return View();
        }
        public ActionResult Inventory()
        {
            return View();
        }
        public ActionResult Quality()
        {
            return View();
        }
        public ActionResult QualityOut()
        {
            return View();
        }
        public ActionResult ItemReturn()
        {
            return View();
        }
        public ActionResult CodeBind()
        {
            return View();
        }
        public ActionResult OrderSearch()
        {
            return View();
        }
        public ActionResult LKPickNoBN()
        {
            return View();
        }

        public ActionResult LKPickBN()
        {
            return View();
        }

        public ActionResult PKPickNoBN()
        {
            return View();
        }

        public ActionResult PKPickBN()
        {
            return View();
        }

        public ActionResult PKPickNoBNList()
        {
            return View();
        }

        public ActionResult PKPickBNList()
        {
            return View();
        }
        /// <summary>
        /// RF上架单任务执行（有码）
        /// </summary>
        /// <returns></returns>
        public ActionResult PutawayDoing()
        {
            return View();
        }
        /// <summary>
        /// RF上架单任务执行(无码)
        /// </summary>
        /// <returns></returns>
        public ActionResult PutawayDoingWithoutSn()
        {
            return View();
        }

        /// <summary>
        /// 上架单直接上架
        /// </summary>
        /// <returns></returns>
        public ActionResult PutawayDoingWithoutTask()
        {
            return View();
        }

        /// <summary>
        /// 组盘上架
        /// </summary>
        /// <returns></returns>
        public ActionResult PutawayPallet()
        {
            return View();
        }
        public ActionResult QcInbound()
        {
            return View();
        }
        public ActionResult LocationAdjustSN()
        {
            return View();
        }
        public ActionResult LocationAdjustNoneSN()
        {
            return View();
        }
        public ActionResult LKCountChoose()
        {
            return View();
        }
        public ActionResult LKCountSingle()
        {
            return View();
        }

        public ActionResult LKCountScanBN()
        {
            return View();
        }
        public ActionResult ApplyTask()
        {
            return View();
        }
        public ActionResult PKPickForContainerList()
        {
            return View();
        }
        public ActionResult PKPickForContainer()
        {
            return View();
        }
        public ActionResult ItemReplacement()
        {
            return View();
        }
        public ActionResult ReceiptZP_Barcode()
        {
            return View();
        }
        public ActionResult ReceiptZP_WithoutBarcode()
        {
            return View();
        }
        public ActionResult ItemReplaceGJ()
        {
            return View();
        }
    }
}