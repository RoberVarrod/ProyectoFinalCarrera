using FrontEnd.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace FrontEnd.Controllers
{
    public class UsuarioClienteController : Controller
    {
        private readonly ProyectoPaqueteriaContext _context;
        private readonly CorreoController _correoController;

        public UsuarioClienteController(ProyectoPaqueteriaContext context, CorreoController correoController)
        {
            _context = context;
            _correoController = correoController;
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
            // Se notifica al cliente el cambio de contraseña

            Task<ActionResult> taskSendEmail = _correoController.enviarCorreoCambioContrasenaCompletadoCliente(cliente);

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
            if (!string.IsNullOrEmpty(sessionId))
            {
                if (int.TryParse(sessionId, out int clienteId))
                {
                    var paquetes = _context.Paquetes
                        .Where(p => p.IdCliente == clienteId)
                        .ToList();

                    return View(paquetes ?? new List<Paquete>()); // Asegurar que nunca sea null
                }
            }

            return RedirectToAction("InicioSesionCliente", "Acceso");
        }



        [HttpGet]
        public IActionResult ObtenerPagosPorUsuario()
        {
            var clienteId = HttpContext.Session.GetString("ClienteId");

            if (string.IsNullOrEmpty(clienteId))
            {
                return StatusCode(401, "No hay sesión activa.");
            }

            try
            {
                var pagosDb = _context.Pagos
                    .Include(p => p.IdPaqueteNavigation)
                    .Where(p => p.IdCliente == int.Parse(clienteId))
                    .ToList(); // Ejecutar la consulta primero para evitar errores de LINQ

                var pagos = pagosDb.Select(p => new
                {
                    IdPago = p.IdPago,
                    NumeroRegistro = p.IdPaqueteNavigation != null ? p.IdPaqueteNavigation.NumeroRegistro : "No asignado",
                    FechaPago = p.FechaPago.HasValue ? p.FechaPago.Value.ToString("dd/MM/yyyy") : "Sin registrar",
                    EstadoPago = !string.IsNullOrEmpty(p.PagoEstado) ? p.PagoEstado : "Sin estado",
                    Monto = decimal.TryParse(p.Total, out decimal monto) ? monto : 0.00m, // Ahora está fuera del LINQ
                    MetodoPago = !string.IsNullOrEmpty(p.PagoMetodo) ? p.PagoMetodo : "No especificado",
                    Descripcion = p.Descripcion ?? "Sin descripción"
                }).ToList();

                Console.WriteLine("Pagos enviados al cliente: " + System.Text.Json.JsonSerializer.Serialize(pagos));

                return Json(pagos);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en el servidor: " + ex.Message);
                return StatusCode(500, $"Error en el servidor: {ex.Message}");
            }
        }


        [HttpGet]
        public IActionResult ObtenerDetallesPago(int idPago)
        {
            var pagoDb = _context.Pagos
                .Include(p => p.IdPaqueteNavigation)
                .Where(p => p.IdPago == idPago)
                .ToList() // Ejecutamos la consulta en memoria
                .FirstOrDefault(); // Obtenemos el primer resultado

            if (pagoDb == null)
            {
                return NotFound("Pago no encontrado.");
            }

            // Convertimos el monto fuera de LINQ
            decimal.TryParse(pagoDb.Total, out decimal monto);

            var pago = new
            {
                IdPago = pagoDb.IdPago,
                NumeroRegistro = pagoDb.IdPaqueteNavigation != null ? pagoDb.IdPaqueteNavigation.NumeroRegistro : "No asignado",
                FechaPago = pagoDb.FechaPago.HasValue ? pagoDb.FechaPago.Value.ToString("dd/MM/yyyy") : "Sin registrar",
                EstadoPago = !string.IsNullOrEmpty(pagoDb.PagoEstado) ? pagoDb.PagoEstado : "Sin estado",
                Monto = monto, // Ahora es seguro
                MetodoPago = !string.IsNullOrEmpty(pagoDb.PagoMetodo) ? pagoDb.PagoMetodo : "No especificado",
                Descripcion = pagoDb.Descripcion ?? "Sin descripción"
            };

            return Json(pago);
        }
        [HttpGet]
        public IActionResult ObtenerPaquetesUsuario()
        {
            var clienteId = HttpContext.Session.GetString("ClienteId");

            if (string.IsNullOrEmpty(clienteId))
            {
                return StatusCode(401, "No hay sesión activa.");
            }

            try
            {
                var paquetes = _context.Paquetes
                    .Where(p => p.IdCliente == int.Parse(clienteId))
                    .Select(p => new
                    {
                        TrackingId = p.NumeroRegistro,
                        Nombre = p.Nombre,
                        EstadoRuta = p.EstadoRuta ?? "No disponible",
                        FechaEntrega = p.FechaEntrega.HasValue ? p.FechaEntrega.Value.ToString("dd/MM/yyyy") : "Sin registrar",
                        TipoEntrega = p.TipoEntrega ?? "No especificado",
                        Precio = p.Precio,
                        Detalles = new
                        {
                            Alto = p.PaqueteAlto,  // ← Nombre corregido
                            Largo = p.PaqueteLargo,  // ← Nombre corregido
                            Ancho = p.PaqueteAncho,  // ← Nombre corregido
                            Descripcion = p.Descripcion ?? "Sin descripción",
                            EstadoPago = p.EstadoPago ?? "No disponible"
                        },
                        DireccionEntrega = new
                        {
                            p.DireccionEntrega,
                            Cliente = new
                            {
                                Provincia = p.IdClienteNavigation.Provincia ?? "No especificado",
                                Canton = p.IdClienteNavigation.Canton ?? "No especificado",
                                Distrito = p.IdClienteNavigation.Distrito ?? "No especificado",
                                CodigoPostal = p.IdClienteNavigation.CodigoPostal ?? "No especificado",
                                Direccion = p.IdClienteNavigation.Direccion ?? "No especificado"
                            }
                        }
                    })
                    .ToList();

                return Json(paquetes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error en el servidor: {ex.Message}");
            }
        }






    }
}




