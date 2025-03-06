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



        [HttpPost]
        public IActionResult CambiarContrasena(int IdCliente, string ContrasenaActual, string NuevaContrasena, string ConfirmarContrasena)
        {
            var cliente = _context.Clientes.FirstOrDefault(c => c.IdCliente == IdCliente);

            if (cliente == null)
            {
                return NotFound("Cliente no encontrado.");
            }

            // Validar que la contraseña actual sea correcta
            if (cliente.Contrasena != ContrasenaActual)
            {
                TempData["ErrorContrasena"] = "La contraseña actual es incorrecta.";
                return RedirectToAction("Configuracion");
            }

            // Validar que las nuevas contraseñas coincidan
            if (NuevaContrasena != ConfirmarContrasena)
            {
                TempData["ErrorContrasena"] = "Las nuevas contraseñas no coinciden.";
                return RedirectToAction("Configuracion");
            }

            // Actualizar la contraseña sin encriptar
            cliente.Contrasena = NuevaContrasena;
            _context.SaveChanges();

            TempData["ExitoContrasena"] = "Contraseña actualizada correctamente.";
            return RedirectToAction("Configuracion");
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
            var sessionId = HttpContext.Session.GetString("ClienteId");
            if (sessionId != null)
            {
                var clienteId = int.Parse(sessionId);
                var paquetes = _context.Paquetes.Where(p => p.IdCliente == clienteId).ToList();
                return View(paquetes);
            }
            return RedirectToAction("InicioSesionCliente", "Acceso");
        }

        [HttpGet]
        public IActionResult ObtenerPaquetesPorTracking(string trackingId)
        {
            if (string.IsNullOrEmpty(trackingId))
            {
                return BadRequest("El Número de Registro no puede estar vacío.");
            }

            var paquetes = _context.Paquetes
                .Where(p => p.NumeroRegistro == trackingId)
                .Select(p => new
                {
                    p.NumeroRegistro,
                    p.Nombre,
                    p.EstadoRuta,
                    p.FechaEntrega,
                    p.TipoEntrega,
                    p.Precio,
                    p.IdPaquete
                })
                .ToList();

            return Json(paquetes);
        }

        // Método para obtener detalles de un paquete específico
        [HttpGet]
        public IActionResult ObtenerDetallesPaquete(int idPaquete)
        {
            var paquete = _context.Paquetes
                .Where(p => p.IdPaquete == idPaquete)
                .Select(p => new
                {
                    p.NumeroRegistro,
                    p.Nombre,
                    p.EstadoRuta,
                    p.FechaEntrega,
                    p.TipoEntrega,
                    p.Precio,
                    // Asegúrate de incluir todas las propiedades necesarias
                })
                .FirstOrDefault();

            if (paquete == null)
            {
                return NotFound("Paquete no encontrado.");
            }

            return Json(paquete);
        }
        [HttpPost]
        public IActionResult ActualizarTipoEntrega(int idPaquete, string tipoEntrega)
        {
            if (idPaquete <= 0 || string.IsNullOrEmpty(tipoEntrega))
            {
                return Json(new { success = false, message = "Datos inválidos" });
            }

            // Busca el paquete en la base de datos
            var paquete = _context.Paquetes.FirstOrDefault(p => p.IdPaquete == idPaquete);

            if (paquete == null)
            {
                return Json(new { success = false, message = "Paquete no encontrado" });
            }

            // Actualiza el tipo de entrega
            paquete.TipoEntrega = tipoEntrega;

            try
            {
                _context.SaveChanges(); // Guarda los cambios en la base de datos
                return Json(new { success = true, message = "Método de entrega actualizado correctamente" });
            }
            catch (Exception ex)
            {
                // Manejo de errores si ocurre algún problema al guardar los cambios
                return Json(new { success = false, message = "Error al actualizar el tipo de entrega: " + ex.Message });
            }
        }
    }

}
