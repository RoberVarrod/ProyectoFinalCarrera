using System;
using System.Collections.Generic;

namespace FrontEnd.Models;

public partial class Pago
{
    public int IdPago { get; set; }

    public string Total { get; set; } = null!;

    public string? Descripcion { get; set; }

    public DateTime? FechaPago { get; set; }

    public string PagoEstado { get; set; } = null!;

    public string PagoMetodo { get; set; } = null!;

    public int IdPaquete { get; set; }

    public int IdCliente { get; set; }

    public virtual Cliente IdClienteNavigation { get; set; } = null!;

    public virtual Paquete IdPaqueteNavigation { get; set; } = null!;
}
