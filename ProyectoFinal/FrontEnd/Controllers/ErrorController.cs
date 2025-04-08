using Microsoft.AspNetCore.Mvc;
using System;

namespace FrontEnd.Controllers
{
    public class ErrorController : Controller
    {
        [NonAction]
        public IActionResult HandleAllExceptions(Exception ex)
        {
            TempData["ErrorMessage"] = "Hubo un error inesperado. Por favor, inténtalo de nuevo más tarde o contactate con soporte";

            return View("Error");
        }
    }
}
