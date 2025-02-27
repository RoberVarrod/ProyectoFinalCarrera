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


    


        public async Task<IActionResult> Paquetes()
        {
            var listaPaquetes = await _context.Paquetes.ToListAsync();

            List<PaqueteUsuarioSucursal> listaFinalPaquetes = new List<PaqueteUsuarioSucursal>();   //// esta es mi lista final Ienumerable para la vista

            foreach (var item in listaPaquetes) // se recorre la lista de paquetes en la base de datos
            {

                //se extraen los datos del cliente y la sucursal linkeados al paquete

                // buscar Datos del cliente.
                var cliente = _context.Clientes
                   .FirstOrDefault(u => u.IdCliente == item.IdCliente);

                var NombreCliente = cliente.Nombre;

                // buscar Datos de Sucursal
                var sucursal = _context.Sucursals
                   .FirstOrDefault(u => u.IdSucursal == item.IdSucursal);

                var NombreSucursal = sucursal.Nombre;

                PaqueteUsuarioSucursal paqueteNuevo = new PaqueteUsuarioSucursal(); // se hace el nuevo objeto 

                paqueteNuevo.IdPaquete = item.IdPaquete;
                paqueteNuevo.NumeroRegistro = item.NumeroRegistro;
                paqueteNuevo.Nombre = item.Nombre;
                paqueteNuevo.Precio = item.Precio;
                paqueteNuevo.PaqueteLargo = item.PaqueteLargo;
                paqueteNuevo.PaqueteAncho = item.PaqueteAncho;
                paqueteNuevo.PaqueteAlto = item.PaqueteAlto;
                paqueteNuevo.TipoPaquete = item.TipoPaquete;
                paqueteNuevo.TipoEntrega = item.TipoEntrega;
                paqueteNuevo.Descripcion = item.Descripcion;
                paqueteNuevo.EstadoPago = item.EstadoPago;
                paqueteNuevo.EstadoRuta = item.EstadoRuta;
                paqueteNuevo.FechaRegistro = item.FechaRegistro;
                paqueteNuevo.FechaEntrega = item.FechaEntrega;
                paqueteNuevo.FechaEntregaEstimada = item.FechaEntregaEstimada;
                paqueteNuevo.DireccionEntrega = item.DireccionEntrega;
                paqueteNuevo.RetiroSucursal = item.RetiroSucursal;
                //////////////// Nombre del usuario del paquete, aqui se asigna los nuevos valores
                paqueteNuevo.PaqueteUsuarioNombre = NombreCliente;
                //////////////// Nombre de la sucursal del paquete, aqui se asigna los nuevos valores
                paqueteNuevo.PaqueteSucursalNombre = NombreSucursal;
                //////////////// Ids opcionales, pero pueden ser utilies para otros methodos.
                paqueteNuevo.IdSucursal = item.IdSucursal;
                paqueteNuevo.IdUsuario = item.IdUsuario;
                paqueteNuevo.IdCliente = item.IdCliente;

               listaFinalPaquetes.Add(paqueteNuevo); // paquete agregado a la lista

            }

            return View(listaFinalPaquetes); // retornar la lista de la clase nueva con la lista de paquetes .. listo
        }



        // El metodo de actuaalizar y borraar al final tiene que hacer un redirect to action a public async Task<IActionResult> Paquetes()

        // POST: Usuarios/Delete/5
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> BorrarPaquete(int id)
        {
            var paquete = await _context.Paquetes.FindAsync(id);
            if (paquete != null)
            {
                _context.Paquetes.Remove(paquete);
                await _context.SaveChangesAsync();
                TempData["MensajeExito"] = "El paquete se eliminó correctamente."; // Mensaje de éxito
            }
            return RedirectToAction(nameof(Paquetes));
        }




        public IActionResult Historial()
        {
            return View();
        }
        /*
        public IActionResult Paquetes()
        {
            return View();
        }
        */
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
