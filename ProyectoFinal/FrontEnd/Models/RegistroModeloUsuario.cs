using FrontEnd.Models;

namespace FrontEnd.Models
{
    public class RegistroModeloUsuario
    {

        public string Nombre { get; set; }
        public string PrimerApellido { get; set; }
        public string SegundoApellido { get; set; }
        public string TelefonoPrincipal { get; set; }
        public string Correo { get; set; }
        public string Cedula { get; set; }
        public string Contrasena { get; set; }
        public string Direccion { get; set; }
        public string Oficina { get; set; }
        public int IdRol { get; set; } = 1; // rol regular por defecto
        public int IdSucursal { get; set; } = 1; // sucursal virtual por defecto
        public string FotoPerfil { get; set; } = "/imagenesperfil/profile.jpg"; // foro de perfil por defecto

    }
}
