using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace FrontEnd.Models;

public partial class PaqueteUsuarioSucursal
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

    public string? PaqueteUsuarioNombre { get; set; }

    public string? PaqueteSucursalNombre { get; set; }

    public int? IdSucursal { get; set; }

    public int? IdUsuario { get; set; }

    public int? IdCliente { get; set; }

    public string? PaqueteUsuarioProvincia { get; set; }

    public string? PaqueteUsuarioCanton { get; set; }

    public string? PaqueteUsuarioDistrito { get; set; }

    public string? PaqueteUsuarioCodigoPostal { get; set; }

    public string? PaqueteUsuarioDireccion { get; set; }

    public DbSet<PaqueteUsuarioSucursal> PaquetesUsuarioSucursal { get; set; }



}
