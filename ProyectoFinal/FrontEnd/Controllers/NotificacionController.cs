using FrontEnd.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FrontEnd.Controllers
{
    public class NotificacionController : Controller
    {


        private readonly ProyectoPaqueteriaContext _context;

        public NotificacionController(ProyectoPaqueteriaContext context)
        {
            _context = context;
        }


     

      public async Task<IActionResult> Notificaciones()
            {
            var listaNotificaciones = await _context.Notificacions.OrderBy(n => n.IdNotificacion).ToListAsync();
            List<Notificacion> listaDeNotificaciones = new List<Notificacion>();   //// esta es mi lista final Ienumerable para la vista
            listaDeNotificaciones = listaNotificaciones;
                return View(listaDeNotificaciones);
           }




        [AllowAnonymous]
        [HttpGet]
        public IActionResult RegistrarNotificacion()
        {
            return View();
        }





        [AllowAnonymous]
        [HttpPost]
        public IActionResult RegistrarNotificacion(Notificacion modelo)
        {

            //modelo.IdUsuarioNavigation = modelo.IdUsuario;

            if (ModelState.IsValid)
            {

                // Crear un nueva notificacion
                Notificacion nuevaNotificacion = new Notificacion();

                nuevaNotificacion.Titulo = modelo.Titulo;
                nuevaNotificacion.Cuerpo = modelo.Cuerpo;
                nuevaNotificacion.Tipo = modelo.Tipo;
                nuevaNotificacion.FechaRegistro = DateTime.Now;

                // Guardar el paquete en la base de datos
                _context.Notificacions.Add(nuevaNotificacion);
                _context.SaveChanges();

                TempData["MensajeNotificacionAgregada"] = "Notificación agregada";
                return View();
            }
            else
            {
                return View();
            }
        }










    }
}

