using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MVC.implement;
using MVC.interfaces;
using MVC.Models;

namespace MVC.Controllers
{
    public class AdminGraphController : Controller
    {
        private readonly ILogger<AdminGraphController> _logger;

        private readonly IAdminGraphRepository _adminGraph;
        public AdminGraphController(ILogger<AdminGraphController> logger, IAdminGraphRepository adminRepository)
        {
            _logger = logger;
            _adminGraph = adminRepository;
        }

        public IActionResult Dashboard()
        {
            string userJson = HttpContext.Session.GetString("UserData");

            if (!string.IsNullOrEmpty(userJson))
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }
        public IActionResult UserCountSummary()
        {
            string userJson = HttpContext.Session.GetString("UserData");

            if (!string.IsNullOrEmpty(userJson))
            {
                var data = _adminGraph.GetActiveUserCount();
                return Json(data);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }
        public IActionResult InactiveUserCount()
        {

            string userJson = HttpContext.Session.GetString("UserData");

            if (!string.IsNullOrEmpty(userJson))
            {
                var data = _adminGraph.GetInactiveUserCount();
                return Json(data);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        public IActionResult TotalUserCount()
        {
            string userJson = HttpContext.Session.GetString("UserData");
            if (!string.IsNullOrEmpty(userJson))
            {
                var data = _adminGraph.GetTotalUserCount();
                return Json(data);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }


        public IActionResult ExpenseSummary()
        {
            string userJson = HttpContext.Session.GetString("UserData");
            if (!string.IsNullOrEmpty(userJson))
            {
                var data = _adminGraph.GetCategoryWiseExpense();
                return Json(data);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        // 2️ Monthly Expense Trend
        public IActionResult MonthlyExpenseTrend()
        {
            string userJson = HttpContext.Session.GetString("UserData");
            if (!string.IsNullOrEmpty(userJson))
            {
                var data = _adminGraph.GetMonthlyExpenseTrend();
                return Json(data);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        // 3️ Payment Mode Summary
        public IActionResult PaymentModeSummary()
        {
            string userJson = HttpContext.Session.GetString("UserData");
            if (!string.IsNullOrEmpty(userJson))
            {
                var data = _adminGraph.GetPaymentModeSummary();
                return Json(data);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        // 4️ Top Spending Users
        public IActionResult TopSpendingUsers()
        {
            string userJson = HttpContext.Session.GetString("UserData");
            if (!string.IsNullOrEmpty(userJson))
            {
                var data = _adminGraph.GetTopSpendingUsers();
                return Json(data);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}