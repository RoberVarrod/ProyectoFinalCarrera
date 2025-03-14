using Microsoft.AspNetCore.Mvc;
using FrontEnd.Services;
using FrontEnd.Models;

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


        // Cuando se registra paquete
        // POST api/<CorreoController>
        [HttpPost]
        public async Task<ActionResult> enviarCorreoActualizarPaquete(Paquete paquete)
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
