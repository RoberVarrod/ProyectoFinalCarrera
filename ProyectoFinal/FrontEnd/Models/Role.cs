using System;
using System.Collections.Generic;

namespace FrontEnd.Models;

public partial class Role
{
    public int IdRol { get; set; }

    public string Rol { get; set; } = null!;

    public virtual ICollection<Cliente> Clientes { get; set; } = new List<Cliente>();

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
