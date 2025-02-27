﻿using FrontEnd.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FrontEnd.Controllers
{
    public class UsuarioController : Controller
    {

        private readonly ProyectoPaqueteriaContext _context;

        public UsuarioController(ProyectoPaqueteriaContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Configuracion()
        {
            var usuarioId = HttpContext.Session.GetString("UsuarioId");

            if (!string.IsNullOrEmpty(usuarioId))
            {
                var usuario = _context.Usuarios.FirstOrDefault(c => c.IdUsuario == int.Parse(usuarioId));
                return View(usuario);
            }

            return RedirectToAction("InicioSesionUsuario", "Acceso");
        }

        [HttpPost]
        public IActionResult EditarInformacion(Usuario usuario)
        {
            var usuarioExistente = _context.Usuarios.FirstOrDefault(c => c.IdUsuario == usuario.IdUsuario);
            if (usuarioExistente != null)
            {
                usuarioExistente.Nombre = usuario.Nombre;
                usuarioExistente.PrimerApellido = usuario.PrimerApellido;
                usuarioExistente.SegundoApellido = usuario.SegundoApellido;
                usuarioExistente.Cedula = usuario.Cedula;
                usuarioExistente.Correo = usuario.Correo;
                usuarioExistente.TelefonoPrincipal = usuario.TelefonoPrincipal;
                usuarioExistente.Direccion = usuario.Direccion;

                _context.SaveChanges();
                return RedirectToAction("Configuracion");
            }
            return NotFound();
        }

        [HttpPost]
        public IActionResult CambiarContrasena(int IdUsuario, string ContrasenaActual, string NuevaContrasena, string ConfirmarContrasena)
        {
            var usuario = _context.Usuarios.FirstOrDefault(c => c.IdUsuario == IdUsuario);

            if (usuario == null)
            {
                return NotFound("Usuario no encontrado.");
            }

            // Validar que la contraseña actual sea correcta
            if (usuario.Contrasena != ContrasenaActual)
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
            usuario.Contrasena = NuevaContrasena;
            _context.SaveChanges();

            TempData["ExitoContrasena"] = "Contraseña actualizada correctamente.";
            return RedirectToAction("Configuracion");
        }

        public IActionResult Transportes()
        {
            return View();
        }


        public IActionResult Notificaciones()
        {
            return View();
        }

        public IActionResult Pagos()
        {
            var paquetes = _context.Paquetes
                .Include(p => p.IdClienteNavigation) // Incluye los datos del cliente
                .ToList(); // Convierte a lista para evitar referencias nulas

            if (paquetes == null || paquetes.Count == 0)
            {
                paquetes = new List<Paquete>(); // Devuelve una lista vacía en lugar de null
            }

            return View(paquetes);
        }


        [HttpGet]
        public IActionResult ObtenerPagosPorPaquete(int idPaquete)
        {
            var pagos = _context.Pagos
                .Where(p => p.IdPaquete == idPaquete)
                .Select(p => new
                {
                    p.IdPago,
                    numeroRegistro = p.IdPaqueteNavigation.NumeroRegistro,
                    p.Total,
                    p.Descripcion,
                    FechaPago = p.FechaPago.HasValue ? p.FechaPago.Value.ToString("dd/MM/yyyy") : "N/A",
                    p.PagoEstado,
                    p.PagoMetodo,
                    ClienteNombre = p.IdClienteNavigation.Nombre + " " + p.IdClienteNavigation.PrimerApellido + " " + p.IdClienteNavigation.SegundoApellido
                })
                .ToList();

            return Json(pagos);
        }

        [HttpGet]
        public IActionResult ObtenerPagosPorTracking(string trackingId)
        {
            var pagos = _context.Pagos
                .Where(p => p.IdPaqueteNavigation.NumeroRegistro == trackingId)
                .Select(p => new
                {
                    idPago = p.IdPago,
                    numeroRegistro = p.IdPaqueteNavigation.NumeroRegistro,
                    descripcion = p.Descripcion,
                    total = p.Total,
                    pagoMetodo = p.PagoMetodo,
                    FechaPago = p.FechaPago.HasValue ? p.FechaPago.Value.ToString("dd/MM/yyyy") : "N/A",
                    pagoEstado = p.PagoEstado,
                    clienteNombre = p.IdClienteNavigation.Nombre + " " + p.IdClienteNavigation.PrimerApellido
                })
                .ToList();

            return Json(pagos);
        }


        [AllowAnonymous]
        [HttpGet]
        public IActionResult RegistrarPago()
        {
            return View();
        }

        [HttpPost]
        public IActionResult BuscarPaquetePorTrackingAjax(string NumeroRegistro)
        {
            if (string.IsNullOrEmpty(NumeroRegistro))
            {
                return Json(new { idPaquete = 0 });
            }

            var paquete = _context.Paquetes
                .Include(p => p.IdClienteNavigation)
                .Where(p => p.NumeroRegistro == NumeroRegistro && p.EstadoPago == "Pendiente")
                .FirstOrDefault();

            if (paquete == null)
            {
                return Json(new { idPaquete = 0 });
            }

            var cliente = paquete.IdClienteNavigation;

            var resultado = new
            {
                idPaquete = paquete.IdPaquete,
                numeroRegistro = paquete.NumeroRegistro,
                precio = paquete.Precio,
                idCliente = paquete.IdCliente,
                nombreCliente = cliente.Nombre + " " + cliente.PrimerApellido + " " + cliente.SegundoApellido,
                cedula = cliente.Cedula
            };

            return Json(resultado);
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult RegistrarPago(Pago modelo)
        {
            try
            {
                // Validar que existan los IDs necesarios
                if (modelo.IdPaquete <= 0 || modelo.IdCliente <= 0)
                {
                    ModelState.AddModelError("", "ID de paquete o cliente no válido.");
                    return View(modelo);
                }

                // Verificar que el paquete exista y esté pendiente de pago
                var paquete = _context.Paquetes.Find(modelo.IdPaquete);
                if (paquete == null)
                {
                    ModelState.AddModelError("", "El paquete no existe.");
                    return View(modelo);
                }

                if (paquete.EstadoPago != "Pendiente")
                {
                    ModelState.AddModelError("", "El paquete ya ha sido pagado.");
                    return View(modelo);
                }

                // Crear un nuevo pago con la información proporcionada
                var nuevoPago = new Pago
                {
                    Total = modelo.Total,
                    Descripcion = modelo.Descripcion ?? "Pago de paquete " + paquete.NumeroRegistro,
                    FechaPago = DateTime.Now,
                    PagoEstado = "Cancelado",
                    PagoMetodo = modelo.PagoMetodo,
                    IdPaquete = modelo.IdPaquete,
                    IdCliente = modelo.IdCliente
                };

                // Actualizar el estado del pago en el paquete
                paquete.EstadoPago = "Cancelado";
                _context.Update(paquete);

                // Guardar el pago en la base de datos
                _context.Pagos.Add(nuevoPago);
                _context.SaveChanges();

                TempData["MensajePagoRegistrado"] = "Pago registrado correctamente.";
                return RedirectToAction("RegistrarPago");
            }
            catch (Exception ex)
            {
                // Log de errores - Esto ayudará a diagnosticar problemas
                Console.WriteLine("Error al registrar pago: " + ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine("Inner Exception: " + ex.InnerException.Message);
                }

                ModelState.AddModelError("", "Error al registrar el pago. Detalle: " + ex.Message);
                return View(modelo);
            }
        }


        public async Task<IActionResult> Administracion()
        {
            var usuarios = await _context.Usuarios.ToListAsync();

            return View(usuarios);
        }

        // GET: Usuarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(m => m.IdUsuario == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();
                TempData["MensajeExito"] = "El usuario se eliminó correctamente."; // Mensaje de éxito
            }
            return RedirectToAction(nameof(Administracion));
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
        [HttpPost]
        public IActionResult RegistroSucursal(Sucursal modelo)
        {
            if (ModelState.IsValid)
            {
                // Validar que la sucursal no exista previamente
                if (_context.Sucursals.Any(s => s.Nombre == modelo.Nombre))
                {
                    ModelState.AddModelError("", "Ya se seleccionó una sucursal con este nombre.");
                    return View(modelo);
                }

                // Crear nueva sucursal con la información proporcionada
                var nuevaSucursal = new Sucursal
                {
                    Nombre = modelo.Nombre,
                    Horario = modelo.Horario,
                    Telefono = modelo.Telefono,
                    Direccion = modelo.Direccion
                };

                // Guardar en la base de datos
                _context.Sucursals.Add(nuevaSucursal);
                _context.SaveChanges();

                TempData["MensajeRegistroCorrecto"] = "Sucursal registrada correctamente.";
                return RedirectToAction("RegistroSucursal");
            }

            return RedirectToAction("RegistroSucursal");
        }

        public IActionResult ListaSucursal()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpGet]
        public IActionResult UsuarioSucursal()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult UsuarioSucursal(Usuario modelo)
        {
            if (ModelState.IsValid)
            {
                // Validar que el usuario existe
                var usuario = _context.Usuarios.SingleOrDefault(u => u.Nombre == modelo.Nombre);
                if (usuario == null)
                {
                    ModelState.AddModelError("", "Usuario no encontrado");
                    return View(modelo);
                }

                // Cambiar la sucursal del usuario
                usuario.IdSucursal = modelo.IdSucursal;

                try
                {
                    // Guardar los cambios en la base de datos
                    _context.SaveChanges();
                    ViewBag.Message = "Sucursal cambiada exitosamente";
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al cambiar la sucursal: " + ex.Message);
                }

                return View(modelo);
            }

            return View(modelo);
        }


    }
}
