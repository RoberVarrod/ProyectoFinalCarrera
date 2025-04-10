using FrontEnd.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FrontEnd.Controllers
{
    public class UsuarioTransportistaController : Controller
    {

        private readonly ProyectoPaqueteriaContext _context;
        private readonly CorreoController _correoController;

        public UsuarioTransportistaController(ProyectoPaqueteriaContext context, CorreoController correoController)
        {
            _context = context;
            _correoController = correoController;
        }

        [HttpGet]
        public IActionResult Configuracion()
        {
            var usuarioId = HttpContext.Session.GetString("UsuarioId");

            if (!string.IsNullOrEmpty(usuarioId))
            {
                var usuario = _context.Usuarios.FirstOrDefault(c => c.IdUsuario == int.Parse(usuarioId));
                // Verifica que el campo FotoPerfil tenga la ruta de la imagen
                // if (usuario != null && !string.IsNullOrEmpty(usuario.FotoPerfil))
                if (usuario != null)
                {
                    return View(usuario);
                }
            }

            return RedirectToAction("InicioSesionUsuario", "Acceso");
        }


        [HttpPost]
        public IActionResult Configuracion(IFormFile profileImage)
        {
            var usuarioId = HttpContext.Session.GetString("UsuarioId");

            if (!string.IsNullOrEmpty(usuarioId) && profileImage != null)
            {
                var usuario = _context.Usuarios.FirstOrDefault(c => c.IdUsuario == int.Parse(usuarioId));
                if (usuario != null)
                {
                    // Usar la cédula del cliente como nombre del archivo
                    var extension = Path.GetExtension(profileImage.FileName);
                    var nombreArchivo = usuario.Cedula + extension;
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imagenesperfil", nombreArchivo);

                    // Guardar la imagen en el servidor
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        profileImage.CopyTo(stream);
                    }

                    // Guardar la ruta de la imagen en el campo FotoPerfil
                    usuario.FotoPerfil = "/imagenesperfil/" + nombreArchivo;
                    _context.SaveChanges();

                    return RedirectToAction("Configuracion");
                }
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

                
            }
            return RedirectToAction("Configuracion");
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



    }

}
