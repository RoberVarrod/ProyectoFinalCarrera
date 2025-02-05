using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace FrontEnd.Models;

public partial class ProyectoPaqueteriaContext : DbContext
{
    public ProyectoPaqueteriaContext()
    {
    }

    public ProyectoPaqueteriaContext(DbContextOptions<ProyectoPaqueteriaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cliente> Clientes { get; set; }

    public virtual DbSet<HistorialCambiosPaquete> HistorialCambiosPaquetes { get; set; }

    public virtual DbSet<Notificacion> Notificacions { get; set; }

    public virtual DbSet<Pago> Pagos { get; set; }

    public virtual DbSet<Paquete> Paquetes { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Sucursal> Sucursals { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

  /*  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-82180HC\\SQLEXPRESS;Database=ProyectoPaqueteria;Integrated Security=True;Trusted_Connection=True; TrustServerCertificate=True;");
  */
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.IdCliente).HasName("PK_id_cliente");

            entity.ToTable("Cliente");

            entity.Property(e => e.IdCliente).HasColumnName("id_cliente");
            entity.Property(e => e.Canton)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("canton");
            entity.Property(e => e.Cedula)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("cedula");
            entity.Property(e => e.CodigoPostal)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("codigo_postal");
            entity.Property(e => e.Contrasena)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("contrasena");
            entity.Property(e => e.Correo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("correo");
            entity.Property(e => e.Direccion)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("direccion");
            entity.Property(e => e.Distrito)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("distrito");
            entity.Property(e => e.FotoPerfil)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("foto_perfil");
            entity.Property(e => e.IdRol).HasColumnName("id_rol");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.PrimerApellido)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("primer_apellido");
            entity.Property(e => e.Provincia)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("provincia");
            entity.Property(e => e.SegundoApellido)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("segundo_apellido");
            entity.Property(e => e.TelefonoPrincipal)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("telefono_principal");
            entity.Property(e => e.TelefonoSecundario)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("telefono_secundario");

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.Clientes)
                .HasForeignKey(d => d.IdRol)
                .HasConstraintName("FK_id_rol_Cliente");
        });

        modelBuilder.Entity<HistorialCambiosPaquete>(entity =>
        {
            entity.HasKey(e => e.IdHistorialCambiosPaquete).HasName("PK_id_historial_cambios_paquete");

            entity.ToTable("Historial_Cambios_Paquete");

            entity.Property(e => e.IdHistorialCambiosPaquete).HasColumnName("id_historial_cambios_paquete");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.FechaRegistro)
                .HasColumnType("datetime")
                .HasColumnName("fecha_registro");
            entity.Property(e => e.IdPaquete).HasColumnName("id_paquete");
            entity.Property(e => e.Informacion)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("informacion");
            entity.Property(e => e.Sequencia).HasColumnName("sequencia");

            entity.HasOne(d => d.IdPaqueteNavigation).WithMany(p => p.HistorialCambiosPaquetes)
                .HasForeignKey(d => d.IdPaquete)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_id_paquete_Historial_Cambios_Paquete");
        });

        modelBuilder.Entity<Notificacion>(entity =>
        {
            entity.HasKey(e => e.IdNotificacion).HasName("PK_id_notificacion");

            entity.ToTable("Notificacion");

            entity.Property(e => e.IdNotificacion).HasColumnName("id_notificacion");
            entity.Property(e => e.Cuerpo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("cuerpo");
            entity.Property(e => e.FechaRegistro)
                .HasColumnType("datetime")
                .HasColumnName("fecha_registro");
            entity.Property(e => e.IdCliente).HasColumnName("id_cliente");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.Leida).HasColumnName("leida");
            entity.Property(e => e.Tipo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("tipo");
            entity.Property(e => e.Titulo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("titulo");

            entity.HasOne(d => d.IdClienteNavigation).WithMany(p => p.Notificacions)
                .HasForeignKey(d => d.IdCliente)
                .HasConstraintName("FK_id_cliente_Notificacion");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Notificacions)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("FK_id_usuario_Notificacion");
        });

        modelBuilder.Entity<Pago>(entity =>
        {
            entity.HasKey(e => e.IdPago).HasName("PK_id_pago");

            entity.ToTable("Pago");

            entity.Property(e => e.IdPago).HasColumnName("id_pago");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.FechaPago)
                .HasColumnType("datetime")
                .HasColumnName("fecha_pago");
            entity.Property(e => e.IdCliente).HasColumnName("id_cliente");
            entity.Property(e => e.IdPaquete).HasColumnName("id_paquete");
            entity.Property(e => e.PagoEstado)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("pago_estado");
            entity.Property(e => e.PagoMetodo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("pago_metodo");
            entity.Property(e => e.Total)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("total");

            entity.HasOne(d => d.IdClienteNavigation).WithMany(p => p.Pagos)
                .HasForeignKey(d => d.IdCliente)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_id_cliente_Pago");

            entity.HasOne(d => d.IdPaqueteNavigation).WithMany(p => p.Pagos)
                .HasForeignKey(d => d.IdPaquete)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_id_paquete_Pago");
        });

        modelBuilder.Entity<Paquete>(entity =>
        {
            entity.HasKey(e => e.IdPaquete).HasName("PK_id_paquete");

            entity.ToTable("Paquete");

            entity.Property(e => e.IdPaquete).HasColumnName("id_paquete");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.DireccionEntrega)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("direccion_entrega");
            entity.Property(e => e.EstadoPago)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("estado_pago");
            entity.Property(e => e.EstadoRuta)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("estado_ruta");
            entity.Property(e => e.FechaEntrega)
                .HasColumnType("datetime")
                .HasColumnName("fecha_entrega");
            entity.Property(e => e.FechaEntregaEstimada)
                .HasColumnType("datetime")
                .HasColumnName("fecha_entrega_estimada");
            entity.Property(e => e.FechaRegistro)
                .HasColumnType("datetime")
                .HasColumnName("fecha_registro");
            entity.Property(e => e.IdCliente).HasColumnName("id_cliente");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.NumeroRegistro)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("numero_registro");
            entity.Property(e => e.PaqueteAlto)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("paquete_alto");
            entity.Property(e => e.PaqueteAncho)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("paquete_ancho");
            entity.Property(e => e.PaqueteLargo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("paquete_largo");
            entity.Property(e => e.Precio).HasColumnName("precio");
            entity.Property(e => e.RetiroSucursal).HasColumnName("retiro_sucursal");
            entity.Property(e => e.TipoEntrega)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("tipo_entrega");
            entity.Property(e => e.TipoPaquete)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("tipo_paquete");

            entity.HasOne(d => d.IdClienteNavigation).WithMany(p => p.Paquetes)
                .HasForeignKey(d => d.IdCliente)
                .HasConstraintName("FK_id_cliente_Paquete");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Paquetes)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_id_usuario_Paquete");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.IdRol).HasName("PK_id_rol");

            entity.Property(e => e.IdRol).HasColumnName("id_rol");
            entity.Property(e => e.Rol)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("rol");
        });

        modelBuilder.Entity<Sucursal>(entity =>
        {
            entity.HasKey(e => e.IdSucursal).HasName("PK_id_sucursal");

            entity.ToTable("Sucursal");

            entity.Property(e => e.IdSucursal).HasColumnName("id_sucursal");
            entity.Property(e => e.Direccion)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("direccion");
            entity.Property(e => e.Horario)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("horario");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.Telefono)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("telefono");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("PK_id_usuario");

            entity.ToTable("Usuario");

            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.Cedula)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("cedula");
            entity.Property(e => e.Contrasena)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("contrasena");
            entity.Property(e => e.Correo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("correo");
            entity.Property(e => e.Direccion)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("direccion");
            entity.Property(e => e.FotoPerfil)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("foto_perfil");
            entity.Property(e => e.IdRol).HasColumnName("id_rol");
            entity.Property(e => e.IdSucursal).HasColumnName("id_sucursal");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.Oficina)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("oficina");
            entity.Property(e => e.PrimerApellido)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("primer_apellido");
            entity.Property(e => e.SegundoApellido)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("segundo_apellido");
            entity.Property(e => e.TelefonoPrincipal)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("telefono_principal");

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdRol)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_id_rol_Usuario");

            entity.HasOne(d => d.IdSucursalNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdSucursal)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_id_sucursal_Usuario");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
