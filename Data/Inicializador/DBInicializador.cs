using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models.Entidades;

namespace Data.Inicializador
{
    public class DBInicializador : IdbInicializador
    {
        public readonly AplicationDbContext _db;
        public readonly UserManager<UsuarioAplicacion> _userManager;
        public readonly RoleManager<RolAplicacion> _rolManager;

        public DBInicializador(AplicationDbContext db, UserManager<UsuarioAplicacion> userManager, RoleManager<RolAplicacion> rolManager)
        {
            _db = db;
            _userManager = userManager;
            _rolManager = rolManager;
        }

        public async void Inicializar()
        {
            try
            {
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            //Datos Iniciales
            //Crear Roles
            if (_db.Roles.Any(x => x.Name == "Admin")) return;

            _rolManager.CreateAsync(new RolAplicacion { Name = "admin" }).GetAwaiter().GetResult();
            _rolManager.CreateAsync(new RolAplicacion { Name = "agendador" }).GetAwaiter().GetResult();
            _rolManager.CreateAsync(new RolAplicacion { Name = "doctor" }).GetAwaiter().GetResult();

            //Crear Usuario Administrador
            var usuario = new UsuarioAplicacion
            {
                Nombres = "Wilbert",
                Apellidos = "Pineda",
                Email = "administrador@gmail.com",
                UserName = "administrador"
            };
            _userManager.CreateAsync(usuario, "Admin123").GetAwaiter().GetResult();

            UsuarioAplicacion _usuarioAdmin = _db.UsuarioAplicacion.Where(x => x.UserName == "administrador").FirstOrDefault();
            _userManager.AddToRoleAsync(_usuarioAdmin, "admin").GetAwaiter().GetResult();
        }
    }
}