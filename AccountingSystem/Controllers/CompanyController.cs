using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AccountingSystem.Models;

namespace AccountingSystem.Controllers
{
    public class CompanyController : Controller
    {
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
        public ActionResult Index(string page)
        {
            AccountingDbGateway.OpenConnection();
            if (this.RenewSession() == false)
                return RedirectToAction("Index", "Home");
            ViewBag.AccordionId = "collapsethree,1";
            ViewBag.District = AccountingDbGateway.GetDistricts();
            AccountingDbGateway.CloseConnection();
            if (page != null)
                ViewBag.Page = page;
            return View();
        }

        public ActionResult SetDistrict()
        {
            AccountingDbGateway.OpenConnection();
            if (this.RenewSession() == false)
                return RedirectToAction("Index", "Home");
            ViewBag.AccordionId = "collapsethree,1";
            ViewBag.District = AccountingDbGateway.GetDistricts();
            var companies = AccountingDbGateway.GetCompanyListWithoutDistrict();
            AccountingDbGateway.CloseConnection();
            return View(companies);
        }

        [HttpPost]
        public JsonResult Index(Company aCompany)
        {
            AccountingDbGateway.OpenConnection();
            AccountingDbGateway.InsertOrUpdateCompany(aCompany);
            AccountingDbGateway.CloseConnection();
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Delete(int companyId)
        {
            AccountingDbGateway.OpenConnection();
            AccountingDbGateway.DeleteCompany(companyId);
            AccountingDbGateway.CloseConnection();
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckCompanyName(string name)
        {
            AccountingDbGateway.OpenConnection();
            var returnValue = AccountingDbGateway.GetCompanyByName(name) == null;
            AccountingDbGateway.CloseConnection();
            return Json(returnValue, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCompanyById(int companyId)
        {
            AccountingDbGateway.OpenConnection();
            var returnValue = AccountingDbGateway.GetCompanyById(companyId);
            AccountingDbGateway.CloseConnection();
            return Json(returnValue, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetContactPersonByCompanyId(int companyId)
        {
            AccountingDbGateway.OpenConnection();
            var returnValue = AccountingDbGateway.GetContactPersonsByCompanyId(companyId);
            AccountingDbGateway.CloseConnection();
            return Json(returnValue, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetContactPersonById(int id)
        {
            AccountingDbGateway.OpenConnection();
            var returnValue = AccountingDbGateway.GetContactPersonById(id);
            AccountingDbGateway.CloseConnection();
            return Json(returnValue, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddOrUpdatePerson(ContactPerson aPerson)
        {
            AccountingDbGateway.OpenConnection();
            if (aPerson.Id == 0)
                AccountingDbGateway.AddPerson(aPerson);
            else
                AccountingDbGateway.UpdatePerson(aPerson);
            var returnValue = AccountingDbGateway.GetContactPersonsByCompanyId(aPerson.CId).OrderBy(x => x.Name);
            AccountingDbGateway.CloseConnection();
            return Json(returnValue, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeletePerson(int Id, int CId)
        {
            AccountingDbGateway.OpenConnection();
            AccountingDbGateway.DeletePerson(Id);
            var returnValue = AccountingDbGateway.GetContactPersonsByCompanyId(CId).OrderBy(x => x.Name);
            AccountingDbGateway.CloseConnection();
            return Json(returnValue, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLedgerList(string mainGroup, string companyId)
        {
            AccountingDbGateway.OpenConnection();
            var returnValue = AccountingDbGateway.GetLedgers(mainGroup, companyId);
            AccountingDbGateway.CloseConnection();
            return Json(returnValue, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SearchCompany(string str)
        {
            AccountingDbGateway.OpenConnection();
            var json = Json(AccountingDbGateway.GetCompanyList().Where(x => x.Name.ToLower().StartsWith(str.ToLower())), JsonRequestBehavior.AllowGet);
            AccountingDbGateway.CloseConnection();
            json.MaxJsonLength = int.MaxValue;
            return json;
        }
        public JsonResult GetCompanyList()
        {
            AccountingDbGateway.OpenConnection();
            var json = Json(AccountingDbGateway.GetCompanyList(), JsonRequestBehavior.AllowGet);
            AccountingDbGateway.CloseConnection();
            json.MaxJsonLength = int.MaxValue;
            return json;
        }
        public JsonResult GetCompanyListByKey(string startingKey)
        {
            AccountingDbGateway.OpenConnection();
            var companies = AccountingDbGateway.GetCompanyList(startingKey);
            AccountingDbGateway.CloseConnection();
            var json = Json(companies, JsonRequestBehavior.AllowGet);
            json.MaxJsonLength = int.MaxValue;
            return json;
        }
        public JsonResult CheckCompanyNameWithId(string name,int id)
        {
            AccountingDbGateway.OpenConnection();
            var returnValue = AccountingDbGateway.GetCompanyByName(name, id) == null;
            AccountingDbGateway.CloseConnection();
            return Json(returnValue, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SetCompanyDistrict(int companyId, int districtId)
        {
            AccountingDbGateway.OpenConnection();
            AccountingDbGateway.SetCompanyDistrict(companyId,districtId);
            AccountingDbGateway.CloseConnection();
            return Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}