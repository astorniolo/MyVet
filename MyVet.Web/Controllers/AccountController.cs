using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MyVet.Web.Helpers;
using MyVet.Web.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;


namespace MyVet.Web.Controllers
{
    public class AccountController: Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly IConfiguration _configuration;

        public AccountController(
            IUserHelper userHelper,
            IConfiguration configuration)
        {
            _userHelper = userHelper;
            _configuration = configuration;
            // el controlador necesita el userhelper que se inyecta
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            // el model state se hereda de controller y maneja el estado del controlador.....
            if (ModelState.IsValid) // es valido si cumple las data anotation...
            {
                var result = await _userHelper.LoginAsync(model);
                if (result.Succeeded)
                {
                    if (Request.Query.Keys.Contains("ReturnUrl"))
                    {
                        return Redirect(Request.Query["ReturnUrl"].First());
                    }

                    return RedirectToAction("Index", "Home");
                }
                // manejo de errores personalizados del modelstate......
                ModelState.AddModelError(string.Empty, "User or PassWord not valid.");
                model.Password = string.Empty;
            }
            // Sino es valido lo retornamos a la vista con el mismo modelo para que no pierda lo ya digitado
            //metodos pa loguearte o desloguearte usamos el USERHELPER
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _userHelper.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> CreateToken([FromBody] LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.Username);
                if (user != null)
                {
                    var result = await _userHelper.ValidatePasswordAsync(
                        user,
                        model.Password);

                    if (result.Succeeded)
                    {
                        var claims = new[]
                        {
                            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                        };
                        //encrytamos
                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:Key"]));
                        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                        var token = new JwtSecurityToken(
                            _configuration["Tokens:Issuer"],
                            _configuration["Tokens:Audience"],
                            claims,
                            expires: DateTime.UtcNow.AddMonths(4), //tiempo de expiracion: depende de la expiracion....si es app bancaria expirara cada 3 minutos,
                                                                  // si es una app facebokk cada año,,,,,, si es en dias AddDays(15) .... si es meses AddMonth(4)
                                                                  // si no quiero que expire nunca AddYear(999)
                            signingCredentials: credentials);
                        var results = new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(token),
                            expiration = token.ValidTo
                        };

                        return Created(string.Empty, results);
                    }
                }
            }

            return BadRequest();
        }

    }

}
