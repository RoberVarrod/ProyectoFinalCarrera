using Microsoft.AspNetCore.Mvc;
using FrontEnd.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

//Este es un controller de tipo API.. para usar el API de google de correos.


namespace FrontEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CorreoController : ControllerBase
    {

        private readonly ICorreoService _correoService;


        public CorreoController(ICorreoService _correoService)
        {
            this._correoService = _correoService;
        }




        // POST api/<CorreoController>
        [HttpPost]
        public async Task<ActionResult> EnviarCorreo(string emailReceiver, string subject, string body)
        {

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
