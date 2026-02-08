using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MVC.interfaces;
using MVC.Models;
using Newtonsoft.Json;

namespace MVC.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    private readonly ILoginRegisterRepository _log;

    public HomeController(ILogger<HomeController> logger, ILoginRegisterRepository log)
    {
        _logger = logger;
        _log = log;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    // public IActionResult Login(vm_Login user)
    // {
    //     try
    //     {
    //         if (!ModelState.IsValid)
    //         {
    //             var firstError = ModelState.Values.SelectMany(v => v.Errors)
    //                                               .FirstOrDefault()?.ErrorMessage;
    //             return Json(new { success = false, message = firstError ?? "Invalid data." });
    //         }

    //         var userdata = _log.Login(user);
    //         // if (userdata == null)
    //         // {
    //         //     return Json(new { success = false, message = "Invalid UserEmmail and Password???" });
    //         // }
    //         if (userdata.Status == "InActive" && userdata.Role == "user")
    //         {
    //             return Json(new { success = false, message = "Your Account is InActive now..." });
    //         }
    //         else
    //         {
    //             string userJson = JsonConvert.SerializeObject(userdata);
    //             HttpContext.Session.SetString("UserData", userJson);
    //             return Json(new { success = true, message = "Login Successfully done!!!", userdata = userdata });
    //         }
    //     }
    //     catch (Exception ex)
    //     {
    //         return Json(new { success = false, message = "---->Login Controller Error :" + ex.Message });
    //     }
    // }

 
    [HttpPost]
    public IActionResult Login(vm_Login user)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage).ToList();

                return Json(new { success = false, message = string.Join("<br>", errors) });
            }

            var userdata = _log.Login(user);

            if (userdata == null)
                return Json(new { success = false, message = "Invalid Email or Password." });

            if (userdata.Status == "InActive" && userdata.Role == "user")
                return Json(new { success = false, message = "Your account is inactive." });

            string userJson = JsonConvert.SerializeObject(userdata);
            HttpContext.Session.SetString("UserData", userJson);

            return Json(new { success = true, message = "Login Successful!", userdata = userdata });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "Login Controller Error: " + ex.Message });
        }
    }

    public IActionResult LogOut()
    {
        string userJson = HttpContext.Session.GetString("UserData");

        // Optional: Deserialize it back to an object
        if (!string.IsNullOrEmpty(userJson))
        {
            HttpContext.Session.Remove("UserData");
            return RedirectToAction("Login", "Home");
        }
        return RedirectToAction("Index", "Home");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
