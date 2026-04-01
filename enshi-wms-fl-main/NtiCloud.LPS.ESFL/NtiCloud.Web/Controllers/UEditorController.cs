using Microsoft.AspNetCore.Mvc;
using UEditorNetCore;

namespace NtiCloud.Web.Controllers
{
    [Route("api/[controller]")] //配置路由
    public class UEditorController : Controller
    {
        private readonly UEditorService ue;
        public UEditorController(UEditorService ue)
        {
            this.ue = ue;
        }

        public void Do()
        {
            ue.DoAction(HttpContext);
        }
    }
}