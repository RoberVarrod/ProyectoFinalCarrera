using Microsoft.AspNetCore.Mvc;
using FrontEnd.Services;
using FrontEnd.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

//Este es un controller de tipo API.. para usar el API de google de correos.


namespace FrontEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CorreoController : ControllerBase
    {

        private readonly ProyectoPaqueteriaContext _context;

        private readonly ICorreoService _correoService;


        public CorreoController(ICorreoService _correoService , ProyectoPaqueteriaContext _context)
        {
            this._correoService = _correoService;
            this._context = _context;

        }




        // POST api/<CorreoController>
        [HttpPost]
        public async Task<ActionResult> EnviarCorreo(string emailReceiver, string subject, string body)
        {

            await _correoService.EnviarCorreo(emailReceiver, subject, body);
            return Ok();   


        }



        // Cuando se registra paquete
        // POST api/<CorreoController>
        [HttpPost]
        public async Task<ActionResult> enviarCorreoRegistroPaquete(Paquete paquete)
        {

            // buscar Datos del cliente.
            var cliente = _context.Clientes
               .FirstOrDefault(u => u.IdCliente == paquete.IdCliente);

            // buscar Datos de Sucursal
            var sucursal = _context.Sucursals
               .FirstOrDefault(u => u.IdSucursal == paquete.IdSucursal);


            string emailReceiver = cliente.Correo;

            string subject = "Nuevo paquete recibido " + paquete.NumeroRegistro;
            string body = "Se le comunica que hay un paquete disponible para su entrega" + "\n" +
                "Favor ingresar a su cuenta en nuestro sitio web para proceder con la entrega y seleccionar la sucursal donde desea retirar su paquete o la dirección donde a domicilio donde desea recibir el paquete" + "\n" +
                "\n" +

                "Detalles del paquete:" + "\n" +
                "Precio: " + paquete.Precio + "\n" +
                "Descripción: " + paquete.Descripcion + "\n" +
                "Estado de pago: " + paquete.EstadoPago + "\n" +
                "Dirección de entrega: " + paquete.DireccionEntrega + "\n" +
                "Ubicación: " + sucursal.Nombre;




            await _correoService.EnviarCorreo(emailReceiver, subject, body);
            return Ok();


        }

        // Cuando se registra un pago
        // POST api/<CorreoController>
        [HttpPost]
        public async Task<ActionResult> enviarCorreoPagoPaquete(Pago pago)
        {

            // buscar Datos del cliente.
            var cliente = _context.Clientes
               .FirstOrDefault(u => u.IdCliente == pago.IdCliente);

            // buscar Datos de Sucursal
            var paquete = _context.Paquetes
               .FirstOrDefault(u => u.IdPaquete == pago.IdPaquete);


            string emailReceiver = cliente.Correo;

            string subject = "Su paquete #" + paquete.NumeroRegistro + " ha sido pagado";
            string body = "Se le comunica que su paquete ha sido pagado" + "\n" +
                "Favor ingresar a su cuenta en nuestro sitio web para ver los detalles del mismo, a continuación los detalles del pago" + "\n" +
                "\n" +

                "Detalles de pago:" + "\n" +
                "Número de registro: " + paquete.NumeroRegistro + "\n" +
                "Total cancelado: " + pago.Total + "\n" +
                "Descripción del paquete: " + paquete.Descripcion + "\n" +
                //"Descripción del pago: " + pago.Descripcion + "\n" +  no se si se debe mostrar una descripcion de pago o solo dejar la del paquete.
                "Estado de pago: " + pago.PagoEstado + "\n" +
                "Fecha de pago: " + pago.FechaPago + "\n" +
                "Método de pago: " + pago.PagoMetodo;




            await _correoService.EnviarCorreo(emailReceiver, subject, body);
            return Ok();


        }


        // cuando el paquete se actualiza en ruta
        // POST api/<CorreoController>
        [HttpPost]
        public async Task<ActionResult> enviarCorreoActualizarEstadoPaqueteEnRuta(Paquete paqueteActual)
        {

            // buscar Datos del cliente.
            var cliente = _context.Clientes
               .FirstOrDefault(u => u.IdCliente == paqueteActual.IdCliente);

            // buscar Datos de Sucursal
            var sucursal = _context.Sucursals
               .FirstOrDefault(u => u.IdSucursal == paqueteActual.IdSucursal);


            string emailReceiver = cliente.Correo;

            string subject = "Actualización de datos del estado del paquete: " + paqueteActual.NumeroRegistro + " Paquete en ruta";
            string body = "Se le comunica que hubo una actualización en los datos de su paquete" + "\n" +
                "Favor ingresar a su cuenta en nuestro sitio web para ver mas detalles o el historial de cambios, a continuación los datos nuevos de su paquete" + "\n" +
                "\n" +

                "Número de registro: " + paqueteActual.NumeroRegistro + "\n" +
                "Estado actual del paquete: " + "En ruta a destino acordado" + "\n" +
                "Descripción: " + paqueteActual.Descripcion + "\n" +
                "Estado de pago: " + paqueteActual.EstadoPago + "\n" +
                "Dirección de entrega: " + paqueteActual.DireccionEntrega + "\n" +
                "Ubicación actual: " + sucursal.Nombre + "\n";
                //+"Fecha de entrega estimada: " + paqueteActual.FechaEntregaEstimada  -- se supone que cuando el usuario elige la entrega ahi se debe de estimar los 2 dias habiles para la entrega.


            await _correoService.EnviarCorreo(emailReceiver, subject, body);
            return Ok();


        }

        // cuando el paquete se actualiza en Sucursal
        // POST api/<CorreoController>
        [HttpPost]
        public async Task<ActionResult> enviarCorreoActualizarEstadoPaqueteEnSucursal(Paquete paqueteActual)
        {

            // buscar Datos del cliente.
            var cliente = _context.Clientes
               .FirstOrDefault(u => u.IdCliente == paqueteActual.IdCliente);

            // buscar Datos de Sucursal
            var sucursal = _context.Sucursals
               .FirstOrDefault(u => u.IdSucursal == paqueteActual.IdSucursal);


            string emailReceiver = cliente.Correo;

            string subject = "Actualización de datos del estado del paquete: " + paqueteActual.NumeroRegistro + " Paquete en Sucursal";
            string body = "Se le comunica que hubo una actualización en los datos de su paquete" + "\n" +
                "Favor ingresar a su cuenta en nuestro sitio web para ver mas detalles o el historial de cambios, a continuación los datos nuevos de su paquete" + "\n" +
                "\n" +

  
                "Número de registro: " + paqueteActual.NumeroRegistro + "\n" +
                "Estado actual del paquete: " + "en sucursal " + sucursal.Nombre + "\n" +
                "Descripción: " + paqueteActual.Descripcion + "\n" +
                "Estado de pago: " + paqueteActual.EstadoPago + "\n" +
                "Dirección de entrega: " +"Sucursal acordada" + "\n" +
                "Ubicación: " + sucursal.Nombre;


            await _correoService.EnviarCorreo(emailReceiver, subject, body);
            return Ok();


        }

        // cuando el paquete se actualiza a Entregado
        // POST api/<CorreoController>
        [HttpPost]
        public async Task<ActionResult> enviarCorreoActualizarEstadoPaqueteEntregado(Paquete paqueteActual)
        {

            // buscar Datos del cliente.
            var cliente = _context.Clientes
               .FirstOrDefault(u => u.IdCliente == paqueteActual.IdCliente);

            // buscar Datos de Sucursal
            //var sucursal = _context.Sucursals
            //   .FirstOrDefault(u => u.IdSucursal == paqueteActual.IdSucursal);


            string emailReceiver = cliente.Correo;

            string subject = "Actualización de datos del estado del paquete: " + paqueteActual.NumeroRegistro + " Paquete Entregado";
            string body = "Se le comunica que hubo una actualización en los datos de su paquete" + "\n" +
                "Favor ingresar a su cuenta en nuestro sitio web para ver mas detalles o el historial de cambios, a continuación los datos nuevos de su paquete" + "\n" +
                "\n" +


                "Número de registro: " + paqueteActual.NumeroRegistro + "\n" +
                "Estado actual del paquete: " + "Paquete entregado" + "\n" +
                "Descripción: " + paqueteActual.Descripcion + "\n" +
                "Estado de pago: " + paqueteActual.EstadoPago + "\n" +
                "Fecha de entrega: " + paqueteActual.FechaEntrega + "\n";



            await _correoService.EnviarCorreo(emailReceiver, subject, body);
            return Ok();


        }


        //enviarCorreoInicioSesionCliente
        // cuando el cliente inicia sesion
        // POST api/<CorreoController>
        [HttpPost]
        public async Task<ActionResult> enviarCorreoInicioSesionCliente(Cliente cliente)
        {
            string emailReceiver = cliente.Correo;

            string subject = "Se ha reportado un inicio de sesión en su cuenta";
            string body = "Cliente: " + cliente.Nombre + ", se le comunica que hubo un inicio de sesión en su cuenta, fecha: " + DateTime.Now + "\n" +
                "Si usted no inicio sesión, favor hacer cambio de su contraseña actual en nuestra pagina web";

            await _correoService.EnviarCorreo(emailReceiver, subject, body);
            return Ok();

        }


        //enviarCorreoInicioSesionCliente
        // cuando el cliente inicia sesion
        // POST api/<CorreoController>
        [HttpPost]
        public async Task<ActionResult> enviarCorreoCambioContrasenaCompletadoCliente(Cliente cliente)
        {
            string emailReceiver = cliente.Correo;

            string subject = "Su contraseña se ha cambiado con éxito";
            string body = "Cliente: " + cliente.Nombre + ", se le comunica que su contraseña fue cambiada con éxito, fecha: " + DateTime.Now + "\n" +
                "Ahora puede iniciar sesión con la nueva contraseña";

            await _correoService.EnviarCorreo(emailReceiver, subject, body);
            return Ok();

        }















        /* // methodos genericos que hace el controller de tipo API

                // GET: api/<CorreoController>
                [HttpGet]
                public IEnumerable<string> Get()
                {
                    return new string[] { "value1", "value2" };
                }

                // GET api/<CorreoController>/5
                [HttpGet("{id}")]
                public string Get(int id)
                {
                    return "value";
                }


                // PUT api/<CorreoController>/5
                [HttpPut("{id}")]
                public void Put(int id, [FromBody] string value)
                {
                }

                // DELETE api/<CorreoController>/5
                [HttpDelete("{id}")]
                public void Delete(int id)
                {
                }




                */
    }

}
