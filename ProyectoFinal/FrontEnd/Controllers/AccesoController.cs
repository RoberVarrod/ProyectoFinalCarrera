using FrontEnd.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;


namespace FrontEnd.Controllers
{
    public class AccesoController : Controller
    {
        public IActionResult Registro()
        {
            return View();
        }

        public IActionResult InicioSesion()
        {
            return View();
        }


        public IActionResult RegistroEmpleado()
        {
            return View();
        }

        public IActionResult InicioSesionEmpleado()
        {
            return View();
        }
    }
}

