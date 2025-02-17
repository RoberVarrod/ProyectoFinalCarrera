using System;
using System.Collections.Generic;

namespace FrontEnd.Models;

public partial class Paquete
{
    public int IdPaquete { get; set; }

    public string NumeroRegistro { get; set; } = null!;

    public string? Nombre { get; set; }

    public long? Precio { get; set; }

    public string? PaqueteLargo { get; set; }

    public string? PaqueteAncho { get; set; }

    public string? PaqueteAlto { get; set; }

    public string? TipoPaquete { get; set; }

    public string? TipoEntrega { get; set; }

    public string? Descripcion { get; set; }

    public string? EstadoPago { get; set; }

    public string? EstadoRuta { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public DateTime? FechaEntrega { get; set; }

    public DateTime? FechaEntregaEstimada { get; set; }

    public string? DireccionEntrega { get; set; }

    public bool? RetiroSucursal { get; set; }

    public int? IdSucursal { get; set; }

    public int? IdUsuario { get; set; }

    public int? IdCliente { get; set; }

    public virtual ICollection<HistorialCambiosPaquete> HistorialCambiosPaquetes { get; set; } = new List<HistorialCambiosPaquete>();

    public virtual Cliente? IdClienteNavigation { get; set; }

    public virtual Sucursal? IdSucursalNavigation { get; set; }

    public virtual Usuario? IdUsuarioNavigation { get; set; }

    public virtual ICollection<Pago> Pagos { get; set; } = new List<Pago>();
}
