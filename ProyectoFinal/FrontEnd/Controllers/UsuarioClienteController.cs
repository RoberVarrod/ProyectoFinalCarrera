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
                // Verifica que el campo FotoPerfil tenga la ruta de la imagen
                //if (cliente != null && !string.IsNullOrEmpty(cliente.FotoPerfil))
                if (cliente != null)
                {
                    return View(cliente);
                }
            }

            return RedirectToAction("InicioSesionCliente", "Acceso");
        }


        [HttpPost]
        public IActionResult Configuracion(IFormFile profileImage)
        {
            var clienteId = HttpContext.Session.GetString("ClienteId");

            if (!string.IsNullOrEmpty(clienteId) && profileImage != null)
            {
                var cliente = _context.Clientes.FirstOrDefault(c => c.IdCliente == int.Parse(clienteId));
                if (cliente != null)
                {
                    // Usar la cédula del cliente como nombre del archivo
                    var extension = Path.GetExtension(profileImage.FileName);
                    var nombreArchivo = cliente.Cedula + extension;
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imagenesperfil", nombreArchivo);

                    // Guardar la imagen en el servidor
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        profileImage.CopyTo(stream);
                    }

                    // Guardar la ruta de la imagen en el campo FotoPerfil
                    cliente.FotoPerfil = "/imagenesperfil/" + nombreArchivo;
                    _context.SaveChanges();

                    return RedirectToAction("Configuracion");
                }
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


        public async Task<IActionResult> Transportes(string buscar)
        {
            var sucursales = await _context.Sucursals.ToListAsync();
            ViewBag.Sucursales = sucursales;

            var sessionId = HttpContext.Session.GetString("ClienteId");
            if (string.IsNullOrEmpty(sessionId) || !int.TryParse(sessionId, out int clienteId))
            {
                return RedirectToAction("InicioSesionCliente", "Acceso");
            }

            // Obtener los paquetes para el cliente
            var listaPaquetes = await _context.Paquetes
                .Where(p => p.IdCliente == clienteId)
                .ToListAsync();

            List<PaqueteUsuarioSucursal> listaFinalPaquetes = new List<PaqueteUsuarioSucursal>();

            foreach (var item in listaPaquetes)
            {
                var cliente = await _context.Clientes.FirstOrDefaultAsync(u => u.IdCliente == item.IdCliente);
                var sucursal = await _context.Sucursals.FirstOrDefaultAsync(u => u.IdSucursal == item.IdSucursal);

                PaqueteUsuarioSucursal paqueteNuevo = new PaqueteUsuarioSucursal()
                {
                    IdPaquete = item.IdPaquete,
                    NumeroRegistro = item.NumeroRegistro,
                    Nombre = item.Nombre,
                    Precio = item.Precio,
                    PaqueteLargo = item.PaqueteLargo,
                    PaqueteAncho = item.PaqueteAncho,
                    PaqueteAlto = item.PaqueteAlto,
                    TipoPaquete = item.TipoPaquete,
                    TipoEntrega = item.TipoEntrega,
                    Descripcion = item.Descripcion,
                    EstadoPago = item.EstadoPago,
                    EstadoRuta = item.EstadoRuta,
                    FechaRegistro = item.FechaRegistro,
                    FechaEntrega = item.FechaEntrega,
                    FechaEntregaEstimada = item.FechaEntregaEstimada,
                    DireccionEntrega = item.DireccionEntrega,
                    RetiroSucursal = item.RetiroSucursal,
                    PaqueteUsuarioNombre = cliente?.Nombre ?? "Desconocido",
                    PaqueteUsuarioProvincia = cliente?.Provincia ?? "Desconocido",
                    PaqueteUsuarioCanton = cliente?.Canton ?? "Desconocido",
                    PaqueteUsuarioDistrito = cliente?.Distrito ?? "Desconocido",
                    PaqueteUsuarioCodigoPostal = cliente?.CodigoPostal ?? "Desconocido",
                    PaqueteUsuarioDireccion = cliente?.Direccion ?? "Desconocido",
                    PaqueteSucursalNombre = sucursal?.Nombre ?? "Desconocida",
                    IdSucursal = item.IdSucursal,
                    IdUsuario = item.IdUsuario,
                    IdCliente = item.IdCliente
                };

                listaFinalPaquetes.Add(paqueteNuevo);
            }

            if (!string.IsNullOrEmpty(buscar))
            {
                listaFinalPaquetes = listaFinalPaquetes
                    .Where(p => p.NumeroRegistro.Equals(buscar, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            return View(listaFinalPaquetes);
        }

        public IActionResult Notificaciones()
        {
            return View();
        }


        public IActionResult Pagos()
        {
            return View();
        }

        public async Task<IActionResult> Paquetes(string buscar)
        {
            var sucursales = await _context.Sucursals.ToListAsync();
            ViewBag.Sucursales = sucursales;

            var sessionId = HttpContext.Session.GetString("ClienteId");
            if (string.IsNullOrEmpty(sessionId) || !int.TryParse(sessionId, out int clienteId))
            {
                return RedirectToAction("InicioSesionCliente", "Acceso");
            }

            // Obtener los paquetes para el cliente
            var listaPaquetes = await _context.Paquetes
                .Where(p => p.IdCliente == clienteId)
                .ToListAsync();

            List<PaqueteUsuarioSucursal> listaFinalPaquetes = new List<PaqueteUsuarioSucursal>();

            foreach (var item in listaPaquetes)
            {
                var cliente = await _context.Clientes.FirstOrDefaultAsync(u => u.IdCliente == item.IdCliente);
                var sucursal = await _context.Sucursals.FirstOrDefaultAsync(u => u.IdSucursal == item.IdSucursal);

                PaqueteUsuarioSucursal paqueteNuevo = new PaqueteUsuarioSucursal()
                {
                    IdPaquete = item.IdPaquete,
                    NumeroRegistro = item.NumeroRegistro,
                    Nombre = item.Nombre,
                    Precio = item.Precio,
                    PaqueteLargo = item.PaqueteLargo,
                    PaqueteAncho = item.PaqueteAncho,
                    PaqueteAlto = item.PaqueteAlto,
                    TipoPaquete = item.TipoPaquete,
                    TipoEntrega = item.TipoEntrega,
                    Descripcion = item.Descripcion,
                    EstadoPago = item.EstadoPago,
                    EstadoRuta = item.EstadoRuta,
                    FechaRegistro = item.FechaRegistro,
                    FechaEntrega = item.FechaEntrega,
                    FechaEntregaEstimada = item.FechaEntregaEstimada,
                    DireccionEntrega = item.DireccionEntrega,
                    RetiroSucursal = item.RetiroSucursal,
                    PaqueteUsuarioNombre = cliente?.Nombre ?? "Desconocido",
                    PaqueteUsuarioProvincia = cliente?.Provincia ?? "Desconocido",
                    PaqueteUsuarioCanton = cliente?.Canton ?? "Desconocido",
                    PaqueteUsuarioDistrito = cliente?.Distrito ?? "Desconocido",
                    PaqueteUsuarioCodigoPostal = cliente?.CodigoPostal ?? "Desconocido",
                    PaqueteUsuarioDireccion = cliente?.Direccion ?? "Desconocido",
                    PaqueteSucursalNombre = sucursal?.Nombre ?? "Desconocida",
                    IdSucursal = item.IdSucursal,
                    IdUsuario = item.IdUsuario,
                    IdCliente = item.IdCliente
                };

                listaFinalPaquetes.Add(paqueteNuevo);
            }

            if (!string.IsNullOrEmpty(buscar))
            {
                listaFinalPaquetes = listaFinalPaquetes
                    .Where(p => p.NumeroRegistro.Equals(buscar, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            return View(listaFinalPaquetes);
        }


        [HttpPost]
        public IActionResult ActualizarMetodoEntrega(int idPaquete, string tipoEntrega, int? idSucursal)
        {
            var paquete = _context.Paquetes.FirstOrDefault(p => p.IdPaquete == idPaquete);

            if (paquete == null)
            {
                return NotFound();
            }

            // Actualiza el tipo de entrega (Domicilio o Sucursal)
            paquete.TipoEntrega = tipoEntrega;

            if (idSucursal.HasValue)
            {
                var sucursal = _context.Sucursals.FirstOrDefault(s => s.IdSucursal == idSucursal);
                if (sucursal != null)
                {
                    paquete.IdSucursal = sucursal.IdSucursal;
                }
            }

            _context.SaveChanges();

            return RedirectToAction("Paquetes");
        }


        [HttpPost]
        public async Task<IActionResult> ActualizarDireccion(int idPaquete, string provincia, string canton, string distrito, string codigoPostal, string direccion, string tipoEntrega)
        {
            var sessionId = HttpContext.Session.GetString("ClienteId");
            if (string.IsNullOrEmpty(sessionId) || !int.TryParse(sessionId, out int clienteId))
            {
                return RedirectToAction("InicioSesionCliente", "Acceso");
            }

            var paquete = await _context.Paquetes.FirstOrDefaultAsync(p => p.IdPaquete == idPaquete && p.IdCliente == clienteId);
            if (paquete == null)
            {
                return NotFound();
            }

            paquete.TipoEntrega = tipoEntrega;

            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.IdCliente == clienteId);
            if (cliente == null)
            {
                return NotFound();
            }

            cliente.Provincia = provincia;
            cliente.Canton = canton;
            cliente.Distrito = distrito;
            cliente.CodigoPostal = codigoPostal;
            cliente.Direccion = direccion;

            await _context.SaveChangesAsync();

            return RedirectToAction("Paquetes");
        }


        public async Task<IActionResult> Historial(int pid)
        {

            var lista = await _context.HistorialCambiosPaquetes.Where(h => h.IdPaquete == pid).OrderBy(h => h.Sequencia).ToListAsync();

            List<HistorialCambiosPaquete> listaCambiosPaquete = new List<HistorialCambiosPaquete>();

            listaCambiosPaquete = lista;

            return View(listaCambiosPaquete);

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

        public async Task<IActionResult> EstadoPaquetes(string buscar)
        {
            var sucursales = await _context.Sucursals.ToListAsync();
            ViewBag.Sucursales = sucursales;

            var sessionId = HttpContext.Session.GetString("ClienteId");
            if (string.IsNullOrEmpty(sessionId) || !int.TryParse(sessionId, out int clienteId))
            {
                return RedirectToAction("InicioSesionCliente", "Acceso");
            }

            var listaPaquetes = await _context.Paquetes
                .Where(p => p.IdCliente == clienteId)
                .ToListAsync();

            List<PaqueteUsuarioSucursal> listaFinalPaquetes = new List<PaqueteUsuarioSucursal>();

            foreach (var item in listaPaquetes)
            {
                var cliente = _context.Clientes.FirstOrDefault(u => u.IdCliente == item.IdCliente);
                var sucursal = _context.Sucursals.FirstOrDefault(u => u.IdSucursal == item.IdSucursal);

                PaqueteUsuarioSucursal paqueteNuevo = new PaqueteUsuarioSucursal()
                {
                    IdPaquete = item.IdPaquete,
                    NumeroRegistro = item.NumeroRegistro,
                    Nombre = item.Nombre,
                    Precio = item.Precio,
                    PaqueteLargo = item.PaqueteLargo,
                    PaqueteAncho = item.PaqueteAncho,
                    PaqueteAlto = item.PaqueteAlto,
                    TipoPaquete = item.TipoPaquete,
                    TipoEntrega = item.TipoEntrega,
                    Descripcion = item.Descripcion,
                    EstadoPago = item.EstadoPago,
                    EstadoRuta = item.EstadoRuta,
                    FechaRegistro = item.FechaRegistro,
                    FechaEntrega = item.FechaEntrega,
                    FechaEntregaEstimada = item.FechaEntregaEstimada,
                    DireccionEntrega = item.DireccionEntrega,
                    RetiroSucursal = item.RetiroSucursal,
                    PaqueteUsuarioNombre = cliente?.Nombre ?? "Desconocido",
                    PaqueteUsuarioProvincia = cliente?.Provincia ?? "Desconocido",
                    PaqueteUsuarioCanton = cliente?.Canton ?? "Desconocido",
                    PaqueteUsuarioDistrito = cliente?.Distrito ?? "Desconocido",
                    PaqueteUsuarioCodigoPostal = cliente?.CodigoPostal ?? "Desconocido",
                    PaqueteUsuarioDireccion = cliente?.Direccion ?? "Desconocido",
                    PaqueteSucursalNombre = sucursal?.Nombre ?? "Desconocida",
                    IdSucursal = item.IdSucursal,
                    IdUsuario = item.IdUsuario,
                    IdCliente = item.IdCliente
                };

                listaFinalPaquetes.Add(paqueteNuevo);
            }

            if (!string.IsNullOrEmpty(buscar))
            {
                listaFinalPaquetes = listaFinalPaquetes
                    .Where(p => p.NumeroRegistro.Equals(buscar, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            return View(listaFinalPaquetes);
        }

        [HttpPost]
        public IActionResult ActualizarMetodoEntregaEstado(int idPaquete, string tipoEntrega, int? idSucursal)
        {
            var paquete = _context.Paquetes.FirstOrDefault(p => p.IdPaquete == idPaquete);

            if (paquete == null)
            {
                return NotFound();
            }

            // Actualiza el tipo de entrega (Domicilio o Sucursal)
            paquete.TipoEntrega = tipoEntrega;

            if (idSucursal.HasValue)
            {
                var sucursal = _context.Sucursals.FirstOrDefault(s => s.IdSucursal == idSucursal);
                if (sucursal != null)
                {
                    paquete.IdSucursal = sucursal.IdSucursal;
                }
            }

            _context.SaveChanges();

            return RedirectToAction("EstadoPaquetes");
        }


        [HttpPost]
        public async Task<IActionResult> ActualizarDireccionEstado(int idPaquete, string provincia, string canton, string distrito, string codigoPostal, string direccion, string tipoEntrega)
        {
            var sessionId = HttpContext.Session.GetString("ClienteId");
            if (string.IsNullOrEmpty(sessionId) || !int.TryParse(sessionId, out int clienteId))
            {
                return RedirectToAction("InicioSesionCliente", "Acceso");
            }

            var paquete = await _context.Paquetes.FirstOrDefaultAsync(p => p.IdPaquete == idPaquete && p.IdCliente == clienteId);
            if (paquete == null)
            {
                return NotFound();
            }

            paquete.TipoEntrega = tipoEntrega;

            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.IdCliente == clienteId);
            if (cliente == null)
            {
                return NotFound();
            }

            cliente.Provincia = provincia;
            cliente.Canton = canton;
            cliente.Distrito = distrito;
            cliente.CodigoPostal = codigoPostal;
            cliente.Direccion = direccion;

            await _context.SaveChangesAsync();

            return RedirectToAction("EstadoPaquetes");
        }

        public async Task<IActionResult> OrdenesProceso(string buscar)
        {
            var sucursales = await _context.Sucursals.ToListAsync();
            ViewBag.Sucursales = sucursales;

            var sessionId = HttpContext.Session.GetString("ClienteId");
            if (string.IsNullOrEmpty(sessionId) || !int.TryParse(sessionId, out int clienteId))
            {
                return RedirectToAction("InicioSesionCliente", "Acceso");
            }

            var listaPaquetes = await _context.Paquetes
                .Where(p => p.IdCliente == clienteId)
                .ToListAsync();

            List<PaqueteUsuarioSucursal> listaFinalPaquetes = new List<PaqueteUsuarioSucursal>();

            foreach (var item in listaPaquetes)
            {
                var cliente = _context.Clientes.FirstOrDefault(u => u.IdCliente == item.IdCliente);
                var sucursal = _context.Sucursals.FirstOrDefault(u => u.IdSucursal == item.IdSucursal);

                PaqueteUsuarioSucursal paqueteNuevo = new PaqueteUsuarioSucursal()
                {
                    IdPaquete = item.IdPaquete,
                    NumeroRegistro = item.NumeroRegistro,
                    Nombre = item.Nombre,
                    Precio = item.Precio,
                    PaqueteLargo = item.PaqueteLargo,
                    PaqueteAncho = item.PaqueteAncho,
                    PaqueteAlto = item.PaqueteAlto,
                    TipoPaquete = item.TipoPaquete,
                    TipoEntrega = item.TipoEntrega,
                    Descripcion = item.Descripcion,
                    EstadoPago = item.EstadoPago,
                    EstadoRuta = item.EstadoRuta,
                    FechaRegistro = item.FechaRegistro,
                    FechaEntrega = item.FechaEntrega,
                    FechaEntregaEstimada = item.FechaEntregaEstimada,
                    DireccionEntrega = item.DireccionEntrega,
                    RetiroSucursal = item.RetiroSucursal,
                    PaqueteUsuarioNombre = cliente?.Nombre ?? "Desconocido",
                    PaqueteUsuarioProvincia = cliente?.Provincia ?? "Desconocido",
                    PaqueteUsuarioCanton = cliente?.Canton ?? "Desconocido",
                    PaqueteUsuarioDistrito = cliente?.Distrito ?? "Desconocido",
                    PaqueteUsuarioCodigoPostal = cliente?.CodigoPostal ?? "Desconocido",
                    PaqueteUsuarioDireccion = cliente?.Direccion ?? "Desconocido",
                    PaqueteSucursalNombre = sucursal?.Nombre ?? "Desconocida",
                    IdSucursal = item.IdSucursal,
                    IdUsuario = item.IdUsuario,
                    IdCliente = item.IdCliente
                };

                listaFinalPaquetes.Add(paqueteNuevo);
            }

            if (!string.IsNullOrEmpty(buscar))
            {
                listaFinalPaquetes = listaFinalPaquetes
                    .Where(p => p.NumeroRegistro.Equals(buscar, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            return View(listaFinalPaquetes);
        }

        [HttpPost]
        public IActionResult ActualizarMetodoEntregaOrdenes(int idPaquete, string tipoEntrega, int? idSucursal)
        {
            var paquete = _context.Paquetes.FirstOrDefault(p => p.IdPaquete == idPaquete);

            if (paquete == null)
            {
                return NotFound();
            }

            // Actualiza el tipo de entrega (Domicilio o Sucursal)
            paquete.TipoEntrega = tipoEntrega;

            if (idSucursal.HasValue)
            {
                var sucursal = _context.Sucursals.FirstOrDefault(s => s.IdSucursal == idSucursal);
                if (sucursal != null)
                {
                    paquete.IdSucursal = sucursal.IdSucursal;
                }
            }

            _context.SaveChanges();

            return RedirectToAction("OrdenesProceso");
        }


        [HttpPost]
        public async Task<IActionResult> ActualizarDireccionOrdenes(int idPaquete, string provincia, string canton, string distrito, string codigoPostal, string direccion, string tipoEntrega)
        {
            var sessionId = HttpContext.Session.GetString("ClienteId");
            if (string.IsNullOrEmpty(sessionId) || !int.TryParse(sessionId, out int clienteId))
            {
                return RedirectToAction("InicioSesionCliente", "Acceso");
            }

            var paquete = await _context.Paquetes.FirstOrDefaultAsync(p => p.IdPaquete == idPaquete && p.IdCliente == clienteId);
            if (paquete == null)
            {
                return NotFound();
            }

            paquete.TipoEntrega = tipoEntrega;

            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.IdCliente == clienteId);
            if (cliente == null)
            {
                return NotFound();
            }

            cliente.Provincia = provincia;
            cliente.Canton = canton;
            cliente.Distrito = distrito;
            cliente.CodigoPostal = codigoPostal;
            cliente.Direccion = direccion;

            await _context.SaveChangesAsync();

            return RedirectToAction("OrdenesProceso");
        }
        
    }
}