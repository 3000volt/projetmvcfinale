using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using projetmvcfinale.Models;
using projetmvcfinale.Models.Authentification;

namespace projetmvcfinale.Controllers
{
   
        //[Authorize(Roles = "Super Admin")]
        public class RolesController : Controller
        {
        //propriétés du controlleur
            private readonly RoleManager<LoginRole> _roleManager;
            private readonly ILogger _logger;
            public RolesController(

               RoleManager<LoginRole> roleManager,

                ILogger<LoginController> logger)
            {
                _roleManager = roleManager;
                _logger = logger;
            }


            // GET: Roles
            public ActionResult List()
            {
                return View(this._roleManager.Roles.ToList());
            }

            // GET: Roles/Create
            public ActionResult Create()
            {
                return View();
            }

            // POST: Roles/Create
            [HttpPost]

            public async Task<ActionResult> Create(LoginRole role)
            {
                try
                {

                    var result = await this._roleManager.CreateAsync(role);
                    if (result.Succeeded)
                    {
                        return RedirectToAction(nameof(List));
                    }
                    else
                    {
                        return View();
                    }
                }
                catch
                {
                    return View();
                }
            }


            // GET: Roles/Delete/5
            public async Task<ActionResult> Delete(string id)
            {
                LoginRole role = await this._roleManager.FindByIdAsync(id);
                var result = await this._roleManager.DeleteAsync(role);
                return RedirectToAction(nameof(List));
            }

        }

    
}