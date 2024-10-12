using Microsoft.AspNetCore.Identity;

namespace Models.Entidades
{
    public class RolAplicacion : IdentityRole<int>
    {
        public ICollection<RolUsuarioAplicacion> rolUsuarios { get; set; }
    }
}
