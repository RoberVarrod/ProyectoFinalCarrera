using FrontEnd.Models;
using FrontEnd.Models.modelsDataParameterMethods;
using FrontEnd.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;


namespace FrontEnd.Controllers
{
    public class AccesoController : Controller
    {

        private readonly ProyectoPaqueteriaContext _context;
        private readonly CorreoController _correoController;

        public AccesoController(ProyectoPaqueteriaContext context, CorreoController correoController)
        {
            _context = context;
            _correoController = correoController;
        }


        private void VerificarSesion()
        {
            var lastActivity = HttpContext.Session.GetString("LastActivity");
            if (lastActivity != null)
            {
                var lastActivityTime = DateTime.Parse(lastActivity);
                if (lastActivityTime.AddMinutes(5) < DateTime.Now) // 5 minutos de inactividad
                {
                    HttpContext.Session.Clear();
                    TempData["MensajeSesion"] = "Sesión cerrada por inactividad.";
                }
            }
            HttpContext.Session.SetString("LastActivity", DateTime.Now.ToString()); // Actualizar la última actividad
        }




        [AllowAnonymous]
        [HttpGet]
        public IActionResult RegistroUsuario()
        {
            VerificarSesion();
            return View();
        }


        [AllowAnonymous]
        [HttpPost]
        public IActionResult RegistroUsuario(RegistroModeloUsuario modelo)
        {
            VerificarSesion();
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
                    IdSucursal = modelo.IdSucursal,
                    FotoPerfil = modelo.FotoPerfil


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
            VerificarSesion();
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult InicioSesionUsuario(LoginModeloUsuario modelo)
        {
            VerificarSesion();
            if (ModelState.IsValid)
            {
                // Buscar el usuario en la base de datos
                var usuario = _context.Usuarios
                    .FirstOrDefault(u => u.Correo == modelo.Correo && u.Contrasena == modelo.Contrasena); /// debería ser con cédula también --->Ariel

                if (usuario != null)
                {
                    // Guardar información del usuario en la sesión
                    HttpContext.Session.SetString("UsuarioId", usuario.IdUsuario.ToString()); // Guardar el ID del usuario
                    HttpContext.Session.SetString("UsuarioNombre", usuario.Nombre);
                    HttpContext.Session.SetString("UsuarioRol", usuario.IdRol.ToString()); // Guardar el rol del usuario

                    TempData["Mensaje"] = $"Bienvenido {usuario.Nombre}";

                    // Verificar el rol del usuario
                    if (usuario.IdRol == 3)
                    {
                        // Redirigir a la vista de configuración de transportista, pasando el objeto del usuario
                        return RedirectToAction("Configuracion", "UsuarioTransportista", new { usuarioId = usuario.IdUsuario });
                    }

                    // Redirigir a otra vista si no es rol 3
                    return RedirectToAction("Configuracion", "Usuario", new { usuarioId = usuario.IdUsuario });
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
            VerificarSesion();
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult RegistroCliente(RegistroModeloCliente modelo)
        {
            VerificarSesion();
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
                    Direccion = modelo.Direccion,
                    FotoPerfil = modelo.FotoPerfil


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
            VerificarSesion();
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult InicioSesionCliente(LoginModeloUsuarioCliente modelo)
        {
            VerificarSesion();
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

                    // se envia correo indicando que se inicio sesion y la fecha  de inicio al usuario por seguridad
                    Task<ActionResult> taskSendEmail = _correoController.enviarCorreoInicioSesionCliente(cliente);

                    return RedirectToAction("Configuracion", "UsuarioCliente");
                }

                // Si no coincide usuario o contraseña
                TempData["MensajeInicioFallido"] = "Usuario o contraseña incorrectos.";
                return RedirectToAction("InicioSesionCliente");
            }
            return View();
        }


        // Cerrar sesión
        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Limpiar la sesión
            TempData["MensajeSession"] = "Sesión cerrada correctamente.";
            return RedirectToAction("Index", "Home");
        }



        //// Recuperar contrasena metodos


        [AllowAnonymous]
        [HttpGet]
        public IActionResult EnviarCodigoSeguridad()
        {
            return View();
        }


        [AllowAnonymous]
        [HttpPost]
        public IActionResult EnviarCodigoSeguridad(CorreoParameter correoUsuario)
        {
            // Buscar el cliente en la base de datos
            var cliente = _context.Clientes
                .FirstOrDefault(u => u.Correo == correoUsuario.Correo);
            if (cliente != null)
            {

                    // se envia correo indicando alguien solicito recuperar la contrasena y el codigo de seguridad
                    Task<ActionResult> taskSendEmail = _correoController.enviarCorreoCambioContrasenaCodigoSeguridadCliente(cliente);

                return RedirectToAction("EnviarContrasenaNueva", cliente);
            }
            else
            {
                // Si no coincide usuario o contraseña
                TempData["MensajeCorreoUsuarioNoEncontrado"] = "Correo no registrado en el sistema";
                return RedirectToAction("InicioSesionCliente");
            }            
        }



        [AllowAnonymous]
        [HttpGet]
        public IActionResult EnviarContrasenaNueva(Cliente cliente)
        {
            return View(cliente);
        }



        [AllowAnonymous]
        [HttpPost]
        public IActionResult EnviarContrasenaAleatoria(Cliente clienteRecibido)
        {
            // Buscar el cliente en la base de datos
            var cliente = _context.Clientes
                .FirstOrDefault(u => u.IdCliente == clienteRecibido.IdCliente);

            if (cliente.ClaveRecupera == clienteRecibido.ClaveRecupera)
            {

                // se envia correo indicando la contrasena nueva para que el usuario pueda iniciar sesion.
                Task<ActionResult> taskSendEmail = _correoController.enviarCorreoCambioContrasenaTemporalCliente(cliente);

                TempData["MensajeContrasenaTemporalEnviada"] = "Contaseña temporal enviada a su correo, favor iniciar sesión con la contraseña nueva";
                return RedirectToAction("InicioSesionCliente");


            }
            else
            {
                // Si no coincide usuario o contraseña
                TempData["MensajeCodigoDeSeguridadIncorrecto"] = "El Código de seguridad es incorrecto, favor intente de nuevo el proceso";
                return RedirectToAction("InicioSesionCliente");
            }
        }





    }
  
}

