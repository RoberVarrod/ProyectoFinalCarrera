using FrontEnd.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;


namespace FrontEnd.Controllers
{
    public class AccesoController : Controller
    {
        // Vista para mostrar el formulario de registro
        public IActionResult Registro()
        {
            return View();
        }

        // Vista para mostrar el formulario de inicio de sesión
        public IActionResult InicioSesion()
        {
            return View();
        }
    }
}

