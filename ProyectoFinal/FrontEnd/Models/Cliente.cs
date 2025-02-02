using System;
using System.Collections.Generic;

namespace FrontEnd.Models;

public partial class Cliente
{
    public int IdCliente { get; set; }

    public string Nombre { get; set; } = null!;

    public string PrimerApellido { get; set; } = null!;

    public string SegundoApellido { get; set; } = null!;

    public string TelefonoPrincipal { get; set; } = null!;

    public string? TelefonoSecundario { get; set; }

    public string Correo { get; set; } = null!;

    public string Cedula { get; set; } = null!;

    public string Contrasena { get; set; } = null!;

    public int? IdRol { get; set; }

    public string? Provincia { get; set; }

    public string? Canton { get; set; }

    public string? Distrito { get; set; }

    public string? CodigoPostal { get; set; }

    public string? Direccion { get; set; }

    public string? FotoPerfil { get; set; }

    public virtual Role? IdRolNavigation { get; set; }

    public virtual ICollection<Notificacion> Notificacions { get; set; } = new List<Notificacion>();

    public virtual ICollection<Pago> Pagos { get; set; } = new List<Pago>();

    public virtual ICollection<Paquete> Paquetes { get; set; } = new List<Paquete>();
}
