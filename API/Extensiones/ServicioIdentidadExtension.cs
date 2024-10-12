using Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Models.Entidades;
using System.Text;

namespace API.Extensiones
{
    public static class ServicioIdentidadExtension
    {
        public static IServiceCollection AgregaServiciosIdentidad(this IServiceCollection services, IConfiguration config)
        {
            services.AddIdentityCore<UsuarioAplicacion>(opt =>
            {
                opt.Password.RequireNonAlphanumeric = false;
            }).AddRoles<RolAplicacion>()
              .AddRoleManager<RoleManager<RolAplicacion>>()
              .AddEntityFrameworkStores<AplicationDbContext>();  // wpineda implementar identity

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme) /// servicio de AUTENTICACION
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"])),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                };
            });

            services.AddAuthorization(); ////Agregar politica

            return services.AddAuthorization(opt=>
            {
                opt.AddPolicy("adminrol", policy => policy.RequireRole("admin"));
                opt.AddPolicy("adminagendadorrol", policy => policy.RequireRole("admin","agendador"));
                opt.AddPolicy("admindoctorrol", policy => policy.RequireRole("admin", "agendador"));
            });
        }
    }
}
