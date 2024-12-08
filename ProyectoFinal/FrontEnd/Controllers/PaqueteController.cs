using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FrontEnd.Controllers
{
    public class PaqueteController : Controller
    {

        public IActionResult RegistrarPaquete()
        {
            return View();
        }


        public IActionResult Historial()
        {
            return View();
        }

    }
}
