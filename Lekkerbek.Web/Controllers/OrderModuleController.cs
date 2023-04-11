using Microsoft.AspNetCore.Mvc;

namespace Lekkerbek.Web.Controllers
{
    public class OrderModuleController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
