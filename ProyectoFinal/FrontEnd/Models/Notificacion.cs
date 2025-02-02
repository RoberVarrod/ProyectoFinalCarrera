using System;
using System.Collections.Generic;

namespace FrontEnd.Models;

public partial class Notificacion
{
    public int IdNotificacion { get; set; }

    public string Titulo { get; set; } = null!;

    public string Cuerpo { get; set; } = null!;

    public string Tipo { get; set; } = null!;

    public DateTime FechaRegistro { get; set; }

    public bool Leida { get; set; }

    public int? IdUsuario { get; set; }

    public int? IdCliente { get; set; }

    public virtual Cliente? IdClienteNavigation { get; set; }

    public virtual Usuario? IdUsuarioNavigation { get; set; }
}
