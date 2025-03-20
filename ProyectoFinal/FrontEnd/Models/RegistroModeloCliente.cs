using FrontEnd.Models;

namespace FrontEnd.Models
{
    public class RegistroModeloCliente
    {
        public string Nombre { get; set; } = null!;
        public string PrimerApellido { get; set; } = null!;
        public string SegundoApellido { get; set; } = null!;
        public string TelefonoPrincipal { get; set; } = null!;
        public string? TelefonoSecundario { get; set; }
        public string Correo { get; set; } = null!;
        public string Cedula { get; set; } = null!;
        public string Contrasena { get; set; } = null!;
        public int IdRol { get; set; } = 1; // rol regular por defecto
        public string? Provincia { get; set; }
        public string? Canton { get; set; }
        public string? Distrito { get; set; }
        public string? CodigoPostal { get; set; }
        public string? Direccion { get; set; }
        public string FotoPerfil { get; set; } = "/imagenesperfil/profile.jpg"; // foro de perfil por defecto

    }
}
