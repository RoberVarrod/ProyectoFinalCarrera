using FrontEnd.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;


namespace FrontEnd.Controllers
{
    public class AccesoController : Controller
    {

        private readonly ProyectoPaqueteriaContext _context;

        public AccesoController(ProyectoPaqueteriaContext context)
        {
            _context = context;
        }




        [AllowAnonymous]
        [HttpGet]
        public IActionResult RegistroUsuario()
        {
            return View();
        }


        [AllowAnonymous]
        [HttpPost]
        public IActionResult RegistroUsuario(RegistroModeloUsuario modelo)
        {
            if (ModelState.IsValid)
            {
                // Validar que el usuario no exista previamente
                if (_context.Usuarios.Any(u => u.Cedula == modelo.Cedula))
                {
                    ModelState.AddModelError("", "El nombre de usuario ya existe.");
                    return View(modelo);
                }

                // Crear un nuevo usuario con la información proporcionada
                var nuevoUsuario = new Usuario
                {

                    Nombre = modelo.Nombre,
                    PrimerApellido = modelo.PrimerApellido,
                    SegundoApellido = modelo.SegundoApellido,
                    TelefonoPrincipal = modelo.TelefonoPrincipal,
                    Correo = modelo.Correo,
                    Cedula = modelo.Cedula,
                    Contrasena = modelo.Contrasena,
                    Direccion = modelo.Direccion,
                    Oficina = modelo.Oficina,
                    IdRol = modelo.IdRol,
                    IdSucursal = modelo.IdSucursal


                };

                // Guardar el usuario en la base de datos
                _context.Usuarios.Add(nuevoUsuario);
                _context.SaveChanges();

                TempData["MensajeRegistroCorrecto"] = "Usuario registrado correctamente, ahora puedes iniciar sesión.";
                return RedirectToAction("RegistroUsuario");
            }
            else
            {
                return RedirectToAction("RegistroUsuario");
            }




            return View(modelo);
        }


        [AllowAnonymous]
        [HttpGet]
        public IActionResult InicioSesionUsuario()
        {
            return View();
        }
        
        [AllowAnonymous]
        [HttpPost]
        public IActionResult InicioSesionUsuario(LoginModeloUsuario modelo)
        {
            if (ModelState.IsValid)
            {
                // Buscar el usuario en la base de datos
                var usuario = _context.Usuarios
                    .FirstOrDefault(u => u.Correo == modelo.Correo && u.Contrasena == modelo.Contrasena); /// deberiaa ser con cedula tambien --->Ariel

                if (usuario != null)
                {
                    // Guardar información del usuario en la sesión
                    HttpContext.Session.SetString("UsuarioId", usuario.IdUsuario.ToString()); // Guardar el ID del usuario
                    HttpContext.Session.SetString("UsuarioNombre", usuario.Nombre);
                    HttpContext.Session.SetString("UsuarioRol", usuario.IdRol.ToString()); // Guardar el rol del usuario

                    TempData["Mensaje"] = $"Bienvenido {usuario.Nombre}";

                    // Aqui hay que crear un objeto de tipo Usuario model completo y pasar ese objeto al redirect to aaction para la vista configuracion. --->Ariel



                    return RedirectToAction("Configuracion", "Usuario");
                }

                // Si no coincide usuario o contraseña
                TempData["MensajeInicioFallido"] = "Usuario o contraseña incorrectos.";
                return RedirectToAction("InicioSesionUsuario");
            }

            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult RegistroCliente()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult RegistroCliente(RegistroModeloCliente modelo)
        {
            if (ModelState.IsValid)
            {
                // Validar que el cliente no exista previamente
                if (_context.Clientes.Any(c => c.Cedula == modelo.Cedula))
                {
                    ModelState.AddModelError("", "El nombre de usuario ya existe.");
                    return View(modelo);
                }

                // Crear un nuevo cliente con la información proporcionada
                var nuevoCliente = new Cliente
                {

                    Nombre = modelo.Nombre,
                    PrimerApellido = modelo.PrimerApellido,
                    SegundoApellido = modelo.SegundoApellido,
                    TelefonoPrincipal = modelo.TelefonoPrincipal,
                    TelefonoSecundario = modelo.TelefonoSecundario,
                    Correo = modelo.Correo,
                    Cedula = modelo.Cedula,
                    Contrasena = modelo.Contrasena,
                    IdRol = modelo.IdRol,
                    Provincia = modelo.Provincia,
                    Canton = modelo.Canton,
                    Distrito = modelo.Distrito,
                    CodigoPostal = modelo.CodigoPostal,
                    Direccion = modelo.Direccion


                };

                // Guardar el cliete en la base de datos
                _context.Clientes.Add(nuevoCliente);
                _context.SaveChanges();

                TempData["MensajeRegistroCorrecto"] = "Cliente registrado correctamente, ahora puedes iniciar sesión.";
                return RedirectToAction("RegistroCliente");
            }
            else
            {
                return RedirectToAction("RegistroCliente");
            }

            return View(modelo);
        }


        [AllowAnonymous]
        [HttpGet]
        public IActionResult InicioSesionCliente()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult InicioSesionCliente(LoginModeloUsuarioCliente modelo)
        {
            if (ModelState.IsValid)
            {
                // Buscar el cliente en la base de datos
                var cliente = _context.Clientes
                    .FirstOrDefault(u => u.Correo == modelo.Correo && u.Contrasena == modelo.Contrasena);

                if (cliente != null)
                {

                    HttpContext.Session.SetString("ClienteId", cliente.IdCliente.ToString()); // Guardar el ID del cliente
                    HttpContext.Session.SetString("ClienteNombre", cliente.Nombre);
                    HttpContext.Session.SetString("ClienteRol", cliente.IdRol.ToString()); // Guardar el rol del cliente

                    TempData["Mensaje"] = $"Bienvenido {cliente.Nombre}";

                    // Aqui hay que crear un objeto de tipo Usuario model completo y pasar ese objeto al redirect to aaction para la vista configuracion. --->Ariel



                    return RedirectToAction("Configuracion", "UsuarioCliente");
                }

                // Si no coincide usuario o contraseña
                TempData["MensajeInicioFallido"] = "Usuario o contraseña incorrectos.";
                return RedirectToAction("InicioSesionCliente");
            }
            return View();
        }


        // Cerrar Sesión
        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Limpiar la sesión
           //  HttpContext.Session.Abandon(); /// esto hace falta aqui o la asesion se maantiene 
           // HttpContext.Current.Session.RemoveAll();
            TempData["Mensaje"] = "Sesión cerrada correctamente.";
            return RedirectToAction("Index", "Home");
        }



    }
}

