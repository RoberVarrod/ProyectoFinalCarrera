using FrontEnd.Models;
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


    }
}
