using Data.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Models.Entidades;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Data.Servicios
{
    public class TokenServicio : ITokenServicio
    {
        private readonly UserManager<UsuarioAplicacion> _userManager;
        private readonly SymmetricSecurityKey _key;

        public TokenServicio(IConfiguration configuration, UserManager<UsuarioAplicacion> userManager)
        {
            _key = new SymmetricSecurityKey( Encoding.UTF8.GetBytes(configuration["TokenKey"]) );
            _userManager = userManager;
        }

        public async Task<string> crearToken(UsuarioAplicacion usuario) //(Usuario usuario)wpineda implementar identity 
        {
            var claims = new List<Claim>
            {
                 new Claim(JwtRegisteredClaimNames.NameId, usuario.UserName)  
                 // new Claim(JwtRegisteredClaimNames.NameId, usuario.Username) wpineda implementar identity 
            };

            var roles = await _userManager.GetRolesAsync(usuario);
            claims.AddRange(roles.Select(rol => new Claim(ClaimTypes.Role, rol)));

            var creds = new SigningCredentials( _key, SecurityAlgorithms.HmacSha512Signature );
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity( claims ),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
