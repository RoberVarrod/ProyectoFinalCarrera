using FrontEnd.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FrontEnd.Controllers
{
    public class UsuarioController : Controller
    {

        private readonly ProyectoPaqueteriaContext _context;

        public UsuarioController(ProyectoPaqueteriaContext context)
        {
            _context = context;
        }

        public IActionResult Configuracion()
        {
            return View();
        }

        public IActionResult Transportes()
        {
            return View();
        }

        public IActionResult EstadoPaquetes()
        {
            return View();
        }

        public IActionResult Notificaciones()
        {
            return View();
        }

        public IActionResult OrdenesProceso()
        {
            return View();
        }

        public IActionResult Pagos()
        {
            return View();
        }

        public IActionResult ActualizarPago()
        {
            return View();
        }

        public IActionResult Paquetes()
        {
            return View();
        }

        public IActionResult Administracion()
        {
            return View();
        }

        public IActionResult AdministracionCliente()
        {
            return View();
        }

        public IActionResult ConfiguracionAdmin()
        {
            return View();
        }

        public IActionResult NotificacionesAdmin()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult RegistroSucursal()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpGet]

        public IActionResult RegistroSucursal(Sucursal modelo) 
        {
            if (ModelState.IsValid)
            {
              if (_context.Sucursals.Any(u => u.Nombre == modelo.Nombre)) 
                {
                    ModelState.AddModelError("", "Ya se selecciono una surcursal.");
                    return View(modelo);
                }
            }

            var nuevaSucursal = new Sucursal
            {
                Nombre = modelo.Nombre,
                Horario = modelo.Horario,
                Telefono = modelo.Telefono,
                Direccion = modelo.Direccion
            };

            _context.Sucursals.Add(nuevaSucursal);
            _context.SaveChanges();
            return View();
        }
        




        public IActionResult ListaSucursal()
        {
            return View();
        }


    }
}
