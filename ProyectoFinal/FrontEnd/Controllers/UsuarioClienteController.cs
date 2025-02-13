using FrontEnd.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FrontEnd.Controllers
{
    public class UsuarioClienteController : Controller
    {
        private readonly ProyectoPaqueteriaContext _context;

        public UsuarioClienteController(ProyectoPaqueteriaContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Configuracion()
        {
            var clienteId = HttpContext.Session.GetString("ClienteId");

            if (!string.IsNullOrEmpty(clienteId))
            {
                var cliente = _context.Clientes.FirstOrDefault(c => c.IdCliente == int.Parse(clienteId));
                return View(cliente);
            }

            return RedirectToAction("InicioSesionCliente", "Acceso");
        }

        [HttpPost]
        public IActionResult EditarInformacion(Cliente cliente)
        {
            var clienteExistente = _context.Clientes.FirstOrDefault(c => c.IdCliente == cliente.IdCliente);
            if (clienteExistente != null)
            {
                clienteExistente.Nombre = cliente.Nombre;
                clienteExistente.PrimerApellido = cliente.PrimerApellido;
                clienteExistente.SegundoApellido = cliente.SegundoApellido;
                clienteExistente.Cedula = cliente.Cedula;
                clienteExistente.Correo = cliente.Correo;
                clienteExistente.TelefonoPrincipal = cliente.TelefonoPrincipal;
                clienteExistente.TelefonoSecundario = cliente.TelefonoSecundario;
                clienteExistente.Provincia = cliente.Provincia;
                clienteExistente.Canton = cliente.Canton;
                clienteExistente.Distrito = cliente.Distrito;
                clienteExistente.CodigoPostal = cliente.CodigoPostal;
                clienteExistente.Direccion = cliente.Direccion;

                _context.SaveChanges();
                return RedirectToAction("Configuracion");
            }
            return NotFound();
        }



        [HttpPost]
        public JsonResult BuscarClientePorCedulaAjax(string cedula) //Para llamar con AJAX
        {
            var cliente = _context.Clientes
                   .FirstOrDefault(u => u.Cedula == cedula);

            if (cliente != null)
            {
                Cliente clienteModelo = new Cliente();

                clienteModelo.IdCliente = cliente.IdCliente;
                clienteModelo.Nombre = cliente.Nombre;
                clienteModelo.TelefonoPrincipal = cliente.TelefonoPrincipal;
                clienteModelo.Correo = cliente.Correo;
                clienteModelo.Cedula = cliente.Cedula;
                clienteModelo.Provincia = cliente.Provincia;
                clienteModelo.Canton = cliente.Canton;
                clienteModelo.Distrito = cliente.Distrito;
                clienteModelo.CodigoPostal = cliente.CodigoPostal;
                clienteModelo.Direccion = cliente.Direccion;

                return Json(clienteModelo);
            }
            else
            {
                Cliente clienteModelo = new Cliente(); // iria como null
                return Json(clienteModelo);
            }      

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

        public IActionResult Paquetes()
        {
            return View();
        }
    }
}
