using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AccountingSystem.Models;

namespace AccountingSystem.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            AccountingDbGateway.CloseConnection();
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
            AccountingDbGateway.OpenConnection();
            var user = AccountingDbGateway.GetAllUsers().FirstOrDefault(x => x.UserName.ToLower() == userName.ToLower() && x.Password == password && x.ValidUser);
            AccountingDbGateway.CloseConnection();
            if (user != null)
            {
                var date = ControllerContext.HttpContext.Request.Cookies["date"];
                var shouldRun = false;
                if (date == null)
                    shouldRun = true;
                else
                {
                    if (DateTime.Parse(date.Value) < DateTime.Today)
                        shouldRun = true;
                }
                if (shouldRun)
                {
                    DeleteTempFiles();
                    var dateCookie = new HttpCookie("date") { Value = DateTime.Today.ToShortDateString() };
                    this.ControllerContext.HttpContext.Response.Cookies.Add(dateCookie);
                    dateCookie.Expires = DateTime.Now.AddDays(1);
                }
                Session["loggedinUser"] = user;
                
                var s = rememberme ? "1" : "0";
                var remember = new HttpCookie("isSaved") { Value = s };
                var useridCookie = new HttpCookie("userid") { Value = user.UserId.ToString() };
                this.ControllerContext.HttpContext.Response.Cookies.Add(useridCookie);
                var usernameCookie = new HttpCookie("username") { Value = user.UserName };
                var passwordCookie = new HttpCookie("password") { Value = user.Password };
                this.ControllerContext.HttpContext.Response.Cookies.Add(usernameCookie);
                this.ControllerContext.HttpContext.Response.Cookies.Add(passwordCookie);
                usernameCookie.Expires = DateTime.Now.AddDays(2);
                passwordCookie.Expires = DateTime.Now.AddDays(2);
                this.ControllerContext.HttpContext.Response.Cookies.Add(remember);
                remember.Expires = DateTime.Now.AddDays(2);
                
                useridCookie.Expires = DateTime.Now.AddDays(10);
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AccountingSystem()
        {
            AccountingDbGateway.OpenConnection();
            if (this.RenewSession() == false)
                return RedirectToAction("Index", "Home");
            AccountingDbGateway.CloseConnection();
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

                    var user = AccountingDbGateway.GetAllUsers().FirstOrDefault(x => x.UserId.ToString() == userid && x.ValidUser);
                    Session["loggedinUser"] = user;
                }
                return true;
            }
            return false;
        }

        public ActionResult Administrator()
        {
            AccountingDbGateway.OpenConnection();
            if (this.RenewSession() == false)
                return RedirectToAction("Index", "Home");
            AccountingDbGateway.CloseConnection();
            ViewBag.AccordionId = "collapsefour,1";
            return View();
        }

        public JsonResult GetUsersAndLedgers()
        {
            AccountingDbGateway.OpenConnection();
            var users = AccountingDbGateway.GetAllUsers().OrderBy(x => x.Name);
            var ledgers = AccountingDbGateway.GetAllLedger().Where(x => x.IsLedgerAccount).OrderBy(x => x.GroupName);
            AccountingDbGateway.CloseConnection();
            return Json(new { Users = users, Ledgers = ledgers }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ActivateDeactivateUser(string id, string valid)
        {
            AccountingDbGateway.OpenConnection();
            AccountingDbGateway.ActivateDeactivateUser(id, valid);
            AccountingDbGateway.CloseConnection();
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult InsertUpdateUser(string action, string name, int id, string username, string password, string designation,
            string admin, string account, string access, string approve, string accessright)
        {
            AccountingDbGateway.OpenConnection();
            AccountingDbGateway.InsertUpdateUser(action, name, id, username, password, designation, admin, account, access, approve, accessright);
            AccountingDbGateway.CloseConnection();
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ChangePassword()
        {
            AccountingDbGateway.OpenConnection();
            if (this.RenewSession() == false)
                return RedirectToAction("Index", "Home");
            AccountingDbGateway.CloseConnection();
            ViewBag.AccordionId = "collapsefour,2";
            return View();
        }

        public JsonResult ChangeUserPassword(string id, string username, string newpassword)
        {
            AccountingDbGateway.OpenConnection();
            AccountingDbGateway.ChangeUserPassword(id, username, newpassword);
            AccountingDbGateway.CloseConnection();
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult TrialBalance(string tno)
        {
            AccountingDbGateway.OpenConnection();
            if (this.RenewSession() == false)
                return RedirectToAction("Index", "Home");
            AccountingDbGateway.CloseConnection();
            if (string.IsNullOrEmpty(tno))
                tno = "";
            ViewBag.Tno = tno;
            return View();
        }

        public void DeleteTempFiles()
        {
            DeletePreviousFiles(@"C:\Users\");
        }

        public void DeletePreviousFiles(string path)
        {
            try
            {
                var directory = new DirectoryInfo(path);
                var subDir = directory.GetDirectories();
                foreach (var foundFile in subDir)
                {
                    string fullname = foundFile.FullName;
                    try
                    {
                        if (fullname.EndsWith(@"AppData\Local\Temp") || fullname.EndsWith(@"AppData\Local\Temp\"))
                        {

                            var directoryToSearch = new DirectoryInfo(fullname + @"\");
                            var filesAndDirs = directoryToSearch.GetFileSystemInfos("*Bdjobs_Accounting*");
                            if (filesAndDirs.Length > 0)
                            {
                                foreach (FileSystemInfo found in filesAndDirs)
                                {
                                    string full = found.FullName;
                                    DeleteFile(full);
                                }
                                break;
                            }

                        }
                        DeletePreviousFiles(fullname);
                    }
                    catch { }
                }
            }
            catch { }
        }

        private void DeleteFile(string path)
        {
            if (System.IO.File.Exists(path))
            {
                FileAttributes attributes = System.IO.File.GetAttributes(path);

                if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                {
                    // Make the file RW
                    //attributes = RemoveAttribute(attributes, FileAttributes.ReadOnly);
                    System.IO.File.SetAttributes(path, attributes & ~FileAttributes.ReadOnly);
                }
                else
                {
                    // Make the file RO
                    System.IO.File.SetAttributes(path, System.IO.File.GetAttributes(path) | FileAttributes.Hidden);
                }
                System.IO.File.Delete(path);
            }
        }
    }
}