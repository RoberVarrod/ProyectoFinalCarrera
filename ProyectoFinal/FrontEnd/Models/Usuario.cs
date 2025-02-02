using System;
using System.Collections.Generic;

namespace FrontEnd.Models;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public string Nombre { get; set; } = null!;

    public string PrimerApellido { get; set; } = null!;

    public string SegundoApellido { get; set; } = null!;

    public string TelefonoPrincipal { get; set; } = null!;

    public string Correo { get; set; } = null!;

    public string Cedula { get; set; } = null!;

    public string Contrasena { get; set; } = null!;

    public string? Direccion { get; set; }

    public string? Oficina { get; set; }

    public string? FotoPerfil { get; set; }

    public int IdRol { get; set; }

    public int IdSucursal { get; set; }

    public virtual Role IdRolNavigation { get; set; } = null!;

    public virtual Sucursal IdSucursalNavigation { get; set; } = null!;

    public virtual ICollection<Notificacion> Notificacions { get; set; } = new List<Notificacion>();

    public virtual ICollection<Paquete> Paquetes { get; set; } = new List<Paquete>();
}
