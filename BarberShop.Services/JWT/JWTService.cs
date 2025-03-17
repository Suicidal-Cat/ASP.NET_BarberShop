using BarberShop.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BarberShop.Services.JWT
{
	public class JWTService
	{
		private readonly IConfiguration _config;
        private readonly UserManager<IdentityUser> userManager;
        private readonly SymmetricSecurityKey _jwtKey;

		public JWTService(IConfiguration config,UserManager<IdentityUser> userManager)
        {
			this._config = config;
            this.userManager = userManager;
            this._jwtKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Key"]));

		}
        public async Task<string> CreateJWT(ApplicationUser user)
		{
			var userClaims = new List<Claim>
			{
				new Claim(ClaimTypes.NameIdentifier,user.Id),
				new Claim(ClaimTypes.Email,user.Email),
				new Claim(ClaimTypes.GivenName,user.FirstName),
				new Claim(ClaimTypes.Surname,user.LastName),
			};

			var roles = await userManager.GetRolesAsync(user);
			userClaims.AddRange(roles.Select(role=>new Claim(ClaimTypes.Role,role)));


			var credentials = new SigningCredentials(_jwtKey, SecurityAlgorithms.HmacSha256Signature);

			var tokenDescriptior = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(userClaims),
				Expires = DateTime.UtcNow.AddDays(int.Parse(_config["JWT:ExpiersInDays"])),
				SigningCredentials = credentials,
				Issuer = _config["JWT:Issuer"],
			};

			var tokenHandler = new JwtSecurityTokenHandler();
			var jwt = tokenHandler.CreateToken(tokenDescriptior);
			return tokenHandler.WriteToken(jwt);
		}
	}
}
