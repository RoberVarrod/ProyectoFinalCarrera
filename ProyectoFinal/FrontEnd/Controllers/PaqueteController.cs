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
            if (ModelState.IsValid)
            {
                // Validar que el paquete no exista previamente
                if (_context.Paquetes.Any(p => p.NumeroRegistro == modelo.NumeroRegistro))
                {
                    ModelState.AddModelError("", "El Número de Registro ya existe.");
                    return View(modelo);
                }

                // Crear un nuevo paquete con la información proporcionada
                var nuevoPaquete = new Paquete
                {
                    NumeroRegistro = modelo.NumeroRegistro,
                    Nombre = modelo.Nombre,
                    Precio = modelo.Precio,
                    PaqueteLargo = modelo.PaqueteLargo,
                    PaqueteAncho = modelo.PaqueteAncho,
                    PaqueteAlto = modelo.PaqueteAlto,
                    TipoPaquete = modelo.TipoPaquete,
                    TipoEntrega = modelo.TipoEntrega,
                    Descripcion = modelo.Descripcion,
                    EstadoPago = modelo.EstadoPago,
                    EstadoRuta = modelo.EstadoRuta,
                    FechaRegistro = DateTime.Now,
                    FechaEntrega = modelo.FechaEntrega,
                    FechaEntregaEstimada = modelo.FechaEntregaEstimada,
                    DireccionEntrega = modelo.DireccionEntrega,
                    RetiroSucursal = modelo.RetiroSucursal,
                    IdUsuario = modelo.IdUsuario,
                    IdCliente = modelo.IdCliente
                };

                // Guardar el paquete en la base de datos
                _context.Paquetes.Add(nuevoPaquete);
                _context.SaveChanges();

                TempData["Mensaje"] = "Paquete registrado correctamente.";
                return RedirectToAction("RegistroPaquete");
            }
            else
            {
                return View(modelo);
            }
        }



        public IActionResult Historial()
        {
            return View();
        }

    }
}
