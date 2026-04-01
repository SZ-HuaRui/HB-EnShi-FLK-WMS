using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Led.Web.ViewModels
{
    public class LedInfoModel
    {
        [Display(Name = "牌号编码")]
        public string ItemCode { get; set; }

        [Display(Name = "牌号名称")]
        public string ItemName { get; set; }

        [Display(Name = "数量")]
        public string Qty { get; set; }

    }
}