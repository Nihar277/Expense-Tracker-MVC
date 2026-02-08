using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using MVC.implement;
using MVC.interfaces;
using MVC.Models;

namespace MVC.Controllers
{
    [Route("[controller]")]
    public class ChangePasswordController : Controller
    {
        private readonly IUserRepository _repo;

        public ChangePasswordController(IUserRepository repo)
        {
            _repo = repo;
        }

        // GET : Change password
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        //POST: Change password 
        [HttpPost]
        public JsonResult ChangePassword([FromBody]vm_ChangePassword model)
        {
            string userdat = HttpContext.Session.GetString("UserData");

            var userData = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(userdat);

            int userid = userData["UserID"].GetInt32();
            if (userid == null)
            {
                return Json(new { success = false, message = "Session expired. Please Login again" });
            }

            // int userId = 17; // directly use 17 for testing,remember to comment this 

            if(!ModelState.IsValid)
            {
                return Json(new{success = false,message= "Please fill all required fields properly."});
            }
            if(model.NewPassword != model.ConfirmPassword)
            {
                return Json(new { success = false, message = "New password and Confirm password do not match." });
            }

        

            //uncomment this 

            bool result = _repo.ChangePassword(userid, model.CurrentPassword, model.NewPassword);

            if(result)
            {
                return Json(new{success=true, message = "Password changed successfully."});
            }
            else
            {
                return Json(new{success=false, message="Invalid Current password"});
            }
        }
    }
}