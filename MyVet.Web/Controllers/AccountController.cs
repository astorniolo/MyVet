using Microsoft.AspNetCore.Mvc;
using MyVet.Web.Helpers;
using MyVet.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace MyVet.Web.Controllers
{
    public class AccountController: Controller
    {
        private readonly IUserHelper _userHelper;

        public AccountController(
            IUserHelper userHelper)
        {
            _userHelper = userHelper;
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
    }

}
