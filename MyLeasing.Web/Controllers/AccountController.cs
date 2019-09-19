using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyLeasing.Web.Data;
using MyLeasing.Web.Data.Entities;
using MyLeasing.Web.Helpers;
using MyLeasing.Web.Models;

namespace MyLeasing.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly ICombosHelpers _combosHelpers;
        private readonly DataContext _dataContext;

        public AccountController(IUserHelper userHelper, ICombosHelpers combosHelpers, DataContext dataContext)
        {
            _userHelper    = userHelper;
            _combosHelpers = combosHelpers;
            _dataContext   = dataContext;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                Microsoft.AspNetCore.Identity.SignInResult result = await _userHelper.LoginAsync(model);

                if (result.Succeeded)
                {
                    if (Request.Query.Keys.Contains("ReturnUrl"))
                    {
                        return Redirect(Request.Query["ReturnUrl"].First());
                    }

                    return RedirectToAction("Index", "Home");
                }
            }

            ModelState.AddModelError(string.Empty, "User or password incorect.");
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _userHelper.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult NotAuthorized()
        {
            return View();
        }

        public IActionResult Register()
        {
            var view = new AddUserViewModel
            {
                Roles = _combosHelpers.GetComboRoles()
            };

            return View(view);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(AddUserViewModel view)
        {
            if (ModelState.IsValid)
            {
                var role = "Owner";
                if (view.RoleId == 1)
                {
                    role = "Lessee";
                }

                var user = await _userHelper.AddUser(view, role);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "This email is already used.");
                    return View(view);
                }

                if (view.RoleId == 1)
                {
                    var lessee = new Lessee
                    {
                        Contracts = new List<Contract>(),
                        User      = user
                    };

                    _dataContext.Lessees.Add(lessee);
                    await _dataContext.SaveChangesAsync();
                }
                else
                {
                    var owner = new Owner
                    {
                        Contracts  = new List<Contract>(),
                        Properties = new List<Property>(),
                        User       = user
                    };

                    _dataContext.Owners.Add(owner);
                    await _dataContext.SaveChangesAsync();
                }

                var loginViewModel = new LoginViewModel
                {
                    Password   = view.Password,
                    RememberMe = false,
                    Username   = view.Username
                };

                var result2 = await _userHelper.LoginAsync(loginViewModel);

                if (result2.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            return View(view);
        }


    }
}
