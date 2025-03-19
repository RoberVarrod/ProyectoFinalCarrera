using FrontEnd.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FrontEnd.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.CodeAnalysis.Options;
using Newtonsoft.Json.Linq;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace FrontEnd.Controllers
{
    public class PaqueteController : Controller
    {

        private readonly ProyectoPaqueteriaContext _context;
        private readonly CorreoController _correoController;
 

        public PaqueteController(ProyectoPaqueteriaContext context , CorreoController correoController)
        {
            _context = context;
            _correoController = correoController;
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
                    TipoEntrega = "Sucursal",
                    Descripcion = modelo.Descripcion,//
                    EstadoPago = "Pendiente",
                    EstadoRuta = "Pendiente",
                    FechaRegistro = DateTime.Now,
                    FechaEntrega = null,
                    FechaEntregaEstimada = null, // se agregan dos dias estimados
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

                // se envia correo al cliente
                Task<ActionResult> taskSendEmail = _correoController.enviarCorreoRegistroPaquete(nuevoPaquete);

                //se agrega el registro en el historial
                registrarCambioHistorialPaquete(nuevoPaquete,"Registro");


                TempData["MensajePaqueteRegistrado"] = "Paquete registrado correctamente, Usuario notificado via email";
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



        // El metodo de actualizar y borrar al final tiene que hacer un redirect to action a public async Task<IActionResult> Paquetes()

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


        // POST: Usuarios/Edit/5
        [HttpPost]
        public IActionResult ActualizarPaquete(Paquete paquete)
        {
            var paqueteExistente = _context.Paquetes.FirstOrDefault(p => p.IdPaquete == paquete.IdPaquete);

            var huboCambioDeEstadoRuta = false;
            //Se hace un IF que define si el estado del paquete cambio, esto para solo notificar al cliente por correo en caso de cambiar el estado, si el estado es el mismo, no se notifica pues no es necesario.
            if (paquete.EstadoRuta != paqueteExistente.EstadoRuta)
            {
                huboCambioDeEstadoRuta = true;
            }

            if (paqueteExistente != null)
            {
                // Actualización de datos
                paqueteExistente.NumeroRegistro = paquete.NumeroRegistro;
                paqueteExistente.Nombre = null;
                paqueteExistente.Precio = paquete.Precio;
                paqueteExistente.PaqueteLargo = paquete.PaqueteLargo;
                paqueteExistente.PaqueteAncho = paquete.PaqueteAncho;
                paqueteExistente.PaqueteAlto = paquete.PaqueteAlto;
                paqueteExistente.TipoEntrega = paquete.TipoEntrega;
                paqueteExistente.Descripcion = paquete.Descripcion;
                paqueteExistente.EstadoPago = paquete.EstadoPago;
                paqueteExistente.EstadoRuta = paquete.EstadoRuta;
                paqueteExistente.FechaEntregaEstimada = paquete.FechaEntregaEstimada;

                _context.SaveChanges();

                //Se hace un IF que define si el estado del paquete cambio, esto para solo notificar al cliente por correo en caso de cambiar el estado, si el estado es el mismo, no se notifica pues no es necesario.
                if (huboCambioDeEstadoRuta) {

                    // se create un nuevo objeto paquete con los datos nuevos para enviar al metodo
                    Paquete paqueteActualizadoParaCorreo = new Paquete();
                    paqueteActualizadoParaCorreo = paqueteExistente;
                    //Se envia correo al usuario con el cambio de los datos del paquete

                    // se envia correo al cliente en ruta si el estado cambio a En ruta
                    if (paqueteActualizadoParaCorreo.EstadoRuta == "En ruta")
                    {
                        Task<ActionResult> taskSendEmail = _correoController.enviarCorreoActualizarEstadoPaqueteEnRuta(paqueteActualizadoParaCorreo);

                        //Se registra el cambio en el historial
                        registrarCambioHistorialPaquete(paqueteActualizadoParaCorreo, "En ruta");

                    }
                    // se envia correo al cliente en sucursal si el estado cambio a En sucursal
                    if (paqueteActualizadoParaCorreo.EstadoRuta == "En sucursal")
                    {
                        Task<ActionResult> taskSendEmail = _correoController.enviarCorreoActualizarEstadoPaqueteEnSucursal(paqueteActualizadoParaCorreo);

                        //Se registra el cambio en el historial
                         registrarCambioHistorialPaquete(paqueteActualizadoParaCorreo, "En sucursal");
                    }
                    // se envia correo al cliente a entregado si el estado cambio a Entregado
                    if (paqueteActualizadoParaCorreo.EstadoRuta == "Entregado")
                    {
                        Task<ActionResult> taskSendEmail = _correoController.enviarCorreoActualizarEstadoPaqueteEntregado(paqueteActualizadoParaCorreo);

                        //Se registra el cambio en el historial
                       registrarCambioHistorialPaquete(paqueteActualizadoParaCorreo, "Entregado");
                    }

                } // Termina el if que valida si se envia el correo o no.


                return RedirectToAction("Paquetes");

            }

            
            return NotFound();
        }
  
        public async Task<IActionResult> EstadoPaquetes()
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

        // El metodo de actualizar y borrar al final tiene que hacer un redirect to action a public async Task<IActionResult> Paquetes()

        // POST: Usuarios/Delete/5
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> BorrarPaqueteEstado(int id)
        {
            var paquete = await _context.Paquetes.FindAsync(id);
            if (paquete != null)
            {
                _context.Paquetes.Remove(paquete);
                await _context.SaveChangesAsync();
                TempData["MensajeExito"] = "El paquete se eliminó correctamente."; // Mensaje de éxito
            }
            return RedirectToAction(nameof(EstadoPaquetes));
        }


        // POST: Usuarios/Edit/5
        [HttpPost]
        public IActionResult ActualizarPaqueteEstado(Paquete paquete)
        {
            var paqueteExistente = _context.Paquetes.FirstOrDefault(p => p.IdPaquete == paquete.IdPaquete);
            if (paqueteExistente != null)
            {
                // Actualización de datos
                paqueteExistente.NumeroRegistro = paquete.NumeroRegistro;
                paqueteExistente.Nombre = null;
                paqueteExistente.Precio = paquete.Precio;
                paqueteExistente.PaqueteLargo = paquete.PaqueteLargo;
                paqueteExistente.PaqueteAncho = paquete.PaqueteAncho;
                paqueteExistente.PaqueteAlto = paquete.PaqueteAlto;
                paqueteExistente.TipoEntrega = paquete.TipoEntrega;
                paqueteExistente.Descripcion = paquete.Descripcion;
                paqueteExistente.EstadoPago = paquete.EstadoPago;
                paqueteExistente.EstadoRuta = paquete.EstadoRuta;
                paqueteExistente.FechaEntregaEstimada = paquete.FechaEntregaEstimada;

                _context.SaveChanges();
                return RedirectToAction("EstadoPaquetes");
            }
            return NotFound();
        }

        public async Task<IActionResult> OrdenesProceso()
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

        // El metodo de actualizar y borrar al final tiene que hacer un redirect to action a public async Task<IActionResult> Paquetes()

        // POST: Usuarios/Delete/5
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> BorrarPaqueteOrden(int id)
        {
            var paquete = await _context.Paquetes.FindAsync(id);
            if (paquete != null)
            {
                _context.Paquetes.Remove(paquete);
                await _context.SaveChangesAsync();
                TempData["MensajeExito"] = "El paquete se eliminó correctamente."; // Mensaje de éxito
            }
            return RedirectToAction(nameof(OrdenesProceso));
        }


        // POST: Usuarios/Edit/5
        [HttpPost]
        public IActionResult ActualizarPaqueteOrden(Paquete paquete)
        {
            var paqueteExistente = _context.Paquetes.FirstOrDefault(p => p.IdPaquete == paquete.IdPaquete);
            if (paqueteExistente != null)
            {
                // Actualización de datos
                paqueteExistente.NumeroRegistro = paquete.NumeroRegistro;
                paqueteExistente.Nombre = null;
                paqueteExistente.Precio = paquete.Precio;
                paqueteExistente.PaqueteLargo = paquete.PaqueteLargo;
                paqueteExistente.PaqueteAncho = paquete.PaqueteAncho;
                paqueteExistente.PaqueteAlto = paquete.PaqueteAlto;
                paqueteExistente.TipoEntrega = paquete.TipoEntrega;
                paqueteExistente.Descripcion = paquete.Descripcion;
                paqueteExistente.EstadoPago = paquete.EstadoPago;
                paqueteExistente.EstadoRuta = paquete.EstadoRuta;
                paqueteExistente.FechaEntregaEstimada = paquete.FechaEntregaEstimada;

                _context.SaveChanges();
                return RedirectToAction("OrdenesProceso");
            }
            return NotFound();
        }


        public async Task<IActionResult> Historial(int pid)
        {

            var lista = await _context.HistorialCambiosPaquetes.Where(h => h.IdPaquete == pid).OrderBy(h => h.Sequencia).ToListAsync();

            List<HistorialCambiosPaquete> listaCambiosPaquete  = new List<HistorialCambiosPaquete>();

            listaCambiosPaquete = lista;

            return View(listaCambiosPaquete);
    
        }

        //registrarCambioHistorial
        // POST api/<CorreoController>
        [HttpPost]
        public ActionResult registrarCambioHistorialPaquete(Paquete paquete, string accion)
        {

            HistorialCambiosPaquete nuevoRegistro = new HistorialCambiosPaquete();

            string infoTitulo = "";
            string descCuerpo = "";
          


            //Titulo y cuerpo de acuerdo a la accion 
            if(accion == "Registro")
            {
                // buscar Datos de Sucursal
                var sucursal = _context.Sucursals
                .FirstOrDefault(u => u.IdSucursal == paquete.IdSucursal);

                infoTitulo = "Paquete: " + paquete.NumeroRegistro + " registrado en el sistema";
                descCuerpo = "Estado actual del paquete: " + paquete.EstadoPago + "\n" +
               "Descripción: " + paquete.Descripcion + "\n" +
               "Sucursal: " + sucursal.Nombre;

            }else if(accion == "En ruta")
            {
                // buscar Datos de Sucursal
                var sucursal = _context.Sucursals
                .FirstOrDefault(u => u.IdSucursal == paquete.IdSucursal);

                infoTitulo = "Paquete: " + paquete.NumeroRegistro + " Se encuentra en ruta al destino solicitado";
                descCuerpo = "Estado actual del paquete: " + "En ruta a destino acordado" + "\n" +
               "Dirección de entrega: " + paquete.DireccionEntrega + "\n" +
               "Ubicación actual: " + sucursal.Nombre + "\n";

            }else if(accion == "En sucursal")
            {

                // buscar Datos de Sucursal
                var sucursal = _context.Sucursals
                .FirstOrDefault(u => u.IdSucursal == paquete.IdSucursal);

                infoTitulo = "Paquete: " + paquete.NumeroRegistro + " Se encuentra en la sucursal solicitada";
                descCuerpo = "Estado actual del paquete: " + "en sucursal " + sucursal.Nombre+ "\n" +
                "Dirección de entrega: " + "Sucursal acordada" + "\n" +
                "Ubicación: " + sucursal.Nombre;

            }else if (accion == "Entregado")
            {
                infoTitulo = "Paquete: " + paquete.NumeroRegistro + " ha sido entregado";
                descCuerpo = "Estado actual del paquete: " + "paquete entregado" + "\n" +
                "Dirección de entrega: " + paquete.DireccionEntrega + "\n";

            }
            else
            {
                infoTitulo = "error";
                descCuerpo = "error";
            }


            //sacar la sequencia
            /*
           int seqMaxima = (from seq in _context.HistorialCambiosPaquetes
                            where seq.IdPaquete == paquete.IdPaquete
                            select seq.Sequencia).DefaultIfEmpty(0).Max();
            */
            //int seqMax = _context.HistorialCambiosPaquetes.Where(h => h.IdPaquete == paquete.IdPaquete).Select(h => h.Sequencia).DefaultIfEmpty(0).Max();
            int seqMax = _context.HistorialCambiosPaquetes.Where(x => x.IdPaquete == paquete.IdPaquete).Max(x => x.Sequencia as int?) ?? 0; /// solo esta funciona...



            nuevoRegistro.FechaRegistro = DateTime.Now;
            nuevoRegistro.Informacion = infoTitulo;
            nuevoRegistro.Descripcion = descCuerpo;
            nuevoRegistro.IdPaquete = paquete.IdPaquete;
            nuevoRegistro.Sequencia = seqMax + 1;  /// le sumo uno a la sequencia cada vez, se supone que la primera vez tiene que dar 0



            _context.HistorialCambiosPaquetes.Add(nuevoRegistro);
            _context.SaveChanges();

            return Ok();

        }

    }
}
