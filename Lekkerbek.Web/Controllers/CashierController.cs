using Microsoft.AspNetCore.Mvc;

namespace Lekkerbek.Web.Controllers
{
    public class CashierController : Controller
    {
        public IActionResult Index()
        {
            return View(); 
        }
    }
}
