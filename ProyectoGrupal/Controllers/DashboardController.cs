using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ProyectoGrupal.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {      
        public IActionResult Index()
        {
            return View();
        }
    }
}
