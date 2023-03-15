using Microsoft.AspNetCore.Mvc;

namespace Lekkerbek.Web.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            //return View("~/Views/Shared/_LayoutAdmin.cshtml");
            return View(); //can't give the layout name. It needs to be the view name (controller name)
            //return View();
        }
    }
}
