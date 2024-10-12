using Models.Entidades;

namespace Data.Interfaces
{
    public interface ITokenServicio
    {
       Task<string> crearToken(UsuarioAplicacion usuario); // (Usuario usuario); wpineda implementar identity 
    }
}
