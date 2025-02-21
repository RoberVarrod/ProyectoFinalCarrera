using FrontEnd.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FrontEnd.Controllers
{
    public class PaqueteController : Controller
    {

        private readonly ProyectoPaqueteriaContext _context;

        public PaqueteController(ProyectoPaqueteriaContext context)
        {
            _context = context;
        }


        [AllowAnonymous]
        [HttpGet]
        public IActionResult RegistrarPaquete()
        {
            return View();
        }


        [AllowAnonymous]
        [HttpPost]
        public IActionResult RegistrarPaquete(Paquete modelo)
        {

            //modelo.IdUsuarioNavigation = modelo.IdUsuario;

            if (ModelState.IsValid)
            {
                // Validar que el paquete no exista previamente
                if (_context.Paquetes.Any(p => p.NumeroRegistro == modelo.NumeroRegistro))
                {
                    ModelState.AddModelError("", "El Número de Registro ya existe.");
                    return View();
                }

                // Crear un nuevo paquete con la información proporcionada
                var nuevoPaquete = new Paquete
                {
                    NumeroRegistro = modelo.NumeroRegistro,//
                    Nombre = null,
                    Precio = modelo.Precio,//
                    PaqueteLargo = modelo.PaqueteLargo,//
                    PaqueteAncho = modelo.PaqueteAncho,//
                    PaqueteAlto = modelo.PaqueteAlto,//
                    TipoPaquete = null,
                    TipoEntrega = null,
                    Descripcion = modelo.Descripcion,//
                    EstadoPago = "Pendiente",
                    EstadoRuta = null,
                    FechaRegistro = DateTime.Now,
                    FechaEntrega = null,
                    FechaEntregaEstimada = DateTime.Now.AddDays(2), // se agregan dos dias estimados
                    DireccionEntrega = modelo.DireccionEntrega,//
                    RetiroSucursal = null,
                    // estos ids se podrian agregaar aqui menos el de id cliente.
                    IdUsuario = modelo.IdUsuario,
                    IdCliente = modelo.IdCliente,
                    IdSucursal = modelo.IdSucursal
                };

                // Guardar el paquete en la base de datos
                _context.Paquetes.Add(nuevoPaquete);
                _context.SaveChanges();

                TempData["MensajePaqueteRegistrado"] = "Paquete registrado correctamente.";
                return View();
            }
            else
            {
                return View();
            }
        }


        /*

        public async Task<IActionResult> PaqueteSucursalNombre()
        {
            var listaPaquetes = await _context.Paquetes.ToListAsync();

            return View(listaPaquetes);
        }

        */
        public IActionResult Historial()
        {
            return View();
        }

        public IActionResult Paquetes()
        {
            return View();
        }

        public IActionResult EstadoPaquetes() 
        {
            return View();
        }
        public IActionResult OrdenesProceso() 
        {
            return View();
        }


    }
}
