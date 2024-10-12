using Microsoft.AspNetCore.Identity;

namespace Models.Entidades
{
    public class RolUsuarioAplicacion : IdentityUserRole<int>
    {
        public UsuarioAplicacion usuarioAplicacion { get; set; }
        public RolAplicacion roleAplicacion { get; set; }
    }
}
