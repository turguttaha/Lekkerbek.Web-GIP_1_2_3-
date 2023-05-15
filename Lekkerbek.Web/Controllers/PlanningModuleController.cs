using Microsoft.AspNetCore.Mvc;

namespace Lekkerbek.Web.Controllers
{
    public class PlanningModuleController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
