using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AccountingSystem.Models;

namespace AccountingSystem.Controllers
{
    public class HomeController : Controller
    {
        private AccountingDbGateway AccountingDb = new AccountingDbGateway();
        public ActionResult Index()
        {
            if (Session["loggedinUser"] == null)
            {
                var isSaved = ControllerContext.HttpContext.Request.Cookies["isSaved"];
                if (isSaved != null && isSaved.Value == "1")
                {
                    var username = ControllerContext.HttpContext.Request.Cookies["username"].Value;
                    var pass = ControllerContext.HttpContext.Request.Cookies["password"].Value;
                    ViewBag.Username = username;
                    ViewBag.Password = pass;
                }
                return View();
            }
            return RedirectToAction("AccountingSystem", "Home");
        }
        [HttpPost]
        public JsonResult Index(string userName, string password, bool rememberme)
        {
            var user = AccountingDb.GetAllUsers().FirstOrDefault(x => x.UserName.ToLower() == userName.ToLower() && x.Password == password && x.ValidUser);
            if (user != null)
            {
                Session["loggedinUser"] = user;
                var s = rememberme ? "1" : "0";
                var remember = new HttpCookie("isSaved") { Value = s };
                var useridCookie = new HttpCookie("userid") { Value = user.UserId.ToString() };
                var usernameCookie = new HttpCookie("username") { Value = user.UserName };
                var passwordCookie = new HttpCookie("password") { Value = user.Password };
                this.ControllerContext.HttpContext.Response.Cookies.Add(useridCookie);
                this.ControllerContext.HttpContext.Response.Cookies.Add(remember);
                this.ControllerContext.HttpContext.Response.Cookies.Add(usernameCookie);
                this.ControllerContext.HttpContext.Response.Cookies.Add(passwordCookie);
                remember.Expires = DateTime.Now.AddDays(2);
                usernameCookie.Expires = DateTime.Now.AddDays(2);
                passwordCookie.Expires = DateTime.Now.AddDays(2);
                useridCookie.Expires = DateTime.Now.AddDays(10);
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AccountingSystem()
        {
            if (this.RenewSession() == false)
                return RedirectToAction("Index", "Home");
            ViewBag.AccordionId = "collapseOne,0";
            return View();
        }

        public ActionResult LogOut()
        {
            Session.Remove("loggedinUser");
            var cookie = ControllerContext.HttpContext.Request.Cookies["userid"];
            cookie.Expires = DateTime.Now.AddDays(-1);
            Response.Cookies.Add(cookie);
            return RedirectToAction("Index", "Home");
        }

        public bool RenewSession()
        {
            if (ControllerContext.HttpContext.Request.Cookies["userid"] != null)
            {
                var userid = ControllerContext.HttpContext.Request.Cookies["userid"].Value;
                if (Session["loggedinUser"] == null)
                {
                    var user = AccountingDb.GetAllUsers().FirstOrDefault(x => x.UserId.ToString() == userid && x.ValidUser);
                    Session["loggedinUser"] = user;
                }
                return true;
            }
            return false;
        }

        public ActionResult Administrator()
        {
            if (this.RenewSession() == false)
                return RedirectToAction("Index", "Home");
            ViewBag.AccordionId = "collapsefour,1";
            return View();
        }

        public JsonResult GetUsersAndLedgers()
        {
            return Json(new { Users = AccountingDb.GetAllUsers().OrderBy(x => x.Name), Ledgers = AccountingDb.GetAllLedger().Where(x => x.IsLedgerAccount).OrderBy(x => x.GroupName) }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ActivateDeactivateUser(string id, string valid)
        {
            AccountingDb.ActivateDeactivateUser(id, valid);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult InsertUpdateUser(string action, string name, int id, string username, string password, string designation,
            string admin, string account, string access, string approve, string accessright)
        {
            AccountingDb.InsertUpdateUser(action, name, id, username, password, designation, admin, account, access, approve, accessright);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ChangePassword()
        {
            if (this.RenewSession() == false)
                return RedirectToAction("Index", "Home");
            ViewBag.AccordionId = "collapsefour,2";
            return View();
        }

        public JsonResult ChangeUserPassword(string id, string username, string newpassword)
        {
            AccountingDb.ChangeUserPassword(id, username, newpassword);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult TrialBalance(string tno)
        {
            if (this.RenewSession() == false)
                return RedirectToAction("Index", "Home");
            if (string.IsNullOrEmpty(tno))
                tno = "";
            ViewBag.Tno = tno;
            return View();
        }
    }
}