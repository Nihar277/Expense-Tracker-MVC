using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MVC.implement;
using MVC.interfaces;
using MVC.Models;

namespace MVC.Controllers
{

    // [Route("[controller]")]
    public class ExpenseController : Controller
    {

        private readonly IExpenseRepository _user;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<ExpenseController> _logger;


        public ExpenseController(ILogger<ExpenseController> logger, IExpenseRepository user, IWebHostEnvironment env)
        {
            _logger = logger;
            _user = user;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {
            return View();
        }
        public async Task<JsonResult> GetAllExpense()
        {
            string userdat = HttpContext.Session.GetString("UserData");

            var userData = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(userdat);

            int userid = userData["UserID"].GetInt32();

            Console.WriteLine(userid);
            List<t_transaction> expenseData = await _user.getAllExpense(userid);
            return Json(expenseData);
        }

        // [HttpGet("Expense/totalExpense/{id}")]
        public async Task<IActionResult> totalExpense()
        {
            string userdat = HttpContext.Session.GetString("UserData");

            var userData = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(userdat);

            int userid = userData["UserID"].GetInt32();
            int totalExpense = await _user.totalExpense(userid);
            return Json(totalExpense);
        }


        [HttpGet]
        public IActionResult addExpense()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create(t_transaction model)
        {
            try
            {
                string filePath = null;

                // Handle file upload if provided
                if (model.transaction_ReceiptImage != null && model.transaction_ReceiptImage.Length > 0)
                {
                    string uploadDir = Path.Combine(_env.WebRootPath, "transaction_images");

                    // Create folder if not present
                    if (!Directory.Exists(uploadDir))
                        Directory.CreateDirectory(uploadDir);

                    // Use original filename
                    string fileName = Path.GetFileName(model.transaction_ReceiptImage.FileName);
                    string fullPath = Path.Combine(uploadDir, fileName);

                    // Save file
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await model.transaction_ReceiptImage.CopyToAsync(stream);
                    }

                    // Store relative path in model for DB
                    filePath = "/transaction_images/" + fileName;
                    model.ReceiptImage = filePath;
                }

                // If date not provided, use current date
                if (model.TransDate == DateTime.MinValue)
                    model.TransDate = DateTime.Now;

                string userdat = HttpContext.Session.GetString("UserData");

                var userData = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(userdat);

                int userid = userData["UserID"].GetInt32();
                model.UserID = userid;
                // Save to database
                int result = await _user.addExpense(model);

                if (result == 1)
                    return Json(new { success = true });
                else
                    return Json(new { success = false, message = "Database insertion failed." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        [HttpPost]
        public async Task<IActionResult> UploadReceipt(IFormFile files)
        {
            try
            {
                if (files == null || files.Length == 0)
                    return Json(new { success = false, message = "No file uploaded." });

                string uploadDir = Path.Combine(_env.WebRootPath, "transaction_images");
                if (!Directory.Exists(uploadDir))
                    Directory.CreateDirectory(uploadDir);

                string fileName = Path.GetFileName(files.FileName);
                string fullPath = Path.Combine(uploadDir, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await files.CopyToAsync(stream);
                }

                string dbPath = "/transaction_images/" + fileName;
                return Json(new { success = true, fileName });

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            _user.deleteExpense(id);
            return RedirectToAction("index");
        }

        [HttpGet]
        public async Task<IActionResult> GetExpenseById(int id)
        {
            var expense = await _user.getExpenseById(id);
            if (expense == null)
                return Json(new { success = false, message = "Expense not found" });

            return Json(new { success = true, data = expense });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateExpense(t_transaction model)
        {
            try
            {
                string filePath = model.ReceiptImage; // Keep old image path by default

                // Handle new image upload if provided
                if (model.transaction_ReceiptImage != null && model.transaction_ReceiptImage.Length > 0)
                {
                    string uploadDir = Path.Combine(_env.WebRootPath, "transaction_images");
                    if (!Directory.Exists(uploadDir))
                        Directory.CreateDirectory(uploadDir);

                    string fileName = Path.GetFileName(model.transaction_ReceiptImage.FileName);
                    string fullPath = Path.Combine(uploadDir, fileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await model.transaction_ReceiptImage.CopyToAsync(stream);
                    }

                    filePath = "/transaction_images/" + fileName;
                }

                model.ReceiptImage = filePath;
                int result = await _user.updateExpense(model);

                if (result == 1)
                    return Json(new { success = true });
                else
                    return Json(new { success = false, message = "Update failed" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> addExpense(t_transaction transaction)
        {
            // var userId = HttpContext.Session.GetInt32("UserData");

            int result = await _user.addExpense(transaction);

            if (result == 1)
            {
                TempData["Message"] = "Expense Added Successfully";
                return RedirectToAction("Index", "Expense");
            }
            else
            {
                TempData["Message"] = "Failed To Add Expense";
                return View(transaction);
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}