using System;
using System.Collections.Generic;

namespace FrontEnd.Models;

public partial class HistorialCambiosPaquete
{
    public int IdHistorialCambiosPaquete { get; set; }

    public int Sequencia { get; set; }

    public string Descripcion { get; set; } = null!;

    public string Informacion { get; set; } = null!;

    public DateTime FechaRegistro { get; set; }

    public int IdPaquete { get; set; }

    public virtual Paquete IdPaqueteNavigation { get; set; } = null!;
}
