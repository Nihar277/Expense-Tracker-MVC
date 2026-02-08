using Microsoft.AspNetCore.Mvc;
using MVC.interfaces;

namespace MyApp.Namespace
{
    public class AdminController : Controller
    {

        private readonly IAdminRepository _repo;

        public AdminController(IAdminRepository repo)
        {
            _repo = repo;
        }

        // GET: AdminController
        public ActionResult Index()
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

        public IActionResult GetUsers()
        {
            string userJson = HttpContext.Session.GetString("UserData");


            if (!string.IsNullOrEmpty(userJson))
            {
                var data = _repo.GetAllUsers();
                return Json(new { success = true, users = data });
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }

        }



        [HttpPost]
        public JsonResult ToggleStatus(int id)
        {

            bool result = _repo.ToggleStatus(id);
            return Json(new { success = result });
        }

        public IActionResult ViewExpenses(int id)
        {

            string userJson = HttpContext.Session.GetString("UserData");

            if (!string.IsNullOrEmpty(userJson))
            {
                var transactions = _repo.GetTransactionsByUser(id);

                ViewBag.UserId = id;


                return View(transactions);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }
    }
}


