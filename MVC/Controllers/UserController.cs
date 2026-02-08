using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MVC.interfaces;
using MVC.Models;
using MVC.Repositories;
using MVC.implement;
using System.Text.Json;


namespace MVC.Controllers
{
    // [Route("[controller]")]
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private readonly IWebHostEnvironment _environment;

        private readonly IUserRepository _userRepo;
        private readonly IUserGraphRepositories _userGraphRepo;

        public UserController(ILogger<UserController> logger, IWebHostEnvironment environment, IUserRepository userRepo, IUserGraphRepositories userGraph)
        {
            _logger = logger;
            _environment = environment;
            _userRepo = userRepo;
            _userGraphRepo = userGraph;
        }

        public IActionResult Register()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Register(t_Users user)
        {
            if (ModelState.IsValid)
            {
                if (user.User_ProfileImage != null && user.User_ProfileImage.Length > 0)
                {
                    var fileName = user.Email + Path.GetExtension(user.User_ProfileImage.FileName);
                    var filePath = Path.Combine(_environment.WebRootPath, "profile_images");

                    if (!Directory.Exists(filePath))
                    {
                        Directory.CreateDirectory(filePath);
                    }

                    user.ProfileImage = fileName;
                    using (var stream = new FileStream(Path.Combine(filePath, fileName), FileMode.Create))
                    {
                        user.User_ProfileImage.CopyTo(stream);
                    }
                }
                else
                {
                    user.ProfileImage = "";
                }

                var status = await _userRepo.Register(user);
                if (status == 1)
                {
                    // ViewData["message"] = "User Registered";
                    return Ok(new { success = true, message = "User Registered" });
                }
                else if (status == 0)
                {
                    return BadRequest(new { success = false, message = "User Already Registered" });
                }
                else
                {
                    return BadRequest(new { success = false, message = "There was some error while Registration" });
                }

            }
            return View(user);
        }

        [HttpGet]
        public IActionResult UserGraph()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> getUserGraph(int id)
        {
            var result = _userGraphRepo.getAllTransactionByUserId(id);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> getAllTransactionByMonth(int id)
        {
            var result = _userGraphRepo.getAllTransactionByMonth(id);
            return Ok(result);
        }

        [HttpGet]
        public JsonResult GetPredictedExpense(int userId)
        {
            string userdat = HttpContext.Session.GetString("UserData");

            var userData = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(userdat);

            int userid = userData["UserID"].GetInt32();
            // Get user's monthly transactions
            var monthlyData = _userGraphRepo.getAllTransactionByMonth(userid);

            // If no data, return 0
            if (monthlyData == null || monthlyData.Count == 0)
                return Json(new { predictedExpense = 0 });

            // Convert to list of amounts safely
            var amounts = monthlyData
                .Where(m => m.Amount > 0)
                .Select(m => (double)m.Amount)
                .ToList();

            if (amounts.Count == 0)
                return Json(new { predictedExpense = 0 });

            // Basic average
            double avg = amounts.Average();

            // Calculate trend (growth)
            double growthRate = 0;
            if (amounts.Count > 1 && amounts.First() != 0)
            {
                growthRate = (amounts.Last() - amounts.First()) / amounts.First();
            }

            // Weighted average with a small trend factor (smooth)
            double predicted = avg * (1 + (growthRate / 3));

            // Return safely rounded value
            return Json(new { predictedExpense = Math.Round(predicted, 2) });
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }

        //update profile
        // GET: User/Edit
        public IActionResult Edit()
        {
            // ⚠️ Temporary: hardcode a user ID for testing
            // HttpContext.Session.SetInt32("UserID", 17);
            string userdat = HttpContext.Session.GetString("UserData");

            var userData = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(userdat);

            int userid = userData["UserID"].GetInt32();
            if (userid == null)
                return RedirectToAction("Login", "Account");

            var user = _userRepo.GetById(userid);
            if (user == null)
                return RedirectToAction("Login", "Account");
            var vm = new vm_UpdateProfile
            {
                UserID = user.UserID,
                Name = user.Name,
                Email = user.Email,
                Gender = user.Gender,
                ProfileImage = user.ProfileImage
            };
            ViewBag.Genders = new List<string> { "Male", "Female", "Other" };

            return View(vm);
        }

        // POST
        [HttpPost]
        public IActionResult Edit(vm_UpdateProfile model)
        {
            string userdat = HttpContext.Session.GetString("UserData");

            var userData = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(userdat);

            int userid = userData["UserID"].GetInt32();
            if (userid == null || userid != model.UserID)
                return Json(new { success = false, message = "Unauthorized access!" });

            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Please fill all required fields." });

            try
            {
                var user = _userRepo.GetById(userid);
                if (user == null)
                    return Json(new { success = false, message = "User not found!" });

                // update fields
                user.Name = model.Name;
                // user.Email = model.Email;
                user.Gender = model.Gender;
                // user.Status = model.Status;
                // user.Role = model.Role;


                // handle image upload
                if (model.User_ProfileImage != null)
                {
                    string wwwRootPath = _environment.WebRootPath;
                     string uploadFolder = Path.Combine(wwwRootPath, "profile_images");
                            if (!Directory.Exists(uploadFolder))
                                Directory.CreateDirectory(uploadFolder);
                    string fileName = Path.GetFileNameWithoutExtension(model.User_ProfileImage.FileName);
                    string extension = Path.GetExtension(model.User_ProfileImage.FileName);
                    string newFileName = fileName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + extension;
                    string path = Path.Combine(wwwRootPath, "profile_images", newFileName);

                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        model.User_ProfileImage.CopyTo(fileStream);
                    }

                    // Delete old image if exists
                    if (!string.IsNullOrEmpty(user.ProfileImage))
                    {
                        string oldFilePath = Path.Combine(uploadFolder, user.ProfileImage);
                        if (System.IO.File.Exists(oldFilePath))
                            System.IO.File.Delete(oldFilePath);
                    }

                    user.ProfileImage = newFileName;
                }

                bool updated = _userRepo.UpdateUser(user);

                if (updated)
                    return Json(new { success = true, message = "Profile updated successfully!" });
                else
                    return Json(new { success = false, message = "Profile update failed!" });
            }
            catch (Exception ex)
            { 
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }
    }
}