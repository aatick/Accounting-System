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
        private AccountingDbGateway accountingDb = new AccountingDbGateway();

        public bool RenewSession()
        {
            if (ControllerContext.HttpContext.Request.Cookies["userid"] != null)
            {
                var userid = ControllerContext.HttpContext.Request.Cookies["userid"].Value;
                if (Session["loggedinUser"] == null)
                {
                    var user = accountingDb.GetAllUsers().FirstOrDefault(x => x.UserId.ToString() == userid && x.ValidUser);
                    Session["loggedinUser"] = user;
                }
                return true;
            }
            return false;
        }
        public ActionResult Index()
        {
            if (this.RenewSession() == false)
                return RedirectToAction("Index", "Home");
            ViewBag.AccordionId = "collapsethree,1";
            ViewBag.CompanyList = accountingDb.GetCompanyList().OrderBy(x => x.Name);
            return View();
        }

        [HttpPost]
        public JsonResult Index(Company aCompany)
        {
            accountingDb.InsertOrUpdateCompany(aCompany);
            return Json(accountingDb.GetCompanyByName(aCompany.Name), JsonRequestBehavior.AllowGet);
        }

        public JsonResult Delete(int companyId)
        {
            accountingDb.DeleteCompany(companyId);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckCompanyName(string name)
        {
            if (accountingDb.GetCompanyByName(name) == null)
                return Json(true, JsonRequestBehavior.AllowGet);
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCompanyById(int companyId)
        {
            return Json(accountingDb.GetCompanyById(companyId), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetContactPersonByCompanyId(int companyId)
        {
            return Json(accountingDb.GetContactPersonsByCompanyId(companyId), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetContactPersonById(int id)
        {
            return Json(accountingDb.GetContactPersonById(id), JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddOrUpdatePerson(ContactPerson aPerson)
        {
            if (aPerson.Id == 0)
                accountingDb.AddPerson(aPerson);
            else
                accountingDb.UpdatePerson(aPerson);
            return Json(accountingDb.GetContactPersonsByCompanyId(aPerson.CId).OrderBy(x => x.Name), JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeletePerson(int Id, int CId)
        {
            accountingDb.DeletePerson(Id);
            return Json(accountingDb.GetContactPersonsByCompanyId(CId).OrderBy(x => x.Name), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Reports(string reportId)
        {
            if (this.RenewSession() == false)
                return RedirectToAction("Index", "Home");
            if (reportId != null)
                ViewBag.ReportId = reportId;
            ViewBag.AccordionId = "collapsethree,3";
            ViewBag.CompanyList = accountingDb.GetCompanyList();
            return View();
        }

        public JsonResult GetSubreports()
        {
            return Json(accountingDb.GetSubreports(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetType2Report(string type, string startingDate, string endDate, string info, int num)
        {
            if (num == 0)
            {
                var ledgers = accountingDb.GetTrialBalanceReport(type, startingDate, endDate);
                Session["reportName"] = "rptTrialBalance.rpt";
                Session["Report"] = ledgers;
                var title = type == "Month"
                    ? "For the Month of " +
                      new DateTime(Convert.ToInt32(endDate), Convert.ToInt32(startingDate), 1).ToString("MMMM, yyyy")
                    : "For the date " + startingDate + " to " + endDate;
                Session["ReportTitle"] = title;
            }
            else if (num == 6 || num == 8)
            {
                object products;
                switch (num)
                {
                    case 6:
                        {
                            products = accountingDb.GetReportRpt(startingDate, endDate, type, num, 1);
                            Session["ReportTitle"] = info;
                        }
                        break;
                    default:
                        {
                            var str = info.Split('/');
                            products = accountingDb.GetReportRpt(startingDate, endDate, type, num,
                                Convert.ToInt32(str[1]));
                            Session["ReportTitle"] = str[0];
                        }
                        break;
                }
                Session["reportName"] = "rptGeneralLedger.rpt";
                Session["Report"] = products;
                Session["ReportSubTitle"] = "For the date " + startingDate + " to " + endDate;
            }
            else if (num == 9)
            {
                var ledgers = accountingDb.GetReportRpt(startingDate, endDate, type, num, 0);
                Session["reportName"] = "rptFixedAssets.rpt";
                Session["Report"] = ledgers;
                var subtitle = "Depreciation Schedule as at " + DateTime.Parse(startingDate).ToString("MMM dd, yyyy");
                Session["ReportTitle"] = info;
                Session["ReportSubTitle"] = subtitle;
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAccountReceivableReport(string type)
        {
            var products = accountingDb.GetAccountReceivableReport(type);
            Session["reportName"] = "rptAccountsReceivable.rpt";
            Session["Report"] = products;
            var title = type == "PRO" ? "Product wise Account Receivables Today" : "Company wise Account Receivables Today";
            Session["ReportTitle"] = title;
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetReportType1(string startingDate, string endDate, string type, string info, int num)
        {
            var start = startingDate.Split(',');
            var end = endDate.Split(',');
            var startDate = new DateTime(Convert.ToInt32(start[1]), Convert.ToInt32(start[0]), 1);
            var endingDate = new DateTime(Convert.ToInt32(end[1]), Convert.ToInt32(end[0]),
                DateTime.DaysInMonth(Convert.ToInt32(end[1]), Convert.ToInt32(end[0])));
            var diff = 0;
            var cp = 0;
            if (Convert.ToInt32(start[1]) == Convert.ToInt32(end[1]))
            {
                diff = Convert.ToInt32(end[0]) - Convert.ToInt32(start[0]);
                cp = diff + 1;
            }
            else
            {
                diff = (Convert.ToInt32(end[1]) - Convert.ToInt32(start[1])) * 12 -
                       (Convert.ToInt32(start[0]) - Convert.ToInt32(end[0])) + 1;
                cp = diff;
            }
            var products = accountingDb.GetReportRpt(startDate.ToString("MM/dd/yyyy"),
                endingDate.ToString("MM/dd/yyyy"), type, num, cp);
            var name = "";
            Session["Report"] = products;
            switch (num)
            {
                case 4:
                    name = "rptProfitLoss1.rpt";
                    break;
                case 7:
                    name = "rptBalanceSheet1.rpt";
                    break;
                default:
                    name = "rptRevenue.rpt";
                    break;
            }
            Session["reportName"] = name;

            Session["diff"] = diff;
            Session["month"] = start[0] + "," + end[0];
            Session["year"] = start[1] + "," + end[1];
            Session["info"] = info;
            Session["reportSubtitle"] = "For the Month " + startDate.ToString("MMM-yy") + " to " +
                                            endingDate.ToString("MMM-yy");
            Session["num"] = num;
            var title = "";
            switch (num)
            {
                case 0:
                    title = "Monthly Account Receivable (";
                    break;
                case 1:
                    title = "Monthly Revenue Statement (";
                    break;
                case 2:
                    title = "Monthly Expense Statement (";
                    break;
                case 3:
                    title = "Monthly Cash Collection Statement (";
                    break;
                case 4:
                    title = "Monthly Income Statement (";
                    break;
                case 5:
                    title = "Monthly Cash & Bank Details(";
                    break;
                case 7:
                    title = "Monthly Balance Sheet(" + info + ")";
                    break;
            }
            switch (type)
            {
                case "PRO":
                    title += "Product wise-";
                    break;
                case "COM":
                    title += "Company wise-";
                    break;
            }
            if (num == 4 && type == "PL")
            {
                title = "Monthly Profit & Loss Statement (";
            }
            switch (info)
            {
                case "0":
                    title += "Summary)";
                    break;
                case "1":
                    title += "Detail)";
                    break;
            }
            Session["reportTitle"] = title;
            Session["type"] = type;
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLedgerList(string mainGroup, string companyId)
        {
            return Json(accountingDb.GetLedgers(mainGroup, companyId), JsonRequestBehavior.AllowGet);
        }

        public JsonResult SearchCompany(string str)
        {
            var json = Json(accountingDb.GetCompanyList().Where(x => x.Name.ToLower().StartsWith(str.ToLower())), JsonRequestBehavior.AllowGet);
            json.MaxJsonLength = int.MaxValue;
            return json;
        }
        public JsonResult GetCompanyList()
        {
            var json = Json(accountingDb.GetCompanyList(), JsonRequestBehavior.AllowGet);
            json.MaxJsonLength = int.MaxValue;
            return json;
        }
    }
}